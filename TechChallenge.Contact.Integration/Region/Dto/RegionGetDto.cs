using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechChallenge.Contact.Integration.Region.Dto
{
    public record RegionGetDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
 
        public string Ddd { get; init; }
    }
}
