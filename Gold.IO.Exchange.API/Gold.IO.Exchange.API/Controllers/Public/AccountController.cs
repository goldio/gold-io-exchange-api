using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain.Enum;
using Gold.IO.Exchange.API.Domain.User;
using Gold.IO.Exchange.API.ViewModels;
using Gold.IO.Exchange.API.ViewModels.Public;
using Gold.IO.Exchange.API.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gold.IO.Exchange.API.Controllers.Public
{
    [Route("api/public/account")]
    [ApiController]
    [Authorize]
    public class AccountController : Controller
    {
        private IUserService UserService { get; set; }
        private IPersonService PersonService { get; set; }
        private IUserWalletService WalletService { get; set; }
        private ICoinService CoinService { get; set; }
        private IOrderService OrderService { get; set; }

        public AccountController([FromServices] 
            IUserService userService,
            IPersonService personService,
            IUserWalletService walletService,
            ICoinService coinService,
            IOrderService orderService)
        {
            UserService = userService;
            PersonService = personService;
            WalletService = walletService;
            CoinService = coinService;
            OrderService = orderService;
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetAccountInfo()
        {
            if (!User.IsInRole("OnlyAccount") && !User.IsInRole("AllActions"))
                return Forbid();

            var result = new AccountInfo();

            var user = UserService.GetAll()
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            var person = PersonService.GetAll()
                .FirstOrDefault(x => x.User == user);

            result.FullName = person.FullName;
            result.BirthDate = person.BirthDate;
            result.Email = person.Email;
            result.PhoneNumber = person.PhoneNumber;
            result.City = new CityViewModel(person.City);
            result.Address = person.Address;
            result.Balance = WalletService.GetAll()
                .Where(x => x.User == user)
                .Select(x => WalletToViewModel(x))
                .ToList();

            return Ok(new DataResponse<AccountInfo>
            {
                Data = result
            });
        }

        private UserWalletViewModel WalletToViewModel(UserWallet wallet)
        {
            var user = UserService.GetAll()
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            var result = new UserWalletViewModel(wallet);

            var btcCoin = CoinService.GetAll().FirstOrDefault(x => x.ShortName == "BTC");

            var orders = OrderService.GetAll()
                .Where(x => x.User == user &&
                    x.Status == OrderStatus.Open)
                .ToList();

            foreach (var order in orders)
            {
                if (order.Side == OrderSide.Buy && order.QuoteAsset == wallet.Coin)
                    result.InOrders += order.Balance * order.Price;
                else if (order.Side == OrderSide.Sell && order.BaseAsset == wallet.Coin)
                    result.InOrders += order.Balance;
            }

            if (wallet.Coin.ShortName != "BTC")
            {
                var price = OrderService.GetAll()
                    .FirstOrDefault(x => x.BaseAsset == wallet.Coin && x.QuoteAsset == btcCoin);

                if (price != null)
                    result.Cost = result.TotalBalance * price.Price;
                else
                    result.Cost = 0;
            }
            else
            {
                result.Cost = result.TotalBalance;
            }

            return result;
        }
    }
}