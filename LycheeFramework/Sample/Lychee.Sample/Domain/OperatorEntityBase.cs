using Lychee.EntityFramework;

namespace Lychee.Sample.Domain
{
    public abstract class OperatorEntityBase : EntityBase
    {
        public int CreatedBy { get; set; }

        public int ChangedBy { get; set; }

        public virtual User CreatedUser { get; set; }

        public virtual User ChangedUser { get; set; }
    }
}