using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rd = TBotCore.Config.RawData;
namespace TBotCore.Core.Dialogs
{
    /// <summary>
    /// Allow buttons and dialogs pagination.
    /// </summary>
    class MultiButtonsDialog : PaginatedDialog
    {
        public MultiButtonsDialog(Rd.Dialog dialog, Dialog owner) : base(dialog, owner) { }
    }
}
