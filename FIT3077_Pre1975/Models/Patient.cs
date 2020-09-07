using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FIT3077_Pre1975.Models
{
    /// <summary>
    /// Model a Patient in the Application
    /// </summary>
    public class Patient
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Gender Gender { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public Address Address { get; set; }

        public List<Observation> Observations { get; set; }

        public bool HasObservations { get; set; } = false;

        public bool Selected { get; set; } = false;

        public bool ContainsObservation(string Text)
        {
            foreach (Observation observation in Observations)
            {
                if (observation.CodeText.Equals(Text))
                {
                    return true;
                }
            }
            return false;
        }

        public Observation GetObservationByCodeText(string Text)
        {
            foreach (Observation observation in Observations)
            {
                if (observation.CodeText.Equals(Text))
                {
                    return observation;
                }
            }
            return null;
        }

        public List<Observation> GetAllObservationsByCodeText(string Text)
        {
            List<Observation> observations = new List<Observation>();
            foreach (Observation observation in Observations)
            {
                if (observation.CodeText.Equals(Text))
                {
                    observations.Add(observation);
                }
            }
            return observations;
        }
    }
}
