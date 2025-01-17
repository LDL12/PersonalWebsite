using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Algorithm
{
    /// <summary>
    /// 回归直线 
    /// Y = b * X + a;
    /// b=(n∑xy-∑x∑y)÷[n∑x^2-(∑x)^2]
    /// a=(∑x^2∑y-∑x∑xy)÷[n∑x^2-(∑x)^2]
    /// </summary>
    public class LinearRegressionAlgorithm
    {
        /// <summary>
        /// 根据数组计算预测值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static decimal CalcPredictiveValue(decimal[] data)
        {
            if (data == null || data.Length <= 0)
            {
                throw new Exception("计算回归直线，数据列表不能为空！");
            }

            var n = data.Length;
            var sumY = 0M;
            var sumX = 0M;
            var sumXY = 0M;
            var sumXX = 0M;
            for (var i = 1; i <= n; i++)
            {
                sumX += i;
                sumY += data[i - 1];
                sumXY += i * data[i - 1];
                sumXX += i * i;
            }

            var b = (n * sumXY - sumX * sumY) / (n * sumXX - sumX * sumX);
            var a = (sumXX * sumY - sumX * sumXY) / (n * sumXX - sumX * sumX);

            var value = b * (n + 1) + a;
            return value < 0 ? 0 : value;
        }
    }
}
