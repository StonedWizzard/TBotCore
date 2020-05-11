using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rd = TBotCore.Config.RawData;
namespace TBotCore.Core.Dialogs
{
    /// <summary>
    /// Root dialog. Can be onhe and only one!
    /// It's like start point for bot conversation
    /// </summary>
    sealed class RootDialog : MultiButtonsDialog
    {

        public RootDialog(Rd.Dialog dialog) : base(dialog, null)
        {

        }
    }
}
