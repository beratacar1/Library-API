using DataAccessLayer.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataAccessLayer.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryDbContext _context;

        public BookRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Book entity)
        {
            await _context.Books.AddAsync(entity);
        }

        public void Delete(Book entity)
        {
            _context.Books.Remove(entity);
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await _context.Books.Include(b => b.Author).ToListAsync(); // Tüm kitapları yazar bilgileriyle birlikte getirir, Include ile yazar(author) bilgisi ekleniir.
        }

        public async Task<List<Book>> GetBooksByAuthorIdAsync(int authorId) // belirli bir yazara
        {
            return await _context.Books
                .Include(b => b.Author)
                .Where(b => b.AuthorId == authorId)
                .ToListAsync();
        }

        public async Task<List<IGrouping<string, Book>>> GetBooksGroupedByCategoryAsync() // Kitapları kategorisine göre gruplar
        {
            var books = await _context.Books.Include(b => b.Author).ToListAsync();
            return books.GroupBy(b => b.Category).ToList();
        }

        public async Task<Book> GetByIdAsync(int id) // Girilen id'ye göre kitapları getirir
        {
            return await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id); // Yazar bilgiside dahil edilir
        }

        public async Task<Book> GetLongestBookAsync() // Sayfa sayısı en fazla olan kitabı getirir
        {
            return await _context.Books
                .Include(b => b.Author)
                .OrderByDescending(b => b.PageCount) // Önce azalan şekilde sıralar 
                .FirstOrDefaultAsync(); // Sonrada ilk kitabı alır
        }

        public async Task SaveAsync() // Yapılan tüm değişiklikleri veri tabanında kaydeder.
        {
            await _context.SaveChangesAsync();

        }

        public void Update(Book entity)
        {
            _context.Books.Update(entity);
        }

            

        //public async Task AddAsync(Book entity)
        //{
        //    await _context.Books.AddAsync(entity);
        //}

        //public void Delete(Book entity)
        //{
        //    _context.Books.Remove(entity);
        //}

        //public async Task<List<Book>> GetAllAsync()
        //{
        //    return await _context.Books.Include(b => b.Author).ToListAsync();
        //}

        //public async Task<Book> GetByIdAsync(int id)
        //{
        //    return await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id);
        //}

        //public async Task<List<Book>> GetBooksByAuthorIdAsync(int authorId)
        //{
        //    return await _context.Books
        //        .Include(b => b.Author)
        //        .Where(b => b.AuthorId == authorId)
        //        .ToListAsync();
        //}

        //public async Task<Book> GetLongestBookAsync()
        //{
        //    return await _context.Books
        //        .Include(b => b.Author)
        //        .OrderByDescending(b => b.PageCount)
        //        .FirstOrDefaultAsync();
        //}

        //public async Task<List<IGrouping<string, Book>>> GetBooksGroupedByCategoryAsync()
        //{
        //    var books = await _context.Books.Include(b => b.Author).ToListAsync();
        //    return books.GroupBy(b => b.Category).ToList();
        //}

        //public async Task SaveAsync() => await _context.SaveChangesAsync();

        //public void Update(Book entity)
        //{
        //    _context.Books.Update(entity);
        //}

    }
}
