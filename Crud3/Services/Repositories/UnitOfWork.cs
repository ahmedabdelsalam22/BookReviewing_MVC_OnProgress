﻿using BookReviewing_MVC.Models;
using BookReviewing_MVC.Services.IRepositories;

namespace BookReviewing_MVC.Services.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            _bookRepository = new BookRepository(_context);
        }
        public IBookRepository _bookRepository { get; private set; }

        public async Task save()
        {
            await _context.SaveChangesAsync();
        }
    }
}