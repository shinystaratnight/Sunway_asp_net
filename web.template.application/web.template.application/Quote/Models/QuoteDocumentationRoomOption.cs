namespace Web.Template.Application.Quote.Models
{
    using Web.Template.Application.Results.ResultModels;
    using Web.Template.Domain.Entities.Property;

    /// <summary>
    /// Class QuoteDocumentationRoomOption
    /// </summary>
    public class QuoteDocumentationRoomOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteDocumentationRoomOption" /> class.
        /// </summary>
        public QuoteDocumentationRoomOption()
        {
        }

        /// <summary>
        /// Gets or sets the current Room Option
        /// </summary>
        public RoomOption RoomOption { get; set; }

        /// <summary>
        /// Gets or sets the current Meal Basis
        /// </summary>
        public MealBasis MealBasis { get; set; }
    }
}
