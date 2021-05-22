using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Domain
{
    public class SettingForCreation
    {
        [Required]
        public bool AutomaticOfferApproval { get; set; }
    }
}
