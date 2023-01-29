using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruleta2023.Data.Access.Redis.RedisCacheClient.Contract
{
    public interface ICacheClient
    {
        Task<string> Get(string key);
        Task Set(string key, string value, int TtlKeyS);
        Task DeleteKey(string key);        
    }
}
