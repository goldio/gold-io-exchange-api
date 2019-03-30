using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

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

        [HttpGet("eth/{address}/balance")]
        public async Task<IActionResult> EthBalance(string address)
        {
            // 0x5d6E78DdD81A59e2e562e5629d13217a50f237dA
            var web3 = new RpcClient(new Uri("https://mainnet.infura.io/v3/95eecca9b719440d8d60600b8928aa9b"));
            var balance = await web3.SendRequestAsync<string>(new RpcRequest(1, "eth_getBalance", new[] { address, "latest" }));
            return Json(Web3.Convert.FromWei(new HexBigInteger(balance)));
        }

        [HttpGet("btc/{address}/balance")]
        public async Task<IActionResult> BtcBalance(string address)
        {
            NetworkCredential credentials = new NetworkCredential("goldio", "goldiorpcadmin");
            RPCClient rpc = new RPCClient(credentials, "http://127.0.0.1:8333/", Network.Main);
            var receivedMoney = rpc.GetReceivedByAddress(BitcoinAddress.Create(address));
            return Json(receivedMoney.ToUnit(MoneyUnit.BTC));
        }

        [HttpPost("fee")]
        public async Task<IActionResult> GetTransactionFee([FromBody] GetTransactionFeeRequest request)
        {
            if (request.Coin.Equals("BTC"))
                return Json(await CalculateBitcoinFee(request.Amount));

            if (request.Coin.Equals("ETH"))
                return Json(await CalculateEthereumFee(request.Amount));

            return Json(new ResponseModel
            {
                Success = false
            });
        }

        private async Task<GetTransactionFeeResponse> CalculateEthereumFee(double amount)
        {
            return new GetTransactionFeeResponse
            {
                Fee = 0.00042,
                FinalAmount = amount - 0.00042
            };
        }

        private async Task<GetTransactionFeeResponse> CalculateBitcoinFee(double amount)
        {
            return new GetTransactionFeeResponse
            {
                Fee = 0.00005,
                FinalAmount = amount - 0.00005
            };
        }

        [HttpPost("withdrawalVerify")]
        public async Task<IActionResult> WithdrawalVerify([FromBody] VerifyWithdrawal request)
        {
            var address = CoinAddressService.GetAll().FirstOrDefault(x => x.PublicAddress.Equals(request.Address));
            if (address == null)
                return Json(new ResponseModel { Success = false, Message = "Address not found" });

            var operation = WalletOperationService.GetAll().FirstOrDefault(x => x.Address == address);
            if (operation == null)
                return Json(new ResponseModel { Success = false, Message = "Operation not found" });

            if (address.Wallet.Balance < (double)request.Amount)
                return Json(new ResponseModel { Success = false, Message = "Insufficient funds" });

            address.IsUsing = false;
            CoinAddressService.Update(address);

            operation.Confirmations = 1;
            operation.Status = UserWalletOperationStatus.Completed;
            WalletOperationService.Update(operation);

            address.Wallet.Balance -= (double)request.Amount;
            WalletService.Update(address.Wallet);

            return Json(new ResponseModel());
        }

        [HttpPost("depositWebhook")]
        public async Task<IActionResult> DepositWebhook([FromBody] DepositNotification request)
        {
            var address = CoinAddressService.GetAll().FirstOrDefault(x => x.PublicAddress.Equals(request.Address));
            if (address == null)
                return Json(new ResponseModel { Success = false, Message = "Address not found" });

            var operation = WalletOperationService.GetAll().FirstOrDefault(x => x.Address == address);
            if (operation == null)
                return Json(new ResponseModel { Success = false, Message = "Operation not found" });

            address.IsUsing = false;
            CoinAddressService.Update(address);

            operation.Amount = (double)request.Amount;
            operation.Confirmations = 1;
            operation.Time = DateTime.UtcNow;
            operation.Status = UserWalletOperationStatus.Completed;
            WalletOperationService.Update(operation);

            address.Wallet.Balance += (double)request.Amount;
            WalletService.Update(address.Wallet);

            return Ok();
        }

        private long ParseHexString(string hexNumber)
        {
            hexNumber = hexNumber.Replace("x", string.Empty);
            long.TryParse(hexNumber, System.Globalization.NumberStyles.HexNumber, null, out long result);
            return result;
        }

        static class TableConvert
        {
            static sbyte[] unhex_table =
            { -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
       ,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
       ,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
       , 0, 1, 2, 3, 4, 5, 6, 7, 8, 9,-1,-1,-1,-1,-1,-1
       ,-1,10,11,12,13,14,15,-1,-1,-1,-1,-1,-1,-1,-1,-1
       ,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
       ,-1,10,11,12,13,14,15,-1,-1,-1,-1,-1,-1,-1,-1,-1
       ,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
      };

            public static int Convert(string hexNumber)
            {
                int decValue = unhex_table[(byte)hexNumber[0]];
                for (int i = 1; i < hexNumber.Length; i++)
                {
                    decValue *= 16;
                    decValue += unhex_table[(byte)hexNumber[i]];
                }
                return decValue;
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

            var address = CoinAddressService.GetAll().FirstOrDefault(x => x.Wallet == wallet && x.IsUsing && x.Type == CoinAddressType.Deposit);
            if (address == null)
            {
                if (wallet.Coin.ShortName.Equals("ETH"))
                {
                    var ethAddress = EthereumBlockchainHelper.GetAddress();
                    if (ethAddress == null)
                        return Json(new ResponseModel { Success = false, Message = "Address error" });

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
                    var btcAddress = BitcoinBlockchainHelper.GetAddress();
                    if (btcAddress == null)
                        return Json(new ResponseModel { Success = false, Message = "Address error" });

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

            if (wallet.Coin.ShortName.Equals("BTC"))
            {
                var tx = BitcoinBlockchainHelper.SendTransaction(request.Address, (decimal)request.Amount);
            }
            else if (wallet.Coin.ShortName.Equals("ETH"))
            {
                var tx = EthereumBlockchainHelper.SendTransaction(request.Address, (decimal)request.Amount);
            }
            else if (wallet.Coin.ShortName.Equals("EOS") || wallet.Coin.ShortName.Equals("GIO"))
            {
                using (var cryptolions = new СryptolionsClient())
                {
                    await cryptolions.CreateWithdrawalRequest(withdrawOrder.Address.PublicAddress, withdrawOrder.Amount, wallet.Coin.ShortName);
                    wallet.Balance -= request.Amount;
                    WalletService.Update(wallet);
                }
            }

            return Json(new ResponseModel());
        }
    }
}