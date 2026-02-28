using System.Threading.Tasks;

namespace Airport_Managment_SYS.Utilities
{
    public interface IDbInitializer
    {
        Task InitializeAsync();
    }
}
