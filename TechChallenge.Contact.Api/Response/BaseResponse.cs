namespace TechChallenge.Contact.Api.Response
{
    public record BaseResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
