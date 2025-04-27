using Refit;
using TechChallenge.Contact.Integration.Region.Dto;
using TechChallenge.Contact.Integration.Response;

namespace TechChallenge.Contact.Integration.Region
{
    public interface IRegionIntegration
    {
        [Get("/Region/get-by-id/{id}")]
        Task<IntegrationBaseResponseDto<RegionGetDto>> GetById(Guid id);

        [Get("/Region/get-by-ddd/{ddd}")]
        Task<IntegrationBaseResponseDto<RegionGetDto>> GetByDDD(string ddd);
    }
}
