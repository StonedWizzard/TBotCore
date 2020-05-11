using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rd = TBotCore.Config.RawData;

namespace TBotCore.Core.Dialogs
{
    /// <summary>
    /// Basic abstract class for all paginateble or large-contented dialogs.
    /// </summary>
    abstract class PaginatedDialog : DialogContainer
    {
        public PaginatedDialog(Rd.Dialog dialog, Dialog owner) : base(dialog, owner) { }
    }
}
