using System;
using System.Collections.Generic;

namespace MyLib.Models
{
    public partial class Role
    {
        public Role()
        {
            AccountRole = new HashSet<AccountRole>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AccountRole> AccountRole { get; set; }
    }
}
