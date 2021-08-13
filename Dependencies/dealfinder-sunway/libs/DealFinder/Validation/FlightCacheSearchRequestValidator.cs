namespace DealFinder.Validation
{
    using DealFinder.Request;
    using FluentValidation;

    public class FlightCacheSearchRequestValidator : AbstractValidator<FlightCacheSearchRequest>
    {
        public const string DepartureAirportsWarning = "At least 1 departure airport must be included";
        public const string ArrivalAirportsWarning = "At least 1 arrival airport must be included";
        public const string MonthWarning = "A valid mm/YYYY date must be specified";

        public FlightCacheSearchRequestValidator()
        {
            RuleFor(r => r.DepartureAirports).NotNull().WithMessage(DepartureAirportsWarning);
            RuleFor(r => r.DepartureAirports).NotEmpty().WithMessage(DepartureAirportsWarning);
            RuleFor(r => r.ArrivalAirports).NotNull().WithMessage(ArrivalAirportsWarning);
            RuleFor(r => r.ArrivalAirports).NotEmpty().WithMessage(ArrivalAirportsWarning);
            RuleFor(r => r.Month).NotNull().WithMessage(MonthWarning);
            RuleFor(r => r.Month).Matches(@"^((0\d)|(1[0-2]))\/\d{4}$").WithMessage(MonthWarning);
        }
    }
}
