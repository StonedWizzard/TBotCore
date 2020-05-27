using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Db
{
    public interface IUserRole
    {
        public int Id { get; set; }

        /// <summary>
        /// Foreign key in users collection
        /// </summary>
        public int UserId { get; set; }
    }
}
