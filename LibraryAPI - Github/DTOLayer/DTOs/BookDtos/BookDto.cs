using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOLayer.DTOs.BookDtos
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int PageCount { get; set; }
        public int PublishYear { get; set; }
        public string Category { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
    }
}

 
  
