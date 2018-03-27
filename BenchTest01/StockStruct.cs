namespace BenchTest01
{
    public enum TradeType
    {
        Buy  = 1,
        Sell = 2
    }

    public struct StockStruct
    {
        public StockStruct(string name)
        {
            Name = name;
            _tradeArray = new RefArray<TradeStruct>();
        }

        public string Name { get;  }

        private readonly RefArray<TradeStruct> _tradeArray;
        
        public int TradeCount => _tradeArray.Count;

        public ref TradeStruct GetTrade(int index)
        {
            return ref _tradeArray[index];
        }

        public int AddTrade(ref TradeStruct trade)
        {
            return _tradeArray.Add(ref trade);
        }
    }
    public struct TradeStruct
    {
        public TradeStruct(TradeType type)
        {
            Type = type;
            _tickets = new RefArray<TicketStruct>(8);
            _tmp = new int[1024];
        }

        public TradeType Type { get;  }

        public int AddTicket(ref TicketStruct ticket)
        {
            return _tickets.Add(ref ticket);
        }

        public int TicketCount => _tickets.Count;

        public ref TicketStruct GetTicket(int index)
        {
            return ref _tickets[index];
        }

        private readonly RefArray<TicketStruct> _tickets;

        private int[] _tmp;
    }

    public struct TicketStruct
    {
        public TicketStruct(int quantity, float unitPrice)
        {
            Quantity = quantity;
            UnitPrice = unitPrice;
            _tmp = new int[1024];
        }

        public int Quantity { get;  }
        public float UnitPrice { get;  }

        private int[] _tmp;
    }
}