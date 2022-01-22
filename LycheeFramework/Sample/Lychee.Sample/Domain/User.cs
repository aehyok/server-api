using Lychee.EntityFramework;

namespace Lychee.Sample.Domain
{
    public class User : EntityBase, ISoftDelete
    {
        public new int Id { get; set; }

        public bool IsDeleted { get; set; }
    }
}