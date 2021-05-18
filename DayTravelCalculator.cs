using System;
using System.Collections.Generic;
using System.Linq;

namespace DistanceDrivenCalc
{
    internal class DayTravelCalculator
    {
        public DayTravelCalculator()
        {
        }

        internal DayTravelDict Execute(List<TravelItem> travelItems)
        {
            var result = new DayTravelDict();
            var groups = travelItems
                    .GroupBy(t => t.Date)
                    .Select(dl => (date: dl.Key, travel: dl.ToList()));

            foreach(var group in groups)
            {
                result.Add(group.date, group.travel);
            }

            return result;
        }
    }
}