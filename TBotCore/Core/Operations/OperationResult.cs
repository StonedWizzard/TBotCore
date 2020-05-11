using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Core.Operations
{
    /// <summary>
    /// Result of any operation
    /// </summary>
    struct OperationResult
    {
        /// <summary>
        /// Result of operation
        /// </summary>
        public readonly object Result;
        /// <summary>
        /// Exception message with additional info to put into console
        /// </summary>
        public readonly string ExceptionMessage;
        /// <summary>
        /// Result of operation
        /// </summary>
        public readonly OperationResultType ResultType;

        public OperationResult(object result, OperationResultType resultType, string msg = null)
        {
            Result = result;
            ResultType = resultType;
            ExceptionMessage = msg;
        }

        public enum OperationResultType
        {
            Unknown = 0x0,
            Success,
            Failed,
        }
    }
}
