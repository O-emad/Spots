using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Domain
{
    public class Offer
    {
        public double Value { get; set; }
        public ValueType Type { get; set; }
        public string Description { get; set; }

    }

    public enum ValueType
    {
        Value = 0,
        Percentage
    }
}
