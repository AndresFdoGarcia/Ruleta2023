using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruleta2023.Domain.Data.Bets
{
    public class BetClass
    {
        public string Id { get; set; }
        public string ClientId { get; set; }        
        public int MoneyBet { get; set; }
        public TypeBet typeBet { get; set; }
        public BetData BetDone { get; set; }
    }

    public enum TypeBet
    {
        COLOR,
        NUMBER
    }

    public class BetData
    {
        public int NumberSelected { get; set; }
        public string ColorSelected { get; set; }
    }
}
