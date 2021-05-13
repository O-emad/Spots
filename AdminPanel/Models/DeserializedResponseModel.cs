using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Models
{
    public class DeserializedResponseModel
    {
            public int StatusCode { get; set; }
            public string Message { get; set; }
            public Category[] Data { get; set; }
    }
}
