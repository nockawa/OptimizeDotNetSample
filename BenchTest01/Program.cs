using System;
using System.Collections.Generic;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace BenchTest01
{
    public class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            var b = new Benchmark();
            b.Setup();
            b.ComputeAveragePriceOnClass();
            b.ComputeAveragePriceOnStruct();
#else
            var summary = BenchmarkRunner.Run<Benchmark>();
#endif
        }
    }
    [MemoryDiagnoser]
    public class Benchmark
    {
        [GlobalSetup]
        public void Setup()
        {
            const int tradeCount = 50;

            var stockClass = new StockClass("A Stock as Class");
            _stockAsStruct = new StockStruct("A Stock as Struct");

            var buyCurrentQuantity = 0;
            var sellCurrentQuantity = 0;
            var totalBuy = 0;
            var totalSell = 0;

            var r = new Random(DateTime.UtcNow.Millisecond);

            for (int i = 0; i < tradeCount; i++)
            {
                var quantity = r.Next(1, 1000);
                var type = r.Next(1, 3);

                if (type == 2 && (sellCurrentQuantity + quantity) > buyCurrentQuantity)
                {
                    type = 1;
                }

                switch (type)
                {
                    case 1:
                        buyCurrentQuantity += quantity;
                        ++totalBuy;
                        break;
                    case 2:
                        sellCurrentQuantity += quantity;
                        ++totalSell;
                        break;
                }

                // Generate Class Part
                var tradeClass = new TradeClass((TradeType) type);
                stockClass.Trades.Add(tradeClass);

                var curQuantity = 0;
                while (curQuantity < quantity)
                {
                    var price = 3.2f + (float)r.NextDouble()/2.0f;
                    var ticketQuantity = r.Next(Math.Max(quantity / 10, 1), Math.Max(quantity / 4, 2));

                    ticketQuantity = Math.Min(ticketQuantity, quantity - curQuantity);
                    var ticket = new TicketClass(ticketQuantity, price);
                    
                    tradeClass.Tickets.Add(ticket);
                    curQuantity += ticketQuantity;
                }

                // Generate Struct part
                var tradeStruct = new TradeStruct((TradeType) type);
                for (int j = 0; j < tradeClass.Tickets.Count; j++)
                {
                    var tradeTicket = tradeClass.Tickets[j];
                    var ticket = new TicketStruct(tradeTicket.Quantity, tradeTicket.UnitPrice);
                    tradeStruct.AddTicket(ref ticket);
                }

                _stockAsStruct.AddTrade(ref tradeStruct);
            }

            Debug.WriteLine($"Total Buy: {totalBuy}, Total Sell: {totalSell}");

            _stockAsClass = stockClass;
        }

        [Benchmark(Baseline = true)]
        public (float, float) ComputeAveragePriceOnClass()
        {
            var buyTotalQuantity = 0;
            var sellTotalQuantity = 0;
            var buyTotalPrice = 0.0f;
            var sellTotalPrice = 0.0f;

            foreach (var trade in _stockAsClass.Trades)
            {
                foreach (var ticket in trade.Tickets)
                {
                    if (trade.Type == TradeType.Buy)
                    {
                        buyTotalPrice += ticket.Quantity * ticket.UnitPrice;
                        buyTotalQuantity += ticket.Quantity;
                    }
                    else
                    {
                        sellTotalPrice += ticket.Quantity * ticket.UnitPrice;
                        sellTotalQuantity += ticket.Quantity;
                    }
                }
            }

            var averageBuyPrice = buyTotalPrice / buyTotalQuantity;
            var averageSellPrice = sellTotalPrice / sellTotalQuantity;

            Debug.WriteLine($"Average Buy Price: {averageBuyPrice}, Average Sell Price: {averageSellPrice}");
            return (averageBuyPrice, averageSellPrice);

        }

        [Benchmark]
        public (float, float) ComputeAveragePriceOnStruct()
        {
            var buyTotalQuantity = 0;
            var sellTotalQuantity = 0;
            var buyTotalPrice = 0.0f;
            var sellTotalPrice = 0.0f;

            for (int i = 0; i < _stockAsStruct.TradeCount; i++)
            {
                ref var trade = ref _stockAsStruct.GetTrade(i);

                for (int j = 0; j < trade.TicketCount; j++)
                {
                    ref var ticket = ref trade.GetTicket(j);

                    if (trade.Type == TradeType.Buy)
                    {
                        buyTotalPrice += ticket.Quantity * ticket.UnitPrice;
                        buyTotalQuantity += ticket.Quantity;
                    }
                    else
                    {
                        sellTotalPrice += ticket.Quantity * ticket.UnitPrice;
                        sellTotalQuantity += ticket.Quantity;
                    }
                }
            }
            var averageBuyPrice = buyTotalPrice / buyTotalQuantity;
            var averageSellPrice = sellTotalPrice / sellTotalQuantity;

            Debug.WriteLine($"Average Buy Price: {averageBuyPrice}, Average Sell Price: {averageSellPrice}");
            return (averageBuyPrice, averageSellPrice);
        }

        [Benchmark]
        public (float, float) ComputeAveragePriceOnStructNoRef()
        {
            var buyTotalQuantity = 0;
            var sellTotalQuantity = 0;
            var buyTotalPrice = 0.0f;
            var sellTotalPrice = 0.0f;

            for (int i = 0; i < _stockAsStruct.TradeCount; i++)
            {
                var trade = _stockAsStruct.GetTrade(i);

                for (int j = 0; j < trade.TicketCount; j++)
                {
                    var ticket = trade.GetTicket(j);

                    if (trade.Type == TradeType.Buy)
                    {
                        buyTotalPrice += ticket.Quantity * ticket.UnitPrice;
                        buyTotalQuantity += ticket.Quantity;
                    }
                    else
                    {
                        sellTotalPrice += ticket.Quantity * ticket.UnitPrice;
                        sellTotalQuantity += ticket.Quantity;
                    }
                }
            }
            var averageBuyPrice = buyTotalPrice / buyTotalQuantity;
            var averageSellPrice = sellTotalPrice / sellTotalQuantity;

            Debug.WriteLine($"Average Buy Price: {averageBuyPrice}, Average Sell Price: {averageSellPrice}");
            return (averageBuyPrice, averageSellPrice);
        }

        private StockClass _stockAsClass;
        private StockStruct _stockAsStruct;
    }
}
