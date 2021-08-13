namespace DealFinder.Validation
{
    using DealFinder.Request;
    using FluentValidation;
    using NodaTime;

    public class CalendarRequestValidator : AbstractValidator<CalendarRequest>
    {
        public const string AdultsWarning = "Adults must be specified";
        public const string AirportWarning = "Airport must be specified";
        public const string DurationWarning = "Duration must be specified";
        public const string EndAfterStartWarning = "End date must not be before start date";
        public const string EndWarning = "End date must not be in the past";
        public const string PropertyWarning = "Property must be specified";
        public const string StartWarning = "Start date must not be in the past";

        public CalendarRequestValidator(IClock clock)
        {
            RuleFor(m => m.PropertyReferenceID).GreaterThan(0).WithMessage(PropertyWarning);
            RuleFor(m => m.DepartureAirportID).GreaterThan(0).WithMessage(AirportWarning);
            RuleFor(m => m.Duration).GreaterThan(0).WithMessage(DurationWarning);
            RuleFor(m => m.Adults).GreaterThan(0).WithMessage(AdultsWarning);
            RuleFor(m => m.StartDate).GreaterThanOrEqualTo(m => clock.GetCurrentInstant().ToDateTimeUtc().Date).WithMessage(StartWarning);
            RuleFor(m => m.EndDate).GreaterThanOrEqualTo(m => clock.GetCurrentInstant().ToDateTimeUtc().Date).WithMessage(EndWarning);
            RuleFor(m => m.EndDate).GreaterThanOrEqualTo(m => m.StartDate).WithMessage(EndAfterStartWarning);
        }
    }
}
