using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int PageCount { get; set; }
        public int PublishYear { get; set; }
        public string Category { get; set; }

        public int AuthorId { get; set; }

        public Author Author { get; set; }
    }
}
