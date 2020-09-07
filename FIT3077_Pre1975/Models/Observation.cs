using System;
using System.ComponentModel.DataAnnotations;

namespace FIT3077_Pre1975.Models
{
    /// <summary>
    /// Model an Observation of a Patient
    /// </summary>
    public class Observation
    {

        public string Id { get; set; }

        [Display(Name = "Date of Issue")]
        public DateTime? Issued { get; set; }

        public string Code { get; set; }

        public string CodeText { get; set; }

        public Measurement MeasurementResult { get; set; }

        public override string ToString()
        {
            return this.MeasurementResult.ToString() + "(" + Issued.ToString() + ")";
        }
    }
}
