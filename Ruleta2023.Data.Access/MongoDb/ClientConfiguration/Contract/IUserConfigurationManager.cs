using Ruleta2023.Domain.Data.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruleta2023.Data.Access.MongoDb.ClientConfiguration.Contract
{
    public interface IUserConfigurationManager
    {
        Task<UserClass> GetClient(string id);
        Task Save(UserClass entity);
        Task Update(UserClass entity);
        Task Delete(string id);
    }
}
