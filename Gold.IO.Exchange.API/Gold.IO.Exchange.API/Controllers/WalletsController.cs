using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.ViewModels;
using Gold.IO.Exchange.API.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gold.IO.Exchange.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WalletsController : Controller
    {
        private IUserService UserService { get; set; }
        private IWalletService WalletService { get; set; }

        public WalletsController([FromServices]
            IUserService userService, 
            IWalletService walletService)
        {
            UserService = userService;
            WalletService = walletService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMeWallets()
        {
            var user = UserService.GetAll()
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            var wallets = WalletService.GetAll()
                .Where(x => x.User == user)
                .Select(x => new WalletViewModel(x))
                .ToList();

            return Json(new DataResponse<List<WalletViewModel>> { Data = wallets });
        }
    }
}