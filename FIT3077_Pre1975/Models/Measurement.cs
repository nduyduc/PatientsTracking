namespace FIT3077_Pre1975.Models
{
    /// <summary>
    /// Models a single Measurement
    /// </summary>
    public class Measurement
    {

        public decimal? Value { get; set; }

        public string Unit { get; set; }

        public override string ToString()
        {
            return Value + " " + Unit;
        }
    }
}
