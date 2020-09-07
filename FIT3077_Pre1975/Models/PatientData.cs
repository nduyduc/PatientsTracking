using Microsoft.ML.Data;
using System;
using System.Collections.Generic;

namespace FIT3077_Pre1975.Models
{
    /// <summary>
    /// Model a Patient Data used for ML Task
    /// </summary>
    public class PatientData
    {

        public const int NUMBER_OF_FEATURES = 13;

        // List of features
        public static List<string> attributes = new List<string>(new string[NUMBER_OF_FEATURES] 
        {
            "Body Height", 
            "Body Weight", 
            "Body Mass Index", 
            "Urea Nitrogen", 
            "Glucose", 
            "Calcium", 
            "Creatinine", 
            "Chloride",
            "Triglycerides",
            "Potassium",
            "Sodium",
            "Carbon Dioxide",
            "Potassium"
        });

        [LoadColumn(0)]
        [ColumnName("Label")]
        public bool HighCholesterol { get; set; }

        [LoadColumn(1, NUMBER_OF_FEATURES)]
        [VectorType(NUMBER_OF_FEATURES)]
        public Single[] Features { get; set; } 

        /// <summary>
        /// To csv line format
        /// </summary>
        /// <returns> a string represents a single record in csv file</returns>
        public override string ToString()
        {
            string output = $"{HighCholesterol}";
            for (int i = 0; i < NUMBER_OF_FEATURES; i++)
            {
                output += $",{Features[i]}";
            }
            return output;
        }

        public static string[] Headers()
        {
            string[] headers = new string[NUMBER_OF_FEATURES + 1];
            headers[0] = "Label";
            for (int i = 0; i < NUMBER_OF_FEATURES; i++)
            {
                headers[i + 1] = attributes[i];
            }
            return headers;
        }
        
    }
}
