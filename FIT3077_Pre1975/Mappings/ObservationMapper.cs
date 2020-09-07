using FIT3077_Pre1975.Models;
using System;

/// <summary>
/// Concrete Mapper class to map Fhir model observation into an Observation object
/// </summary>
namespace FIT3077_Pre1975.Mappings
{
    public class ObservationMapper : MapperBase<Models.Observation, Hl7.Fhir.Model.Observation>
    {
        /// <summary>
        /// Method to map a Fhir Model observation element to an Observation object
        /// </summary>
        /// <param name="element"> Observation element from Fhir model </param>
        /// <returns> An Observation object with values from Fhir model element </returns>
        public override Models.Observation Map(Hl7.Fhir.Model.Observation element)
        {   
            /// create a new Observation object and map values from Fhir model to it
            var observation = new Observation
            {
                Id = element.Id,
                Code = element.Code.Coding[0].Code,
                CodeText = element.Code.Text
            };

            if (element.Issued != null)
            {
                observation.Issued = ((DateTimeOffset)element.Issued).DateTime;
            }
            else
            {
                observation.Issued = null;
            }

            Hl7.Fhir.Model.Quantity fhirQuantity = (Hl7.Fhir.Model.Quantity)element.Value;

            observation.MeasurementResult = new Measurement
            {
                Value = fhirQuantity.Value,
                Unit = fhirQuantity.Unit
            };

            return observation;
        }

        public override Hl7.Fhir.Model.Observation Map(Models.Observation element)
        {
            throw new NotImplementedException();
        }
    }
}
