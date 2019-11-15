using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static MessageBoxTouch.MessageBoxColor;

namespace MessageBoxTouch
{
    public class PropertiesSetter
    {
        // 锁定高度
        private bool lockHeight = false;
        public bool LockHeight { get => lockHeight; set => lockHeight = value; }

        // 消息区域换行模式
        private TextWrapping textWrappingMode = TextWrapping.Wrap;
        public TextWrapping TextWrappingMode { get => textWrappingMode; set => textWrappingMode = value; }

        // 窗口宽度
        private int windowWidth = 800;
        public int WindowWidth { get => windowWidth; set => windowWidth = value; }

        // 窗口最小高度
        private int windowMinHeight = 450;
        public int WindowMinHeight { get => windowMinHeight; set => windowMinHeight = value; }

        // 标题字体大小
        private int titleFontSize = 30;
        public int TitleFontSize { get => titleFontSize; set => titleFontSize = value; }

        // 消息文本字体大小
        private int messageFontSize = 25;
        public int MessageFontSize { get => messageFontSize; set => messageFontSize = value; }

        // 按钮字体大小
        private int buttonFontSize = 30;
        public int ButtonFontSize { get => buttonFontSize; set => buttonFontSize = value; }

        // 窗口透明度
        private double windowOpacity = 0.95;
        public double WindowOpacity { get => windowOpacity; set => windowOpacity = value; }

        // 标题栏透明度
        private double titleBarOpacity = 1;
        public double TitleBarOpacity { get => titleBarOpacity; set => titleBarOpacity = value; }

        // 消息栏透明度
        private double messageBarOpacity = 1;
        public double MessageBarOpacity { get => messageBarOpacity; set => messageBarOpacity = value; }

        // 按钮栏透明度
        private double buttonBarOpacity = 1;
        public double ButtonBarOpacity { get => buttonBarOpacity; set => buttonBarOpacity = value; }

        // 标题区域背景颜色
        private MessageBoxColor titlePanelColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
        public MessageBoxColor TitlePanelColor { get => titlePanelColor; set => titlePanelColor = value; }

        // 消息区域背景颜色
        private MessageBoxColor messagePanelColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
        public MessageBoxColor MessagePanelColor { get => messagePanelColor; set => messagePanelColor = value; }

        // 按钮区域背景颜色
        private MessageBoxColor buttonPanelColor = new MessageBoxColor("#DDDDDD", ColorType.HEX);
        public MessageBoxColor ButtonPanelColor { get => buttonPanelColor; set => buttonPanelColor = value; }

        // 窗口边框颜色
        private MessageBoxColor wndBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
        public MessageBoxColor WndBorderColor { get => wndBorderColor; set => wndBorderColor = value; }

        // 标题区域边框颜色
        private MessageBoxColor titleBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
        public MessageBoxColor TitleBorderColor { get => titleBorderColor; set => titleBorderColor = value; }

        // 消息区域边框颜色
        private MessageBoxColor messageBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
        public MessageBoxColor MessageBorderColor { get => messageBorderColor; set => messageBorderColor = value; }

        // 按钮区域边框颜色
        private MessageBoxColor buttonBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
        public MessageBoxColor ButtonBorderColor { get => buttonBorderColor; set => buttonBorderColor = value; }

        // 窗口边框宽度
        private Thickness wndBorderThickness = new Thickness(2);
        public Thickness WndBorderThickness { get => wndBorderThickness; set => wndBorderThickness = value; }

        // 标题区域边框宽度
        private Thickness titleBorderThickness = new Thickness(0, 0, 0, 1);
        public Thickness TitleBorderThickness { get => titleBorderThickness; set => titleBorderThickness = value; }

        // 消息区域边框宽度
        private Thickness messageBorderThickness = new Thickness(0);
        public Thickness MessageBorderThickness { get => messageBorderThickness; set => messageBorderThickness = value; }

        // 按钮区域边框宽度
        private Thickness buttonBorderThickness = new Thickness(0);
        public Thickness ButtonBorderThickness { get => buttonBorderThickness; set => buttonBorderThickness = value; }
    }
}
