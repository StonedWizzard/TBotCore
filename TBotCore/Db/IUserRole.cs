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
        public long UserId { get; set; }

        /// <summary>
        /// Integer value recieved and transformed to Enum<int>
        /// on client side by validator
        /// </summary>
        public int Role { get; set; }

        /// <summary>
        /// String with separeted access tags, wich processed by validator
        /// when user try access to dialog or operation
        /// </summary>
        public string AccessTags { get; set; }

        public List<string> GetAccessTags(char separator = ';');
    }
}
