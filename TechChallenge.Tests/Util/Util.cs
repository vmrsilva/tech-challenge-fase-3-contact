using TechChallenge.Contact.Integration.Region.Dto;
using TechChallenge.Contact.Integration.Response;

namespace TechChallenge.Contact.Tests.Util
{
    public static class Util
    {

        public static Guid dddPr = Guid.Parse("178C350E-E241-46B6-88F8-0FE298153726");

        public static Guid dddSp = Guid.Parse("DB31F0C2-49B9-4591-83B2-D7396915A3CF");

        public static IntegrationBaseResponseDto<RegionGetDto> GenerateFakeRegionServiceResponse(Guid mockRegionId, string ddd = "11", string name = "São Paulo")
        {
            var fakeRegionGetDto = new RegionGetDto
            {
                Id = mockRegionId,
                Name = name,
                Ddd = ddd
            };

            var fakeBaseResponseDto = new IntegrationBaseResponseDto<RegionGetDto>
            {
                Success = true,
                Error = null,
                Errors = Enumerable.Empty<string>(),
                Data = fakeRegionGetDto
            };

            return fakeBaseResponseDto;
        }
    }
}
