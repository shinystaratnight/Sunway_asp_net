// Copyight © intuitive Ltd. All rights reserved

namespace Intuitive.Web.Api
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a set of items.
    /// </summary>
    public class SetResponse<T> : ResponseBase
    {

        private SetResponse()
        {
            Success = false;
            Data = null!;
            Meta = null!;
        }

        public SetResponse(T[] data, int currentPage, int pageSize, int totalItems, int totalPages)
            : this(data, new SetMeta(currentPage, pageSize, totalItems, totalPages))
        { }

        [JsonConstructor]
        public SetResponse(T[] data, SetMeta meta)
        {
            Data = Ensure.IsNotNull(data, nameof(data));
            Meta = Ensure.IsNotNull(meta, nameof(meta));

            Success = Data.Length > 0;
        }

        [JsonProperty("data")]
        public T[] Data { get; set; }

        [JsonProperty("meta")]
        public SetMeta Meta { get; set; }
    }
}