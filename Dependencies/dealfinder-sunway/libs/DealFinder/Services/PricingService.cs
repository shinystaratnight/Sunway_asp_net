using System;
using System.Collections.Generic;
using System.Text;

namespace DealFinder.Services
{
    public class PricingService : IPricingService
    {
        public const string PerPerson = "Per Person";
        public const string PercentCost = "Percent (Cost)";
        public const string PercentPrice = "Percent (Price)";
        public const string PerRoom = "Per Room Per Night";
        public const string Amount = "Amount";

        public decimal ApplyMargin(PricingInformation info)
        {
            decimal margin;
            switch (info.MarginType)
            {
                case PerPerson:
                    margin = info.MarginValue * (info.Adults + info.Children);
                    break;
                case PercentCost:
                    margin = info.Cost * (info.MarginValue / 100);
                    break;
                case PercentPrice:
                    margin = (info.Cost * info.MarginValue) / (100 - info.MarginValue);
                    break;
                case PerRoom:
                    margin = info.MarginValue * info.Duration;
                    break;
                case Amount:
                    margin = info.MarginValue;
                    break;
                default:
                    throw new ArgumentException("unsupported margin type");
            }
            if (margin < info.MinMargin) margin = info.MinMargin;
            if (margin > info.MaxMargin) margin = info.MaxMargin;

            return info.Cost + margin;
        }
    }
}
