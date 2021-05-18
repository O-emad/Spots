using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Models
{
    public class DeserializedResponseModel<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<T> Data { get; set; } = new List<T>();
    }
}
