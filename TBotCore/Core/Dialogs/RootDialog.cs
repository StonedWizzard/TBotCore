using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rd = TBotCore.Config.RawData;
namespace TBotCore.Core.Dialogs
{
    /// <summary>
    /// Root dialog - start point of any bot.
    /// Don't add any logic, just filler.
    /// </summary>
    public sealed class RootDialog : Dialog
    {
        public RootDialog(Rd.Dialog dialog) : base(dialog, null) { Path = RootPath; }
    }
}
