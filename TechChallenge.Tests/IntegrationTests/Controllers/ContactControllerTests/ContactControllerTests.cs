using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using System.Text;
using System.Text.Json;
using TechChallenge.Contact.Api.Controllers.Contact.Dto;
using TechChallenge.Contact.Integration.Region.Dto;
using TechChallenge.Tests.IntegrationTests.Setup;
using TechChallenge.Contact.Integration.Service;
using TechChallenge.Contact.Tests.Util;
using TechChallenge.Contact.Integration.Response;
using TechChallenge.Contact.Api.Response;

namespace TechChallenge.Tests.IntegrationTests.Controllers.ContactControllerTests
{
    public class ContactControllerTests(TechChallangeApplicationFactory techChallangeApplicationFactory) : BaseIntegrationTest(techChallangeApplicationFactory)
    {     
        const string defaultMessageExceptionRegion = "Região não encontrada na base dados.";
        const string defaultMessageExceptionContact = "Contato não encontrado na base de dados.";
        const string routeBase = "api/contact";

        private readonly Mock<IIntegrationService> _integrationServiceMock;

        [Fact(DisplayName = "Should Create New Contact With Success")]
        public async Task ShouldCreateNewContactWithSuccess()
        {
            var client = techChallangeApplicationFactory.CreateClient();

            var integrationServiceMock = techChallangeApplicationFactory.GetIntegrationServiceMock();
            var regionId = Guid.NewGuid();            

            var regionResponseMock = Util.GenerateFakeRegionServiceResponse(regionId);

            integrationServiceMock
                .Setup(x => x.SendResilientRequest<IntegrationBaseResponseDto<RegionGetDto>>(It.IsAny<Func<Task<IntegrationBaseResponseDto<RegionGetDto>>>>()))
                .ReturnsAsync(regionResponseMock);

            var contact = new ContactCreateDto
            {
                Name = "Teste",
                RegionId = regionResponseMock.Data.Id,
                Email = "newcontact@teste.com",
                Phone = "36364141"
            };                               

            var content = new StringContent(JsonSerializer.Serialize(contact), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{routeBase}", content);

            var responseParsed = await response.Content.ReadAsStringAsync();

            var contactDb = await _dbContext.Contact.AsNoTracking().FirstOrDefaultAsync(r => r.Phone == contact.Phone);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Equal(contactDb.Name, contact.Name);
            Assert.Equal(contactDb.Email, contact.Email);
            Assert.Equal(contactDb.Phone, contact.Phone);
        }

        [Fact(DisplayName = "Should Create Contact Return Bad Request When Region Does Not Exist")]
        public async Task ShouldCreateContactReturnBadRequestWhenRegionDoesNotExist()
        {
            var client = techChallangeApplicationFactory.CreateClient();

            var integrationServiceMock = techChallangeApplicationFactory.GetIntegrationServiceMock();

            var regionResponseMock = Util.GenerateFakeRegionServiceResponse(Guid.NewGuid());

            integrationServiceMock
                .Setup(x => x.SendResilientRequest<IntegrationBaseResponseDto<RegionGetDto>>(It.IsAny<Func<Task<IntegrationBaseResponseDto<RegionGetDto>>>>()))
                .ReturnsAsync((IntegrationBaseResponseDto<RegionGetDto>)null);

            var contact = new ContactCreateDto
            {
                Name = "Teste",
                RegionId = Guid.NewGuid(),
                Email = "newcontact@teste.com",
                Phone = "36364141"
            };

            var content = new StringContent(JsonSerializer.Serialize(contact), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{routeBase}", content);

            var responseParsed = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponseDto<IEnumerable<ContactResponseDto>>>(responseParsed,
                     new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var contactDb = await _dbContext.Contact.AsNoTracking().FirstOrDefaultAsync(r => r.Phone == contact.Phone);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(result.Success);
            Assert.Equal(defaultMessageExceptionRegion, result.Error);
        }

        [Theory(DisplayName = "Should Return Contacts By Ddd")]
        [InlineData("11")]
        [InlineData("47")]
        public async Task ShouldReturnContactsByDdd(string ddd)
        {
            var client = techChallangeApplicationFactory.CreateClient();

            var integrationServiceMock = techChallangeApplicationFactory.GetIntegrationServiceMock();

            var regionResponseMock = Util.GenerateFakeRegionServiceResponse(GetRegionIdFake(ddd), ddd);

            integrationServiceMock
                .Setup(x => x.SendResilientRequest<IntegrationBaseResponseDto<RegionGetDto>>(It.IsAny<Func<Task<IntegrationBaseResponseDto<RegionGetDto>>>>()))
                .ReturnsAsync(regionResponseMock);

            var contact = new ContactCreateDto
            {
                Name = "Teste",
                RegionId = GetRegionIdFake(ddd),
                Email = "newcontact@teste.com",
                Phone = "36364141"
            };

            var response = await client.GetAsync($"{routeBase}/by-ddd/{ddd}");

            var responseParsed = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponseDto<IEnumerable<ContactResponseDto>>>(responseParsed,
                     new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(result.Data);
            Assert.Equal(regionResponseMock.Data.Id, result.Data.FirstOrDefault().RegionId);
        }

        [Theory(DisplayName = "Should Return Contact By Id")]
        [InlineData("11")]
        [InlineData("47")]
        public async Task ShouldReturnContactById(string ddd)
        {
            var client = techChallangeApplicationFactory.CreateClient();

            var integrationServiceMock = techChallangeApplicationFactory.GetIntegrationServiceMock();

            var regionResponseMock = Util.GenerateFakeRegionServiceResponse(GetRegionIdFake(ddd), ddd);

            integrationServiceMock
                .Setup(x => x.SendResilientRequest<IntegrationBaseResponseDto<RegionGetDto>>(It.IsAny<Func<Task<IntegrationBaseResponseDto<RegionGetDto>>>>()))
                .ReturnsAsync(regionResponseMock);

            var contactDb = await _dbContext.Contact.AsNoTracking().FirstOrDefaultAsync(r => r.RegionId == regionResponseMock.Data.Id);

            var response = await client.GetAsync($"{routeBase}/by-id/{contactDb.Id}");

            var responseParsed = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponseDto<ContactResponseDto>>(responseParsed,
                     new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal(contactDb.Id, result.Data.Id);
        }

        [Fact(DisplayName = "Should Get By Id Return Bad Request When It Does Not Exist")]
        public async Task ShouldGetByIdReturnBadRequestWhenItDoesNotExist()
        {
            var client = techChallangeApplicationFactory.CreateClient();

            var response = await client.GetAsync($"{routeBase}/by-id/{Guid.NewGuid()}");

            var responseParsed = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BaseResponseDto<ContactResponseDto>>(responseParsed,
                     new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Null(result.Data);
            Assert.False(result.Success);
            Assert.Equal(defaultMessageExceptionContact, result.Error);
        }

        private Guid GetRegionIdFake(string ddd)
        {
            switch (ddd)
            {
                case "47":
                    return Util.dddPr;
                    
                default:
                    return Util.dddSp;
                    
            }
        }
    }
}
