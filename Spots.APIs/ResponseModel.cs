using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.APIs
{
    public class ResponseModel
    {
        public ResponseModel()
        {

        }

        public ResponseModel(int statusCode, string message, object value)
        {
            StatusCode = statusCode;
            Message = message;
            Data = value;
        }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; } = new { };
    }
}
