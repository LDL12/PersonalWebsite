using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Algorithm
{
    /// <summary>
    /// 三次指数平滑
    /// </summary>
    public class TripleExponentialSmoothingAlgorithm
    {
        /// <summary>
        /// 计算预测值（通过遍历0-1选取一个合适的alpha）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="increment"></param>
        /// <returns></returns>
        public static decimal CalcAccuratePredictiveValue(decimal[] data, decimal increment = 0.1M)
        {
            if (data == null || data.Length <= 0)
            {
                return 0M;
            }

            //如果数据较少,直接取平均
            if (data.Length < 3)
            {
                return data.Sum() / data.Length;
            }

            var alphaRate = new Dictionary<decimal, decimal>();
            for (var i = increment; i < 1M; i += increment)
            {
                //计算预测值
                var list = TripleExponentialSmoothing(data, i);//三次指数平滑计算

                //计算误差
                var a = 0M;
                for (var t = 3; t < data.Length; t++)
                {
                    var st1 = list.Item1[t];//一次指数平滑数组
                    var st2 = list.Item2[t];//二次指数平滑数组
                    var st3 = list.Item3[t];//三次指数平滑数组
                    var predictiveValue = CalcPredictiveValue(st1, st2, st3, i);

                    a += Math.Abs((predictiveValue - data[t]) / data[t]);
                }

                alphaRate.Add(i, a);
            }

            //选取误差最小的alpha
            var alpha = alphaRate.OrderBy(o => o.Value).First().Key;
            return CalcAlphaPredictiveValue(data, alpha);
        }

        /// <summary>
        /// 计算预测值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="alpha">平滑系数</param>
        /// <returns></returns>
        public static decimal CalcAlphaPredictiveValue(decimal[] data, decimal alpha)
        {
            var t = data.Length;//期数
            var list = TripleExponentialSmoothing(data, alpha);//三次指数平滑计算
            var st1 = list.Item1[t];//一次指数平滑数组
            var st2 = list.Item2[t];//二次指数平滑数组
            var st3 = list.Item3[t];//三次指数平滑数组

            //计算下一期预测结果
            return CalcPredictiveValue(st1, st2, st3, alpha);
        }

        /// <summary>
        /// 计算第T期预测结果
        /// </summary>
        /// <param name="st1"></param>
        /// <param name="st2"></param>
        /// <param name="st3"></param>
        /// <param name="alpha"></param>
        /// <param name="T">预测未来T期的值</param>
        /// <returns></returns>
        static decimal CalcPredictiveValue(decimal st1, decimal st2, decimal st3, decimal alpha, int T = 1)
        {
            var at = 3 * st1 - 3 * st2 + st3;
            var bt = alpha / (2 * (1 - alpha) * (1 - alpha)) * ((6 - 5 * alpha) * st1 - 2 * (5 - 4 * alpha) * st2 + (4 - 3 * alpha) * st3);
            var ct = alpha * alpha / (2 * (1 - alpha) * (1 - alpha)) * (st1 - 2 * st2 + st3);
            var value = at + bt * T + ct * T * T;

            return value < 0 ? 0 : value;
        }

        /// <summary>
        /// 一次指数平滑
        /// </summary>
        /// <param name="data"></param>
        /// <param name="alpha"></param>
        /// <param name="s0">第一期默认值</param>
        /// <returns></returns>
        static decimal[] SingleExponentialSmoothing(decimal[] data, decimal alpha, decimal s0)
        {
            var singleList = new decimal[data.Length];//一次平滑值数组
            singleList[0] = s0;//第0次平滑值

            for (var i = 1; i < data.Length; i++)
            {
                singleList[i] = alpha * data[i] + (1 - alpha) * singleList[i - 1];//计算平滑值
            }

            return singleList;
        }

        /// <summary>
        /// 三次指数平滑
        /// </summary>
        /// <param name="data"></param>
        /// <param name="alpha">0-1之间的一个值</param>
        /// <returns></returns>
        static Tuple<decimal[], decimal[], decimal[]> TripleExponentialSmoothing(decimal[] data, decimal alpha)
        {
            if (data.Length < 3)
            {
                throw new Exception("计算三次指数平滑，至少有三个数据！");
            }

            //计算第0次平滑值，取前三个数据的均值
            var baseList = data.Take(3);
            var s0 = baseList.Sum() / baseList.Count();

            //一次平滑
            var singleList = SingleExponentialSmoothing(data.Prepend(0M).ToArray(), alpha, s0);

            //二次平滑
            var doubleList = SingleExponentialSmoothing(singleList, alpha, s0);

            //三次平滑
            var tripleList = SingleExponentialSmoothing(doubleList, alpha, s0);

            return new Tuple<decimal[], decimal[], decimal[]>(singleList, doubleList, tripleList);
        }
    }
}
