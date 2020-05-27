using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TBotCore.Core.Dialogs;
using TBotCore.Core.Operations;
using TBotCore.Debug;
using TBotCore.Config;
using TBotCore.Core.Data;
using TBotCore.Db;

using Telegram.Bot.Types.ReplyMarkups;

namespace TBotCore.Core
{
    /// <summary>
    /// Translate dialogs (from response) into telegram messages
    /// </summary>
    public class UIDispatcher
    {
        protected readonly BotConfigs Configs;
        protected readonly UserInputContextController ContextController;
        protected readonly OperationsContainer Operations;
        protected readonly DialogsProvider Dialogs;
        protected readonly DataParser CallbackDataParser;

        public UIDispatcher(UserInputContextController uicc)
        {
            // just setup references 
            ContextController = uicc;
            Configs = BotManager.Core.Configs;
            Operations = BotManager.Core.Operations;
            CallbackDataParser = BotManager.Core.Repository.CreateCallbackParser();
        }

        /// <summary>
        /// Reads response and build UI for user
        /// consider user language, preferences and context
        /// </summary>
        public async void HandleResponse(BotResponse response, long chatId)
        {
            // get user preferences first
            IUserPreferences userPreferences = response.User.UserPreferences;
            UserContextState userContext = ContextController.GetUserState(response.User);

            // set dictionary of 
            Dictionary<string, object> OpArgs = new Dictionary<string, object>();
            OpArgs.Add("ChatId", chatId);           // chat id is setted here and unchangeble!
            OpArgs.Add("Content", null);
            OpArgs.Add("ReplyMarkdown", null);

            bool IsAddHeader = Configs["ShowDialogPath"].GetValue<bool>();
            int msgId = -1;
            bool replaceMsg = true;

            #region service method(s)
            async Task SendBack()
            {
                Dialog dialog =     // owner dialog is like step aback
                    response.Dialog.Owner == null ? Dialogs.RootDialog : response.Dialog.Owner;

                OpArgs["Content"] = ProcessContent(TranslateContent(dialog.Content, userPreferences), dialog, IsAddHeader);
                OpArgs["ReplyMarkdown"] = GetMarkup(dialog, response.User);
                OperationResult rMsg = await Operations["SendMessageOperation"].Execute(new OperationArgs(response.User, OpArgs));
                msgId = (int)rMsg.Result;

                // set user context and return to upper dialog
                ContextController.SetState(new UserContextState(response.User, dialog, msgId));
            }
            async Task SendToDialog()
            {
                Dialog dialog = response.Dialog;
                OpArgs["Content"] = ProcessContent(TranslateContent(dialog.Content, userPreferences), dialog, IsAddHeader);
                OpArgs["ReplyMarkdown"] = GetMarkup(dialog, response.User);
                OperationResult rMsg = await Operations["SendMessageOperation"].Execute(new OperationArgs(response.User, OpArgs));
                msgId = (int)rMsg.Result;

                // set user context and return to upper dialog
                ContextController.SetState(new UserContextState(response.User, dialog, msgId));
            }
            #endregion

            // handle exception
            if (response is BotExceptionResponse)
            {
                var rsp = response as BotExceptionResponse;
                // print warning message to log
                BotManager.Core?.LogController?
                    .LogWarning(new DebugMessage("Exception occured during dialog execution!", "Uidispatcher.HandleResponse()", rsp.Message));

                // print response to user if responce allow it
                if (rsp.ShowNotification)
                {
                    OpArgs["Content"] = TranslateContent((string)response.Data, userPreferences);
                    OperationArgs opArgs = new OperationArgs(response.User, OpArgs);

                    OperationResult result = await Operations["SendMessageOperation"].Execute(opArgs);
                    if (result.ResultType == OperationResult.OperationResultType.Failed || result.ResultType == OperationResult.OperationResultType.Unknown)
                    {
                        BotManager.Core?.LogController?
                            .LogError(new DebugMessage("Can't show user error notification!", "Uidispatcher.HandleResponse()", result.ExceptionMessage));

                        // nullify user context - we definitely have something uncommon
                        ContextController.ClearState(response.User);
                        return;             // and do nothing. no menu and etc...
                    }
                }

                await Task.Delay(BotManager.Core.Configs.BasicDelay);

                // exception message sended succesfully, so display new one, above error
                await SendBack();

                // state changed, dialog showed, nothing more to do...
                return;
            }

            // <<handle other responses>>
            await Task.Delay(BotManager.Core.Configs.BasicDelay);

            // dialog response recieved - redirect user to this dialog
            // and, of course change state
            if (response.Type == BotResponse.ResponseType.Dialog)
            {
                if (response.Dialog == null)
                    await SendBack();
                else
                    await SendToDialog();
            }
            // response is data so parse it and act accordingly
            else if(response.Type == BotResponse.ResponseType.Data)
            {
                // how act????
                // i know how, bitch)

                // first transform raw data
                CallbackData callback = CallbackDataParser.ParseData((string)response.Data);

                // next - do something with this callback
                // supose make some separete method - too much logic here...
            }
            // response is plain text, so display it and keep state unchanged
            else if(response.Type == BotResponse.ResponseType.Text)
            {
                // check dialog - it should stay same cos it's just a text wich placed above
                Dialog dialog = userContext.CurrentDialog;
                OpArgs["Content"] = ProcessContent(TranslateContent((string)response.Data, userPreferences), dialog, false);
                OpArgs["ReplyMarkdown"] = null;
                OperationResult rMsg = await Operations["SendMessageOperation"].Execute(new OperationArgs(response.User, OpArgs));
                msgId = (int)rMsg.Result;

                // set user context and return to upper dialog
                ContextController.SetState(new UserContextState(response.User, dialog, msgId));
            }
            else
            {
                // response is null or exception type, witch is wrong
                // make log entry, send user back and change user state
                BotManager.Core?.LogController?.LogWarning(new DebugMessage("Response can't be processed!", "UiDispatcher.HandleResponse()", 
                    $"UserId: {response.User.UserId}"));

                await SendBack();

                // state changed, dialog showed, nothing more to do...
                return;
            }
        }


        /// <summary>
        /// Build markup depends on dialog
        /// </summary>
        private IReplyMarkup GetMarkup(Dialog dialog, IUser user)
        {
            // quiz dialogs - group by sortOrderVal
            // pagination works by pageState from userState (or first by default)


            // anyway build support buttons

            return null;
        }

        /// <summary>
        /// Return translated content using user prefs
        /// </summary>
        private string TranslateContent(string content, IUserPreferences prefs)
        {
            return Configs.TextStrings[prefs.Language, content];
        }

        /// <summary>
        /// Build and add header to content (due to telegram restrictions)
        /// </summary>
        private string ProcessContent(string content, Dialog dialog, bool header)
        {
            if (header) return content;
            else
            {
                if (dialog == null)
                    return $"*>???\r\n{content}";

                string result = content;
                string path = "";
                Dialog dia = dialog;

                // build header
                do
                {
                    path = $"{dia.DisplayedName}/{path}";
                    dia = dia.Owner;
                }
                while (dia != null);
                result = $"*>{path}\r\n{content}";

                return result;
            }
        }
    }
}
