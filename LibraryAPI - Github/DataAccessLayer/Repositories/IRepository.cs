
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface IRepository<T> where T : class
    {
        // T türü farklı türlerle çalışmayı sağlar  , bu arayüzü hem kitap hemde yazar türünde yeniden yazamadan kullanabiliriz 
        // Metotların imzası burda yer alır
        Task<List<Book>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveAsync();
    }
}
