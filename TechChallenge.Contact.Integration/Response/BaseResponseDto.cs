namespace TechChallenge.Contact.Integration.Response
{
    public record IntegrationBaseResponseDto<T> : IntegrationBaseResponse
    {

        public T Data { get; init; }
    }
}
