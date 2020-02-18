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
        // 颜色种类
        public enum ColorType
        {
            // 十六进制颜色码字符串
            HEX,
            // COLOR类的实例
            COLORNAME
        }
        public object color;
        public ColorType colorType;

        /// <summary>
        /// 构造函数, 自动判断颜色分类
        /// </summary>
        /// <param name="color">十六进制颜色码字符串或者COLOR类的实例</param>
        public MessageBoxColor(object color)
        {
            this.color = color;
            if (color is Color)
            {
                colorType = ColorType.COLORNAME;
            }
            else if (color is string)
            {
                colorType = ColorType.HEX;
            }
            else
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// 构造函数, 手动输入颜色分类
        /// </summary>
        /// <param name="color">十六进制颜色码字符串或者COLOR类的实例</param>
        public MessageBoxColor(object color, ColorType colorType)
        {
            this.color = color;
            this.colorType = colorType;
        }

        /// <summary>
        /// 输出这个颜色实例对应的SolidColorBrush
        /// </summary>
        /// <returns>这个颜色实例对应的SolidColorBrush</returns>
        public SolidColorBrush GetSolidColorBrush()
        {
            switch (colorType)
            {
                case ColorType.COLORNAME: return new SolidColorBrush((Color)color);
                case ColorType.HEX: return (SolidColorBrush)(new BrushConverter().ConvertFrom(color));
                default: return null;
            }
        }
    }
}
