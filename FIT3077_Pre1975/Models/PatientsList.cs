using FIT3077_Pre1975.Patterns;
using FIT3077_Pre1975.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Models
{
    /// <summary>
    /// Model a Patient List
    /// </summary>
    public class PatientsList : IteratorAggregate, IObserver
    {
        private List<Patient> _patients;

        private Practitioner _practitioner;

        public bool IsLoading { get; set; } = false;

        public PatientsList() {
            _patients = new List<Patient>();
        }

        public PatientsList(List<Patient> patients) {
            _patients = patients;
        }

        public int Count
        {
            get { return _patients.Count; }
        }

        /// <summary>
        /// Add patient to a list
        /// </summary>
        /// <param name="patient"> added patient </param>
        public void AddPatient(Patient patient) 
        {
            _patients.Add(patient);
        }

        /// <summary>
        /// Check if the list contains a patient
        /// </summary>
        /// <param name="paramPatient"> patient to be checked </param>
        /// <returns></returns>
        public bool Contains(Patient paramPatient)
        {
            foreach (Patient patient in this)
            {
                if (patient.Id.Equals(paramPatient.Id))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get Patient at index in the list
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Patient GetPatientAt(int index) 
        {
            if (index >= 0 && index < Count)
            {
                return _patients[index];
            } 
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get Patient by id, return null if 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Patient GetPatientByID(string ID) {
            foreach (Patient patient in _patients) { 
                if (patient.Id == ID)
                {
                    return patient;
                }
            }
            return null;
        }

        public override IEnumerator GetEnumerator()
        {
            return new PatientsListIterator(this);
        }

        /// <summary>
        /// Update PatientLists when the Practitioner changes
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public async Task UpdateAsync(IObservableSubject subject)
        {
            IsLoading = true;
            if (_practitioner == null || (subject as Practitioner).Id != _practitioner.Id)
            {
                _practitioner = (Practitioner)subject;
                _patients = await FhirService.GetPatientsOfPractitioner(_practitioner.Id);
                IsLoading = false;
            }
        }

        public void Update(IObservableSubject subject)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove a Patient by ID from a list
        /// </summary>
        /// <param name="Id">ID of patient to be removed</param>
        public void RemovePatientByID(string Id)
        {
            foreach (Patient patient in _patients)
            {
                if (patient.Id == Id)
                {
                    _patients.Remove(patient);
                    break;
                }
            }
        }
    }
}
