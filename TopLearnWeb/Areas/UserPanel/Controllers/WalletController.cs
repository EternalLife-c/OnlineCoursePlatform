﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.DTOs;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Areas.UserPanel.Controllers
{

    [Area("UserPanel")]
    [Authorize]
    public class WalletController : Controller
    {
        private IUserService _userService;
        public WalletController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("UserPanel/Wallet")]
        public IActionResult Index()
        {
            ViewBag.WalletList = _userService.GetUserWallet(User.Identity.Name);
            return View();
        }

        [HttpPost]
        [Route("UserPanel/Wallet")]
        public IActionResult Index(ChargeWalletViewModel charge)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.WalletList = _userService.GetUserWallet(User.Identity.Name);
                return View(charge);
            }

            int walletId = _userService.ChargeWallet(User.Identity.Name, charge.Amount, "شارژ حساب");

            #region Online Payment

            var payment = new ZarinpalSandbox.Payment(charge.Amount);

            var response = payment.PaymentRequest("شارژ حساب", "https://localhost:7114/OnlinePayment/"+walletId);

            if(response.Result.Status == 100)
            {
                Response.Redirect("https://sandbox.zarinpal.com/pg/StartPay/"+response.Result.Authority);
            }

            #endregion

            return null;
        }
    }
}
