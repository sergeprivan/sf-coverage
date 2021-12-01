using Nager.Date;
using Nager.Date.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Coverage // Note: actual namespace depends on the project name.
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var countryCodes = new List<string>() { "FR", "RO", "EG", "PT" };
            var countryEngsDictonary = new Dictionary<string, int>() { { "RO", 28 }, { "PT", 31 }, { "EG", 3 }, { "FR", 1 } };
            var publicHolidayEngsDictonary = new Dictionary<DateTime, int>();
            var maxEngs = countryEngsDictonary.Sum(x => x.Value);

            foreach (var countryCode in countryCodes)
            {
                var publicHolidays = DateSystem.GetPublicHolidays(2021, countryCode);

                foreach (var publicHoliday in publicHolidays)
                {
                    if (!publicHolidayEngsDictonary.ContainsKey(publicHoliday.Date))
                    {
                        publicHolidayEngsDictonary.Add(publicHoliday.Date, maxEngs - countryEngsDictonary[countryCode]);
                    }
                    else
                    {
                        publicHolidayEngsDictonary[publicHoliday.Date] -= countryEngsDictonary[countryCode];
                    }
                }
            }

            var keys = publicHolidayEngsDictonary.Keys.ToList();

            keys.Sort();

            foreach (var key in keys)
            {
                var publicHolidayEng = publicHolidayEngsDictonary.FirstOrDefault(x => x.Key == key);
                var value = publicHolidayEng.Value * 100.0 / maxEngs;

                if (value < 50)
                {
                    Console.WriteLine("Possible coverage issue on {0} with {1} engs out of {2}", publicHolidayEng.Key.ToLongDateString(), publicHolidayEng.Value, maxEngs);
                }

            }

            Console.ReadKey();
        }
    }
}