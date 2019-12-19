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
using static MessageBoxTouch.MessageBoxColor;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;

namespace MessageBoxTouch
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

        // 是否是在调用ShowDialog后触发的SizeChanged事件
        private bool isSizeChangedByShowDialog = true;

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

        static WrapMode wrapMode = WrapMode.WORD;

        // Messagebox实例
        private static MessageBox mb;

        // 用户选择的结果
        private static int currentClickIndex = -1;

        // 自定按钮列表
        private static List<object> btnList = null;

        // 锁定高度
        private static bool lockHeight = false;
        public static bool LockHeight { get => lockHeight; set => lockHeight = value; }

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
            }
        }

        // 窗口宽度
        private static double windowWidth = 800;
        public static double WindowWidth { get => windowWidth; set => windowWidth = value; }

        // 窗口最小高度
        private static double windowMinHeight = 450;
        public static double WindowMinHeight { get => windowMinHeight; set => windowMinHeight = value; }

        // 标题文本大小
        private static int titleFontSize = 30;
        public static int TitleFontSize { get => titleFontSize; set => titleFontSize = value; }

        // 消息文本大小
        private static int messageFontSize = 25;
        public static int MessageFontSize { get => messageFontSize; set => messageFontSize = value; }

        // 按钮文本大小
        private static int buttonFontSize = 30;
        public static int ButtonFontSize { get => buttonFontSize; set => buttonFontSize = value; }

        // 标题文本颜色
        private static MessageBoxColor titleFontColor = new MessageBoxColor(Colors.Black);
        public static MessageBoxColor TitleFontColor { get => titleFontColor; set => titleFontColor = value; }

        // 消息文本颜色
        private static MessageBoxColor messageFontColor = new MessageBoxColor(Colors.Black);
        public static MessageBoxColor MessageFontColor { get => messageFontColor; set => messageFontColor = value; }

        // 按钮文本颜色
        private static MessageBoxColor buttonFontColor = new MessageBoxColor(Colors.Black);
        public static MessageBoxColor ButtonFontColor { get => buttonFontColor; set => buttonFontColor = value; }

        // 窗口透明度
        private static double windowOpacity = 0.95;
        public static double WindowOpacity { get => windowOpacity; set => windowOpacity = value; }

        // 标题栏透明度
        private static double titleBarOpacity = 1;
        public static double TitleBarOpacity { get => titleBarOpacity; set => titleBarOpacity = value; }

        // 消息栏透明度
        private static double messageBarOpacity = 1;
        public static double MessageBarOpacity { get => messageBarOpacity; set => messageBarOpacity = value; }

        // 按钮栏透明度
        private static double buttonBarOpacity = 1;
        public static double ButtonBarOpacity { get => buttonBarOpacity; set => buttonBarOpacity = value; }

        // 标题区域背景颜色
        private static MessageBoxColor titlePanelColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
        public static MessageBoxColor TitlePanelColor { get => titlePanelColor; set => titlePanelColor = value; }

        // 消息区域背景颜色
        private static MessageBoxColor messagePanelColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
        public static MessageBoxColor MessagePanelColor { get => messagePanelColor; set => messagePanelColor = value; }

        // 按钮区域背景颜色
        private static MessageBoxColor buttonPanelColor = new MessageBoxColor("#DDDDDD", ColorType.HEX);
        public static MessageBoxColor ButtonPanelColor { get => buttonPanelColor; set => buttonPanelColor = value; }

        // 窗口边框颜色
        private static MessageBoxColor wndBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
        public static MessageBoxColor WndBorderColor { get => wndBorderColor; set => wndBorderColor = value; }

        // 标题区域边框颜色
        private static MessageBoxColor titlePanelBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
        public static MessageBoxColor TitlePanelBorderColor { get => titlePanelBorderColor; set => titlePanelBorderColor = value; }

        // 消息区域边框颜色
        private static MessageBoxColor messagePanelBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
        public static MessageBoxColor MessagePanelBorderColor { get => messagePanelBorderColor; set => messagePanelBorderColor = value; }

        // 按钮区域边框颜色
        private static MessageBoxColor buttonPanelBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
        public static MessageBoxColor ButtonPanelBorderColor { get => buttonPanelBorderColor; set => buttonPanelBorderColor = value; }

        // 按钮边框颜色
        private static MessageBoxColor buttonBorderColor = new MessageBoxColor(Colors.Black, ColorType.COLORNAME);
        public static MessageBoxColor ButtonBorderColor { get => buttonBorderColor; set => buttonBorderColor = value; }

        // 窗口边框宽度
        private static Thickness wndBorderThickness = new Thickness(2);
        public static Thickness WndBorderThickness { get => wndBorderThickness; set => wndBorderThickness = value; }

        // 标题区域边框宽度
        private static Thickness titlePanelBorderThickness = new Thickness(0, 0, 0, 1);
        public static Thickness TitlePanelBorderThickness { get => titlePanelBorderThickness; set => titlePanelBorderThickness = value; }

        // 消息区域边框宽度
        private static Thickness messagePanelBorderThickness = new Thickness(0);
        public static Thickness MessagePanelBorderThickness { get => messagePanelBorderThickness; set => messagePanelBorderThickness = value; }

        // 按钮区域边框宽度
        private static Thickness buttonPanelBorderThickness = new Thickness(0);
        public static Thickness ButtonPanelBorderThickness { get => buttonPanelBorderThickness; set => buttonPanelBorderThickness = value; }

        // 按钮边框宽度
        private static Thickness buttonBorderThickness = new Thickness(0);
        public static Thickness ButtonBorderThickness { get => buttonBorderThickness; set => buttonBorderThickness = value; }

        // 标题文本字体
        private static FontFamily titleFontFamily = new FontFamily("Times New Roman");
        public static FontFamily TitleFontFamily { get => titleFontFamily; set => titleFontFamily = value; }

        // 消息文本字体
        private static FontFamily messageFontFamily = new FontFamily("Times New Roman");
        public static FontFamily MessageFontFamily { get => messageFontFamily; set => messageFontFamily = value; }

        // 按钮文本字体
        private static FontFamily buttonFontFamily = new FontFamily("Times New Roman");
        public static FontFamily ButtonFontFamily { get => buttonFontFamily; set => buttonFontFamily = value; }

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
        public static BitmapImage CloseIcon { get => closeIcon; set => closeIcon = value; }

        // 自定义警告图标
        private static BitmapImage warningIcon = new BitmapImage(new Uri(".\\Image\\warn.png", UriKind.RelativeOrAbsolute));
        public static BitmapImage WarningIcon { get => warningIcon; set => warningIcon = value; }

        // 自定义错误图标
        private static BitmapImage errorIcon = new BitmapImage(new Uri(".\\Image\\error.png", UriKind.RelativeOrAbsolute));
        public static BitmapImage ErrorIcon { get => errorIcon; set => errorIcon = value; }

        // 自定义信息图标
        private static BitmapImage infoIcon = new BitmapImage(new Uri(".\\Image\\info.png", UriKind.RelativeOrAbsolute));
        public static BitmapImage InfoIcon { get => infoIcon; set => infoIcon = value; }

        // 自定义问题图标
        private static BitmapImage questionIcon = new BitmapImage(new Uri(".\\Image\\question.png", UriKind.RelativeOrAbsolute));
        public static BitmapImage QuestionIcon { get => questionIcon; set => questionIcon = value; }

        // 应用窗口关闭按钮
        private static bool enableCloseButton = false;
        public static bool EnableCloseButton { get => enableCloseButton; set => enableCloseButton = value; }

        // 按钮动作样式
        private static List<Style> styleList = new List<Style> { new ResourceDictionary { Source = new Uri("pack://application:,,,/MessageBoxTouch;component/WndStyles.xaml") }["MessageBoxButtonStyle"] as Style };
        public static List<Style> StyleList { get => styleList; set => styleList = value; }

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
                StyleList = value.StyleList;
                CloseTimer = value.CloseTimer;
            }
        }

        // 像素密度
        private static double pixelsPerDip;

        // 构造函数
        private MessageBox()
        {
            InitializeComponent();

            // 设置各种属性
            if (WindowWidth > 0)
            {
                Width = WindowWidth;
            }
            if (WindowMinHeight > 0)
            {
                MinHeight = WindowMinHeight;
                Height = WindowMinHeight;
            }
            if (TitleFontSize > 0)
            {
                l_title.FontSize = TitleFontSize;
            }
            if (MessageFontSize > 0)
            {
                tb_msg.FontSize = MessageFontSize;
            }
            if (WindowOpacity > 0)
            {
                //Opacity = WindowOpacity;
                da_win.To = WindowOpacity;
            }
            if (TitleBarOpacity > 0)
            {
                g_titlegrid.Opacity = TitleBarOpacity;
            }
            if (MessageBarOpacity > 0)
            {
                g_messagegrid.Opacity = MessageBarOpacity;
            }
            if (ButtonBarOpacity > 0)
            {
                g_buttongrid.Opacity = ButtonBarOpacity;
            }
            if (TitleFontColor != null)
            {
                l_title.Foreground = TitleFontColor.GetSolidColorBrush();
            }
            if (MessageFontColor != null)
            {
                tb_msg.Foreground = MessageFontColor.GetSolidColorBrush();
            }
            if (TitlePanelColor != null)
            {
                g_titlegrid.Background = TitlePanelColor.GetSolidColorBrush();
            }
            if (MessagePanelColor != null)
            {
                g_messagegrid.Background = MessagePanelColor.GetSolidColorBrush();
            }
            if (ButtonPanelColor != null)
            {
                g_buttongrid.Background = ButtonPanelColor.GetSolidColorBrush();
            }
            if (WndBorderColor != null)
            {
                b_wndborder.BorderBrush = WndBorderColor.GetSolidColorBrush();
            }
            if (TitlePanelBorderColor != null)
            {
                b_titleborder.BorderBrush = TitlePanelBorderColor.GetSolidColorBrush();
            }
            if (MessagePanelBorderColor != null)
            {
                b_messageborder.BorderBrush = MessagePanelBorderColor.GetSolidColorBrush();
            }
            if (ButtonPanelBorderColor != null)
            {
                b_buttonborder.BorderBrush = ButtonPanelBorderColor.GetSolidColorBrush();
            }
            if (WndBorderThickness != null)
            {
                b_wndborder.BorderThickness = WndBorderThickness;
            }
            if (TitlePanelBorderThickness != null)
            {
                b_titleborder.BorderThickness = TitlePanelBorderThickness;
            }
            if (MessagePanelBorderThickness != null)
            {
                b_messageborder.BorderThickness = MessagePanelBorderThickness;
            }
            if (ButtonPanelBorderThickness != null)
            {
                b_buttonborder.BorderThickness = ButtonPanelBorderThickness;
            }
            l_title.FontFamily = TitleFontFamily;
            tb_msg.FontFamily = MessageFontFamily;
            tb_msg.TextWrapping = TextWrappingMode;
            da_win.Duration = WindowShowDuration;
            i_close.Source = CloseIcon;
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

            try
            {
                // 获取返回值字符串
                string resultStr = btnList[Show(btnList, msg, title, img)].ToString();

                // 找到对应的MessageBoxResult元素并返回
                result = (MessageBoxResult)System.Enum.Parse(typeof(MessageBoxResult), resultStr);
            }
            catch
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
            MessageBox.btnList = btnList;

            try
            {
                if (mb != null)
                    return -1;

                // 初始化窗口
                mb = new MessageBox();

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

                // 关闭按钮
                mb.i_close.Source = new BitmapImage(new Uri(".\\Image\\close.png", UriKind.RelativeOrAbsolute));

                // 显示标题
                mb.l_title.Content = title;
                mb.Title = title;

                // 判断消息类型并显示相应的图像
                switch (img)
                {
                    case MessageBoxImage.Warning:
                        mb.i_img.Source = WarningIcon;
                        mb.i_img.Visibility = Visibility.Visible;
                        break;

                    case MessageBoxImage.Error:
                        mb.i_img.Source = ErrorIcon;
                        mb.i_img.Visibility = Visibility.Visible;
                        break;

                    case MessageBoxImage.Information:
                        mb.i_img.Source = InfoIcon;
                        mb.i_img.Visibility = Visibility.Visible;
                        break;

                    case MessageBoxImage.Question:
                        mb.i_img.Source = QuestionIcon;
                        mb.i_img.Visibility = Visibility.Visible;
                        break;

                    case MessageBoxImage.None:
                        break;

                    default:
                        break;
                }

                // 显示关闭图标
                if (enableCloseButton)
                {
                    mb.i_close.Visibility = Visibility.Visible;
                }

                // 显示消息内容
                mb.tb_msg.Text = mb.tb_msg.Text + msg;

                // 根据按钮字体计算自定按钮文本中最长的文本的宽度和字符高度
                double maxContentWidth = 0;
                double contentHeight = 0;

                double highestItemHeight = -1;

                int styleIndex = 0;

                // 判断按钮类型并显示相应的按钮
                for (int i = 0; i < btnList.Count; ++i)
                {
                    // 当有多于两个选项时, 增加Grid的列数
                    if (i >= 2)
                    {
                        mb.g_buttongrid.ColumnDefinitions.Add(new ColumnDefinition());
                    }
                    Button newBtn = null;
                    if (btnList[i] is string)
                    {
                        // 实例化一个新的按钮
                        newBtn = new Button();
                        // 将按钮加入Grid中
                        mb.g_buttongrid.Children.Add(newBtn);
                        // 设置按钮在Grid中的行列
                        newBtn.SetValue(Grid.RowProperty, 0);
                        newBtn.SetValue(Grid.ColumnProperty, i);
                        // 设置按钮样式
                        newBtn.Style = styleList.Count > styleIndex + 1 ? styleList[styleIndex++] : styleList[styleIndex];

                        //设置按钮文本
                        newBtn.Content = btnList[i];
                        // 设置按钮外边距
                        newBtn.Margin = new Thickness(10, 6, 10, 6);
                        // 设置按钮可见
                        newBtn.Visibility = Visibility.Visible;
                        // 绑定按钮点击事件
                        newBtn.Click += BtnClicked;

                        newBtn.FontSize = ButtonFontSize;
                        newBtn.FontFamily = ButtonFontFamily;
                        newBtn.Foreground = ButtonFontColor.GetSolidColorBrush();
                        newBtn.BorderBrush = ButtonBorderColor.GetSolidColorBrush();
                        newBtn.BorderThickness = ButtonBorderThickness;
                    }
                    else if (btnList[i] is ButtonSpacer)
                    {
                        double length = ((ButtonSpacer)btnList[i]).GetLength();
                        if (length != -1)
                        {
                            // 修改列宽度
                            mb.g_buttongrid.ColumnDefinitions[i].Width = new GridLength(length);
                        }
                    }
                    else if (btnList[i] is FrameworkElement)
                    {
                        FrameworkElement fe = (FrameworkElement)btnList[i];
                        // 将按钮加入Grid中
                        mb.g_buttongrid.Children.Add(fe);
                        // 修改列宽度
                        mb.g_buttongrid.ColumnDefinitions[i].Width = new GridLength(!fe.Width.Equals(Double.NaN) ? fe.Width : 1, !fe.Width.Equals(Double.NaN) ? GridUnitType.Pixel : GridUnitType.Star);
                        // 设置按钮在Grid中的行列
                        fe.SetValue(Grid.RowProperty, 0);
                        fe.SetValue(Grid.ColumnProperty, i);
                        if (fe.Height > highestItemHeight)
                        {
                            highestItemHeight = fe.Height;
                        }
                        fe.SizeChanged += ButtonObjectSizeChanged;
                    }

                    // 当该变量还没计算时
                    if (newBtn != null && maxContentWidth == 0)
                    {
                        foreach (object s in btnList)
                        {
                            if (s is string)
                            {
                                // 使用字符串和字体设置作为参数实例化FormattedText
                                FormattedText ft = new FormattedText(s.ToString(), CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(newBtn.FontFamily.ToString()), newBtn.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip);
                                // 计算字符串在按钮中的宽度
                                double contentWidth = ft.Width;
                                // 如果值比maxContentWidth值更大则赋值给maxContentWidth
                                maxContentWidth = contentWidth > maxContentWidth ? contentWidth : maxContentWidth;
                                if (contentHeight == 0)
                                {
                                    contentHeight = ft.Height;
                                }
                            }
                        }
                    }

                    // 设置按钮的宽高
                    // newBtn.Width = maxContentWidth + 7;
                    //if (newBtn != null)
                    //{
                    //    newBtn.Height = contentHeight + 7;
                    //}
                }

                // 设置消息区域宽度
                if (mb.i_img.Visibility == Visibility.Visible)
                {
                    mb.tb_msg.Width = mb.Width - mb.i_img.Width - mb.i_img.Margin.Left - mb.i_img.Margin.Right - mb.tb_msg.Margin.Left - mb.tb_msg.Margin.Right;
                }
                else
                {
                    mb.tb_msg.Width = mb.Width - mb.tb_msg.Margin.Left - mb.tb_msg.Margin.Right;
                }

                // 根据字体计算标题字符串高度
                double height = new FormattedText(" ", CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.l_title.FontFamily.ToString()), mb.l_title.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip).Height;
                // 设置标题栏高度
                mb.g_titlegrid.Height = height + 14;
                mb.rd_title.Height = new GridLength(height + 14);
                mb.b_titleborder.Height = height + 14;

                // 设置按钮栏高度
                mb.buttonHeight = contentHeight + 7;
                if (highestItemHeight > mb.buttonHeight)
                {
                    mb.g_buttongrid.Height = highestItemHeight + 13;
                    mb.rd_button.Height = new GridLength(highestItemHeight + 13, GridUnitType.Pixel);
                    mb.b_buttonborder.Height = highestItemHeight + 13;
                }
                else
                {
                    mb.g_buttongrid.Height = mb.buttonHeight + 13;
                    mb.rd_button.Height = new GridLength(mb.buttonHeight + 13, GridUnitType.Pixel);
                    mb.b_buttonborder.Height = mb.buttonHeight + 13;
                }

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
                mb.Owner = Application.Current.MainWindow;

                // 显示窗口
                mb.ShowDialog();
            }
            catch
            {
                // 调用系统MessageBox
                currentClickIndex = (int)System.Windows.MessageBox.Show(msg, title, MessageBoxButton.OK, img);

                // 关闭窗口
                mb.Hide();
                mb.Close();
            }

            // 返回用户选择的结果
            return currentClickIndex;
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
        /// 
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
        /// 获取按钮列表
        /// </summary>
        /// <returns>按钮列表</returns>
        public static List<object> GetBtnList()
        {
            return btnList;
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
            currentClickIndex = btnList.IndexOf(btn.Content.ToString());

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
            LockHeight = false;
            TextWrappingMode = TextWrapping.Wrap;
            WindowWidth = -1;
            WindowMinHeight = -1;
            TitleFontSize = -1;
            MessageFontSize = -1;
            ButtonFontSize = -1;
            TitleFontColor = new MessageBoxColor(Colors.Black);
            MessageFontColor = new MessageBoxColor(Colors.Black);
            ButtonFontColor = new MessageBoxColor(Colors.Black);
            WindowOpacity = -1;
            TitleBarOpacity = -1;
            MessageBarOpacity = -1;
            ButtonBarOpacity = -1;
            TitlePanelColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
            MessagePanelColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
            ButtonPanelColor = new MessageBoxColor("#DDDDDD", ColorType.HEX);
            WndBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
            TitlePanelBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
            MessagePanelBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
            ButtonPanelBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
            ButtonBorderColor = new MessageBoxColor(Colors.Black, ColorType.COLORNAME);
            WndBorderThickness = new Thickness(2);
            TitlePanelBorderThickness = new Thickness(0, 0, 0, 1);
            MessagePanelBorderThickness = new Thickness(0);
            ButtonPanelBorderThickness = new Thickness(0);
            ButtonBorderThickness = new Thickness(0);
            TitleFontFamily = new FontFamily("Times New Roman");
            MessageFontFamily = new FontFamily("Times New Roman");
            ButtonFontFamily = new FontFamily("Times New Roman");
            WindowShowDuration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
            WindowShowAnimations = null;
            WindowCloseAnimations = null;
            CloseIcon = new BitmapImage(new Uri(".\\Image\\close.png", UriKind.RelativeOrAbsolute));
            WarningIcon = new BitmapImage(new Uri(".\\Image\\warn.png", UriKind.RelativeOrAbsolute));
            ErrorIcon = new BitmapImage(new Uri(".\\Image\\error.png", UriKind.RelativeOrAbsolute));
            InfoIcon = new BitmapImage(new Uri(".\\Image\\info.png", UriKind.RelativeOrAbsolute));
            QuestionIcon = new BitmapImage(new Uri(".\\Image\\question.png", UriKind.RelativeOrAbsolute));
            EnableCloseButton = false;
            StyleList = new List<Style> { new ResourceDictionary { Source = new Uri("pack://application:,,,/MessageBoxTouch;component/WndStyles.xaml") }["MessageBoxButtonStyle"] as Style };
            CloseTimer = null;

            PropertiesSetter = new PropertiesSetter();

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
                    // 单字符高度
                    double height = new FormattedText(" ", CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.tb_msg.FontFamily.ToString()), mb.tb_msg.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip).Height;
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
                                    ft = new FormattedText(nextWord, CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.tb_msg.FontFamily.ToString()), mb.tb_msg.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip);
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
                                    ft = new FormattedText(mb.tb_msg.Text[i].ToString(), CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.tb_msg.FontFamily.ToString()), mb.tb_msg.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip);
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
                                    ft = new FormattedText(nextWord, CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.tb_msg.FontFamily.ToString()), mb.tb_msg.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip);
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
                                    ft = new FormattedText(mb.tb_msg.Text[i].ToString(), CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.tb_msg.FontFamily.ToString()), mb.tb_msg.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip);
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
                                ft = new FormattedText(mb.tb_msg.Text[i].ToString(), CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.tb_msg.FontFamily.ToString()), mb.tb_msg.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip);
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
                    mb.Height = height * lineCount + rd_title.Height.Value + rd_button.Height.Value + mb.tb_msg.Margin.Top + mb.tb_msg.Margin.Bottom;
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
        /// 当用户改变自定控件的大小时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ButtonObjectSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 如果是在调用ShowDialog后触发的SizeChanged事件则无视
            if (mb.isSizeChangedByShowDialog)
            {
                mb.isSizeChangedByShowDialog = false;
                return;
            }

            // 如果新高度大于按钮标准高度
            if (e.NewSize.Height > mb.buttonHeight)
            {
                mb.g_buttongrid.Height = e.NewSize.Height + 13;
                mb.rd_button.Height = new GridLength(e.NewSize.Height + 13, GridUnitType.Pixel);
                mb.b_buttonborder.Height = e.NewSize.Height + 13;
            }
            else
            {
                mb.g_buttongrid.Height = mb.buttonHeight + 13;
                mb.rd_button.Height = new GridLength(mb.buttonHeight + 13, GridUnitType.Pixel);
                mb.b_buttonborder.Height = mb.buttonHeight + 13;
            }

            // 如果宽度有变化, 重新设置列宽
            if (e.WidthChanged)
            {
                for (int i = 0; i < btnList.Count; i++)
                {
                    if (btnList[i].Equals(sender))
                    {
                        FrameworkElement fe = (FrameworkElement)sender;
                        mb.g_buttongrid.ColumnDefinitions[i].Width = new GridLength(!fe.Width.Equals(Double.NaN) ? fe.Width : 1, !fe.Width.Equals(Double.NaN) ? GridUnitType.Pixel : GridUnitType.Star);
                    }
                }
            }
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
    }
}