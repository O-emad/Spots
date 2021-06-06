using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.DTO
{
    public class CategoryForUpdateDto
    {

        public string Name { get; set; }
        public string NameAR { get; set; }
        public int SortOrder { get; set; }
        public byte[] Bytes { get; set; }
        public Guid CategoryId { get; set; }
    }
}
