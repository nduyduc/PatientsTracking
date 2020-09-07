using FIT3077_Pre1975.Models;
using System;

/// <summary>
/// Concrete Mapper class to map Fhir model Patient into an Patient object
/// </summary>
namespace FIT3077_Pre1975.Mappings
{
    public class PatientMapper : MapperBase<FIT3077_Pre1975.Models.Patient, Hl7.Fhir.Model.Patient>
    {
        /// <summary>
        /// Method to map a Fhir Model Patient element to an Patient object
        /// </summary>
        /// <param name="element"> Patient element from Fhir model </param>
        /// <returns> A Patient object with values from Fhir model element </returns>
        public override Models.Patient Map(Hl7.Fhir.Model.Patient element)
        {
            /// create a new Patient object and map values from Fhir model to it
            var patient = new Models.Patient
            {
                Id = element.Id,
                Name = element.Name[0].ToString()
            };

            if (element.BirthDateElement != null)
            {
                patient.BirthDate = ((DateTimeOffset)element.BirthDateElement.ToDateTimeOffset()).DateTime;
            }
            else
            {
                patient.BirthDate = null;
            }

            if (Enum.TryParse(element.GenderElement.TypeName, out Gender gender))
            {
                patient.Gender = gender;
            }

            /// use Address mapper to map Patient's Address
            if (element.Address.Count > 0)
            {
                Hl7.Fhir.Model.Address address = element.Address[0];
                AddressMapper mapper = new AddressMapper();
                patient.Address = mapper.Map(address);
            }

            return patient;
        }

        public override Hl7.Fhir.Model.Patient Map(Models.Patient element)
        {
            throw new NotImplementedException();
        }
    }
}
