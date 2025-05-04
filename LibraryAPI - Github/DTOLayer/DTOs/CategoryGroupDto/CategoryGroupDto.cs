using DTOLayer.DTOs.BookDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOLayer.DTOs.CategoryGroupDto
{
    public class CategoryGroupDto
    {
        public string Category { get; set; }
        public List<BookDto> Books { get; set; }
    }
}
