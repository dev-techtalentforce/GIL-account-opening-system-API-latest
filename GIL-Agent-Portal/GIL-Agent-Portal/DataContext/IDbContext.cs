using System.Data;

namespace GIL_Agent_Portal.DataContext
{
    public interface IDbContext
    {
        IDbConnection Connection { get; }
    }
}
