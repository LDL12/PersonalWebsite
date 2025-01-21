using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Common.Result
{
    public class Result<T> : IResult<T>
    {
        /// <summary>
        /// 状态码
        /// 1xx，成功状态码，无特殊要求填写100即可
        /// 2xx，失败状态码，无特殊要求填写200即可
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 异常
        /// </summary>
        [JsonIgnore]
        public Exception Exception { get; set; }

        /// <summary>
        /// 返回值
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 成功
        /// </summary>
        public bool IsSuccess => Code >= 100 && Code < 200;

        /// <summary>
        /// 构造成功结果
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result<T> WithSuccess(T data)
        {
            return new Result<T>()
            {
                Code = 100,
                Data = data,
            };
        }

        /// <summary>
        /// 构造失败结果
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static Result<T> WithError(string message)
        {
            return new Result<T>
            {
                Code = 200,
                Message = message,
                Exception = new Exception(message),
            };
        }

        /// <summary>
        /// 构造失败结果
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static Result<T> WithError(Exception exception)
        {
            return new Result<T>
            {
                Code = 200,
                Message = exception.Message,
                Exception = exception,
            };
        }

    }
}
