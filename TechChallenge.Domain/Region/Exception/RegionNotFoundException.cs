namespace TechChallenge.Contact.Domain.Region.Exception
{
    public class RegionNotFoundException : System.Exception
    {
        public RegionNotFoundException() : base(message: "Região não encontrada na base dados.")
        {

        }
    }
}
