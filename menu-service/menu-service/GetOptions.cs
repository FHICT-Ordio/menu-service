namespace menu_service
{
    public class GetOptions
    {
        public enum SortType
        {
            /// <summary>
            /// Alphabetical Ascending
            /// </summary>
            ALP_ASC,
            /// <summary>
            /// Alphabetical Descending
            /// </summary>
            ALP_DES,
            /// <summary>
            /// Price Ascending
            /// </summary>
            PRICE_ASC,
            /// <summary>
            /// Price Descending
            /// </summary>
            PRICE_DES
        }

        public enum FilterType
        {
            /// <summary>
            /// Dont apply filtering
            /// </summary>
            NONE,
            /// <summary>
            /// Apply Name filtering. A String object should be provided in FilterValue as filter condition
            /// </summary>
            /// <example>
            /// "Carpacio"
            /// </example>
            NAME,
            /// <summary>
            /// Apply Regular Expression filtering. A Regex expression should be provided in FilterValue
            /// </summary>
            /// <example>
            /// "([A-Z])\w+"
            /// </example>
            NAME_REGEX,
            /// <summary>
            /// Apply price range filtering. An integer range value should be supplied in FilterValue
            /// </summary>
            /// <example>
            /// { x, x }
            /// </example>
            PRICE_RANGE
        };

        /// <summary>
        /// Specify the type of sorting to apply to the returned values
        /// </summary>
        public SortType Sort { get; set; } = SortType.ALP_ASC;
        public FilterType Filter { get; set; } = FilterType.NONE;
        public Dictionary<string, string>? FilterValue { get; set; } = new();
    }
}
