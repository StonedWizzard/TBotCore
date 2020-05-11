using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rd = TBotCore.Config.RawData;

namespace TBotCore.Core.Dialogs
{
    /// <summary>
    /// Allow dialog content pagination.
    /// </summary>
    class MultiPageDialog : PaginatedDialog
    {
        public new List<string> Content;

        public MultiPageDialog(Rd.Dialog dialog, Dialog owner) : base(dialog, owner) { }
    }
}
