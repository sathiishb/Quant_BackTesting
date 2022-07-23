using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quant.BackTesting.Test
{

    [TestClass]
    public class ExpiryDate
    {
        [TestMethod]
        public void TestMethod1()
        {
            var dates = GetExpiryDate();
            var convertToString = dates.Select(s => s.ToString("ddMMMyyyy")).ToList();
            var dat = dates;
        }


        public static List<DateTime> GetExpiryDate()
        {
            var date = DateTime.Now;
            var thursday = new List<DateTime>();
            for (int i = 0; i <= 30; i++)
            {
                if (date.DayOfWeek == DayOfWeek.Thursday)
                {
                    thursday.Add(date);
                }
                date = date.Date.AddDays(1);
            }
            return thursday;
        }

        //public static List<DateTime> GetExpiryDate()
        //{
        //    var thursday = new List<DateTime>();
        //    var date = DateTime.Now.Date;
            
        //    while (DateTime.Now.Month >= date.Month)
        //    {
        //        if (date.DayOfWeek == DayOfWeek.Thursday)
        //        {
        //            thursday.Add(date);
        //        }
        //        date=date.Date.AddDays(1);
        //    }
        //    return thursday;
        //}

    }


}
