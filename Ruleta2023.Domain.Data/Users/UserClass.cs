using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruleta2023.Domain.Data.Users
{
    public class UserClass
    {
        public string Id { get; set; }
        public string Role { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserNickName { get; set; }
        public List<int> ActivePermissions { get; set; }
        public string idToken { get; set; }
        public bool IsActive { get; set; }
        public int MoneyClient { get; set; }
    }
}
