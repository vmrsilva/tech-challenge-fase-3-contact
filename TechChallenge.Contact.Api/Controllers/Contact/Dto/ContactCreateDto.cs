namespace TechChallenge.Contact.Api.Controllers.Contact.Dto
{
    public record ContactCreateDto
    {
        public required string Name { get; init; }
        public string Phone { get; init; }
        public string Email { get; init; }
        public required Guid RegionId { get; init; }
    }
}
