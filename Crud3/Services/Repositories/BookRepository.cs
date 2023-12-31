﻿using BookReviewing_MVC.Data;
using BookReviewing_MVC.Services.IRepositories;
using BookReviewingMVC.Models;

namespace BookReviewing_MVC.Services.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
