﻿using System;
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
using System.Diagnostics;

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
            Dialogs = BotManager.Core.Dialogs;
            CallbackDataParser = BotManager.Core.Repository.CreateCallbackParser();
        }

        /// <summary>
        /// Reads response and build UI for user
        /// consider user language, preferences and context
        /// </summary>
        public async Task HandleResponse(BotResponse response, long chatId)
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
            bool replaceMsg = Configs["ReplaceDialogs"].GetValue<bool>();
            int msgId = -1;

            #region service method(s)
            async Task<bool> SendBack()
            {
                try
                {
                    Dialog dialog =     // owner dialog is like step aback
                    response.Dialog.Owner == null ? Dialogs.RootDialog : response.Dialog.Owner;

                    OpArgs["Content"] = ProcessContent(TranslateContent(dialog.Content, userPreferences), dialog, IsAddHeader);
                    OpArgs["ReplyMarkdown"] = GetMarkup(dialog, response.User);
                    OperationResult rMsg = await Operations["SendMessageOperation"].Execute(new OperationArgs(response.User, OpArgs));
                    msgId = (int)rMsg.Result;

                    // set user context and return to upper dialog
                    ContextController.SetState(new UserContextState(response.User, dialog, msgId));
                    return true;
                }
                catch(Exception exp)
                {
                    BotManager.Core?.LogController?
                            .LogError(new DebugMessage("Can't show user error notification!", "Uidispatcher.SendBack()", exp));
                    return false;
                }
            }
            async Task<bool> SendToDialog()
            {
                try
                {
                    // work with serial dialogs here (?)

                    Dialog dialog = response.Dialog;
                    OpArgs["Content"] = ProcessContent(TranslateContent(dialog.Content, userPreferences), dialog, IsAddHeader);
                    OpArgs["ReplyMarkdown"] = GetMarkup(dialog, response.User);
                    OpArgs["MsgId"] = msgId = userContext.LastMsgId;

                    if (replaceMsg && msgId > 0)
                    {
                        OperationResult rMsg = await Operations["ReplaceMessageOperation"].Execute(new OperationArgs(response.User, OpArgs));
                        int? msgVal = rMsg.Result as int?;
                        msgId = msgVal == null ? msgId : msgVal.Value;
                    }
                    else
                    {
                        OperationResult rMsg = await Operations["SendMessageOperation"].Execute(new OperationArgs(response.User, OpArgs));
                        int? msgVal = rMsg.Result as int?;
                        msgId = msgVal == null ? msgId : msgVal.Value;
                    }

                    // set user context and return to upper dialog
                    ContextController.SetState(new UserContextState(response.User, dialog, msgId));
                    return true;
                }
                catch (Exception exp)
                {
                    BotManager.Core?.LogController?
                            .LogError(new DebugMessage("Can't show user error notification!", "Uidispatcher.SendToDialog()", exp));
                    return false;
                }
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
                    string content = TranslateContent((string)response.Data, userPreferences);
                    OpArgs["Content"] = content;
                    OperationArgs opArgs = new OperationArgs(response.User, OpArgs);

                    OperationResult result = await Operations["SendMessageOperation"].Execute(opArgs);
                    if (result.ResultType == OperationResult.OperationResultType.Failed || result.ResultType == OperationResult.OperationResultType.Unknown)
                    {
                        BotManager.Core?.LogController?
                            .LogError(new DebugMessage("Can't show user error notification!", "Uidispatcher.HandleResponse()", result.ExceptionMessage));

                        // nullify user context - we definitely have something uncommon
                        ContextController.ClearState(response.User);
                        return;                    // and do nothing. no menu and etc...
                    }
                }
                await Task.Delay(BotManager.Core.Configs.BasicDelay);

                // exception message sended succesfully, so display new one, above error
                await SendBack();

                // state changed, dialog showed, nothing more to do...
                return;
            }

            // dialog response recieved - redirect user to this dialog
            // and, of course change state
            if (response.Type == BotResponse.ResponseType.Dialog)
            {
                await SendToDialog();
            }
            // response is data so parse it and act accordingly
            else if(response.Type == BotResponse.ResponseType.Data)
            {
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
        private InlineKeyboardMarkup GetMarkup(Dialog dialog, IUser user)
        {
            // quiz dialogs - group by sortOrderVal!
            List<List<InlineKeyboardButton>> result = new List<List<InlineKeyboardButton>>();
            List<Dialog> dialogs = dialog.GetSubDialogs(ContextController.GetUserState(user).CurrentDialog);
            foreach(Dialog dia in dialogs)
            {
                List<InlineKeyboardButton> row = new List<InlineKeyboardButton>();
                InlineKeyboardButton btn = new InlineKeyboardButton();
                btn.CallbackData = CallbackDataParser.GetData(dia);
                btn.Text = dia.DisplayedName;

                row.Add(btn);
                result.Add(row);
            }

            // build support buttons
            // store them all in the end of list and group by 'position value'
            var btnsSet = dialog.SupportButtons.GroupBy(x => x.DisplayPriority);
            foreach(var btns in btnsSet)
            {
                List<InlineKeyboardButton> row = new List<InlineKeyboardButton>();
                foreach(Button b in btns)
                {
                    InlineKeyboardButton btn = new InlineKeyboardButton();
                    btn.CallbackData = CallbackDataParser.GetData(b);
                    btn.Text = b.DisplayedName;

                    row.Add(btn);
                }
                result.Add(row);
            }

            return new InlineKeyboardMarkup(result);
        }

        /// <summary>
        /// Return translated content using user prefs
        /// </summary>
        private string TranslateContent(string content, IUserPreferences prefs)
        {
            try
            {
                return Configs.TextStrings[prefs.Language, content];
            }
            catch (NullReferenceException) 
            {
                BotManager.Core?.LogController?.LogWarning(new DebugMessage($"String '{content}' (Lng={prefs.Language}) not found!", "UiDispatcher.TranslateContent()", $"UserId: {prefs.UserId}"));
            }
            catch (Exception exp) 
            {
                BotManager.Core?.LogController?.LogError(new DebugMessage($"{exp.Message}", "UiDispatcher.HandleResponse()", $"UserId: {prefs.UserId}"));
            }

            // if something happen - return raw string
            return content;
        }

        /// <summary>
        /// Build and add header to content (due to telegram restrictions)
        /// </summary>
        private string ProcessContent(string content, Dialog dialog, bool header)
        {
            if (header == false)
                return content;
            else
            {
                if (dialog == null)
                    return $"???\r\n\n{content}";

                return $"{dialog.Path}\r\n\n{content}";
            }
        }
    }
}
