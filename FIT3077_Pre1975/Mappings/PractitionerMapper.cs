using FIT3077_Pre1975.Models;
using System;

/// <summary>
/// Concrete Mapper class to map Fhir model Practitioner into an Practitioner object
/// </summary>
namespace FIT3077_Pre1975.Mappings
{
    public sealed class PractitionerMapper : MapperBase<FIT3077_Pre1975.Models.Practitioner, Hl7.Fhir.Model.Practitioner>
    {
        private const string ID_SYSTEM = "http://hl7.org/fhir/sid/us-npi";

        /// <summary>
        /// Method to map a Fhir Model Practitioner element to an Practitioner object
        /// </summary>
        /// <param name="element"> Practitioner element from Fhir model </param>
        /// <returns> A Practitioner object with values from Fhir model element </returns>
        public override Models.Practitioner Map(Hl7.Fhir.Model.Practitioner element)
        {
            var practitioner = new Models.Practitioner();
            
            /// get valid identifier
            foreach (var identifier in element.Identifier)
            {
                if (identifier.System.Equals(ID_SYSTEM))
                {
                    practitioner.Id = identifier.Value;
                    break;
                }
            }

            practitioner.Name = element.Name[0].ToString();

            if (element.BirthDateElement != null)
            {
                practitioner.BirthDate = ((DateTimeOffset)element.BirthDateElement.ToDateTimeOffset()).DateTime;
            }
            else
            {
                practitioner.BirthDate = null;
            }

            if (Enum.TryParse(element.GenderElement.TypeName, out Gender gender))
            {
                practitioner.Gender = gender;
            }

            /// map practitioner's address with AddressMapper
            if (element.Address.Count > 0)
            {
                Hl7.Fhir.Model.Address address = element.Address[0];
                AddressMapper mapper = new AddressMapper();
                practitioner.Address = mapper.Map(address);
            }
            
            return practitioner;
        }

        public override Hl7.Fhir.Model.Practitioner Map(Models.Practitioner element)
        {
            throw new NotImplementedException();
        }
    }
}
