using Nager.Date;
using Nager.Date.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Microsoft.Coverage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var countryCodes = new List<string>() { "FR", "RO", "EG", "PT", "DE", "ES", "GB" };
            var countryEngsDictonary = new Dictionary<string, int>() { { "FR", 22 }, { "RO", 6 }, { "EG", 36 }, { "PT", 35 }, { "DE", 1 }, { "ES", 12 }, { "GB", 10 } };
            var publicHolidayEngsDictonary = new Dictionary<DateTime, int>();
            var datePublicHolidayDictonary = new Dictionary<DateTime, List<PublicHoliday>>();
            var maxEngs = countryEngsDictonary.Sum(x => x.Value);

            foreach (var countryCode in countryCodes)
            {
                var publicHolidays = DateSystem.GetPublicHolidays(2023, countryCode);

                foreach (var publicHoliday in publicHolidays)
                {
                    if (publicHoliday.Date.DayOfWeek == DayOfWeek.Friday && countryCode == "EG") // We want to skip EG as they dont work on Friday
                    {
                        continue;
                    }

                    if (!publicHolidayEngsDictonary.ContainsKey(publicHoliday.Date))
                    {
                        publicHolidayEngsDictonary.Add(publicHoliday.Date, maxEngs - countryEngsDictonary[countryCode]);
                        datePublicHolidayDictonary.Add(publicHoliday.Date, new List<PublicHoliday>() { publicHoliday });

                        if (publicHoliday.Date.DayOfWeek == DayOfWeek.Friday) // - EG as they dont work on Friday
                        {
                            publicHolidayEngsDictonary[publicHoliday.Date] -= countryEngsDictonary["EG"];
                        }
                    }
                    else
                    {
                        publicHolidayEngsDictonary[publicHoliday.Date] -= countryEngsDictonary[countryCode];
                        datePublicHolidayDictonary[publicHoliday.Date].Add(publicHoliday);
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
                    if (key.Date.DayOfWeek == DayOfWeek.Sunday && datePublicHolidayDictonary[key].FirstOrDefault(x => x.CountryCode.ToString() == "EG") == null)
                    {
                        Console.WriteLine();
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Possible coverage issue on {0} with {1} engs out of {2}", publicHolidayEng.Key.ToLongDateString(), publicHolidayEng.Value, maxEngs);

                        foreach (var test in datePublicHolidayDictonary[key])
                        {
                            Console.WriteLine("{0}, Public Holiday {1}", test.CountryCode, test.Name);
                        }
                    }

                    if (key.Date.DayOfWeek == DayOfWeek.Friday)
                    {
                        Console.WriteLine("{0}, {1}", "EG", "WE");
                    }

                    Console.WriteLine();
                }

            }

            Console.ReadKey();
        }
    }
}