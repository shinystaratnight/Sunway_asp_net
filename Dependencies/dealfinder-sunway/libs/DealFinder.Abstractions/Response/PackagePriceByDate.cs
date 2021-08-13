namespace DealFinder.Response
{
    using System;

    public class PackagePriceByDate
    {
        public PackagePriceByDate() { }

        public PackagePriceByDate(string packageReference, DateTime departureDate, decimal price)
        {
            PackageReference = packageReference;
            DepartureDate = departureDate;
            Price = price;
        }

        public string PackageReference { get; set; }
        public DateTime DepartureDate { get; set; }
        public decimal Price { get; set; }
    }
}
