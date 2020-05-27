using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Db;

namespace TBotCore.Core.Operations
{
    /// <summary>
    /// Just operation filler to support serialization
    /// </summary>
    public sealed class OperationFiller : BaseOperation
    {
        public string OperationName { get; private set; }

        public OperationFiller(string name) : base (null)
        {
            OperationName = name;
            TelegramApi = null;
            RequiredArgsName = new List<string>();
        }

        public override async Task<OperationResult> Execute(IUser user)
        {
            return new OperationResult();
        }

        public override async Task<OperationResult> Execute(OperationArgs args)
        {
            return new OperationResult();
        }

        public override string ToString()
        {
            return OperationName;
        }
    }
}
