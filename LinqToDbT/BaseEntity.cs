using LinqToDB.Mapping;
using System;

namespace LinqToDbT
{
    public class BaseEntity : Entity<long>
    {
        public virtual User CreatedByUser { get; set; }

        public DateTime? CreatedWhen { get; set; }

        public virtual User ModifiedByUser { get; set; }

        public DateTime? ModifiedWhen { get; set; }

        public bool? IsDeleted { get; set; }

        [Column(IsDiscriminator = true)]
        public string Type { get; set; }
    }
}
