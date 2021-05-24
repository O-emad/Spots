using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }

        public string Subject { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool Active { get; set; }

        public List<UserClaim> Claims { get; set; } = new List<UserClaim>();
    }
    public class UserClaim 
    {

        public Guid Id { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }
    }
}
