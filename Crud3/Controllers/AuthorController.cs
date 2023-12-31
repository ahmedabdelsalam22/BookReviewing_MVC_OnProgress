﻿using AutoMapper;
using BookReviewing_MVC.Services.IRepositories;
using BookReviewing_MVC.Utilities;
using BookReviewingMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace BookReviewing_MVC.Controllers
{
    [Authorize]
    public class AuthorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public AuthorController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Author> authors = await _unitOfWork.authorRepository.GetAll(includes: new[] {"Country"});
            if (authors == null)
            {
                return NotFound();
            }
            return View(authors);
        }
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Create()
        {
            List<Country> countries = await _unitOfWork.countryRepository.GetAll();

            List<string> countryList = new List<string>();

            foreach (var country in countries)
            {
                countryList.Add(country.Name);
            }

            ViewBag.countriesListItem = new SelectList(countryList);

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Create(Author author)
        {
            if (author == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("please fill all data");
            }
            Author authorIsFound = await _unitOfWork.authorRepository.Get(filter: x=>x.FirstName.ToLower() == author.FirstName.ToLower());
            if(authorIsFound != null)
            {
                return BadRequest("this author arleady exists");
            }

            // when create new author .. the author country we will added should be found in database.. 
            Country country = await _unitOfWork.countryRepository.Get(filter: x=>x.Name.ToLower() == author.Country.Name.ToLower());
            if (country == null)
            {
                return NotFound("country does't exists");
            }

            author.Country = country;
            
            await _unitOfWork.authorRepository.Create(author);
            await _unitOfWork.save();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Update(int? id)
        {
            if(id == 0|| id == null)
            {
                return BadRequest();
            }
            Author author = await _unitOfWork.authorRepository.Get(filter:x=>x.Id == id, includes: new[] { "Country" });
            if (author == null)
            {
                return BadRequest();
            }

            List<Country> countries = await _unitOfWork.countryRepository.GetAll();

            List<string> countryList = new List<string>();

            foreach (var country in countries)
            {
                countryList.Add(country.Name);
            }

            ViewBag.countriesListItem = new SelectList(countryList);


            return View(author);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Update(Author author)
        {
            if (author == null) 
            {
                return BadRequest();
            }
            if (!ModelState.IsValid) 
            {
                return BadRequest();
            }
            // when update author .. the author country we will added should be found in database.. 
            Country country = await _unitOfWork.countryRepository.Get(filter: x => x.Name.ToLower() == author.Country.Name.ToLower());
            if (country == null)
            {
                return NotFound("country does't exists");
            }

            author.Country = country;

            _unitOfWork.authorRepository.Update(author);
            await _unitOfWork.save();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == 0 || id == null) 
            {
                return BadRequest();
            }
            Author author = await _unitOfWork.authorRepository.Get(filter:x=>x.Id == id);
            
            _unitOfWork.authorRepository.Delete(author);
            await _unitOfWork.save();
            return RedirectToAction("Index");
        }
    }
}
