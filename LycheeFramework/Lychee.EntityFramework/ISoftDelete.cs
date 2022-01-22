namespace Lychee.EntityFramework
{
    public interface ISoftDelete : IEntity
    {
        bool IsDeleted { get; set; }
    }
}