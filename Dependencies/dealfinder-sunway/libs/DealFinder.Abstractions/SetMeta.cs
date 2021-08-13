// Copyight © intuitive Ltd. All rights reserved

namespace Intuitive.Web.Api
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents set meta.
    /// </summary>
    public class SetMeta
    {
        /// <summary>
        /// Initialises a new instance of <see cref="SetMeta"/>
        /// </summary>
        /// <remarks>This constructor is used through deserialisation.</remarks>
        private SetMeta() { }

        /// <summary>
        /// Initialises a new instance of <see cref="SetMeta"/>
        /// </summary>
        public SetMeta(int currentPage, int pageSize, int totalItems, int totalPages)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }

        /// <summary>
        /// Gets the current page.
        /// </summary>
        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets the page size.
        /// </summary>
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        /// <summary>
        /// Gets the number of items.
        /// </summary>
        [JsonProperty("totalItems")]
        public int TotalItems { get; set; }

        /// <summary>
        /// Gets the number of pages.
        /// </summary>
        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }
    }
}
