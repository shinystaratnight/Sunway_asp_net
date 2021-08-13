namespace DealFinder.Validation
{
    using System;
    using DealFinder.Request;
    using FluentValidation;
    using NodaTime;

    public class SearchRequestValidator : AbstractValidator<SearchRequest>
    {
        public const string AdultWarning = "Adults must be specified";
        public const string AirportWarning = "Airports must be specified";
        public const string DurationWarning = "Durations must be specified";
        public const string EndAfterStartWarning = "End date must not be before start date";
        public const string EndWarning = "End date must not be in the past";
        public const string LocationWarning = "Either g1, g2s, g3s or property must be specified";
        public const string StartWarning = "Start date must not be in the past";

        public SearchRequestValidator(IClock clock)
        {
            RuleFor(r => r.DepartureAirportIDs).NotNull().NotEmpty().WithMessage(AirportWarning);
            RuleFor(r => r.Adults).GreaterThan(0).WithMessage(AdultWarning);
            RuleFor(r => r.Durations).NotNull().NotEmpty().WithMessage(DurationWarning);
            RuleFor(r => r.StartDate).GreaterThanOrEqualTo(r => clock.GetCurrentInstant().ToDateTimeUtc().Date).WithMessage(StartWarning);
            RuleFor(r => r.EndDate).GreaterThanOrEqualTo(r => clock.GetCurrentInstant().ToDateTimeUtc().Date).When(r => r.EndDate != DateTime.MinValue).WithMessage(EndWarning);
            RuleFor(r => r.EndDate).GreaterThanOrEqualTo(r => r.StartDate).When(r => r.EndDate != DateTime.MinValue).WithMessage(EndAfterStartWarning);

            RuleFor(r => r.GeographyLevel1IDs).NotNull().NotEmpty()
                .When(r => string.IsNullOrWhiteSpace(r.GeographyLevel2IDs)
                    && string.IsNullOrWhiteSpace(r.GeographyLevel3IDs)
                    && string.IsNullOrWhiteSpace(r.PropertyReferenceIDs))
                .WithMessage(LocationWarning);
        }
    }
}
