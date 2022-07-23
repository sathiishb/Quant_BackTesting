using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quant.BackTesting.PriceNetural
{
    public static class Extensions
    {
        public static decimal GetPrice(this List<DailyValues> currentDay, TimeSpan time)
        {
            var data = currentDay
                .Where(s => s.Time.TimeOfDay == time)
                .ToList();
            if (data.Any() == false)
            {
                for (int i = 0; i < 10; i++)
                {
                    time = time.Add(new TimeSpan(0, 1, 0));
                    data = currentDay
                    .Where(s => s.Time.TimeOfDay == time)
                    .ToList();
                    if (data.Any())
                    {
                        break;
                    }
                }
            }




            return data
                .FirstOrDefault()
                .Open;
        }
    }
}
