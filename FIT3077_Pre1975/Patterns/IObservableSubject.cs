/// <summary>
/// Observer Subject interface to implement Observer pattern
/// </summary>
namespace FIT3077_Pre1975.Patterns
{
    public interface IObservableSubject
    {

        // Attach an observer to the subject
        void Attach(IObserver observer);

        // Detach an observer from the subject
        void Detach(IObserver observer);

        // Notify all observers about an event
        void Notify();
        
    }
}
