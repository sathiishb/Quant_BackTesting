using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Quant.BackTesting.PriceNetural
{
    public class ReadFromCSV
    {
        public List<DailyValues> Read(string path)
        {
            List<DailyValues> values = File.ReadAllLines(path)
                                           .Select(v => DailyValues.FromCsv(v))
                                           .ToList();

            return values;
        }

        public List<DailyValues> ReadByDate(string path, DateTime date)
        {
            return Read(path)
                 .Where(s => s.Date == date.Date)
                    .ToList(); ;
        }
    }

    public class DailyValues
    {
        public string Symbol;
        public DateTime Date;
        public DateTime Time;
        public decimal Open;
        public decimal High;
        public decimal Low;
        public decimal Close;
        public decimal Volume;
        public decimal AdjClose;

        public static DailyValues FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');

            DailyValues dailyValues = new DailyValues
            {
                Symbol = values[0].ToString(),
                Date = ConvertToDateTime(values[1]),
                Time = DateTime.ParseExact(values[2], "HH:mm", CultureInfo.InvariantCulture),
                Open = Convert.ToDecimal(values[3]),
                High = Convert.ToDecimal(values[4]),
                Low = Convert.ToDecimal(values[5]),
                Close = Convert.ToDecimal(values[6])
            };

            return dailyValues;
        }

        static DateTime ConvertToDateTime(string dateTime)
        {
            var isValid = DateTime.TryParse(dateTime, out DateTime convertedDate);
            if (isValid) return convertedDate;


            var year = Split(dateTime, 4);
            var date = Split(year[1], 2);
            return new DateTime(int.Parse(year[0]), int.Parse(date[0]), int.Parse(date[1]));
        }

        static IList<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize)).ToList();
        }
    }
}
