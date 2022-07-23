using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Quant.BackTesting.PriceNetural.Epplus;


namespace Quant.BackTesting.PriceNetural
{
    class Program
    {
        static void Main(string[] args)
        {

            var month = 0;
            var startDate = new DateTime(2019, 01, 01);
            var endDate = new DateTime(2019, 09, 30);
            var year = startDate.Year;
            var reportDays = GetDatesBetween(startDate, endDate);

            var expiryData = GetExpiryData(year);
            var position = new List<PositionModel>();
            foreach (var date in reportDays)
            {
                bool isWeeklyExpiry = FindIsWeeklyExpiry(date, expiryData);
                List<DailyValues> spotPriceData = null;
                
                if (month != date.Month)
                {
                    spotPriceData = GetSpotDataByMonth(date);
                }

                var folderName = ExpiryFolderName(date, expiryData);

                var currentDay = spotPriceData.Where(s => s.Date == date.Date)
                    .ToList();
                if (currentDay.Any() == false) continue;

                var time = new TimeSpan(09, 30, 00);
                var currentSpotPrice = currentDay.GetPrice(time);
                if (currentSpotPrice == 0)
                {
                    break;
                }
                UpdateBound(out decimal upperBound, out decimal lowerBound, currentSpotPrice);

                var strikePrice = IdentifyATMStrike(currentSpotPrice, date, folderName, time, isWeeklyExpiry);

                var ceinformation = GetData(strikePrice, date, folderName, Constant.CE, isWeeklyExpiry);
                var peinformation = GetData(strikePrice, date, folderName, Constant.PE, isWeeklyExpiry);



                var ceSell = ceinformation.data.GetPrice(time);
                var peSell = peinformation.data.GetPrice(time);
                var spotLtp = currentDay.GetPrice(time);
                var entrySpot = currentDay.GetPrice(time);
                decimal pnl = 0;
                while (time < new TimeSpan(15, 0, 0))
                {


                    //var cePnl = ceSell - ceinformation.data.GetPrice(time);
                    //var pePnl = peSell - peinformation.data.GetPrice(time);
                    //var mtm = (pePnl + cePnl) * 25;
                    //var maxLoss = pnl + mtm;
                    spotLtp = currentDay.GetPrice(time);

                    //if (pnl < -8000)
                    //{
                    //    position.Add(ExitPosition(date, time, ceinformation.fileName, spotLtp, ceinformation.data.GetPrice(time), peSell));
                    //    position.Add(ExitPosition(date, time, ceinformation.fileName, spotLtp, ceinformation.data.GetPrice(time), ceSell));
                    //    isPnlSlHit = true;
                    //    break;
                    //}
                    if (spotLtp > upperBound || spotLtp < lowerBound)
                    {
                        var exitPe = ExitPosition(date, time, peinformation.fileName, spotLtp, peinformation.data.GetPrice(time), peSell);
                        pnl += exitPe.PNL;
                        position.Add(exitPe);

                        var exitCe = ExitPosition(date, time, ceinformation.fileName, spotLtp, ceinformation.data.GetPrice(time), ceSell);
                        pnl += exitCe.PNL;
                        position.Add(exitCe);
                        //if (pnl < -8000)
                        //{
                        //    isPnlSlHit = true;
                        //    break;

                        //}

                        var atmStrike = IdentifyATMStrike(spotLtp, date, folderName, time, isWeeklyExpiry);
                        ceinformation = GetData(atmStrike, date, folderName, Constant.CE, isWeeklyExpiry);
                        peinformation = GetData(atmStrike, date, folderName, Constant.PE, isWeeklyExpiry);


                        ceSell = ceinformation.data.GetPrice(time);
                        peSell = peinformation.data.GetPrice(time);
                        UpdateBound(out upperBound, out lowerBound, spotLtp);
                        //var ceLTP = ceinformation.data.GetPrice(time);
                        //roundPrice = RoundSpot(spotLtp);


                        //peinformation.data = FindMatchingStrike(folderName, date, time, spotLtp, ceLTP, Constant.PE);
                        //peSell = ceinformation.data.GetPrice(time);
                    }
                    //else if (spotLtp < lowerBound)
                    //{
                    //    var exitPosition = ExitPosition(date, time, ceinformation.fileName, spotLtp, ceinformation.data.GetPrice(time), ceSell);
                    //    pnl += exitPosition.PNL;
                    //    position.Add(exitPosition);
                    //    var peLTP = peinformation.data.GetPrice(time);
                    //    roundPrice = RoundSpot(spotLtp);
                    //    UpdateBound(out upperBound, out lowerBound, spotLtp);
                    //    ceinformation.data = FindMatchingStrike(folderName, date, time, spotLtp, peLTP, Constant.CE);
                    //    ceSell = ceinformation.data.GetPrice(time);
                    //}
                    time = time.Add(new TimeSpan(0, 5, 0));
                }


                position.Add(ExitPosition(date, time, peinformation.fileName, spotLtp, peinformation.data.GetPrice(time), peSell));
                position.Add(ExitPosition(date, time, ceinformation.fileName, spotLtp, ceinformation.data.GetPrice(time), ceSell));



            }

            var group = position.GroupBy(s => s.DateTime.Date)
                .Select(s => new PositionModel()
                {
                    DateTime = s.Key,
                    PNL = s.Sum(x => x.PNL),
                    BuyPrice = s.Count()


                }).ToList();
            ExcelConvertor.ExportToExcel(position, @"C:\Satz\Market-Data\BackTesting\OptionSellingWOSL", "Test");
            ExcelConvertor.ExportToExcel(group, @"C:\Satz\Market-Data\BackTesting\OptionSellingWOSL", "Test2");
        }

        private static bool FindIsWeeklyExpiry(DateTime date, List<DateTime> expiryData)
        {
            var expiryDay = expiryData
                .Where(s => s >= date)
                .First();
            var count = expiryData.Where(s => s >= date && s.Month == expiryDay.Month).Count();

            return count > 1;
        }

        private static List<DateTime> GetExpiryData(int year)
        {
            var path = $@"{Constant.BasePath}\{year}\expiry.csv";
            var data = File.ReadAllLines(path);
            string[] values = data[0].Split(',');
            return values.Select(s => Convert.ToDateTime(s)).ToList();
        }

        static string ExpiryFolderName(DateTime currentDate, List<DateTime> expiryData)
        {
            var expiryDay = expiryData.Where(s => s >= currentDate)
                .OrderBy(s => s)
                .First();
            var suffix = GetDaySuffix(expiryDay.Day);
            var folder = $"Expiry {expiryDay:dd}{suffix} {expiryDay:MMMM}";
            folder = $@"{expiryDay.Year}\{folder}\csv\";
            return folder;
        }

        static string GetDaySuffix(int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }

        private static List<DailyValues> GetSpotDataByMonth(DateTime date)
        {
            var month = date.ToString("MMM");
            var fileName = $@"{month}\BANKNIFTY.txt";
            var path = $"{Constant.BasePath}\\{date.Year}\\{fileName}";
            var spotPriceData = new ReadFromCSV().Read(path);
            return spotPriceData;
        }

        public static decimal IdentifyATMStrike(decimal ltp, DateTime date, string folderName, TimeSpan time, bool isWeekly)
        {
            var strikePrice = RoundSpot(ltp);

            var ceData = GetData(strikePrice, date, folderName, Constant.CE, isWeekly).data;
            var peData = GetData(strikePrice, date, folderName, Constant.PE, isWeekly).data;
            //Implied Futures
            //Strikeprice+Ce Premium-PE PRemium
            //Round off
            var futureLtp = strikePrice + ceData.GetPrice(time) - peData.GetPrice(time);

            return RoundSpot(futureLtp);
        }


        public static (List<DailyValues> data, string fileName) GetData(decimal strikePrice,
            DateTime date,
            string folderName,
            string type,
            bool isWeeklyExpiry)
        {
            var path = GetPath(strikePrice, folderName, type, isWeeklyExpiry, out string fileName);
            var data = new ReadFromCSV().ReadByDate(path, date);

            return (data, fileName);
        }

        //public static void CreateShortShaddle(decimal currentSpotPrice,string folderName,DateTime date)
        //{
        //    var roundPrice = RoundSpot(currentSpotPrice);
        //    var cepath = GetPath(currentSpotPrice, folderName, Constant.CE, out string ceinformation.fileName);
        //    var pepath = GetPath(currentSpotPrice, folderName, Constant.PE, out string peinformation.fileName);
        //    var ceinformation.data = new ReadFromCSV().ReadByDate(cepath, date);
        //    var peinformation.data = new ReadFromCSV().ReadByDate(pepath, date);

        //    var ceSell = ceinformation.data.GetPrice(time);
        //    var peSell = peinformation.data.GetPrice(time);
        //}

        //private static List<DailyValues> FindMatchingStrike(string folderName,
        //    DateTime date,
        //    TimeSpan time,
        //    decimal spotLtp,
        //    decimal oppositeLegLtp,
        //    string type)
        //{
        //    var data = new List<DailyValues>();
        //    bool foundStrikePrice = false;
        //    var roundPrice = RoundSpot(spotLtp);
        //    while (foundStrikePrice == false)
        //    {

        //        var path = GetPath(roundPrice, folderName, type, out string fileName);
        //        data = new ReadFromCSV().ReadByDate(path, date);
        //        var findltp = data.GetPrice(time);
        //        if (findltp > oppositeLegLtp)
        //        {
        //            foundStrikePrice = true;
        //        }

        //        roundPrice = type == Constant.CE ? roundPrice - 100 : roundPrice + 100;
        //    }

        //    return data;
        //}

        private static string GetPath(decimal strikePrice,
            string folderName,
            string type,
            bool isWeeklyExpiry,
            out string fileName)
        {
            fileName = isWeeklyExpiry ? Constant.FileNameFormat : Constant.FileNameFormat.Replace("WK", "");
            fileName = fileName.Replace("##", strikePrice.ToString());
            fileName = fileName.Replace("**", type);
            folderName += fileName;

            return Constant.BasePath + folderName;
        }

        private static void UpdateBound(out decimal upperBound, out decimal lowerBound, decimal spotLtp)
        {
            var percentage = (spotLtp / 100);// * (decimal)0.50;
            upperBound = Math.Round(spotLtp + percentage);
            lowerBound = Math.Round(spotLtp - percentage);
        }

        private static PositionModel ExitPosition(DateTime date,
            TimeSpan time,
            string symbol,
             decimal spotLtp,
            decimal buyPrice,
            decimal sellPrice
           )
        {
            var exitPosition = new PositionModel()
            {
                DateTime = date.Add(time),
                SpotPrice = spotLtp,
                SellPrice = sellPrice,
                Symbol = symbol,
                BuyPrice = buyPrice
            };
            exitPosition.PNL = (exitPosition.SellPrice - exitPosition.BuyPrice) * 25;
            return exitPosition;
        }

        public static decimal RoundSpot(decimal price)
        {
            return Math.Round(price / 100, 0) * 100;
        }

        public static List<DateTime> GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (!(date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday))
                {
                    allDates.Add(date);
                }

            }

            return allDates;

        }
    }



}
