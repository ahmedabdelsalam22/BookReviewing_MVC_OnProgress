﻿using AutoMapper;
using BookReviewing_MVC.DTOS;
using BookReviewing_MVC.Services.IRepositories;
using BookReviewing_MVC.Utilities;
using BookReviewingMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewing_MVC.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public BookController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var books = await _unitOfWork.bookRepository.GetAll();
            if (books == null) 
            {
                return NotFound("no books found");
            }
            return View(books);
        }
        [Authorize(Roles =SD.Role_Admin)]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookCreateDTO bookCreateDTO)
        {
            if (!ModelState.IsValid || bookCreateDTO == null)
            {
                ModelState.AddModelError("CustomError","book fields not valid");
            }
            Book bookfromDb = await _unitOfWork.bookRepository.Get(filter:x=>x.Title.ToLower() == bookCreateDTO.Title.ToLower());
            if(bookfromDb != null)
            {
                ModelState.AddModelError("CustomError","oops book alreay exists");
            }
            if (ModelState.IsValid)
            {
                Book book = _mapper.Map<Book>(bookCreateDTO);
                await _unitOfWork.bookRepository.Create(book);
                await _unitOfWork.save();
                return RedirectToAction("Index");
            }
            return View(bookCreateDTO);
        }

        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == 0|| id == null)
            {
                return NotFound();
            }
            Book book = await _unitOfWork.bookRepository.Get(filter:x=>x.Id == id);
            if (book == null)
            {
                ModelState.AddModelError("CustomError","this book not found!");
            }
            return View(book);
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(BookUpdateDTO bookUpdateDTO)
        {
            if (!ModelState.IsValid || bookUpdateDTO == null)
            {
                ModelState.AddModelError("CustomError", "book fields not valid");
            }
            if (ModelState.IsValid)
            {
                Book book = _mapper.Map<Book>(bookUpdateDTO);
                _unitOfWork.bookRepository.Update(book);
                await _unitOfWork.save();
                return RedirectToAction("Index");
            }
            return View(bookUpdateDTO);
        }
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int bookId)
        {
            if (bookId == 0)
            {
                ModelState.AddModelError("CustomError","this book not found");
            }
            Book book = await _unitOfWork.bookRepository.Get(filter:x=>x.Id == bookId);
            //if (book == null)
            //{
            //    return NotFound("book not found");
            //}
            _unitOfWork.bookRepository.Delete(book);
            await _unitOfWork.save();
            return RedirectToAction("Index");
        }
    }
}
