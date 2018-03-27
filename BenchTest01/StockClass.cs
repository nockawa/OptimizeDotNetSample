using System.Collections.Generic;

namespace BenchTest01
{
    public class StockClass
    {
        public StockClass(string name)
        {
            Name = name;
            Trades = new List<TradeClass>();
        }
        public string Name { get;  }

        public IList<TradeClass> Trades { get;  }
    }
    public class TradeClass
    {
        public TradeClass(TradeType type)
        {
            Type = type;
            Tickets = new List<TicketClass>();
        }

        public TradeType Type { get;  }

        public IList<TicketClass> Tickets { get;  }
    }

    public class TicketClass
    {
        public TicketClass(int quantity, float unitPrice)
        {
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public int Quantity { get;  }
        public float UnitPrice { get;  }
    }
}