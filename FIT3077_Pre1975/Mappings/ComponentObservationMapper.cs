using FIT3077_Pre1975.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Mappings
{
    /// <summary>
    /// Concrete Mapper class to map Fhir model component observation into Observation object
    /// </summary>
    public class ComponentObservationMapper : MapperBase<Models.Observation, Hl7.Fhir.Model.Observation.ComponentComponent>
    {
        /// <summary>
        /// Method to map a Fhir Model component observation element to an Observation object
        /// </summary>
        /// <param name="element"> Component Observation element from Fhir model </param>
        /// <returns> An Observation object with values from Fhir model element </returns>
        public override Models.Observation Map(Hl7.Fhir.Model.Observation.ComponentComponent element)
        {
            /// create a new Observation object and map values from Fhir model to it
            var observation = new Observation
            {
                Code = element.Code.Coding[0].Code,

                CodeText = element.Code.Text
            };

            Hl7.Fhir.Model.Quantity fhirQuantity = (Hl7.Fhir.Model.Quantity)element.Value;

            observation.MeasurementResult = new Measurement
            {
                Value = fhirQuantity.Value,
                Unit = fhirQuantity.Unit
            };

            return observation;
        }

        public override Hl7.Fhir.Model.Observation.ComponentComponent Map(Models.Observation element)
        {
            throw new NotImplementedException();
        }
    }
}
