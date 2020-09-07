using FIT3077_Pre1975.Models;
using System;
using System.Collections.Generic;

/// <summary>
/// Concrete Mapper class to map patients' address from Fhir model into an Address object
/// </summary>
namespace FIT3077_Pre1975.Mappings
{
    public class AddressMapper : MapperBase<Models.Address, Hl7.Fhir.Model.Address>
    {
        /// <summary>
        /// Method to map a Fhir Model address element to an Address object
        /// </summary>
        /// <param name="element"> Address element from Fhir model </param>
        /// <returns> An Address object with values from Fhir model element </returns>
        public override Models.Address Map(Hl7.Fhir.Model.Address element)
        {
            var address = new Address();
            
            address.Line = new List<string>();
            foreach (var line in element.LineElement)
            {
                address.Line.Add(line.Value);
            }

            /// mapping of individual properties
            address.City = element.City;
            address.District = element.District;
            address.State = element.State;
            address.PostalCode = element.PostalCode;
            address.Country = element.Country;

            return address;
        }

        public override Hl7.Fhir.Model.Address Map(Models.Address element)
        {
            throw new NotImplementedException();
        }
    }
}
