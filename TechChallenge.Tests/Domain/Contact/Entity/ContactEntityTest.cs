using AutoFixture;
using AutoMapper;
using TechChallenge.Contact.Api.Controllers.Contact.Dto;
using TechChallenge.Domain.Contact.Entity;

namespace TechChallenge.Tests.Domain.Contact.Entity
{
    public class ContactEntityTest
    {
        private readonly IMapper _mapper;
        public ContactEntityTest()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ContactCreateDto, ContactEntity>();
            });

            _mapper = config.CreateMapper();
        }

        [Fact(DisplayName = "Should Create Entity Contact With Exactly Props")]
        public void ShouldCreateEntityContactWithExactlyProps()
        {
            var fixture = new Fixture();
            fixture.Customize<ContactCreateDto>(c => c
                .With(p => p.Email, () => fixture.Create<string>() + "@example.com"));

            var contactDto = fixture.Create<ContactCreateDto>();

            var entity = _mapper.Map<ContactEntity>(contactDto);

            Assert.IsType<Guid>(entity.Id);
            Assert.False(entity.IsDeleted);
            Assert.Equal(contactDto.Phone, entity.Phone);
            Assert.Equal(contactDto.Name, entity.Name);
            Assert.Equal(contactDto.Email, entity.Email);
            Assert.Equal(contactDto.RegionId, entity.RegionId);
        }

        [Fact(DisplayName = "Should Set IsDeleted Equal True When Call MarkAsDeleted")]
        public void ShouldSetIsDeletedEqualTrueWhenCallMarkAsDeleted()
        {
            var fixture = new Fixture();
            fixture.Customize<ContactCreateDto>(c => c
                .With(p => p.Email, () => fixture.Create<string>() + "@example.com"));

            var contactDto = fixture.Create<ContactCreateDto>();

            var entity = _mapper.Map<ContactEntity>(contactDto);

            entity.MarkAsDeleted();

            Assert.True(entity.IsDeleted);
        }
    }
}
