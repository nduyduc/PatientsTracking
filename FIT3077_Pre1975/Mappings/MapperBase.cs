/// <summary>
/// Mapper Base interface to implement concrete Mapper classes
/// </summary>
namespace FIT3077_Pre1975.Mappings
{
    public abstract class MapperBase<TFirst, TSecond>
    {
        public abstract TFirst Map(TSecond element);
      
        public abstract TSecond Map(TFirst element);
      
    }
}
