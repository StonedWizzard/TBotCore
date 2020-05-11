using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rd = TBotCore.Config.RawData;

namespace TBotCore.Core.Dialogs
{
    /// <summary>
    /// Represent series of question (witch actualy dialogs returnig user input)
    /// then collect responses from user in to array and process data (and ofcourse display result).
    /// </summary>
    class QuizSeriesDialog : DialogContainer, IUserResponseAwaiter
    {
        public QuizSeriesDialog(Rd.Dialog dialog, Dialog owner) : base(dialog, owner) { }
    }
}

