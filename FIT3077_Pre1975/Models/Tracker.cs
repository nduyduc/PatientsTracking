using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Models
{
    public class Tracker
    {
        /// <summary>
        /// Model a Tracker for Historical Monitoring
        /// </summary>
        public List<Observation> Observations { get; set; }

        public List<decimal> GraphData { get; set; }

        public Patient Patient { get; set; }

        public string Type { get; set; }

        public Tracker(Patient patient, string type)
        {
            this.Type = type;
            this.Patient = patient;
            this.Observations = patient.GetAllObservationsByCodeText(type);
            Observations.Sort((x, y) =>
            {
                if (x.Issued == null || y.Issued == null) return 1;
                return ((DateTime)x.Issued).CompareTo((DateTime)y.Issued);
            });
            this.GraphData = new List<decimal>();
            foreach (Observation observation in Observations)
            {
                GraphData.Add((decimal)observation.MeasurementResult.Value);
            }
        }

        public override string ToString()
        {
            string description = "";
            if (Observations.Count > 0)
            {
                description += Observations[0].ToString();
                for (int i = 1; i < Observations.Count; i++)
                {
                    description += ", " + Observations[i].ToString();
                }
            }
            return description;
        }
    }
}
