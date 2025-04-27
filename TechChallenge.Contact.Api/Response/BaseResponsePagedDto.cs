namespace TechChallenge.Contact.Api.Response
{
    public record BaseResponsePagedDto<T> : BaseResponseDto<T>
    {
        public int TotalPages
        {
            get => CurrentPage == 0 ? 1 : (int)Math.Ceiling((decimal)TotalItems / (TotalItems == 0 ? 1 : (decimal)ItemsPerPage));
        }
        public int TotalItems { get; init; }
        public int CurrentPage { get; init; }
        public int ItemsPerPage { get; init; }
    }
}
