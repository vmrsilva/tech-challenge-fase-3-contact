using AutoMapper;
using TechChallenge.Contact.Api.Controllers.Contact.Dto;
using TechChallenge.Domain.Contact.Entity;

namespace TechChallenge.Contact.Api.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ContactCreateDto, ContactEntity>();
            CreateMap<ContactUpdateDto, ContactEntity>();
            CreateMap<ContactEntity, ContactResponseDto>();
        }
    }
}
