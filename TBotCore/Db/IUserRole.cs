using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Db
{
    interface IUserRole
    {
        /// <summary>
        /// Foreign key in users collection
        /// </summary>
        public int UserDbId { get; set; }
    }
}
