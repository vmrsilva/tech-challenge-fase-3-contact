namespace TechChallenge.Contact.Api.Controllers.Contact.Dto
{
    public record ContactResponseDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Phone { get; init; }
        public string Email { get; init; }
        public Guid RegionId { get; init; }
    }
}
