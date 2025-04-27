namespace TechChallenge.Domain.Contact.Exception
{
    public class ContactNotFoundException : System.Exception
    {
        public ContactNotFoundException() : base(message: "Contato não encontrado na base de dados.")
        {

        }
    }
}
