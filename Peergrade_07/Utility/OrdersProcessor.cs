using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Routing.Matching;

namespace IHW4
{
    public class OrdersProcessor
    {
        private List<long> orderIds = new List<long>();
        
        public void Process()
        {
            while (true)
            {
                orderIds = OrderManager.GetAwaitingOrders();

                Thread.Sleep(50000);

                while (orderIds.Count != 0)
                {
                    OrderManager.UpdateOrderState(orderIds[0], "выполнен");
                    orderIds.RemoveAt(0);
                }
            }
        }

        ~OrdersProcessor()
        {
            foreach (var id in orderIds)
            {
                OrderManager.UpdateOrderState(id, "отменен");
            }
        }
    }
}