using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Debug;

namespace SampleTgBot.DB
{
    /// <summary>
    /// Incapsulate inside data base creation, connections and other stuff
    /// </summary>
    class DbInitializer
    {
        readonly IDebuger Log;

        public DbInitializer()
        {
            Log = Program.Log;
        }

        /// <summary>
        /// Manage DbConnection - set and check connection. 
        /// create new Db if instance not exist
        /// </summary>
        public bool InitializeConnection()
        {
            bool result = false;
            using AppDbContext dbContext = new AppDbContext();
            var dbConnection = dbContext.Database.Connection;

            // check db existance and create if it absent
            if(!dbContext.Database.Exists())
            {
                Log.LogWarning(new DebugMessage($"DB '{dbConnection.Database}' is not found! Create new..."));
                try
                {
                    dbContext.Database.Create();
                    Log.LogSucces(new DebugMessage($"Db '{dbConnection.Database}' created!"));
                }
                catch (Exception e)
                {
                    Log.LogError(new DebugMessage($"Can't create new instance of Db '{dbConnection.Database}'!\r\n" +
                    $"ConnectionString: {dbConnection.ConnectionString}\r\nDataSource: {dbConnection.DataSource}", "InitializeConnection()", e));
                    return false;
                }
            }

            //check connection
            Log.LogSystem(new DebugMessage($"Check Db connection..."));
            try
            {
                dbContext.Database.Connection.Open();
                if (dbContext.Database.Connection.State == ConnectionState.Open)
                {
                    dbContext.Database.Connection.Close();
                    Log.LogSucces(new DebugMessage($"Db '{dbConnection.Database}' connection established!"));
                    result = true;
                }
            }
            catch(Exception e)
            {
                Log.LogError(new DebugMessage($"Can't connect to Db: '{dbConnection.Database}'!\r\n" +
                    $"ConnectionString: {dbConnection.ConnectionString}\r\nDataSource: {dbConnection.DataSource}",
                    "InitializeConnection()", e));
            }

            return result;
        }
    }
}
