using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rd = TBotCore.Config.RawData;
using TBotCore.Core.Data;
using TBotCore.Db;
using TBotCore.Editor;

namespace TBotCore.Core.Dialogs
{
    /// <summary>
    /// Class represent service telegram buttons like back, forward, to main...
    /// That kind of buttons can't lost by telegrams button request limits.
    /// </summary>
    public class Button : IButton, IEditable<Button.EditableButton>
    {
        public string Id { get; protected set; }
        public string DisplayedName { get; protected set; }
        public string Data { get; protected set; }
        public int DisplayPriority { get; protected set; }

        /// <summary>
        /// Do something and returns back
        /// </summary>
        public async virtual Task<BotResponse> Execute(IUser user)
        {
            await Task.Delay(BotManager.Core.Configs.BasicDelay);
            if (ValidateUser(user.UserRole))
                return new BotResponse(Data, BotResponse.ResponseType.Data, user);

            return new BotResponse("txt_accessDenied", BotResponse.ResponseType.Exception, user);
        }

        /// <summary>
        /// Base buttons are public.
        /// Need override if requires some extended validatiob
        /// </summary>
        public bool ValidateUser(IUserRole role)
        {
            return true;
        }


        public Button(Rd.Button button)
        {
            if (String.IsNullOrEmpty(button.Id))
                throw new ArgumentNullException("Id");
            if (button.Id.IsBloated(16))
                throw new Exception($"Id value lenght cant exceed size of 16 chars!\r\nDialog/Button Id = '{button.Id}'");

            if (button.Data.IsBloated(26))
                throw new Exception($"Data lenght cant exceed size of 26 chars!\r\nDialog/Button Id = '{button.Id}'");

            Id = button.Id;
            DisplayedName = button.Name;
            Data = button.Data;
            DisplayPriority = button.DisplayPriority;
        }

        public override string ToString() => Id;


        public EditableButton GetEditable()
        {
            return new EditableButton(this);
        }

        public class EditableButton : IEntityEditor<Button>
        {
            public Button EditableObject { get; private set; }

            public EditableButton(Button owner) { EditableObject = owner; }


            public string Id
            {
                get => EditableObject.Id;
                set => EditableObject.Id = value;
            }
            public string DisplayedName
            {
                get => EditableObject.DisplayedName;
                set => EditableObject.DisplayedName = value;
            }
            public string Data
            {
                get => EditableObject.Data;
                set => EditableObject.Data = value;
            }
            public int DisplayPriority
            {
                get => EditableObject.DisplayPriority;
                set => EditableObject.DisplayPriority = value;
            }
        }
    }
}
