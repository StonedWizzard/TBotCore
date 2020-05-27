using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

using TBotCore.Debug;
using TBotCore.Config;
using TBotCore.Core.Data;
using TBotCore.Db;

namespace TBotCore.Core
{
    /// <summary>
    /// Telegram api wrapper (above one more wrapper;) )
    /// </summary>
    public class BotAPI
    {
        public readonly TelegramBotClient Api;

        protected readonly BotConfigs Configs;
        protected readonly UserInputContextController ContextController;
        protected readonly UIDispatcher UiController;
        protected readonly BaseUserController UsersController;
        protected readonly DialogsProvider Dialogs;
        protected readonly ChatCommandsProvider CommandsProvider;

        public readonly User BotInfo;
        public string BotName { get; private set; }

        public BotAPI(Proxy proxy = null)
        {
            Configs = BotManager.Core.Configs;
            Dialogs = BotManager.Core.Dialogs;

            ContextController = new UserInputContextController();
            UiController = BotManager.Core.Repository.CreateUiDispatcher(ContextController);
            UsersController = BotManager.Core.Repository.CreateUserController();

            // start telegram api initializing
            HttpClient httpClient = null;
            try
            {
                BotManager.Core?.LogController?.LogSystem(new DebugMessage($"Start initializing Telegram Bot Api...\r\nUse proxy: {Configs.UseProxy}"));

                if (Configs.UseProxy)
                {
                    BotManager.Core?.LogController?.LogImportant(new DebugMessage($"Proxy enabled. Use proxy: <<xxx>>"));
                    // init httpClient
                    httpClient = new HttpClient();

                    BotManager.Core?.LogController?.LogImportant(new DebugMessage($"Proxy NOT IMPLEMENTED"));
                }

                Api = new TelegramBotClient(Configs.BotHash, httpClient);
                Api.OnMessage += Api_OnMessage;
                Api.OnCallbackQuery += Api_OnCallbackQuery;
                Api.OnInlineQuery += Api_OnInlineQuery;
                Api.OnReceiveError += Api_OnReceiveError;
                Api.OnReceiveGeneralError += Api_OnReceiveGeneralError;
                Api.OnUpdate += Api_OnUpdate;

                // here we get some useful bot info and at the same time
                // checks if connection established or not.
                BotManager.Core?.LogController?.LogImportant(new DebugMessage($"Conect Api client with bot token: <<{Configs.BotHash}>> ..."));
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                BotInfo = Api.GetMeAsync().Result;
                BotName = BotInfo.Username;

                sw.Stop();
                BotManager.Core?.LogController?
                    .LogSucces(new DebugMessage($"Conection is cuccess (response time - {sw.Elapsed})\r\n--> Bot Api initialized and ready!"));
            }
            catch(Exception e)
            {
                // log critical error
                // invoke LogCritical implying app shutdown!
                BotManager.Core?.LogController?.LogCritical(new DebugMessage("Api initialization failed!", "BotApi()", e));
            }
            finally 
            {
                // IS CORRECT???
                // what if destroy client when all is Ok????
                httpClient?.Dispose();
            }
        }


        public void StartReceiving()
        {
            BotManager.Core?.LogController?.LogImportant(new DebugMessage($"Telegram bot start receiving messages!"));
            Api.StartReceiving();
        }

        public void StopReceiving()
        {
            BotManager.Core?.LogController?.LogImportant(new DebugMessage($"Telegram bot stop receiving messages!"));
            Api.IsReceiving = false;
        }

        #region
        protected void Api_OnUpdate(object sender, Telegram.Bot.Args.UpdateEventArgs e)
        {
            
        }

        protected void Api_OnReceiveGeneralError(object sender, Telegram.Bot.Args.ReceiveGeneralErrorEventArgs e)
        {
           
        }

        protected void Api_OnReceiveError(object sender, Telegram.Bot.Args.ReceiveErrorEventArgs e)
        {
            
        }
        #endregion

        protected void Api_OnInlineQuery(object sender, Telegram.Bot.Args.InlineQueryEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected void Api_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected async void Api_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            // first awaits some time to prevent flooding
            await Task.Delay(Configs.BasicDelay);

            // collect message info
            // and bind tgUser with dbUser
            User tgUser = e.Message.From;
            IUser user = null;
            try
            {
                user = UsersController.ConvertUser(tgUser);
            }
            catch (InvalidCastException exp)
            {
                BotManager.Core?.LogController?.LogError(new DebugMessage("Can't cast user type!\r\nApi call was cancelled.", "Api_OnMessage()", exp));
                return; // breake this api_call
            }
            Message msg = e.Message;

            // First of all checks if user registered in db or wherever else
            if (await UsersController.IsUserExist(tgUser.Id) == false)
            {
                // all required register actions done in method...
                await CheckRegistrationState(user);
                return;
                // ...just await and exit from here
            }

            // initialize empty response and request variables
            BotResponse response = new BotResponse(user);

            // checks from chat or from user sended message
            // and call according methods for handling input
            if(msg.From.Id == msg.Chat.Id)              // <<--- Msg from user
            {
                var handledInput = HandleUserInputMessage(msg, user);
                if (handledInput.Item1 == false)
                {
                    // send user message where says that his input can't parse
                    response = new BotExceptionResponse($"Can't parse user input! User: {user.Id} Input: {msg}", "txt_parseUserInputException", user);
                }
                else
                {
                    // if user didn't start any conversation yet
                    // or start any but not await users response
                    var userState = ContextController.GetUserState(tgUser.Id);
                    if (userState == null)
                    {
                        // conversation never starts in this session
                        // create context, display root dialog
                        response = await Dialogs.RootDialog.Execute(user);
                    }
                    else if(handledInput.Item2.Type == BotRequest.RequestType.Command)
                    {
                        // command detected - breack context, build new and execute it
                    }
                    else if (userState.CurrentState == UserContextState.ContextState.AwaitInput)
                    {
                        // bot awaits user response...
                        // so here we going to collect data
                        response = ContextController.AddRequest(handledInput.Item2);
                    }
                    else
                    {
                        // nothing to do, just send current user dialog
                        response = new BotResponse(null, BotResponse.ResponseType.Dialog, user, userState.CurrentDialog);
                    }
                }
            }

            // message comes from chat, 
            // so handle it in another way
            else           // <<--- Msg from chat
            {
                throw new NotImplementedException();
                // >>>>>>>>>>>>>>>>>>>ToDo<<<<<<<<<<<<<<<<<<<<<
            }

            // display whatewer results come from response
            // user state updates there
            UiController.HandleResponse(response, msg.Chat.Id);
        }

        /// <summary>
        /// Handles user input and converts it to request
        /// </summary>
        public virtual (bool, BotRequest) HandleUserInputMessage(Message msg, IUser user)
        {
            bool result = String.IsNullOrEmpty(msg.Text) && String.IsNullOrEmpty(msg.Caption);
            BotRequest request = new BotRequest();

            // no text to handle - no money, no honey...
            if (result)
                return (!result, request);

            string txt = !String.IsNullOrEmpty(msg.Text) ? msg.Text : msg.Caption;

            // parse message if it command, because them has hiegher priority
            // and able even interrupt serial dialogs!
            var command = CommandsProvider.GetCommand(txt, user);
            if(command.Item1)
            {
                // it's realy command, create request
                string commData = txt;  // ToDo - may be parse it in some way?
                request = new BotRequest(commData, BotRequest.RequestType.Command, user, msg);
            }
            else
            {
                // ...just text
                request = new BotRequest(txt, BotRequest.RequestType.TextMessage, user, msg);
            }

            return (result, request);
        }

        /// <summary>
        /// Controlls user registration.
        /// In case if user new to bot - call registration procedures
        /// </summary>
        private async Task CheckRegistrationState(IUser user)
        {
            if (await UsersController.CreateUser(user))
            {
                // user just registered so show him just root dialog
                BotResponse response = await Dialogs.RootDialog.Execute(user);

                // user in db created, provide additional registration steps
                if (!user.IsRegistered && Configs["IsExtendedRegistration"].GetValue<bool>())
                {
                    // start additional registration steps...
                    if (Dialogs.RegistrationDialog != null)
                    {
                        response = await Dialogs.RegistrationDialog.Execute(user);
                    }
                    // ...except it not missed
                    else
                    {
                        await UsersController.CompleateRegistration(user);
                        BotManager.Core?.LogController?.LogWarning(new DebugMessage("Custom registration steps seems missed!", "BotApi:OnMessage",
                            $"UserId: {user.UserId}, UserName: {user.UserName}"));
                    }
                }

                // display results and set state
                // user.Id the same thing as chatId
                UiController.HandleResponse(response, user.Id);
                return;
            }
            else
            {
                BotManager.Core?.LogController?.LogError(new DebugMessage("The user creation in db failed", "BotApi:OnMessage",
                    $"UserId: {user.UserId}, UserName: {user.UserName}"));

                // nothing more to do - escape from method;
                // inform user about error?
                await Task.Delay(Configs.BasicDelay);
                return;
            }
        }
    }
}
