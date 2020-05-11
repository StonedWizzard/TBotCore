using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Core.Dialogs
{
    /// <summary>
    /// Interface allow store in dialog other dialogs 
    /// </summary>
    interface IDialogsContainer
    {
        List<Dialog> Dialogs { get; }
    }
}
