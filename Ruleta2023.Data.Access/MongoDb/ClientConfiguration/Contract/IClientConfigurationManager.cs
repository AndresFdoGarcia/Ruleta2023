using Ruleta2023.Domain.Data.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruleta2023.Data.Access.MongoDb.ClientConfiguration.Contract
{
    public interface IClientConfigurationManager
    {
        Task<ClientClass> GetClient(string id);
        Task Save(ClientClass entity);
        Task Update(ClientClass entity);
        Task Delete(string id);
    }
}
