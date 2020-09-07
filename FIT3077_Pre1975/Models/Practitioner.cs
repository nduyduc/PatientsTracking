using FIT3077_Pre1975.Patterns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FIT3077_Pre1975.Models
{
    /// <summary>
    /// Model Practitioner in the Application
    /// </summary>
    public class Practitioner : IObservableSubject
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Gender Gender { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public Address Address { get; set; }

        private List<IObserver> _observers = new List<IObserver>();

        public void Attach(IObserver observer)
        {
            this._observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            this._observers.Remove(observer);
        }

        /// <summary>
        /// Notify Practitioner's observers to update
        /// </summary>
        public void Notify()
        {
            foreach (var observer in _observers)
            {
                observer.UpdateAsync(this);
            }
        }
    }
}
