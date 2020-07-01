﻿using System;

// ReSharper disable once CheckNamespace
namespace Bing.Extensions
{
    /// <summary>
    /// 随机数(<see cref="Random"/>) 扩展
    /// </summary>
    public static class RandomExtensions
    {
        #region NextLong(获取下一个随机数)

        /// <summary>
        /// 获取下一个随机数。范围：[0,long.MaxValue]
        /// </summary>
        /// <param name="random">范围</param>
        public static long NextLong(this Random random) => random.NextLong(0, long.MaxValue);

        /// <summary>
        /// 获取下一个随机数。范围：[0,max]
        /// </summary>
        /// <param name="random">随机数</param>
        /// <param name="max">最大值</param>
        public static long NextLong(this Random random, long max) => random.NextLong(0, max);

        /// <summary>
        /// 获取下一个随机数。范围：[min,max]
        /// </summary>
        /// <param name="random">随机数</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        public static long NextLong(this Random random, long min, long max)
        {
            var buf = new byte[8];
            random.NextBytes(buf);
            var longRand = BitConverter.ToInt64(buf, 0);
            return Math.Abs(longRand % (max - min)) + min;
        }

        #endregion

        #region NextDouble(获取下一个随机数)

        /// <summary>
        /// 获取下一个随机数。范围：[0.0,max]
        /// </summary>
        /// <param name="random">随机数</param>
        /// <param name="max">最大值</param>
        public static double NextDouble(this Random random, double max) => random.NextDouble() * max;

        /// <summary>
        /// 获取下一个随机数。范围：[min,max]
        /// </summary>
        /// <param name="random">随机数</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        public static double NextDouble(this Random random, double min, double max) =>
            random.NextDouble() * (max - min) + min;

        #endregion

        #region NormalDouble(标准正态分布生成随机双精度浮点数)

        /// <summary>
        /// 标准正态分布生成随机双精度浮点数
        /// </summary>
        /// <param name="random">随机数</param>
        public static double NormalDouble(this Random random)
        {
            var u1 = random.NextDouble();
            var u2 = random.NextDouble();
            return Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
        }

        /// <summary>
        /// 标准正态分布生成随机双精度浮点数
        /// </summary>
        /// <param name="random">随机数</param>
        /// <param name="mean">均值</param>
        /// <param name="deviation">偏差</param>
        public static double NormalDouble(this Random random, double mean, double deviation) =>
            mean + deviation * random.NormalDouble();

        #endregion

        #region NormalFloat(标准正态分布生成随机单精度浮点数)

        /// <summary>
        /// 标准正态分布生成随机单精度浮点数
        /// </summary>
        /// <param name="random">随机数</param>
        public static float NormalFloat(this Random random) => (float)random.NormalDouble();

        /// <summary>
        /// 标准正态分布生成随机单精度浮点数
        /// </summary>
        /// <param name="random">随机数</param>
        /// <param name="mean">均值</param>
        /// <param name="deviation">偏差</param>
        public static float NormalFloat(this Random random, float mean, float deviation) =>
            mean + (float)(deviation * random.NormalDouble());

        #endregion

        #region NextSign(获取下一个随机数)

        /// <summary>
        /// 获取下一个随机数。范围：[-1,1]
        /// </summary>
        /// <param name="random">随机数</param>
        public static int NextSign(this Random random) => 2 * random.Next(2) - 1;

        #endregion

        
    }
}
