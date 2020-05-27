using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rd = TBotCore.Config.RawData;

namespace TBotCore.Core.Dialogs
{
    /// <summary>
    /// Allow dialog show it's content paginated.
    /// </summary>
    public class PaginatedDialog : Dialog
    {
        List<string> _Content;
        /// <summary>
        /// Hide original Content property and displace it by List<string>
        /// </summary>
        public new IEnumerable<string> Content
        {
            get => _Content.ToImmutableList();
            protected set => _Content = value.ToList();
        }

        public PaginatedDialog(Rd.Dialog dialog, Dialog owner) : base(dialog, owner) 
        {
            Content = new List<string>();
            foreach (string msg in dialog.Message)
                _Content.Add(msg);

            if (Content.Count() == 0) _Content.Add("");
        }

        public string GetContent(int page = 0)
        {
            throw new NotImplementedException();
        }


        public new EditablePaginatedDialog GetEditable()
        {
            return new EditablePaginatedDialog(this);
        }

        public class EditablePaginatedDialog : Dialog.EditableDialog
        {
            public new PaginatedDialog EditableObject { get; private set; }

            public EditablePaginatedDialog(PaginatedDialog owner) : base (owner)
            { EditableObject = owner; }

            public new List<string> Content
            {
                get => EditableObject._Content;
                set => EditableObject._Content = value;
            }
        }
    }
}
