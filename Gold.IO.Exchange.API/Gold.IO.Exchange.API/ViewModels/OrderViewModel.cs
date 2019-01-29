﻿using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.Domain.Enum;
using System;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class OrderViewModel
    {
        public long ID { get; set; }
        public CoinViewModel BaseAsset { get; set; }
        public CoinViewModel QuoteAsset { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
        public OrderType Type { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime Time { get; set; }

        public OrderViewModel() { }

        public OrderViewModel(Order order)
        {
            ID = order.ID;
            BaseAsset = new CoinViewModel(order.BaseAsset);
            QuoteAsset = new CoinViewModel(order.QuoteAsset);
            Amount = order.Amount;
            Price = order.Price;
            Type = order.Type;
            Status = order.Status;
            Time = order.Time;
        }
    }
}
