using ProductManagementSystem.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Entities.Extensions
{
    public static class AddressExtensions
    {
        public static void Map(this Address dbAddress, Address address)
        {
            dbAddress.Country = address.Country;
            dbAddress.City = address.City;
            dbAddress.Street = address.Street;
            dbAddress.Number = address.Number;
            dbAddress.ZipCode = address.ZipCode;
        }
    }
}
