using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string NameRu { get; set; } = string.Empty;
        public string NameUz {  get; set; } = string.Empty;
        public string DescriptionRu { get; set; } = string.Empty;
        public string DescriptionUz { get; set;} = string.Empty;
    }
}
