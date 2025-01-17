using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Result
{
    public interface IResult<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        int Code { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        Exception Exception { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        T Data { get; set; }
    }
}
