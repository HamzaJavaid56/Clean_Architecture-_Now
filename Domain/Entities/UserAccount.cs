using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserAccount : BaseEntity
    {
        public int UserId { get; set; }
        public double Balance { get; set; }
    }
}
