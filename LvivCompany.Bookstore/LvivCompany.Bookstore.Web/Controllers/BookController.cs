﻿using System.Threading.Tasks;
using LvivCompany.Bookstore.BusinessLogic;
using LvivCompany.Bookstore.BusinessLogic.ViewModels;
using LvivCompany.Bookstore.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LvivCompany.Bookstore.Web.Controllers
{
    public class BookController : Controller
    {
        private readonly ILogger<BookController> _logger;
        private UserManager<User> userManager;
        private BookServices services;

        public BookController(UserManager<User> userManager, BookServices services, ILogger<BookController> logger)
        {
            this._logger = logger;
            this.userManager = userManager;
            this.services = services;
        }

        public async Task<IActionResult> BookPage(long id)
        {
            return View("BookPage", await services.GetViewModelForBookPageAsync(id));
        }

        [Authorize(Roles ="Seller")]
        [HttpGet]
        public async Task<IActionResult> AddBook()
        {
            BookViewModel model = new BookViewModel();
            return View("AddBook", await services.GetViewModelForAddBookPageAsync(model));
        }

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> AddBook(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                await services.AddBookAsync(model, (await userManager.GetUserAsync(HttpContext.User)).Id);
                var user = await userManager.GetUserAsync(HttpContext.User);
                _logger.LogInformation("Seller {@User} add new book {@Book}", new { FirstName = user.FirstName, LastName = user.LastName, UserName = user.UserName }, new { Id=model.Id, Name=model.Name, Price=model.Price });
                return RedirectToAction("SellersBook", "Home");
            }

            return View(await services.GetViewModelForAddBookPageAsync(model));
        }

        [HttpGet]
        public async Task<IActionResult> EditBook(long id)
        {
            return View("EditBook", await services.GetViewModelForEditBookPageAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> EditBook(long id, EditBookViewModel model)
        {
            if (ModelState.IsValid)
            {
                await services.EditBookAsync(id, model);
                var user = await userManager.GetUserAsync(HttpContext.User);
                _logger.LogInformation("Seller {@User} edit book {@Book}", new { FirstName = user.FirstName, LastName = user.LastName, UserName = user.UserName }, new { Id=model.Id, Name = model.Name, Price = model.Price });
                return RedirectToAction("SellersBook", "Home");
            }

            await services.PopulateCategoriesSelectListAsync(model);
            return View(model);
        }
    }
}