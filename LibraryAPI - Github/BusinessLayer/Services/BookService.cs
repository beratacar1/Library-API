using AutoMapper;
using DataAccessLayer.Repositories;
using DTOLayer.DTOs.BookDtos;
using DTOLayer.DTOs.CategoryGroupDto;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class BookService
    {
        private readonly IBookRepository _bookRepository; // kitaplara ait veri işlemleri için kullanılır
        private readonly IMapper _mapper; // entity - dto dönüşümlerini sağlar

        public async Task<List<BookDto>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            return _mapper.Map<List<BookDto>>(books);
        }

        public BookService(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<List<BookDto>> GetBooksByAuthorId(int authorId)
        {
            var books = await _bookRepository.GetBooksByAuthorIdAsync(authorId);
            return _mapper.Map<List<BookDto>>(books);
        }

        public async Task<BookDto> GetLongestBook()
        {
            var book = await _bookRepository.GetLongestBookAsync();
            return _mapper.Map<BookDto>(book);
        }

        public async Task<List<CategoryGroupDto>> GetBooksGroupedByCategory()
        {
            var grouped = await _bookRepository.GetBooksGroupedByCategoryAsync(); // Kitapları kategoriye göre gruplayıp CategoryGroupDto listesine çevirir
            return grouped.Select(g => new CategoryGroupDto
            {
                Category = g.Key, // kategori adı
                Books = _mapper.Map<List<BookDto>>(g.ToList()) // kategoriye ait kitaplar
            }).ToList();
        }

        public async Task AddBook(BookDto dto)
        {
            var book = _mapper.Map<Book>(dto); // dto'yu entitye çevirme işlemi
            await _bookRepository.AddAsync(book);
            await _bookRepository.SaveAsync();
        }

        public async Task UpdateBook(BookDto dto)
        {
            var book = _mapper.Map<Book>(dto); // dto dan entity oluşturup kitabı günceller
            _bookRepository.Update(book);
            await _bookRepository.SaveAsync();
        }

        public async Task DeleteBook(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            _bookRepository.Delete(book);
            await _bookRepository.SaveAsync();
        }
    }
}

 

    
   