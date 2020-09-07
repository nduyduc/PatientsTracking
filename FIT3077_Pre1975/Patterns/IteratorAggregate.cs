using System.Collections;

namespace FIT3077_Pre1975.Patterns
{
    public abstract class IteratorAggregate : IEnumerable
    {
        // Returns an Iterator or another IteratorAggregate for the implementing object.
        public abstract IEnumerator GetEnumerator();
    }
}
