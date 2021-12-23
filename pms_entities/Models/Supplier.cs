using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Entities.Models
{
    public class Supplier: IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid AddressId { get; set; }
        public Address Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<SupplierOffer> Offers { get; set; }
        public DateTime TimeCreated { get; set; }
        public bool Deleted { get; set; }
        public DateTime TimeDeleted { get; set; }
    }
}
