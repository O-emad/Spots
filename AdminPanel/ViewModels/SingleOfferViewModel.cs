using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.ViewModels
{
    public class SingleOfferViewModel
    {
        public Offer Offer { get; set; }
        public SingleOfferViewModel(Offer offer)
        {
            Offer = offer;
        }
    }
}
