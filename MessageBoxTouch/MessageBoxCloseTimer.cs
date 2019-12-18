using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MessageBoxTouch
{
    public class MessageBoxCloseTimer
    {
        // 距关闭的时间
        public TimeSpan timeSpan;
        // 窗口关闭后返回的返回值
        public int result;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="timeSpan">TimeSpan实例, 距关闭的时间</param>
        /// <param name="result">窗口关闭后返回的返回值</param>
        public MessageBoxCloseTimer(TimeSpan timeSpan, int result)
        {
            this.timeSpan = timeSpan;
            this.result = result;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="seconds">距关闭的秒数</param>
        /// <param name="result">窗口关闭后返回的返回值</param>
        public MessageBoxCloseTimer(int seconds, int result)
        {
            this.timeSpan = new TimeSpan(0, 0, 0, seconds);
            this.result = result;
        }
    }
}
