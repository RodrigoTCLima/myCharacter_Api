namespace myCharacter.Helpers
{

    public class QueryParameters
    {
        public int PageNumber { get; set; } = 1;
        private const int MaxPageSize = 100;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public string? SearchTerm { get; set; }
    }
}