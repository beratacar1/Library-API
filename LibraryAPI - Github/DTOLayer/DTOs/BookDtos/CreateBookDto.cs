using DTOLayer.DTOs.AuthorDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOLayer.DTOs.BookDtos
{
    public class CreateBookDto
    {
        public string Title { get; set; }
        public int PageCount { get; set; }
        public int PublishYear { get; set; }
        public string Category { get; set; }
        public int AuthorId { get; set; }
        public AuthorDto Author { get; set; }
    }
}
