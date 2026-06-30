using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string NameUz { get; set; } = string.Empty;
        public string NameRu { get; set; } = string.Empty;
        public string DescriptionUz { get; set; } = string.Empty;
        public string DescriptionRu { get; set;} = string.Empty;
    }

    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
