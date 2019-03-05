using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BlockExplorer.Сryptolions;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain.Coin;
using Gold.IO.Exchange.API.Domain.Enum;
using Gold.IO.Exchange.API.Domain.User;
using Gold.IO.Exchange.API.Utils.Helpers;
using Gold.IO.Exchange.API.ViewModels;
using Gold.IO.Exchange.API.ViewModels.Request;
using Gold.IO.Exchange.API.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using NBitcoin.RPC;
using NBXplorer;

namespace Gold.IO.Exchange.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class WalletsController : Controller
    {
        private IUserService UserService { get; set; }
        private IUserWalletService WalletService { get; set; }
        private ICoinAddressService CoinAddressService { get; set; }
        private ICoinAccountService CoinAccountService { get; set; }
        private IUserWalletOperationService WalletOperationService { get; set; }
        private IBitcoinService BitcoinService { get; set; }

        public WalletsController([FromServices]
            IUserService userService, 
            IUserWalletService walletService,
            ICoinAddressService coinAddressService,
            ICoinAccountService coinAccountService,
            IUserWalletOperationService walletOperationService,
            IBitcoinService bitcoinService)
        {
            UserService = userService;
            WalletService = walletService;
            CoinAddressService = coinAddressService;
            CoinAccountService = coinAccountService;
            WalletOperationService = walletOperationService;
            BitcoinService = bitcoinService;
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            using (var client = new СryptolionsClient())
            {
                var actions = await client.GetActions();
                return Json(actions);
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMeWallets()
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);

            var wallets = WalletService.GetAll()
                .Where(x => x.User == user)
                .Select(x => new UserWalletViewModel(x))
                .ToList();

            return Json(new DataResponse<List<UserWalletViewModel>> { Data = wallets });
        }

        [HttpGet("me/history/deposit")]
        public async Task<IActionResult> GetMeDepositHistory()
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);

            var operations = WalletOperationService.GetAll()
                .Where(x => x.Address.Wallet.User == user && x.Address.Type == CoinAddressType.Deposit && x.Status == UserWalletOperationStatus.Completed)
                .Select(x => new UserWalletOperationViewModel(x))
                .ToList();

            return Json(new DataResponse<List<UserWalletOperationViewModel>> { Data = operations });
        }

        [HttpGet("me/history/withdraw")]
        public async Task<IActionResult> GetMeWithdrawHistory()
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);

            var operations = WalletOperationService.GetAll()
                .Where(x => x.Address.Wallet.User == user && x.Address.Type == CoinAddressType.Withdraw)
                .Select(x => new UserWalletOperationViewModel(x))
                .ToList();

            return Json(new DataResponse<List<UserWalletOperationViewModel>> { Data = operations });
        }

        [HttpPost("{id}/deposit")]
        public async Task<IActionResult> Deposit(long id)
        {
            var wallet = WalletService.Get(id);
            if (wallet == null)
                return Json(new ResponseModel { Success = false, Message = "Wallet not found" });

            var address = CoinAddressService.GetAll().FirstOrDefault(x => x.Wallet == wallet);
            if (address == null || !address.IsUsing || address.Type != CoinAddressType.Deposit)
            {
                if (wallet.Coin.ShortName.Equals("ETH"))
                {
                    var ethKey = EthereumBlockchainHelper.GetECKey();
                    var account = new CoinAccount
                    {
                        AccountKey = ethKey,
                        Derivations = 0,
                        Coin = wallet.Coin
                    };

                    CoinAccountService.Create(account);

                    var ethAddress = EthereumBlockchainHelper.GetAddress(account.AccountKey);
                    address = new CoinAddress
                    {
                        PublicAddress = ethAddress,
                        Type = CoinAddressType.Deposit,
                        IsUsing = true,
                        Wallet = wallet
                    };

                    CoinAddressService.Create(address);
                }
                else if (wallet.Coin.ShortName.Equals("BTC"))
                {
                    var btcKey = BitcoinBlockchainHelper.GeneratePrivateKey();
                    var account = new CoinAccount
                    {
                        AccountKey = btcKey,
                        Derivations = 0,
                        Coin = wallet.Coin
                    };

                    CoinAccountService.Create(account);

                    var btcAddress = BitcoinBlockchainHelper.GetAddress(account.AccountKey);
                    if (btcAddress == null)
                        return Json(new ResponseModel { Success = false, Message = "Address error" });

                    //account.Derivations++;
                    //CoinAccountService.Update(account);

                    address = new CoinAddress
                    {
                        PublicAddress = btcAddress,
                        Type = CoinAddressType.Deposit,
                        IsUsing = true,
                        Wallet = wallet
                    };

                    CoinAddressService.Create(address);
                }
                else if (wallet.Coin.ShortName.Equals("EOS") || wallet.Coin.ShortName.Equals("GIO"))
                {
                    var memo = CryptHelper.CreateMD5($"{wallet.User.Login}_{DateTime.UtcNow}").Substring(0, 12);

                    address = new CoinAddress
                    {
                        PublicAddress = memo,
                        Type = CoinAddressType.Deposit,
                        IsUsing = true,
                        Wallet = wallet
                    };

                    CoinAddressService.Create(address);
                }

                var depositOrder = new UserWalletOperation
                {
                    Address = address,
                    Confirmations = 0,
                    Status = UserWalletOperationStatus.InProgress
                };

                WalletOperationService.Create(depositOrder);
            }

            return Json(new DepositResponse { Address = address.PublicAddress });
        }

        [HttpPost("{id}/withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request, long id)
        {
            var wallet = WalletService.Get(id);
            if (wallet == null)
                return Json(new ResponseModel { Success = false, Message = "Wallet not found" });

            if (wallet.Balance < request.Amount)
                return Json(new ResponseModel { Success = false, Message = "Insufficient funds" });

            var address = new CoinAddress
            {
                PublicAddress = request.Address,
                IsUsing = true,
                Type = CoinAddressType.Withdraw,
                Wallet = wallet
            };

            CoinAddressService.Create(address);

            var withdrawOrder = new UserWalletOperation
            {
                Amount = request.Amount,
                Address = address,
                Confirmations = 0,
                Status = UserWalletOperationStatus.InProgress
            };

            WalletOperationService.Create(withdrawOrder);

            wallet.Balance -= request.Amount;
            WalletService.Update(wallet);

            if (wallet.Coin.ShortName.Equals("EOS") || wallet.Coin.ShortName.Equals("GIO"))
            {
                using (var cryptolions = new СryptolionsClient())
                {
                    await cryptolions.CreateWithdrawalRequest(withdrawOrder.Address.PublicAddress, withdrawOrder.Amount, wallet.Coin.ShortName);
                }
            }

            return Json(new ResponseModel());
        }
    }
}