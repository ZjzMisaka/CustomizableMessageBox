using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static CustomizableMessageBox.MessageBoxColor;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;

namespace CustomizableMessageBox
{
    /// <summary>
    /// MessageBox.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class MessageBox : Window
    {
        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern Boolean SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        // 根据用户设定属性计算得到的按钮标准高度
        private double buttonHeight = 0;

        // 关闭窗口计时器
        private DispatcherTimer timer;

        // 换行计算模式
        private enum WrapMode
        {
            // 以字符为换行单位
            TEXT,
            // 以单词为换行单位
            WORD,
            // 以单词为换行单位, 但是因为单词长度超过TextBlock宽度, 该单词以字符为换行单位
            TEXTINWORD,
            // 允许溢出, 以单词为换行单位
            WORDWITHOVERFLOW,
            // 不换行
            NOWRAP
        }

        // 设置 / 取得标题文字
        public static string TitleText
        {
            get
            {
                return mb.l_title.Content == null ? "" : mb.l_title.Content.ToString();
            }
            set
            {
                if (mb != null)
                {
                    mb.l_title.Content = value;
                }
            }
        }

        // 设置 / 取得消息文字
        public static string MessageText
        {
            get
            {
                return mb.tb_msg.Text == null ? "" : mb.tb_msg.Text;
            }
            set
            {
                if (mb != null)
                {
                    mb.tb_msg.Text = value;
                    if (mb == null)
                    {
                        return;
                    }
                    if (!LockHeight)
                    {
                        SetWindowSize();
                    }
                }
            }
        }

        // 设定显示的图标类型
        public static MessageBoxImage MessageBoxImageType
        {
            get
            {
                if (mb.i_img.Visibility == Visibility.Collapsed)
                {
                    return MessageBoxImage.None;
                }
                else
                {
                    if (mb.i_img.Source == WarningIcon)
                        return MessageBoxImage.Warning;
                    if (mb.i_img.Source == ErrorIcon)
                        return MessageBoxImage.Error;
                    if (mb.i_img.Source == InfoIcon)
                        return MessageBoxImage.Information;
                    if (mb.i_img.Source == QuestionIcon)
                        return MessageBoxImage.Question;
                    else
                        return MessageBoxImage.None;
                }
            }
            set
            {
                if (mb != null)
                {
                    SetIconType(value);
                }
            }
        }

        // 自定按钮列表
        private static List<object> buttonList = null;
        public static List<object> ButtonList
        {
            get
            {
                return buttonList;
            }
            set
            {
                if (mb != null)
                {
                    buttonList = value;
                    LoadButtonPanel();
                }
            }
        }

        static WrapMode wrapMode = WrapMode.WORD;

        // Messagebox实例
        private static MessageBox mb;

        // 用户选择的结果
        private static int currentClickIndex = -1;

        // 锁定高度
        private static bool lockHeight = false;
        public static bool LockHeight
        {
            get => lockHeight;
            set
            {
                lockHeight = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
                SetWindowSize();
            }
        }

        // 消息区域换行模式
        private static TextWrapping textWrappingMode = TextWrapping.Wrap;
        public static TextWrapping TextWrappingMode
        {
            get
            {
                return textWrappingMode;
            }
            set
            {
                textWrappingMode = value;
                switch (value)
                {
                    case TextWrapping.Wrap:
                        wrapMode = WrapMode.WORD;
                        break;
                    case TextWrapping.NoWrap:
                        wrapMode = WrapMode.NOWRAP;
                        break;
                    case TextWrapping.WrapWithOverflow:
                        wrapMode = WrapMode.WORDWITHOVERFLOW;
                        break;
                }
                if (mb == null)
                {
                    return;
                }
                InitProperties();
                SetWindowSize();
            }
        }

        // 窗口宽度
        private static double windowWidth = 800;
        public static double WindowWidth
        {
            get => windowWidth;
            set
            {
                windowWidth = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
                SetWindowSize();
            }
        }

        // 窗口最小高度
        private static double windowMinHeight = 450;
        public static double WindowMinHeight
        {
            get => windowMinHeight;
            set
            {
                windowMinHeight = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
                SetWindowSize();
            }
        }

        // 标题文本大小
        private static int titleFontSize = 30;
        public static int TitleFontSize
        {
            get => titleFontSize;
            set
            {
                titleFontSize = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
                LoadTitlePanel();
            }
        }

        // 消息文本大小
        private static int messageFontSize = 25;
        public static int MessageFontSize
        {
            get => messageFontSize;
            set
            {
                messageFontSize = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
                SetWindowSize();
            }
        }

        // 按钮文本大小
        private static int buttonFontSize = 30;
        public static int ButtonFontSize
        {
            get => buttonFontSize;
            set
            {
                buttonFontSize = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
                LoadButtonPanel();
            }
        }

        // 标题文本颜色
        private static MessageBoxColor titleFontColor = new MessageBoxColor(Colors.Black);
        public static MessageBoxColor TitleFontColor
        {
            get => titleFontColor;
            set
            {
                titleFontColor = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 消息文本颜色
        private static MessageBoxColor messageFontColor = new MessageBoxColor(Colors.Black);
        public static MessageBoxColor MessageFontColor
        {
            get => messageFontColor;
            set
            {
                messageFontColor = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 按钮文本颜色
        private static MessageBoxColor buttonFontColor = new MessageBoxColor(Colors.Black);
        public static MessageBoxColor ButtonFontColor
        {
            get => buttonFontColor;
            set
            {
                buttonFontColor = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
                SetButtonStyle();
            }
        }

        // 窗口透明度
        private static double windowOpacity = 0.95;
        public static double WindowOpacity
        {
            get => windowOpacity;
            set
            {
                windowOpacity = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 标题栏透明度
        private static double titleBarOpacity = 1;
        public static double TitleBarOpacity
        {
            get => titleBarOpacity;
            set
            {
                titleBarOpacity = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 消息栏透明度
        private static double messageBarOpacity = 1;
        public static double MessageBarOpacity
        {
            get => messageBarOpacity;
            set
            {
                messageBarOpacity = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 按钮栏透明度
        private static double buttonBarOpacity = 1;
        public static double ButtonBarOpacity
        {
            get => buttonBarOpacity;
            set
            {
                buttonBarOpacity = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 标题区域背景颜色
        private static MessageBoxColor titlePanelColor = new MessageBoxColor(Colors.White);
        public static MessageBoxColor TitlePanelColor
        {
            get => titlePanelColor;
            set
            {
                titlePanelColor = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 消息区域背景颜色
        private static MessageBoxColor messagePanelColor = new MessageBoxColor(Colors.White);
        public static MessageBoxColor MessagePanelColor
        {
            get => messagePanelColor;
            set
            {
                messagePanelColor = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 按钮区域背景颜色
        private static MessageBoxColor buttonPanelColor = new MessageBoxColor("#DDDDDD");
        public static MessageBoxColor ButtonPanelColor
        {
            get => buttonPanelColor;
            set
            {
                buttonPanelColor = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 窗口边框颜色
        private static MessageBoxColor wndBorderColor = new MessageBoxColor(Colors.White);
        public static MessageBoxColor WndBorderColor
        {
            get => wndBorderColor;
            set
            {
                wndBorderColor = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 标题区域边框颜色
        private static MessageBoxColor titlePanelBorderColor = new MessageBoxColor(Colors.White);
        public static MessageBoxColor TitlePanelBorderColor
        {
            get => titlePanelBorderColor;
            set
            {
                titlePanelBorderColor = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 消息区域边框颜色
        private static MessageBoxColor messagePanelBorderColor = new MessageBoxColor(Colors.White);
        public static MessageBoxColor MessagePanelBorderColor
        {
            get => messagePanelBorderColor;
            set
            {
                messagePanelBorderColor = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 按钮区域边框颜色
        private static MessageBoxColor buttonPanelBorderColor = new MessageBoxColor(Colors.White);
        public static MessageBoxColor ButtonPanelBorderColor
        {
            get => buttonPanelBorderColor;
            set
            {
                buttonPanelBorderColor = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 按钮边框颜色
        private static MessageBoxColor buttonBorderColor = new MessageBoxColor(Colors.Black);
        public static MessageBoxColor ButtonBorderColor
        {
            get => buttonBorderColor;
            set
            {
                buttonBorderColor = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
                SetButtonStyle();
            }
        }

        // 窗口边框宽度
        private static Thickness wndBorderThickness = new Thickness(2);
        public static Thickness WndBorderThickness
        {
            get => wndBorderThickness;
            set
            {
                wndBorderThickness = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 标题区域边框宽度
        private static Thickness titlePanelBorderThickness = new Thickness(0, 0, 0, 1);
        public static Thickness TitlePanelBorderThickness
        {
            get => titlePanelBorderThickness;
            set
            {
                titlePanelBorderThickness = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 消息区域边框宽度
        private static Thickness messagePanelBorderThickness = new Thickness(0);
        public static Thickness MessagePanelBorderThickness
        {
            get => messagePanelBorderThickness;
            set
            {
                messagePanelBorderThickness = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 按钮区域边框宽度
        private static Thickness buttonPanelBorderThickness = new Thickness(0);
        public static Thickness ButtonPanelBorderThickness
        {
            get => buttonPanelBorderThickness;
            set
            {
                buttonPanelBorderThickness = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 按钮边框宽度
        private static Thickness buttonBorderThickness = new Thickness(0);
        public static Thickness ButtonBorderThickness
        {
            get => buttonBorderThickness;
            set
            {
                buttonBorderThickness = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
                SetButtonStyle();
            }
        }

        // 标题文本字体
        private static FontFamily titleFontFamily = new FontFamily("Times New Roman");
        public static FontFamily TitleFontFamily
        {
            get => titleFontFamily;
            set
            {
                titleFontFamily = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
                LoadTitlePanel();
            }
        }

        // 消息文本字体
        private static FontFamily messageFontFamily = new FontFamily("Times New Roman");
        public static FontFamily MessageFontFamily
        {
            get => messageFontFamily;
            set
            {
                messageFontFamily = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
                SetWindowSize();
            }
        }

        // 按钮文本字体
        private static FontFamily buttonFontFamily = new FontFamily("Times New Roman");
        public static FontFamily ButtonFontFamily
        {
            get => buttonFontFamily;
            set
            {
                buttonFontFamily = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
                LoadButtonPanel();
            }
        }

        // 窗口渐显时间
        private static Duration windowShowDuration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
        public static Duration WindowShowDuration { get => windowShowDuration; set => windowShowDuration = value; }

        // 窗口显示动画
        private static List<KeyValuePair<DependencyProperty, AnimationTimeline>> windowShowAnimations = null;
        public static List<KeyValuePair<DependencyProperty, AnimationTimeline>> WindowShowAnimations { get => windowShowAnimations; set => windowShowAnimations = value; }

        // 窗口关闭动画
        private static List<KeyValuePair<DependencyProperty, AnimationTimeline>> windowCloseAnimations = null;
        public static List<KeyValuePair<DependencyProperty, AnimationTimeline>> WindowCloseAnimations { get => windowCloseAnimations; set => windowCloseAnimations = value; }

        // 自定义关闭图标
        private static BitmapImage closeIcon = new BitmapImage(new Uri(".\\Image\\close.png", UriKind.RelativeOrAbsolute));
        public static BitmapImage CloseIcon
        {
            get => closeIcon;
            set
            {
                closeIcon = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 自定义警告图标
        private static BitmapImage warningIcon = new BitmapImage(new Uri(".\\Image\\warn.png", UriKind.RelativeOrAbsolute));
        public static BitmapImage WarningIcon
        {
            get => warningIcon;
            set => warningIcon = value;
        }

        // 自定义错误图标
        private static BitmapImage errorIcon = new BitmapImage(new Uri(".\\Image\\error.png", UriKind.RelativeOrAbsolute));
        public static BitmapImage ErrorIcon
        {
            get => errorIcon;
            set => errorIcon = value;
        }

        // 自定义信息图标
        private static BitmapImage infoIcon = new BitmapImage(new Uri(".\\Image\\info.png", UriKind.RelativeOrAbsolute));
        public static BitmapImage InfoIcon
        {
            get => infoIcon;
            set => infoIcon = value;
        }

        // 自定义问题图标
        private static BitmapImage questionIcon = new BitmapImage(new Uri(".\\Image\\question.png", UriKind.RelativeOrAbsolute));
        public static BitmapImage QuestionIcon
        {
            get => questionIcon;
            set => questionIcon = value;
        }

        // 应用窗口关闭按钮
        private static bool enableCloseButton = false;
        public static bool EnableCloseButton
        {
            get => enableCloseButton;
            set
            {
                enableCloseButton = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 按钮动作样式
        private static List<Style> buttonStyleList = new List<Style> { new ResourceDictionary { Source = new Uri("pack://application:,,,/CustomizableMessageBox;component/WndStyles.xaml") }["MessageBoxButtonStyle"] as Style };
        public static List<Style> ButtonStyleList
        {
            get => buttonStyleList;
            set
            {
                buttonStyleList = value;
                if (mb == null)
                {
                    return;
                }
                SetButtonStyle();
            }
        }

        // 窗口计时关闭
        private static MessageBoxCloseTimer closeTimer = null;
        public static MessageBoxCloseTimer CloseTimer { get => closeTimer; set => closeTimer = value; }

        // 属性集合
        private static PropertiesSetter propertiesSetter = new PropertiesSetter();
        public static PropertiesSetter PropertiesSetter
        {
            get => propertiesSetter;
            set
            {
                LockHeight = value.LockHeight;
                TextWrappingMode = value.TextWrappingMode;
                WindowWidth = value.WindowWidth;
                WindowMinHeight = value.WindowMinHeight;
                TitleFontSize = value.TitleFontSize;
                MessageFontSize = value.MessageFontSize;
                ButtonFontSize = value.ButtonFontSize;
                TitleFontColor = value.TitleFontColor;
                MessageFontColor = value.MessageFontColor;
                ButtonFontColor = value.ButtonFontColor;
                WindowOpacity = value.WindowOpacity;
                TitleBarOpacity = value.TitleBarOpacity;
                MessageBarOpacity = value.MessageBarOpacity;
                ButtonBarOpacity = value.ButtonBarOpacity;
                TitlePanelColor = value.TitlePanelColor;
                MessagePanelColor = value.MessagePanelColor;
                ButtonPanelColor = value.ButtonPanelColor;
                WndBorderColor = value.WndBorderColor;
                TitlePanelBorderColor = value.TitlePanelBorderColor;
                MessagePanelBorderColor = value.MessagePanelBorderColor;
                ButtonPanelBorderColor = value.ButtonPanelBorderColor;
                ButtonBorderColor = value.ButtonBorderColor;
                WndBorderThickness = value.WndBorderThickness;
                TitlePanelBorderThickness = value.TitlePanelBorderThickness;
                MessagePanelBorderThickness = value.MessagePanelBorderThickness;
                ButtonPanelBorderThickness = value.ButtonPanelBorderThickness;
                ButtonBorderThickness = value.ButtonBorderThickness;
                TitleFontFamily = value.TitleFontFamily;
                MessageFontFamily = value.MessageFontFamily;
                ButtonFontFamily = value.ButtonFontFamily;
                WindowShowDuration = value.WindowShowDuration;
                WindowShowAnimations = value.WindowShowAnimations;
                WindowCloseAnimations = value.WindowCloseAnimations;
                CloseIcon = value.CloseIcon;
                WarningIcon = value.WarningIcon;
                ErrorIcon = value.ErrorIcon;
                InfoIcon = value.InfoIcon;
                QuestionIcon = value.QuestionIcon;
                EnableCloseButton = value.EnableCloseButton;
                ButtonStyleList = value.ButtonStyleList;
                CloseTimer = value.CloseTimer;
            }
        }

        // 像素密度
        private static double pixelsPerDip;

        // 构造函数
        private MessageBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置各种属性
        /// </summary>
        private static void InitProperties()
        {
            if (WindowWidth > 0)
            {
                mb.Width = WindowWidth;
            }
            if (WindowMinHeight > 0)
            {
                mb.MinHeight = WindowMinHeight;
                mb.Height = WindowMinHeight;
            }
            if (TitleFontSize > 0)
            {
                mb.l_title.FontSize = TitleFontSize;
            }
            if (MessageFontSize > 0)
            {
                mb.tb_msg.FontSize = MessageFontSize;
            }
            if (WindowOpacity > 0)
            {
                //Opacity = WindowOpacity;
                mb.da_win.To = WindowOpacity;
            }
            if (TitleBarOpacity > 0)
            {
                mb.g_titlegrid.Opacity = TitleBarOpacity;
            }
            if (MessageBarOpacity > 0)
            {
                mb.g_messagegrid.Opacity = MessageBarOpacity;
            }
            if (ButtonBarOpacity > 0)
            {
                mb.g_buttongrid.Opacity = ButtonBarOpacity;
            }
            if (TitleFontColor != null)
            {
                mb.l_title.Foreground = TitleFontColor.GetSolidColorBrush();
            }
            if (MessageFontColor != null)
            {
                mb.tb_msg.Foreground = MessageFontColor.GetSolidColorBrush();
            }
            if (TitlePanelColor != null)
            {
                mb.g_titlegrid.Background = TitlePanelColor.GetSolidColorBrush();
            }
            if (MessagePanelColor != null)
            {
                mb.g_messagegrid.Background = MessagePanelColor.GetSolidColorBrush();
            }
            if (ButtonPanelColor != null)
            {
                mb.g_buttongrid.Background = ButtonPanelColor.GetSolidColorBrush();
            }
            if (WndBorderColor != null)
            {
                mb.b_wndborder.BorderBrush = WndBorderColor.GetSolidColorBrush();
            }
            if (TitlePanelBorderColor != null)
            {
                mb.b_titleborder.BorderBrush = TitlePanelBorderColor.GetSolidColorBrush();
            }
            if (MessagePanelBorderColor != null)
            {
                mb.b_messageborder.BorderBrush = MessagePanelBorderColor.GetSolidColorBrush();
            }
            if (ButtonPanelBorderColor != null)
            {
                mb.b_buttonborder.BorderBrush = ButtonPanelBorderColor.GetSolidColorBrush();
            }
            if (WndBorderThickness != null)
            {
                mb.b_wndborder.BorderThickness = WndBorderThickness;
            }
            if (TitlePanelBorderThickness != null)
            {
                mb.b_titleborder.BorderThickness = TitlePanelBorderThickness;
            }
            if (MessagePanelBorderThickness != null)
            {
                mb.b_messageborder.BorderThickness = MessagePanelBorderThickness;
            }
            if (ButtonPanelBorderThickness != null)
            {
                mb.b_buttonborder.BorderThickness = ButtonPanelBorderThickness;
            }
            mb.l_title.FontFamily = TitleFontFamily;
            mb.tb_msg.FontFamily = MessageFontFamily;
            mb.tb_msg.TextWrapping = TextWrappingMode;
            mb.da_win.Duration = WindowShowDuration;
            mb.i_close.Source = CloseIcon;
            if (enableCloseButton)
            {
                mb.i_close.Visibility = Visibility.Visible;
            }
            else
            {
                mb.i_close.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 调出消息窗口
        /// </summary>
        /// <param name="msg">MessageBox消息内容</param>
        /// <param name="title">MessageBox窗口标题</param>
        /// <param name="selectStyle">按钮种类</param>
        /// <param name="img">消息类型</param>
        /// <returns>MessageBoxResult</returns>
        public static MessageBoxResult Show(string msg, string title = "", MessageBoxButton selectStyle = MessageBoxButton.OK, MessageBoxImage img = MessageBoxImage.None)
        {
            // 按钮列表
            List<object> btnList;

            // 判断按钮类型并显示相应的按钮
            switch (selectStyle)
            {
                case MessageBoxButton.OK:
                    btnList = new List<object> { MessageBoxResult.OK.ToString() };
                    break;

                case MessageBoxButton.OKCancel:
                    btnList = new List<object> { MessageBoxResult.OK.ToString(), MessageBoxResult.Cancel.ToString() };
                    break;

                case MessageBoxButton.YesNo:
                    btnList = new List<object> { MessageBoxResult.Yes.ToString(), MessageBoxResult.No.ToString() };
                    break;

                case MessageBoxButton.YesNoCancel:
                    btnList = new List<object> { MessageBoxResult.Yes.ToString(), MessageBoxResult.No.ToString(), MessageBoxResult.Cancel.ToString() };
                    break;

                default:
                    btnList = new List<object> { MessageBoxResult.OK.ToString() };
                    break;
            }

            // 返回值
            MessageBoxResult result;

            int index = Show(btnList, msg, title, img);

            if (Info.IsLastShowSucceed)
            {
                // 获取返回值字符串
                string resultStr = btnList[index].ToString();
                // 找到对应的MessageBoxResult元素并返回
                result = (MessageBoxResult)System.Enum.Parse(typeof(MessageBoxResult), resultStr);
            }
            else
            {
                result = MessageBoxResult.Cancel;
            }

            return result;
        }

        /// <summary>
        /// 调出消息窗口
        /// </summary>
        /// <param name="btnList">自定按钮列表</param>
        /// <param name="msg">MessageBox消息内容</param>
        /// <param name="title">MessageBox窗口标题</param>
        /// <param name="img">消息类型</param>
        /// <returns>选中的按钮索引</returns>
        public static int Show(List<object> btnList, string msg, string title = "", MessageBoxImage img = MessageBoxImage.None)
        {
            try
            {
                if (mb != null)
                    return -1;

                // 重置
                Info.IsLastShowSucceed = true;

                // 初始化窗口
                mb = new MessageBox();

                InitProperties();

                // 计算像素密度
                pixelsPerDip = VisualTreeHelper.GetDpi(mb).PixelsPerDip;

                //设定窗口最大高度为窗口工作区高度
                mb.MaxHeight = SystemInformation.WorkingArea.Height;

                //绑定关闭按钮点击事件
                mb.i_close.MouseLeftButtonUp += I_close_TouchDown;

                // 绑定标题栏鼠标事件, 拖动窗口
                mb.l_title.MouseLeftButtonDown += L_title_MouseLeftButtonDown;

                // 重置选择结果
                currentClickIndex = -1;

                // 显示标题
                TitleText = title;
                mb.Title = title;

                // 判断消息类型并显示相应的图像
                MessageBoxImageType = img;

                // 显示消息内容
                MessageText += msg;

                ButtonList = btnList;

                LoadTitlePanel();

                SetMessagePanelWidth();

                if (CloseTimer != null)
                {
                    CloseTimer.closeWindowByTimer = new MessageBoxCloseTimer.CloseWindowByTimer(CloseWindowByTimer);

                    // 设置定时关闭窗口时间
                    // TODO 换成Timer类 + 委托调用关闭函数的形式是不是更好
                    mb.timer = new DispatcherTimer();
                    mb.timer.Interval = CloseTimer.timeSpan;
                    mb.timer.Tick += CloseWindowByTimer;
                    mb.timer.Start();
                }

                // 设置所属的窗口
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.IsActive)
                    {
                        mb.Owner = window;
                    }
                }
                if (mb.Owner == null)
                {
                    mb.Owner = Application.Current.MainWindow;
                }

                // 显示窗口
                mb.ShowDialog();
            }
            catch (Exception ex)
            {
                // 将异常插入栈中
                Info.StackException.Push(ex);

                // 保存这次调用成功与否
                Info.IsLastShowSucceed = false;

                // 调用系统MessageBox
                currentClickIndex = (int)System.Windows.MessageBox.Show(msg, title, MessageBoxButton.OK, img);

                // 关闭窗口
                mb.Hide();
                mb.Close();
                return currentClickIndex;
            }

            // 保存这次调用成功与否
            Info.IsLastShowSucceed = true;
            // 返回用户选择的结果
            return currentClickIndex;
        }

        /// <summary>
        /// 立即关闭消息框
        /// </summary>
        public static void CloseNow()
        {
            mb.Hide();
            mb.Close();
        }

        /// <summary>
        /// 重新设定标题区域大小
        /// </summary>
        private static void LoadTitlePanel()
        {
            // 根据字体计算标题字符串高度
            double height = new FormattedText(" ", CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.l_title.FontFamily, mb.l_title.FontStyle, mb.l_title.FontWeight, mb.l_title.FontStretch), mb.l_title.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip).Height;
            double titleAndBorderHeight = height + mb.b_titleborder.BorderThickness.Top + mb.b_titleborder.BorderThickness.Bottom;

            // 设置标题栏高度
            mb.g_titlegrid.Height = titleAndBorderHeight + 14;
            mb.rd_title.Height = new GridLength(titleAndBorderHeight + 14);
            mb.b_titleborder.Height = titleAndBorderHeight + 14;
        }

        /// <summary>
        /// 载入按钮区域并设定大小
        /// </summary>
        private static void LoadButtonPanel()
        {
            int styleIndex = 0;

            mb.g_buttongrid.Children.Clear();
            mb.g_buttongrid.ColumnDefinitions.Clear();
            mb.g_buttongrid.ColumnDefinitions.Add(new ColumnDefinition());
            mb.g_buttongrid.ColumnDefinitions.Add(new ColumnDefinition());

            // 判断按钮类型并显示相应的按钮
            for (int i = 0; i < buttonList.Count; ++i)
            {
                // 当有多于两个选项时, 增加Grid的列数
                if (i >= 2)
                {
                    mb.g_buttongrid.ColumnDefinitions.Add(new ColumnDefinition());
                }
                Button newBtn = null;

                if (buttonList[i] is string)
                {
                    // 实例化一个新的按钮
                    newBtn = new Button();
                    // 将按钮加入Grid中
                    mb.g_buttongrid.Children.Add(newBtn);
                    // 设置按钮在Grid中的行列
                    newBtn.SetValue(Grid.RowProperty, 0);
                    newBtn.SetValue(Grid.ColumnProperty, i);
                    // 设置按钮样式
                    newBtn.Style = buttonStyleList.Count > styleIndex + 1 ? buttonStyleList[styleIndex++] : buttonStyleList[styleIndex];

                    //设置按钮文本
                    newBtn.Content = buttonList[i];
                    // 设置按钮可见
                    newBtn.Visibility = Visibility.Visible;
                    // 绑定按钮点击事件
                    newBtn.Click += BtnClicked;
                }
                else if (buttonList[i] is ButtonSpacer)
                {
                    double length = ((ButtonSpacer)buttonList[i]).GetLength();
                    if (length != -1)
                    {
                        // 修改列宽度
                        mb.g_buttongrid.ColumnDefinitions[i].Width = new GridLength(length);
                    }
                }
                else if (buttonList[i] is FrameworkElement)
                {
                    FrameworkElement fe = (FrameworkElement)buttonList[i];
                    // 将按钮加入Grid中
                    mb.g_buttongrid.Children.Add(fe);
                    // 修改列宽度
                    mb.g_buttongrid.ColumnDefinitions[i].Width = new GridLength(!fe.Width.Equals(Double.NaN) ? fe.Width : 1, !fe.Width.Equals(Double.NaN) ? GridUnitType.Pixel : GridUnitType.Star);
                    // 设置按钮在Grid中的行列
                    fe.SetValue(Grid.RowProperty, 0);
                    fe.SetValue(Grid.ColumnProperty, i);

                    fe.SizeChanged += ButtonObjectSizeChanged;
                }
            }

            SetButtonStyle();
        }

        /// <summary>
        /// 设置消息区域宽度
        /// </summary>
        private static void SetMessagePanelWidth()
        {
            if (mb.i_img.Visibility == Visibility.Visible)
            {
                mb.tb_msg.Width = mb.Width - mb.i_img.Width - mb.i_img.Margin.Left - mb.i_img.Margin.Right - mb.tb_msg.Margin.Left - mb.tb_msg.Margin.Right - mb.b_messageborder.BorderThickness.Left - mb.b_messageborder.BorderThickness.Right;
            }
            else
            {
                mb.tb_msg.Width = mb.Width - mb.tb_msg.Margin.Left - mb.tb_msg.Margin.Right - mb.b_messageborder.BorderThickness.Left - mb.b_messageborder.BorderThickness.Right;
            }
        }

        /// <summary>
        /// 调出消息窗口
        /// </summary>
        /// <param name="propertiesSetter">样式</param>
        /// <param name="msg">MessageBox消息内容</param>
        /// <param name="title">MessageBox窗口标题</param>
        /// <param name="selectStyle">按钮种类</param>
        /// <param name="img">消息类型</param>
        /// <returns>MessageBoxResult</returns>
        public static MessageBoxResult Show(PropertiesSetter propertiesSetter, string msg, string title = "", MessageBoxButton selectStyle = MessageBoxButton.OK, MessageBoxImage img = MessageBoxImage.None)
        {
            PropertiesSetter = propertiesSetter;
            return Show(msg, title, selectStyle, img);
        }

        /// <summary>
        /// 调出消息窗口
        /// </summary>
        /// <param name="propertiesSetter">样式</param>
        /// <param name="btnList">自定按钮列表</param>
        /// <param name="msg">MessageBox消息内容</param>
        /// <param name="title">MessageBox窗口标题</param>
        /// <param name="img">消息类型</param>
        /// <returns>选中的按钮索引</returns>
        public static int Show(PropertiesSetter propertiesSetter, List<object> btnList, string msg, string title = "", MessageBoxImage img = MessageBoxImage.None)
        {
            PropertiesSetter = propertiesSetter;
            return Show(btnList, msg, title, img);
        }

        /// <summary>
        /// 选择后保存选择结果并关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void BtnClicked(Object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            // 遍历按钮列表寻找和按钮文本相符的元素的索引
            currentClickIndex = buttonList.IndexOf(btn.Content.ToString());

            if (WindowCloseAnimations != null)
            {
                mb.StartCloseAnimationAndClose();
            }
            else
            {
                mb.Hide();
                mb.Close();
            }
        }

        /// <summary>
        /// 窗口关闭且未保存选择时, 默认情况下选择 "取消"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(Object sender, EventArgs e)
        {
            // 防止上次调用影响
            lockHeight = false;
            textWrappingMode = TextWrapping.Wrap;
            windowWidth = 800;
            windowMinHeight = 450;
            titleFontSize = 30;
            messageFontSize = 25;
            buttonFontSize = 30;
            titleFontColor = new MessageBoxColor(Colors.Black);
            messageFontColor = new MessageBoxColor(Colors.Black);
            buttonFontColor = new MessageBoxColor(Colors.Black);
            windowOpacity = 0.95;
            titleBarOpacity = 1;
            messageBarOpacity = 1;
            buttonBarOpacity = 1;
            titlePanelColor = new MessageBoxColor(Colors.White);
            messagePanelColor = new MessageBoxColor(Colors.White);
            buttonPanelColor = new MessageBoxColor("#DDDDDD");
            wndBorderColor = new MessageBoxColor(Colors.White);
            titlePanelBorderColor = new MessageBoxColor(Colors.White);
            messagePanelBorderColor = new MessageBoxColor(Colors.White);
            buttonPanelBorderColor = new MessageBoxColor(Colors.White);
            buttonBorderColor = new MessageBoxColor(Colors.Black);
            wndBorderThickness = new Thickness(2);
            titlePanelBorderThickness = new Thickness(0, 0, 0, 1);
            messagePanelBorderThickness = new Thickness(0);
            buttonPanelBorderThickness = new Thickness(0);
            buttonBorderThickness = new Thickness(0);
            titleFontFamily = new FontFamily("Times New Roman");
            messageFontFamily = new FontFamily("Times New Roman");
            buttonFontFamily = new FontFamily("Times New Roman");
            windowShowDuration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
            windowShowAnimations = null;
            windowCloseAnimations = null;
            closeIcon = new BitmapImage(new Uri(".\\Image\\close.png", UriKind.RelativeOrAbsolute));
            warningIcon = new BitmapImage(new Uri(".\\Image\\warn.png", UriKind.RelativeOrAbsolute));
            errorIcon = new BitmapImage(new Uri(".\\Image\\error.png", UriKind.RelativeOrAbsolute));
            infoIcon = new BitmapImage(new Uri(".\\Image\\info.png", UriKind.RelativeOrAbsolute));
            questionIcon = new BitmapImage(new Uri(".\\Image\\question.png", UriKind.RelativeOrAbsolute));
            enableCloseButton = false;
            buttonStyleList = new List<Style> { new ResourceDictionary { Source = new Uri("pack://application:,,,/CustomizableMessageBox;component/WndStyles.xaml") }["MessageBoxButtonStyle"] as Style };
            closeTimer = null;

            propertiesSetter = new PropertiesSetter();

            if (mb.timer != null)
            {
                mb.timer.Stop();
            }

            mb = null;
        }

        /// <summary>
        /// 显示窗口时, 根据字体, 字体大小等计算所需的窗口高度, 并设置窗口的初始位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_IsVisibleChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            // 窗口显示的情况
            if ((Boolean)e.NewValue == true)
            {
                if (!LockHeight)
                {
                    SetWindowSize();
                }

                // 将窗口初始位置设置在屏幕中心
                SetWindowPos(new WindowInteropHelper(mb).Handle, new IntPtr(0), (int)(SystemInformation.WorkingArea.Width / 2 - mb.Width / 2), (int)(SystemInformation.WorkingArea.Height / 2 - mb.Height / 2), (int)(mb.Width), (int)(mb.Height), 0x1);

                if (WindowShowAnimations != null)
                {
                    StartOpenAnimation();
                }
            }
        }

        /// <summary>
        /// 设置窗口大小
        /// </summary>
        private static void SetWindowSize()
        {
            // 单字符高度
            double height = new FormattedText(" ", CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.tb_msg.FontFamily, mb.tb_msg.FontStyle, mb.tb_msg.FontWeight, mb.tb_msg.FontStretch), mb.tb_msg.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip).Height;
            // 行数
            double lineCount = 1;
            // 当行字符串累积宽度
            double lineWidth = 0;
            // 格式化的字符串
            FormattedText ft;

            // 遍历消息字符串中的每个字符
            for (int i = 0; i < mb.tb_msg.Text.Count(); ++i)
            {
                if (mb.tb_msg.Text[i] == '\n')
                {
                    lineWidth = 0;
                    ++lineCount;
                }
                else
                {
                    if (wrapMode == WrapMode.NOWRAP)
                    {
                        // DO NOTHING
                    }
                    else if (wrapMode == WrapMode.WORDWITHOVERFLOW)
                    {
                        // 当这个字符为英文或数字, 即为一个单词的开头
                        if (Regex.IsMatch(mb.tb_msg.Text[i].ToString(), "[a-zA-Z0-9]"))
                        {
                            //获取这个单词
                            string nextWord = GetNextWord(mb.tb_msg.Text, i);
                            // 使用单词字符串和字体设置作为参数实例化FormattedText
                            ft = new FormattedText(nextWord, CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.tb_msg.FontFamily, mb.tb_msg.FontStyle, mb.tb_msg.FontWeight, mb.tb_msg.FontStretch), mb.tb_msg.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip);
                            //如果单词长度超过TextBlock宽度, 则直接换一行 //TODO 是否应该是换两行? 
                            if (ft.Width > mb.tb_msg.Width)
                            {
                                lineWidth = 0;
                                ++lineCount;
                                i += nextWord.Length - 1;
                                continue;
                            }
                            // 累加这个单词的宽度
                            lineWidth += ft.Width;
                            // 如果累加后的宽度超过TextBlock宽度, 则直接换一行, 并重新计算
                            if (Math.Ceiling(lineWidth) >= mb.tb_msg.Width)
                            {
                                lineWidth = 0;
                                ++lineCount;
                                --i;
                                continue;
                            }
                            else
                            {
                                i += nextWord.Length - 1;
                            }
                        }
                        else
                        {
                            // 使用字符和字体设置作为参数实例化FormattedText
                            ft = new FormattedText(mb.tb_msg.Text[i].ToString(), CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.tb_msg.FontFamily, mb.tb_msg.FontStyle, mb.tb_msg.FontWeight, mb.tb_msg.FontStretch), mb.tb_msg.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip);
                            // 累加这个字符的宽度
                            lineWidth += ft.Width;
                            // 如果累加后的宽度超过TextBlock宽度, 则直接换一行, 并重新计算
                            if (Math.Ceiling(lineWidth) >= mb.tb_msg.Width)
                            {
                                lineWidth = 0;
                                ++lineCount;
                                --i;
                                continue;
                            }
                        }
                    }
                    else if (wrapMode == WrapMode.WORD)
                    {
                        // 当这个字符为英文或数字, 即为一个单词的开头
                        if (Regex.IsMatch(mb.tb_msg.Text[i].ToString(), "[a-zA-Z0-9]"))
                        {
                            //获取这个单词
                            string nextWord = GetNextWord(mb.tb_msg.Text, i);
                            // 使用单词字符串和字体设置作为参数实例化FormattedText
                            ft = new FormattedText(nextWord, CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.tb_msg.FontFamily, mb.tb_msg.FontStyle, mb.tb_msg.FontWeight, mb.tb_msg.FontStretch), mb.tb_msg.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip);
                            //如果单词长度超过TextBlock宽度, 则直接换一行, 并以C模式开始计算
                            if (ft.Width > mb.tb_msg.Width)
                            {
                                lineWidth = 0;
                                ++lineCount;
                                --i;
                                wrapMode = WrapMode.TEXTINWORD;
                                continue;
                            }
                            // 累加这个单词的宽度
                            lineWidth += ft.Width;
                            // 如果累加后的宽度超过TextBlock宽度, 则直接换一行, 并重新计算
                            if (Math.Ceiling(lineWidth) >= mb.tb_msg.Width)
                            {
                                lineWidth = 0;
                                ++lineCount;
                                --i;
                                continue;
                            }
                            else
                            {
                                i += nextWord.Length - 1;
                            }
                        }
                        else
                        {
                            // 使用字符和字体设置作为参数实例化FormattedText
                            ft = new FormattedText(mb.tb_msg.Text[i].ToString(), CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.tb_msg.FontFamily, mb.tb_msg.FontStyle, mb.tb_msg.FontWeight, mb.tb_msg.FontStretch), mb.tb_msg.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip);
                            // 累加这个字符的宽度
                            lineWidth += ft.Width;
                            // 如果累加后的宽度超过TextBlock宽度, 则直接换一行, 并重新计算
                            if (Math.Ceiling(lineWidth) >= mb.tb_msg.Width)
                            {
                                lineWidth = 0;
                                ++lineCount;
                                --i;
                                continue;
                            }
                        }
                    }
                    else if (wrapMode == WrapMode.TEXTINWORD)
                    {
                        if (!Regex.IsMatch(mb.tb_msg.Text[i].ToString(), "[a-zA-Z0-9]"))
                        {
                            wrapMode = WrapMode.WORD;
                            continue;
                        }
                        // 使用字符串和字体设置作为参数实例化FormattedText
                        ft = new FormattedText(mb.tb_msg.Text[i].ToString(), CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.tb_msg.FontFamily, mb.tb_msg.FontStyle, mb.tb_msg.FontWeight, mb.tb_msg.FontStretch), mb.tb_msg.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip);
                        // 累加这个字符的宽度
                        lineWidth += ft.Width;
                        if (lineWidth > mb.tb_msg.Width)
                        {
                            lineWidth = 0;
                            ++lineCount;
                            --i;
                            continue;
                        }
                    }
                }
            }

            // 计算窗口高度
            mb.Height = height * lineCount + mb.rd_title.Height.Value + mb.rd_button.Height.Value + mb.tb_msg.Margin.Top + mb.tb_msg.Margin.Bottom + mb.b_messageborder.BorderThickness.Top + mb.b_messageborder.BorderThickness.Bottom + height;
        }

        /// <summary>
        /// 获取下一个只包含英文数字的单词, 单词不必须以空格分隔
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private static string GetNextWord(string str, int startIndex)
        {
            string nextWord = "";
            for (int i = startIndex; i <= (str.Length - 1) && Regex.IsMatch(str[i].ToString(), "[a-zA-Z0-9]") == true; ++i)
            {
                nextWord += str[i];
            }
            return nextWord;
        }

        /// <summary>
        /// 单击关闭按钮以关闭窗口
        /// </summary>
        private static void I_close_TouchDown(Object sender, Object e)
        {
            if (WindowCloseAnimations != null)
            {
                mb.StartCloseAnimationAndClose();
            }
            else
            {
                mb.Hide();
                mb.Close();
            }
        }

        /// <summary>
        /// 通过拖动标题栏移动窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void L_title_MouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
        {
            mb.DragMove();
        }

        /// <summary>
        /// 当自定控件的大小被改变时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ButtonObjectSizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)sender;

            // 如果宽度有变化, 重新设置列宽
            if (e.WidthChanged)
            {
                for (int i = 0; i < buttonList.Count; i++)
                {
                    if (buttonList[i].Equals(sender))
                    {
                        mb.g_buttongrid.ColumnDefinitions[i].Width = new GridLength(!fe.Width.Equals(Double.NaN) ? fe.Width : 1, !fe.Width.Equals(Double.NaN) ? GridUnitType.Pixel : GridUnitType.Star);
                    }
                }
            }

            if (e.PreviousSize == new Size(0, 0))
            {
                return;
            }

            SetButtonAndButtonPanelHeight();
        }

        /// <summary>
        /// 播放窗口打开动画
        /// </summary>
        private void StartOpenAnimation()
        {
            Storyboard sb = new Storyboard();
            foreach (KeyValuePair<DependencyProperty, AnimationTimeline> kv in WindowShowAnimations)
            {
                Storyboard.SetTargetName(kv.Value, "main");
                Storyboard.SetTargetProperty(kv.Value, new PropertyPath(kv.Key));
                sb.Children.Add(kv.Value);
            }
            sb.Begin(mb);
        }

        /// <summary>
        /// 播放窗口关闭动画, 并在播放完成后关闭窗口
        /// </summary>
        private void StartCloseAnimationAndClose()
        {
            Storyboard sb = new Storyboard();
            sb.Completed += (a, b) =>
            {
                mb.Hide();
                mb.Close();
            };
            foreach (KeyValuePair<DependencyProperty, AnimationTimeline> kv in WindowCloseAnimations)
            {
                Storyboard.SetTargetName(kv.Value, "main");
                Storyboard.SetTargetProperty(kv.Value, new PropertyPath(kv.Key));
                sb.Children.Add(kv.Value);
            }
            sb.Begin(mb);
        }

        /// <summary>
        /// 计数器到达设定时间间隔后关闭窗口, 并返回设定的返回值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CloseWindowByTimer(object sender, EventArgs e)
        {
            currentClickIndex = CloseTimer.result;
            mb.Hide();
            mb.Close();
        }

        /// <summary>
        /// 运行时设定当前显示的图标类型
        /// </summary>
        /// <param name="iconType">显示的图标类型</param>
        private static void SetIconType(MessageBoxImage iconType)
        {
            switch (iconType)
            {
                case MessageBoxImage.Warning:
                    if (WarningIcon != null)
                        mb.i_img.Source = WarningIcon;
                    mb.i_img.Visibility = Visibility.Visible;
                    break;

                case MessageBoxImage.Error:
                    if (ErrorIcon != null)
                        mb.i_img.Source = ErrorIcon;
                    mb.i_img.Visibility = Visibility.Visible;
                    break;

                case MessageBoxImage.Information:
                    if (InfoIcon != null)
                        mb.i_img.Source = InfoIcon;
                    mb.i_img.Visibility = Visibility.Visible;
                    break;

                case MessageBoxImage.Question:
                    if (QuestionIcon != null)
                        mb.i_img.Source = QuestionIcon;
                    mb.i_img.Visibility = Visibility.Visible;
                    break;

                case MessageBoxImage.None:
                    mb.i_img.Visibility = Visibility.Collapsed;
                    break;

                default:
                    break;
            }

            SetMessagePanelWidth();
        }

        /// <summary>
        /// 设置按钮样式
        /// </summary>
        private static void SetButtonStyle()
        {
            //double maxContentWidth = 0;
            double contentAndBorderHeight = 0;
            int buttonIndex = -1;
            int btnObjIndex = -1;
            for (int i = 0; i < buttonList.Count; i++)
            {
                if (!(buttonList[i] is string || buttonList[i] is FrameworkElement))
                {
                    continue;
                }
                ++btnObjIndex;
                if (buttonList[i] is string)
                {
                    ++buttonIndex;
                    Button btn = (Button)mb.g_buttongrid.Children[btnObjIndex];

                    if (ButtonFontSize > 0)
                        btn.FontSize = ButtonFontSize;
                    if (ButtonFontFamily != null)
                        btn.FontFamily = ButtonFontFamily;
                    if (ButtonFontColor != null)
                        btn.Foreground = ButtonFontColor.GetSolidColorBrush();
                    if (ButtonBorderColor != null)
                        btn.BorderBrush = ButtonBorderColor.GetSolidColorBrush();
                    if (!ButtonBorderThickness.Equals(new Thickness()))
                        btn.BorderThickness = ButtonBorderThickness;

                    Style style = null;

                    if (ButtonStyleList != null)
                    {
                        btn.Style = style = ButtonStyleList.Count > buttonIndex ? ButtonStyleList[buttonIndex] : ButtonStyleList[ButtonStyleList.Count - 1];
                    }

                    // 使用字符串和字体设置作为参数实例化FormattedText
                    FormattedText ft = new FormattedText(btn.Content.ToString(), CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(btn.FontFamily, btn.FontStyle, btn.FontWeight, btn.FontStretch), btn.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip);
                    // 计算字符串在按钮中的宽度
                    //double contentWidth = ft.Width;
                    // 如果值比maxContentWidth值更大则赋值给maxContentWidth
                    //maxContentWidth = contentWidth > maxContentWidth ? contentWidth : maxContentWidth;
                    Thickness thickness = btn.BorderThickness;
                    if (style != null)
                    {
                        bool needSetColumnDefinitionWidth = false;
                        double ColumnDefinitionWidth = 0;
                        foreach (Setter setter in style.Setters)
                        {
                            if (setter.Property == Button.BorderThicknessProperty && setter.Value != null)
                            {
                                thickness = (Thickness)setter.Value;
                            }
                            else if (setter.Property == Button.WidthProperty && setter.Value != null)
                            {
                                needSetColumnDefinitionWidth = true;
                                ColumnDefinitionWidth += (double)setter.Value;
                            }
                            else if (setter.Property == Button.MarginProperty && setter.Value != null)
                            {
                                ColumnDefinitionWidth += ((Thickness)setter.Value).Left + ((Thickness)setter.Value).Right;
                            }

                            if (needSetColumnDefinitionWidth && ColumnDefinitionWidth > 0)
                            {
                                mb.g_buttongrid.ColumnDefinitions[i].Width = new GridLength(ColumnDefinitionWidth, GridUnitType.Pixel);
                            }
                        }
                    }
                    if (ft.Height + thickness.Top + thickness.Bottom > contentAndBorderHeight)
                    {
                        contentAndBorderHeight = ft.Height + btn.BorderThickness.Top + btn.BorderThickness.Bottom + mb.b_buttonborder.BorderThickness.Top + mb.b_buttonborder.BorderThickness.Bottom;
                    }
                }
            }

            // 设置按钮栏高度
            mb.buttonHeight = contentAndBorderHeight + 7;

            SetButtonAndButtonPanelHeight();
        }

        /// <summary>
        /// 根据最大高度设置按钮和按钮区域高度
        /// </summary>
        private static void SetButtonAndButtonPanelHeight()
        {
            double highestItemHeight = -1;
            int btnObjIndex = -1;

            highestItemHeight = mb.buttonHeight;

            for (int i = 0; i < buttonList.Count; i++)
            {
                if (!(buttonList[i] is string || buttonList[i] is FrameworkElement))
                {
                    continue;
                }
                ++btnObjIndex;
                if (buttonList[i] is FrameworkElement)
                {
                    FrameworkElement fe = (FrameworkElement)buttonList[i];
                    if (fe.Height > highestItemHeight)
                    {
                        highestItemHeight = fe.Height + fe.Margin.Top + fe.Margin.Bottom;
                    }
                }
            }

            mb.g_buttongrid.Height = highestItemHeight + 13;
            mb.rd_button.Height = new GridLength(highestItemHeight + 13, GridUnitType.Pixel);
            mb.b_buttonborder.Height = highestItemHeight + 13;
        }
    }
}