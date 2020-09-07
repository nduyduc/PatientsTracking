using System.Threading.Tasks;

/// <summary>
/// Observer interface to implement Observer pattern
/// </summary>
namespace FIT3077_Pre1975.Patterns
{
    public interface IObserver
    {
        // Receive update from model
        void Update(IObservableSubject subject);

        Task UpdateAsync(IObservableSubject subject);
    }
}   
