using Ruleta2023.Domain.Data.Ruleta;
using Ruleta2023.Domain.Data.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruleta2023.Data.Access.MongoDb.RouletteConfiguration.Contract
{
    public interface IRouletteConfigurationManager
    {
        Task<RouletteClass> GetClient(string id);
        Task Save(RouletteClass entity);
        Task Update(RouletteClass entity);
        Task Delete(string id);
    }
}
