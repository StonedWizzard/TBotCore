using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rd = TBotCore.Config.RawData;
namespace TBotCore.Core.Dialogs
{
    /// <summary>
    /// This class only just a filler.
    /// Do the same logic as SerialDialog (show dialogs one by one and collect responses)
    /// but applies to registration process
    /// </summary>
    public sealed class RegistrationDialog : SerialDialog
    {
        public RegistrationDialog(Rd.Dialog dialog, Dialog owner) : base(dialog, owner) { }
    }
}
