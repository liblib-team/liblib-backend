using System;
using System.Collections.Generic;

namespace MyLib.Models
{
    public partial class Log
    {
        public Guid Id { get; set; }
        public string Action { get; set; }
        public Guid AccountId { get; set; }

        public virtual Account Account { get; set; }
    }
}
