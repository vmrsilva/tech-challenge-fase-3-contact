namespace TechChallenge.Contact.Integration.Service
{
    public interface IIntegrationService
    {
        Task<T?> SendResilientRequest<T>(Func<Task<T>> call);
    }
}
