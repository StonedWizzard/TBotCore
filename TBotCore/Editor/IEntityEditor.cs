using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Editor
{
    /// <summary>
    /// Editable entity. Apply to it, not to work-on data class!
    /// </summary>
    interface IEntityEditor<TOwner>
    {
        TOwner EditableObject { get; }
    }
}
