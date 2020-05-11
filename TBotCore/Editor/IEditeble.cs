using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Editor
{
    /// <summary>
    /// Provide methods to edit work-on data througt editor
    /// Apply it to work-on data class
    /// </summary>
    interface IEditable<T>
    {
        T GetEditable();
    }
}
