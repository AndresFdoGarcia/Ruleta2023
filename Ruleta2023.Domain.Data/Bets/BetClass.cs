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
        public TypeBet ColorBet { get; set; }
        public int NumberBet { get; set; }
    }

    public enum TypeBet
    {
        RED,
        BLACK
    }
}
