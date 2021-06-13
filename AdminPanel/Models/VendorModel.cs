using AdminPanel.Models.Category;
using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Models
{
    public sealed class VendorModel
    {
        VendorModel()
        {

        }
        private static readonly object _lock = new object();
        private static VendorModel instance = null;
        public static VendorModel Instance
        {
            get
            {
                lock (_lock)
                {
                    if(instance == null)
                    {
                        instance = new VendorModel();
                    }
                    return instance;
                }
            }
        }
        public List<CategoryModel> Categories { get; set; }
    }
}
