using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TechChallenge.Domain.Base.Repository;
using TechChallenge.Domain.Contact.Entity;
using TechChallenge.Domain.Contact.Repository;
using TechChallenge.Infrastructure.Context;

namespace TechChallenge.Infrastructure.Repository.Contact
{
    public class ContactRepository : IContactRepository
    {
        private readonly IBaseRepository<ContactEntity> _baseRepository;
        private readonly TechChallangeContext _techChallangeContext;

        public ContactRepository(IBaseRepository<ContactEntity> baseRepository, TechChallangeContext techChallangeContext)
        {
            _baseRepository = baseRepository;
            _techChallangeContext = techChallangeContext;
        }

        public async Task Create(ContactEntity contact)
        {
            await _baseRepository.AddAsync(contact).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ContactEntity>> GetAllPagedAsync(Expression<Func<ContactEntity, bool>> search, int pageSize, int page, Expression<Func<ContactEntity, dynamic>> orderDesc)
        {
            return await _baseRepository.GetPagedAsync(search, pageSize, page, orderDesc).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ContactEntity>> GetByDddAsync(Guid regionId)
        {

            using (_techChallangeContext)
            {
                var contacts = await _techChallangeContext.Contact.Where(c => c.RegionId == regionId && !c.IsDeleted).ToListAsync();

                return contacts;
            }
        }

        public async Task<ContactEntity> GetByIdAsync(Guid id)
        {
            return await _baseRepository.GetByIdAsync(id).ConfigureAwait(false);
        }

        public async Task<int> GetCountAsync(Expression<Func<ContactEntity, bool>> search)
        {
            return await _baseRepository.GetCountAsync(search).ConfigureAwait(false);
        }

        public async Task UpdateAsync(ContactEntity contact)
        {
            await _baseRepository.UpdateAsync(contact).ConfigureAwait(false);
        }
    }
}
