using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Result
{
    public class Result<T> : IResult<T>
    {
        public int Code { get; set; }

        public Exception Exception { get; set; }

        public T Data { get; set; }

        /// <summary>
        /// 成功
        /// </summary>
        public bool IsSuccess => Code == 1;

        public static Result<T> WithError(string message, Exception exception = null)
        {
            return new Result<T>
            {
                
            };
        }
    }
}
