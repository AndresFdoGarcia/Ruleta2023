using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruleta2023.Domain.Data.Ruleta
{
    public class RouletteClass
    {
        public string Id { get; set; }
        public string State { get; set; }
        public List<string> Transaccions { get; set; }
    }
}
