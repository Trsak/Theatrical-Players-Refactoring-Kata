using System;
using System.Collections.Generic;
using System.Globalization;

namespace TheatricalPlayersRefactoringKata
{
    public class StatementPrinter
    {
        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            var cultureInfo = new CultureInfo("en-US");
            var performanceResults = new Dictionary<Performance, decimal>();

            var (totalAmount, volumeCredits) = CalculatePerformanceAmount(invoice, plays, performanceResults);

            
            return ConstructStringResult(performanceResults, plays, invoice.Customer, totalAmount, volumeCredits);
        }

        private static Tuple<int, int> CalculatePerformanceAmount(Invoice invoice, Dictionary<string, Play> plays,
            Dictionary<Performance, decimal> performanceResults)
        {
            var totalAmount = 0;
            var volumeCredits = 0;
            
            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayID];
                var thisAmount = 0;
                switch (play.Type)
                {
                    case "tragedy":
                        thisAmount = CalculateAmount(40000, perf.Audience, 30, 1000, 0);
                        break;
                    case "comedy":
                        thisAmount = CalculateAmount(30000, perf.Audience, 20, 500, 10000);
                        thisAmount += 300 * perf.Audience;
                        break;
                    default:
                        throw new Exception("unknown type: " + play.Type);
                }

                // add volume credits
                volumeCredits += Math.Max(perf.Audience - 30, 0);
                // add extra credit for every ten comedy attendees
                if ("comedy" == play.Type) volumeCredits += (int) Math.Floor((decimal) perf.Audience / 5);

                performanceResults.Add(perf, Convert.ToDecimal(thisAmount / 100));
                // print line for this order
                totalAmount += thisAmount;
            }

            return new Tuple<int, int>(totalAmount, volumeCredits);
        }

        private static string ConstructStringResult(Dictionary<Performance, decimal> performaceResults, Dictionary<string, Play> plays, String customer, decimal totalAmount, int volumeCredits)
        {
            var cultureInfo = new CultureInfo("en-US");
            var result = string.Format("Statement for {0}\n", customer);

            foreach (var performance in performaceResults)
            {
                var play = plays[performance.Key.PlayID];

                result += string.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", play.Name, performance.Value, performance.Key.Audience);
            }
            
            result += string.Format(cultureInfo, "Amount owed is {0:C}\n", Convert.ToDecimal(totalAmount / 100));
            result += string.Format("You earned {0} credits\n", volumeCredits);

            return result;
        }

        private static int CalculateAmount(int thisAmount, int perfAudience, int audienceLimit, int baseAmount,
            int addition)
        {
            if (perfAudience > audienceLimit)
            {
                thisAmount += addition + baseAmount * (perfAudience - audienceLimit);
            }

            return thisAmount;
        }
    }
}
