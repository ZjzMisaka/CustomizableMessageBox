using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CustomizableMessageBox
{
    public class MessageBoxColor
    {
        public SolidColorBrush solidColorBrush;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="color">COLOR类的实例</param>
        public MessageBoxColor(Color color)
        {
            this.solidColorBrush = new SolidColorBrush((Color)color);
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="color">十六进制颜色码字符串的实例</param>
        public MessageBoxColor(string color)
        {
            this.solidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(color));
        }

        /// <summary>
        /// 返回这个颜色实例对应的SolidColorBrush
        /// </summary>
        /// <returns>这个颜色实例对应的SolidColorBrush</returns>
        public SolidColorBrush GetSolidColorBrush()
        {
            return solidColorBrush;
        }
    }
}
