using DataAccessLayer.Repositories;
using DTOLayer.DTOs.BookDtos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }


        [AllowAnonymous]
        //[Authorize(Roles = "Yazar,Kullanıcı")]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("BooksController çalışıyor");
        }


        // Tüm kitapları getir

        //[Authorize(Roles = "Yazar,Kullanıcı")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Yazar,Kullanıcı")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var books = await _bookRepository.GetAllAsync();
            return Ok(books);
        }

        // Belirli bir kitap ID'ye göre getir

        //[Authorize(Roles = "Yazar,Kullanıcı")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Yazar,Kullanıcı")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
                return NotFound("Kitap bulunamadı.");
            return Ok(book);
        }

        // Belirli bir yazarın kitaplarını getir

        //[Authorize(Roles = "Yazar,Kullanıcı")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Yazar,Kullanıcı")]
        [HttpGet("GetByAuthorId/{authorId}")]
        public async Task<IActionResult> GetByAuthorId(int authorId)
        {
            var books = await _bookRepository.GetBooksByAuthorIdAsync(authorId);
            return Ok(books);
        }

        // Kitapları kategoriye göre grupla

        //[Authorize(Roles = "Yazar,Kullanıcı")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Yazar,Kullanıcı")]
        [HttpGet("GetGroupedByCategory")]
        public async Task<IActionResult> GetGroupedByCategory()
        {
            var groupedBooks = await _bookRepository.GetBooksGroupedByCategoryAsync();
            var result = groupedBooks.Select(g => new
            {
                Category = g.Key,
                Books = g.ToList()
            });
            return Ok(result);
        }

        // En uzun kitabı getir

        //[Authorize(Roles = "Yazar,Kullanıcı")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Yazar,Kullanıcı")]
        [HttpGet("GetLongestBook")]
        public async Task<IActionResult> GetLongestBook()
        {
            var book = await _bookRepository.GetLongestBookAsync();
            return Ok(book);
        }


        //[Authorize(Roles = "Yazar")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Yazar")]
        [HttpPost]
        public async Task<IActionResult> Add(Book book)
        {
            // Kitap ekleme işlemi
            await _bookRepository.AddAsync(book);
            await _bookRepository.SaveAsync();

            return Ok("Kitap başarıyla eklendi");
        }

        //[Authorize(Roles = "Yazar")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Yazar")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Book book)
        {

            _bookRepository.Update(book);
            await _bookRepository.SaveAsync();

            return Ok("Güncelleme işlemi başarılı");
        }

        // Kitap sil
        //[Authorize(Roles = "Yazar")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Yazar")]
        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
                return NotFound("Silinecek kitap bulunamadı.");

            _bookRepository.Delete(book);
            await _bookRepository.SaveAsync();

            return Ok("Kitap Silindi");
        }
    }
}


       
      
       