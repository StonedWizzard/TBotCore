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
using TBotCore.Core.Dialogs;

namespace TBotCore.Core
{
    /// <summary>
    /// Telegram api wrapper (above one more wrapper;) )
    /// </summary>
    public class BotAPI
    {
        public readonly TelegramBotClient Api;
        public readonly UserInputContextController ContextController;
        public readonly BaseUserController UsersController;

        protected readonly BotConfigs Configs;
        protected readonly UIDispatcher UiController;
        protected readonly DialogsProvider Dialogs;
        protected readonly ChatCommandsProvider Commands;
        protected readonly DataParser DataParser;

        public readonly User BotInfo;
        public string BotName { get; private set; }

        public BotAPI(Proxy proxy = null)
        {
            Configs = BotManager.Core.Configs;
            Dialogs = BotManager.Core.Dialogs;
            Commands = BotManager.Core.Commands;

            ContextController = new UserInputContextController();
            UiController = BotManager.Core.Repository.CreateUiDispatcher(ContextController);
            UsersController = BotManager.Core.Repository.CreateUserController();
            DataParser = new DataParser();

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
                ContextController.SetIgnored(BotInfo.Id);

                sw.Stop();
                BotManager.Core?.LogController?
                    .LogSucces(new DebugMessage($"Conection is cuccess (response time = {sw.Elapsed})\r\n--> Bot Api initialized and ready!"));
            }
            catch(Exception e)
            {
                BotManager.Core?.LogController?.LogCritical(new DebugMessage("Api initialization failed!", "BotApi()", e));
            }
            finally 
            {
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


        protected async void Api_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            // first awaits some time to prevent flooding
            await Task.Delay(Configs.BasicDelay);

            CallbackData data = DataParser.ParseData(e.CallbackQuery.Data);
            IUser user = await UsersController.GetOrCreateUser(e.CallbackQuery.From);
            BotResponse response = new BotResponse(user);

            // lock the context, so user cant spam thousand messages
            // while some operations execute!
            if (ContextController.GetUserState(user)?.OccupieContext() == false) return;
            var userState = ContextController.GetUserState(user);

            // juat show dialog, stored in button
            if (data.T == CallbackData.ContentTypeEnum.Dialog)
            {
                response = Dialogs.GetDialog(data.Id, user);
            }
            // service button pressed, there can be stored and operation
            // and data, retranslated to user input (used in serial dialogs)
            else if(data.T == CallbackData.ContentTypeEnum.Button)
            {
                Dialog currentDia = ContextController.GetUserState(user).CurrentDialog;
                var Operation = BotManager.Core.Operations.GetOperation(data.D);
                BotRequest request = new BotRequest();

                // data is operation - execute it
                if (Operation != null)
                {
                    var result = await BotManager.Core.Operations[data.D]
                        .Execute(new Operations.OperationArgs(user, new Dictionary<string, object> { { "CurrentDialog", currentDia } }));
                    if(result.ResultType == Operations.OperationResult.OperationResultType.Failed)
                    {
                        // put error to console
                        BotManager.Core?.LogController?
                            .LogError(new DebugMessage("Operation '{}' fails!", "BotApi.Api_OnCallbackQuery()", result.ExceptionMessage));
                    }
                    response = result.Result as BotResponse;

                    // if data as string == null, so it be null
                    // request here used to support input awaiting
                    request = new BotRequest(response?.Data as string, BotRequest.RequestType.CallbackData, user);

                    // something wrong, go to root and notify user
                    if (response == null)
                    {
                        // notify someone (DoTo)
                        response = new BotResponse(null, BotResponse.ResponseType.Dialog, user, currentDia);
                    }
                }
                // data, just data
                // expected using in serial dialog!
                else
                    request = new BotRequest(data.D, BotRequest.RequestType.CallbackData, user);
                
                // if input awited - put response to user cache
                // and show next dialog (provided by AddRequest method)
                if (userState.CurrentState == UserContextState.ContextState.AwaitInput)
                    response = ContextController.AddRequest(user, request);
            }

            await UiController.HandleResponse(response, e.CallbackQuery.Message.Chat.Id);
            ContextController.GetUserState(user).RealiseContex();
        }


        protected async void Api_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            // first awaits some time to prevent flooding
            await Task.Delay(Configs.BasicDelay);

            // collect message info
            // and bind tgUser with dbUser
            Message msg = e.Message;
            User tgUser = e.Message.From;
            IUser user = await UsersController.GetOrCreateUser(tgUser);
            BotResponse response = new BotResponse(user);

            // lock the context, so user cant spam thousand messages
            // while some operations execute!
            if(ContextController.GetUserState(user)?.OccupieContext() == false) return;


            // First of all checks if user registered in db or wherever else
            if (user.IsRegistered == false)
            {
                // all required register actions done in method...
                response = await DoRegistration(user);
                await UiController.HandleResponse(response, msg.Chat.Id);
                ContextController.GetUserState(user).RealiseContex();
                return;
                // ...just await and exit from here
            }


            // checks from chat or from user sended message
            // and call according methods for handling input
            if(!IsChat())             // <<--- Msg from user
            {
                var userState = ContextController.GetUserState(user);
                var handledInput = HandleUserInputMessage(msg, user);

                // if user didn't start any conversation yet
                if (userState.CurrentDialog == null)
                    response = await Dialogs.RootDialog.Execute(user);

                else if (handledInput.Type == BotRequest.RequestType.Command)
                {
                    // command detected - breack context, build new and execute it

                    // something like stub
                    // change later!!!!!!!!!!!!!!!!!!!!!!
                    // nothing to do, just send current user dialog
                    response = new BotResponse(null, BotResponse.ResponseType.Dialog, user, userState.CurrentDialog);

                }
                else if (userState.CurrentState == UserContextState.ContextState.AwaitInput)
                    response = ContextController.AddRequest(user, handledInput);
                else
                {
                    // nothing to do, just send current user dialog
                    response = new BotResponse(null, BotResponse.ResponseType.Dialog, user, userState.CurrentDialog);
                }
            }

            // message comes from chat, 
            // so handle it in another way
            else           // <<--- Msg from chat
            {
                throw new NotImplementedException("ToDo!");
                // >>>>>>>>>>>>>>>>>>>ToDo<<<<<<<<<<<<<<<<<<<<<
            }

            // display whatewer results come from response
            // user state updates there
            await UiController.HandleResponse(response, msg.Chat.Id);
            ContextController.GetUserState(user).RealiseContex();
            // next message can be handled again

            #region microservice methods
            // some inline service methods
            bool IsChat() => msg.From.Id != msg.Chat.Id;
            #endregion
        }

        /// <summary>
        /// transforms user input to request
        /// </summary>
        public virtual BotRequest HandleUserInputMessage(Message msg, IUser user)
        {
            bool isEmpty = String.IsNullOrEmpty(msg.Text) && String.IsNullOrEmpty(msg.Caption);
            BotRequest request = new BotRequest();

            // no text to handle - no money, no honey...
            if (isEmpty)
                return request;

            string txt = !String.IsNullOrEmpty(msg.Text) ? msg.Text : msg.Caption;

            // parse message if it command, because them has hiegher priority
            // and able even interrupt serial dialogs!
            var command = Commands.GetCommand(txt, user);
            if(command != null)
            {
                // it's realy command, create request
                string commData = txt;  // ToDo - may be parse it in some way?
                request = new BotRequest(commData, BotRequest.RequestType.Command, user, msg);
            }
            else
                // ...just text
                request = new BotRequest(txt, BotRequest.RequestType.TextMessage, user, msg);

            return request;
        }

        /// <summary>
        /// In case if user new to bot - call registration procedures
        /// </summary>
        private async Task<BotResponse> DoRegistration(IUser user) => await Dialogs.RegistrationDialog.Execute(user);
    }
}
