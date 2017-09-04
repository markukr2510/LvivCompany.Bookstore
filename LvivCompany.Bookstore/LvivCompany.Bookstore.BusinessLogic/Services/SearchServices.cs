﻿using LvivCompany.Bookstore.BusinessLogic.Mapper;
using LvivCompany.Bookstore.BusinessLogic.ViewModels;
using LvivCompany.Bookstore.DataAccess.Repo;
using LvivCompany.Bookstore.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LvivCompany.Bookstore.BusinessLogic
{
    public class SearchServices
    {
        private IRepo<Book> _bookRepo;
        private IMapper<Book, BookViewModel> _bookmapper;

        public SearchServices(IRepo<Book> bookRepo, IMapper<Book, BookViewModel> bookmapper)
        {
            _bookRepo = bookRepo;
            _bookmapper = bookmapper;
        }

        public async Task<HomePageListViewModel> GetViewModelForHomePage(string searchText)
        {
            List<Book> books = (await _bookRepo.Get(x => x.Name.Contains(searchText))).ToList();
            return new HomePageListViewModel() { Books = _bookmapper.Map(books) };
        }
    }
}