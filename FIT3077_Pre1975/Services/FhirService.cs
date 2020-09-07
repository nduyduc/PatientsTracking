using FIT3077_Pre1975.Mappings;
using FIT3077_Pre1975.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.FhirPath.Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Services
{
    /// <summary>
    /// This class handles interactions with HAPI FHIR API library
    /// Including getting data for the web application, mapping data and getting data for ML task
    /// </summary>
    public static class FhirService
    {
        private const string SERVICE_ROOT_URL = "https://fhir.monash.edu/hapi-fhir-jpaserver/fhir";

        private const int SERVICE_TIMEOUT = 60 * 1000;

        private const int LIMIT_ENTRY = 200;

        private const int NUMBER_OF_DATA_RECORD = 200;

        private static readonly FhirClient Client = new FhirClient(SERVICE_ROOT_URL) { Timeout = SERVICE_TIMEOUT };

        private const string CHOLESTEROL_CODE = "2093-3";

        private const string BLOOD_PRESSURE_CODE = "55284-4";

        /// <summary>
        /// Get Practitioner by ID
        /// </summary>
        /// <param name="practitionerId"> ID of practitioner to be fetched </param>
        /// <returns> Practitioner </returns>
        public static async Task<Models.Practitioner> GetPractitioner(string practitionerId)
        {
            Models.Practitioner practitioner = null;
            Hl7.Fhir.Model.Practitioner fhirPractitioner = null;

            try
            {
                // limit to 1 to avoid huge response Bundle
                var PractitionerQuery = new SearchParams()
                    .Where("identifier=http://hl7.org/fhir/sid/us-npi|" + practitionerId)
                    .LimitTo(1);

                Bundle PractitionerResult = await Client.SearchAsync<Hl7.Fhir.Model.Practitioner>(PractitionerQuery);

                if (PractitionerResult.Entry.Count > 0)
                {
                    // Map the FHIR Practitioner object to App's Practitioner object
                    fhirPractitioner = (Hl7.Fhir.Model.Practitioner)PractitionerResult.Entry[0].Resource;
                    PractitionerMapper mapper = new PractitionerMapper();
                    practitioner = mapper.Map(fhirPractitioner);
                }
            }
            catch (FhirOperationException FhirException)
            {
                System.Diagnostics.Debug.WriteLine("Fhir error message: " + FhirException.Message);
            }
            catch (Exception GeneralException)
            {
                System.Diagnostics.Debug.WriteLine("General error message: " + GeneralException.Message);
            }
            return practitioner;
        }

        /// <summary>
        /// Get all patients of a given Practitioner
        /// </summary>
        /// <param name="practitionerId"></param>
        /// <returns> a list of Patients </returns>
        public static async Task<List<Models.Patient>> GetPatientsOfPractitioner(string practitionerId)
        {
            List<Models.Patient> patientList = new List<Models.Patient>();

            SortedSet<string> patientIdList = new SortedSet<string>();

            try
            {
                var encounterQuery = new SearchParams()
                    .Where("participant.identifier=http://hl7.org/fhir/sid/us-npi|" + practitionerId)
                    .Include("Encounter.participant.individual")
                    .Include("Encounter.patient")
                    .LimitTo(LIMIT_ENTRY);
                Bundle Result = await Client.SearchAsync<Encounter>(encounterQuery);

                // implement paging for HAPI FHIR API Bundle
                while (Result != null)
                {
                    foreach (var Entry in Result.Entry)
                    {
                        // Get patient id and add to a list
                        Encounter encounter = (Encounter)Entry.Resource;
                        string patientRef = encounter.Subject.Reference;
                        string patientId = patientRef.Split('/')[1];
                        patientIdList.Add(patientId);
                    }

                    Result = Client.Continue(Result, PageDirection.Next);
                }

                // fetch patient data from the list of patient ids
                foreach (var patientId in patientIdList)
                {
                    Bundle PatientResult = await Client.SearchByIdAsync<Hl7.Fhir.Model.Patient>(patientId);

                    if (PatientResult.Entry.Count > 0)
                    {
                        // Map the FHIR Patient object to App's Patient object
                        Hl7.Fhir.Model.Patient fhirPatient = (Hl7.Fhir.Model.Patient)PatientResult.Entry[0].Resource;
                        PatientMapper mapper = new PatientMapper();
                        Models.Patient patient = mapper.Map(fhirPatient);
                        patientList.Add(patient);
                    }
                }
            }
            catch (FhirOperationException FhirException)
            {
                System.Diagnostics.Debug.WriteLine("Fhir error message: " + FhirException.Message);
            }
            catch (Exception GeneralException)
            {
                System.Diagnostics.Debug.WriteLine("General error message: " + GeneralException.Message);
            }

            return patientList;
        }

        /// <summary>
        /// Get Observation values of a patients list (Total Cholesterol, Blood Pressure)
        /// </summary>
        /// <param name="patients"> given list of patients </param>
        /// <returns> list of patients that contain Cholesterol and Blood Pressure values </returns>
        public static async Task<PatientsList> GetObservationValues(PatientsList patients)
        {
            PatientsList MonitoredPatientList = new PatientsList();

            foreach (Models.Patient MonitoredPatient in patients)
            {
                try
                {

                    Models.Patient patient = MonitoredPatient;
                    patient.Observations = new List<Models.Observation>();
                    patient.HasObservations = true;

                    // sort by date, limit to 1 so only take the newest Cholesterol observation
                    var ObservationQuery = new SearchParams()
                            .Where("patient=" + MonitoredPatient.Id)
                            .Where("code=" + CHOLESTEROL_CODE)
                            .OrderBy("-date")
                            .LimitTo(1);

                    Bundle ObservationResult = await Client.SearchAsync<Hl7.Fhir.Model.Observation>(ObservationQuery);

                    if (ObservationResult.Entry.Count > 0)
                    {
                        // Map the FHIR Observation object to App's Observation object
                        Hl7.Fhir.Model.Observation fhirObservation = (Hl7.Fhir.Model.Observation)ObservationResult.Entry[0].Resource;
                        ObservationMapper mapper = new ObservationMapper();
                        Models.Observation cholesterolObservation = mapper.Map(fhirObservation);
                        patient.Observations.Add(cholesterolObservation);

                    }

                    // sort by date, limit to 5 so only take the lastest Blood Pressure observation
                    ObservationQuery = new SearchParams()
                            .Where("patient=" + MonitoredPatient.Id)
                            .Where("code=" + BLOOD_PRESSURE_CODE)
                            .OrderBy("-date")
                            .LimitTo(5);

                    ObservationResult = await Client.SearchAsync<Hl7.Fhir.Model.Observation>(ObservationQuery);

                    for (int i = 0; i < ObservationResult.Entry.Count; i++)
                    {
                        // Map the FHIR Observation object to App's Observation object
                        Hl7.Fhir.Model.Observation fhirObservation = (Hl7.Fhir.Model.Observation)ObservationResult.Entry[i].Resource;
                        ComponentObservationMapper mapper = new ComponentObservationMapper();

                        Models.Observation diastolicObservation = mapper.Map(fhirObservation.Component[0]);
                        diastolicObservation.Id = fhirObservation.Id;

                        Models.Observation systolicObservation = mapper.Map(fhirObservation.Component[1]);
                        systolicObservation.Id = fhirObservation.Id;

                        if (fhirObservation.Issued != null)
                        {
                            diastolicObservation.Issued = ((DateTimeOffset)fhirObservation.Issued).DateTime;
                            systolicObservation.Issued = ((DateTimeOffset)fhirObservation.Issued).DateTime;
                        }
                        else
                        {
                            diastolicObservation.Issued = null;
                            systolicObservation.Issued = null;
                        }

                        patient.Observations.Add(diastolicObservation);
                        patient.Observations.Add(systolicObservation);
                    }

                    MonitoredPatientList.AddPatient(patient);
                }
                catch (FhirOperationException FhirException)
                {
                    System.Diagnostics.Debug.WriteLine("Fhir error message: " + FhirException.Message);
                }
                catch (Exception GeneralException)
                {
                    System.Diagnostics.Debug.WriteLine("General error message: " + GeneralException.Message);
                }
            }
            return MonitoredPatientList;
        }

        /// <summary>
        /// Get patient data for ML task
        /// </summary>
        /// <param name="patient"> given patient </param>
        /// <returns> a patient contains observation values for ML task </returns>
        public static async Task<Models.Patient> GetDataForAnalysis(Models.Patient patient)
        {
            Models.Patient currentPatient = patient;
            currentPatient.Observations = new List<Models.Observation>();
            currentPatient.HasObservations = true;
            try
            {
                var ObservationQuery = new SearchParams()
                        .Where("patient=" + currentPatient.Id)
                        .OrderBy("-date")
                        .LimitTo(LIMIT_ENTRY);

                Bundle ObservationResult = await Client.SearchAsync<Hl7.Fhir.Model.Observation>(ObservationQuery);

                foreach (var Entry in ObservationResult.Entry)
                {
                    Hl7.Fhir.Model.Observation fhirObservation = (Hl7.Fhir.Model.Observation)Entry.Resource;
                    if (fhirObservation.Value != null && fhirObservation.Value is Hl7.Fhir.Model.Quantity)
                    {
                        ObservationMapper mapper = new ObservationMapper();
                        Models.Observation observation = mapper.Map(fhirObservation);
                        if (!currentPatient.ContainsObservation(observation.CodeText))
                        {
                            currentPatient.Observations.Add(observation);
                        }
                    }
                }
            }
            catch (FhirOperationException FhirException)
            {
                System.Diagnostics.Debug.WriteLine("Fhir error message: " + FhirException.Message);
            }
            catch (Exception GeneralException)
            {
                System.Diagnostics.Debug.WriteLine("General error message: " + GeneralException.Message);
            }

            return currentPatient;
        }

        /// <summary>
        /// Get Data for ML Task
        /// </summary>
        /// <returns>a list of patients contain observation data for ML task</returns>
        public static async Task<PatientsList> GetData()
        {
            int count = 0;
            bool stop = false;

            PatientsList data = new PatientsList();

            try
            {

                var PatientQuery = new SearchParams()
                    .OrderBy("birthdate")
                    .LimitTo(LIMIT_ENTRY);

                Bundle PatientResult = await Client.SearchAsync<Hl7.Fhir.Model.Patient>(PatientQuery);

                // paging to search for all patient until reaching NUMBER_OF_DATA_RECORD value
                while (PatientResult != null)
                {
                    if (stop) break;

                    foreach (var Entry in PatientResult.Entry)
                    {
                        Hl7.Fhir.Model.Patient fhirPatient = (Hl7.Fhir.Model.Patient) Entry.Resource;
                        PatientMapper mapper = new PatientMapper();
                        Models.Patient patient = mapper.Map(fhirPatient);
                        
                        if (!AppContext.AnalysisData.Contains(patient) && !data.Contains(patient))
                        {
                            var CholesterolQuery = new SearchParams()
                                .Where("patient=" + patient.Id)
                                .Where("code=2093-3")
                                .OrderBy("-date")
                                .LimitTo(1);

                            // Only GetData for patient that has Cholesterol value
                            Bundle CholesterolResult = await Client.SearchAsync<Hl7.Fhir.Model.Observation>(CholesterolQuery);
                            if (CholesterolResult.Entry.Count > 0)
                            {
                                data.AddPatient(await GetDataForAnalysis(patient));
                                count++;
                            }
                        }

                        // Stop if count reachs NUMBER_OF_DATA_RECORD
                        if (count == NUMBER_OF_DATA_RECORD)
                        {
                            stop = true;
                            break;
                        }
                    }

                    PatientResult = Client.Continue(PatientResult, PageDirection.Next);
                }
            }
            catch (FhirOperationException FhirException)
            {
                System.Diagnostics.Debug.WriteLine("Fhir error message: " + FhirException.Message);
            }
            catch (Exception GeneralException)
            {
                System.Diagnostics.Debug.WriteLine("General error message: " + GeneralException.Message);
            }

            return data;
        }
    }
}
