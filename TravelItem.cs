using System;

namespace DistanceDrivenCalc
{
    internal class TravelItem
    {
        private Data data;

        public TravelItem(Data data)
        {
            this.data = data;
            Date = DateTime.Parse(data.ArrivalDateStr);
            Name = data.Name;
            StreetAddress = data.Street;
            PostalCode = data.Zip;
            City = data.City;
        }

        public DateTime Date { get; set; }
        public String Name { get; set; }
        public String StreetAddress { get; set; }
        public String PostalCode { get; set; }
        public String City { get; set; }

        public override String ToString()
        {
            return $"({City}) {StreetAddress}";
        }
    }
}