
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface IBookRepository : IRepository<Book> // Kitaplarla ilgili daha özel metodlar yazmak için bu interface'i oluşuturulur
    {
        Task<List<Book>> GetAllAsync();
        Task<List<Book>> GetBooksByAuthorIdAsync(int authorId);
        Task<Book> GetLongestBookAsync();
        Task<List<IGrouping<string, Book>>> GetBooksGroupedByCategoryAsync();
    }
}


    


