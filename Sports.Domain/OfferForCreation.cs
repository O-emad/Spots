using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Domain
{
    public class OfferForCreation
    {
        public double Value { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int AllowedUses { get; set; }
        public bool Enabled { get; set; }
        public string Description { get; set; }
    }
}
