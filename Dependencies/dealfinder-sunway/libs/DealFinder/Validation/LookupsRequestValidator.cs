namespace DealFinder.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DealFinder.Request;
    using Intuitive;
    using FluentValidation;

    public class LookupsRequestValidator : AbstractValidator<LookupsRequest>
    {
        public static readonly string[] AllowedLookups = new[]
        {
            "airports",
            "allairports",
            "g1s",
            "g2s",
            "g3s",
            "attributes",
            "mealbases",
            "facilities"
        };

        public const string InvalidLookupsWarning = "The lookups requested were not recognised. Supported lookups are: {0}";
        public const string NoLookupsWarning = "No lookups specified";

        public LookupsRequestValidator()
        {
            RuleFor(r => r.Lookups).NotNull().WithMessage(NoLookupsWarning);
            RuleFor(r => r.Lookups).NotEmpty().WithMessage(NoLookupsWarning);
            RuleFor(r => r.Lookups)
                .Must(l => !l.Except(AllowedLookups, StringComparer.OrdinalIgnoreCase).Any())
                .When(r => r.Lookups != null).WithMessage(
                    InvalidLookupsWarning.FormatWith(string.Join(", ", AllowedLookups)));
        }
    }
}
