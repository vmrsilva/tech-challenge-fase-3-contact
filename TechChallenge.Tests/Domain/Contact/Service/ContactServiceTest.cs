using Moq;
using System.Linq.Expressions;
using TechChallenge.Contact.Domain.Region.Exception;
using TechChallenge.Contact.Integration.Region;
using TechChallenge.Contact.Integration.Region.Dto;
using TechChallenge.Contact.Integration.Response;
using TechChallenge.Contact.Integration.Service;
using TechChallenge.Contact.Tests.Util;
using TechChallenge.Domain.Cache;
using TechChallenge.Domain.Contact.Entity;
using TechChallenge.Domain.Contact.Exception;
using TechChallenge.Domain.Contact.Repository;
using TechChallenge.Domain.Contact.Service;

namespace TechChallenge.Tests.Domain.Contact.Service
{
    public class ContactServiceTest
    {
        private readonly Mock<IContactRepository> _contactRepositoryMock;
        private readonly Mock<ICacheRepository> _cacheRepositoryMock;
        private readonly ContactService _contactServiceMock;
        private readonly Mock<IIntegrationService> _integrationServiceMock;
        private readonly Mock<IRegionIntegration> _regionIntegrationMock;

        public ContactServiceTest()
        {
            _contactRepositoryMock = new Mock<IContactRepository>();
            _cacheRepositoryMock = new Mock<ICacheRepository>();
            _integrationServiceMock = new Mock<IIntegrationService>();
            _regionIntegrationMock = new Mock<IRegionIntegration>();

            _contactServiceMock = new ContactService(_contactRepositoryMock.Object, _cacheRepositoryMock.Object, _integrationServiceMock.Object, _regionIntegrationMock.Object);
        }

        [Fact(DisplayName = "Should Create A New Contact With Success")]

        public async Task ShouldCreateANewContactWithSuccess()
        {
            var mockRegionId = Guid.NewGuid();

            var contact = new ContactEntity("Test", "12345678", "mail@test.com", mockRegionId);

            var fakeBaseResponseDto = Util.GenerateFakeRegionServiceResponse(mockRegionId);

            _integrationServiceMock
                 .Setup(x => x.SendResilientRequest<IntegrationBaseResponseDto<RegionGetDto>>(It.IsAny<Func<Task<IntegrationBaseResponseDto<RegionGetDto>>>>()))
                 .ReturnsAsync((IntegrationBaseResponseDto<RegionGetDto>)fakeBaseResponseDto);

            await _contactServiceMock.CreateAsync(contact);

            _contactRepositoryMock.Verify(cr => cr.Create(It.IsAny<ContactEntity>()), Times.Once);
            _integrationServiceMock.Verify(ins => ins.SendResilientRequest(It.IsAny<Func<Task<IntegrationBaseResponseDto<RegionGetDto>>>>()), Times.Once);

        }

        [Fact(DisplayName = "Should Create Return Exception When Region Does Not Exists")]
        public async Task ShouldCreateReturnExceptionWhenRegionDoesNotExists()
        {
            var mockRegionId = Guid.NewGuid();
            var contact = new ContactEntity("Test", "12345678", "mail@test.com", mockRegionId);

            _integrationServiceMock
           .Setup(x => x.SendResilientRequest<IntegrationBaseResponseDto<RegionGetDto>>(It.IsAny<Func<Task<IntegrationBaseResponseDto<RegionGetDto>>>>()))
           .ReturnsAsync((IntegrationBaseResponseDto<RegionGetDto>)null);

            await Assert.ThrowsAsync<RegionNotFoundException>(
              () => _contactServiceMock.CreateAsync(contact));

            _contactRepositoryMock.Verify(cr => cr.Create(It.IsAny<ContactEntity>()), Times.Never);
            _integrationServiceMock.Verify(ins => ins.SendResilientRequest(It.IsAny<Func<Task<IntegrationBaseResponseDto<RegionGetDto>>>>()), Times.Once);

        }

        [Fact(DisplayName = "Should Update Throw Exception When Contact Does Not Exist")]
        public async Task ShouldUpdateThrowExceptionWhenContactDoesNotExist()
        {
            _contactRepositoryMock.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ContactEntity)null);

            var contact = new ContactEntity("Test", "12345678", "mail@test.com", Guid.NewGuid());

            var fakeBaseResponseDto = Util.GenerateFakeRegionServiceResponse(contact.RegionId);

            _integrationServiceMock
                 .Setup(x => x.SendResilientRequest<IntegrationBaseResponseDto<RegionGetDto>>(It.IsAny<Func<Task<IntegrationBaseResponseDto<RegionGetDto>>>>()))
                 .ReturnsAsync((IntegrationBaseResponseDto<RegionGetDto>)fakeBaseResponseDto);

            await Assert.ThrowsAsync<ContactNotFoundException>(
                () => _contactServiceMock.UpdateAsync(contact));

            _contactRepositoryMock.Verify(cr => cr.UpdateAsync(It.IsAny<ContactEntity>()), Times.Never);
            _integrationServiceMock.Verify(ins => ins.SendResilientRequest(It.IsAny<Func<Task<IntegrationBaseResponseDto<RegionGetDto>>>>()), Times.Never);
            _contactRepositoryMock.Verify(cr => cr.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }


        [Fact(DisplayName = "Should Update Throw Exception When Region Does Not Exist")]
        public async Task ShouldUpdateThrowExceptionWhenRegionDoesNotExist()
        {
            _contactRepositoryMock.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new ContactEntity("Paul", "41410303", "email@mock.com", Guid.NewGuid()));

            var contact = new ContactEntity("Test", "12345678", "mail@test.com", Guid.NewGuid());

            _integrationServiceMock
                            .Setup(x => x.SendResilientRequest<IntegrationBaseResponseDto<RegionGetDto>>(It.IsAny<Func<Task<IntegrationBaseResponseDto<RegionGetDto>>>>()))
                            .ReturnsAsync((IntegrationBaseResponseDto<RegionGetDto>)null);

            await Assert.ThrowsAsync<RegionNotFoundException>(
                () => _contactServiceMock.UpdateAsync(contact));

            _contactRepositoryMock.Verify(cr => cr.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            _contactRepositoryMock.Verify(cr => cr.UpdateAsync(It.IsAny<ContactEntity>()), Times.Never);
            _integrationServiceMock.Verify(ins => ins.SendResilientRequest(It.IsAny<Func<Task<IntegrationBaseResponseDto<RegionGetDto>>>>()), Times.Once);
        }

        [Fact(DisplayName = "Should Update When Contact Exist")]
        public async Task ShouldUpdateWhenContactExist()
        {
            var contactMock = new ContactEntity("Test", "12345678", "mail@test.com", Guid.NewGuid());

            _contactRepositoryMock.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(contactMock);

            var fakeBaseResponseDto = Util.GenerateFakeRegionServiceResponse(contactMock.RegionId);

            _integrationServiceMock
                 .Setup(x => x.SendResilientRequest<IntegrationBaseResponseDto<RegionGetDto>>(It.IsAny<Func<Task<IntegrationBaseResponseDto<RegionGetDto>>>>()))
                 .ReturnsAsync((IntegrationBaseResponseDto<RegionGetDto>)fakeBaseResponseDto);


            await _contactServiceMock.UpdateAsync(contactMock);

            _contactRepositoryMock.Verify(cr => cr.UpdateAsync(It.IsAny<ContactEntity>()), Times.Once);
            _integrationServiceMock.Verify(ins => ins.SendResilientRequest(It.IsAny<Func<Task<IntegrationBaseResponseDto<RegionGetDto>>>>()), Times.Once);
            _contactRepositoryMock.Verify(cr => cr.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact(DisplayName = "Should Delete Contact Updating Delete Flag")]
        public async Task ShouldDeleteContactUpdatingDeleteFlag()
        {
            var contactMock = new ContactEntity("Test", "12345678", "mail@test.com", Guid.NewGuid());
            var IdMock = Guid.NewGuid();

            _contactRepositoryMock.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(contactMock);

            await _contactServiceMock.RemoveByIdAsync(IdMock);

            _contactRepositoryMock.Verify(cr => cr.UpdateAsync(It.IsAny<ContactEntity>()), Times.Once);
        }

        [Fact(DisplayName = "Should Delete Throw Exception When Contact Does Not Exist")]
        public async Task ShouldRemoveThrowExceptionWhenContactDoesNotExist()
        {
            _contactRepositoryMock.Setup(cr => cr.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ContactEntity)null);

            var idMock = Guid.NewGuid();

            await Assert.ThrowsAsync<ContactNotFoundException>(
                () => _contactServiceMock.RemoveByIdAsync(idMock));

            _contactRepositoryMock.Verify(cr => cr.UpdateAsync(It.IsAny<ContactEntity>()), Times.Never);
        }

        [Fact(DisplayName = "Should GetById Return Contact")]
        public async Task ShouldGetByIdReturnContact()
        {
            var contactMock = new ContactEntity("Test", "12345678", "mail@test.com", Guid.NewGuid());
            var IdMock = Guid.NewGuid();

            _cacheRepositoryMock.Setup(c => c.GetAsync<ContactEntity>(It.IsAny<string>(), It.IsAny<Func<Task<ContactEntity>>>()))
                               .ReturnsAsync(contactMock);

            await _contactServiceMock.GetByIdAsync(IdMock);

            _cacheRepositoryMock.Verify(c => c.GetAsync<ContactEntity>(It.IsAny<string>(), It.IsAny<Func<Task<ContactEntity>>>()), Times.Once);
        }

        [Fact(DisplayName = "Should GetById Throw Exception When Contact Does Not Exist")]
        public async Task ShouldGetByIdThrowExceptionWhenContactDoesNotExist()
        {
            var contactMock = new ContactEntity("Test", "12345678", "mail@test.com", Guid.NewGuid());
            var IdMock = Guid.NewGuid();


            _cacheRepositoryMock.Setup(c => c.GetAsync<ContactEntity>(It.IsAny<string>(), It.IsAny<Func<Task<ContactEntity>>>()))
                               .ReturnsAsync((ContactEntity)null);



            await Assert.ThrowsAsync<ContactNotFoundException>(
                        () => _contactServiceMock.GetByIdAsync(IdMock));

            _cacheRepositoryMock.Verify(c => c.GetAsync<ContactEntity>(It.IsAny<string>(), It.IsAny<Func<Task<ContactEntity>>>()), Times.Once);
        }

        [Fact(DisplayName = "Should GetAll Return List Of Contacts")]
        public async Task ShouldGetAllReturnListOfContacts()
        {
            var contactMock = new ContactEntity("Test", "12345678", "mail@test.com", Guid.NewGuid());
            var contactsList = new List<ContactEntity> { contactMock };

            _contactRepositoryMock
                .Setup(cr => cr.GetAllPagedAsync(
                                           It.IsAny<Expression<Func<ContactEntity, bool>>>(),
                                           It.IsAny<int>(),
                                           It.IsAny<int>(),
                                           It.IsAny<Expression<Func<ContactEntity, dynamic>>>()
                                       ))
                .ReturnsAsync(contactsList);

            var result = await _contactServiceMock.GetAllPagedAsync(5, 1);

            _contactRepositoryMock.Verify(c => c.GetAllPagedAsync(
                                           It.IsAny<Expression<Func<ContactEntity, bool>>>(),
                                           It.IsAny<int>(),
                                           It.IsAny<int>(),
                                           It.IsAny<Expression<Func<ContactEntity, dynamic>>>()
                                       ), Times.Once);

            Assert.NotNull(result);
            Assert.Equal(contactMock, result.First());
        }

 
    }
}
