namespace TechChallenge.Domain.Base.Entity
{
    public class BaseEntity
    {

        public Guid Id { get; set; } = Guid.NewGuid();
        public bool IsDeleted { get; set; } = false;


        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }
    }
}
