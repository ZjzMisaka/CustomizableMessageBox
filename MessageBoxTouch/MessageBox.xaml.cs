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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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

        // 换行计算模式
        enum WarpMode
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
            NOWARP
        }

        static WarpMode warpMode = WarpMode.WORD;

        // Messagebox实例
        private static MessageBox mb;

        // 用户选择的结果
        private static int currentClickIndex = -1;

        // 自定按钮列表
        private static List<string> btnList = null;

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
                        warpMode = WarpMode.WORD;
                        break;
                    case TextWrapping.NoWrap:
                        warpMode = WarpMode.NOWARP;
                        break;
                    case TextWrapping.WrapWithOverflow:
                        warpMode = WarpMode.WORDWITHOVERFLOW;
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

        // 标题字体大小
        private static int titleFontSize = 30;
        public static int TitleFontSize { get => titleFontSize; set => titleFontSize = value; }

        // 消息文本字体大小
        private static int messageFontSize = 25;
        public static int MessageFontSize { get => messageFontSize; set => messageFontSize = value; }

        // 按钮字体大小
        private static int buttonFontSize = 30;
        public static int ButtonFontSize { get => buttonFontSize; set => buttonFontSize = value; }

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
        private static MessageBoxColor titleBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
        public static MessageBoxColor TitleBorderColor { get => titleBorderColor; set => titleBorderColor = value; }

        // 消息区域边框颜色
        private static MessageBoxColor messageBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
        public static MessageBoxColor MessageBorderColor { get => messageBorderColor; set => messageBorderColor = value; }

        // 按钮区域边框颜色
        private static MessageBoxColor buttonBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
        public static MessageBoxColor ButtonBorderColor { get => buttonBorderColor; set => buttonBorderColor = value; }

        // 窗口边框宽度
        private static Thickness wndBorderThickness = new Thickness(2);
        public static Thickness WndBorderThickness { get => wndBorderThickness; set => wndBorderThickness = value; }

        // 标题区域边框宽度
        private static Thickness titleBorderThickness = new Thickness(0, 0, 0, 1);
        public static Thickness TitleBorderThickness { get => titleBorderThickness; set => titleBorderThickness = value; }

        // 消息区域边框宽度
        private static Thickness messageBorderThickness = new Thickness(0);
        public static Thickness MessageBorderThickness { get => messageBorderThickness; set => messageBorderThickness = value; }

        // 按钮区域边框宽度
        private static Thickness buttonBorderThickness = new Thickness(0);
        public static Thickness ButtonBorderThickness { get => buttonBorderThickness; set => buttonBorderThickness = value; }

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
                WindowOpacity = value.WindowOpacity;
                TitleBarOpacity = value.TitleBarOpacity;
                MessageBarOpacity = value.MessageBarOpacity;
                ButtonBarOpacity = value.ButtonBarOpacity;
                TitlePanelColor = value.TitlePanelColor;
                MessagePanelColor = value.MessagePanelColor;
                ButtonPanelColor = value.ButtonPanelColor;
                WndBorderColor = value.WndBorderColor;
                TitleBorderColor = value.TitleBorderColor;
                MessageBorderColor = value.MessageBorderColor;
                ButtonBorderColor = value.ButtonBorderColor;
                WndBorderThickness = value.WndBorderThickness;
                TitleBorderThickness = value.TitleBorderThickness;
                MessageBorderThickness = value.MessageBorderThickness;
                ButtonBorderThickness = value.ButtonBorderThickness;
            }
        }

        // 像素密度
        private static double pixelsPerDip;

        // 构造函数
        public MessageBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 调出消息窗口
        /// </summary>
        /// <param name="msg">MessageBox消息内容</param>
        /// <param name="title">MessageBox窗口标题</param>
        /// <param name="selectStyle">按钮种类</param>
        /// <param name="img">消息类型</param>
        /// <returns></returns>
        public static MessageBoxResult Show(string msg, string title = "", MessageBoxButton selectStyle = MessageBoxButton.OK, MessageBoxImage img = MessageBoxImage.None)
        {
            // 按钮列表
            List<string> btnList;

            // 判断按钮类型并显示相应的按钮
            switch (selectStyle)
            {
                case MessageBoxButton.OK:
                    btnList = new List<string> { MessageBoxResult.OK.ToString() };
                    break;

                case MessageBoxButton.OKCancel:
                    btnList = new List<string> { MessageBoxResult.OK.ToString(), MessageBoxResult.Cancel.ToString() };
                    break;

                case MessageBoxButton.YesNo:
                    btnList = new List<string> { MessageBoxResult.Yes.ToString(), MessageBoxResult.No.ToString() };
                    break;

                case MessageBoxButton.YesNoCancel:
                    btnList = new List<string> { MessageBoxResult.Yes.ToString(), MessageBoxResult.No.ToString(), MessageBoxResult.Cancel.ToString() };
                    break;

                default:
                    btnList = new List<string> { MessageBoxResult.OK.ToString() };
                    break;
            }

            // 获取返回值字符串
            string resultStr = btnList[Show(btnList, msg, title, img)];

            // 找到对应的MessageBoxResult元素并返回
            return (MessageBoxResult)System.Enum.Parse(typeof(MessageBoxResult), resultStr);
        }

        /// <summary>
        /// 调出消息窗口
        /// </summary>
        /// <param name="btnList">自定按钮列表</param>
        /// <param name="msg">MessageBox消息内容</param>
        /// <param name="title">MessageBox窗口标题</param>
        /// <param name="img">消息类型</param>
        /// <returns></returns>
        public static int Show(List<string> btnList, string msg, string title = "", MessageBoxImage img = MessageBoxImage.None)
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
                        mb.i_img.Source = new BitmapImage(new Uri(".\\Image\\warn.png", UriKind.RelativeOrAbsolute));
                        mb.i_img.Visibility = Visibility.Visible;
                        mb.tb_msg.Width = mb.Width - mb.i_img.Width - mb.i_img.Margin.Left - mb.i_img.Margin.Right - mb.tb_msg.Margin.Left - mb.tb_msg.Margin.Right;
                        break;

                    case MessageBoxImage.Error:
                        mb.i_img.Source = new BitmapImage(new Uri(".\\Image\\error.png", UriKind.RelativeOrAbsolute));
                        mb.i_img.Visibility = Visibility.Visible;
                        mb.tb_msg.Width = mb.Width - mb.i_img.Width - mb.i_img.Margin.Left - mb.i_img.Margin.Right - mb.tb_msg.Margin.Left - mb.tb_msg.Margin.Right;
                        break;

                    case MessageBoxImage.Information:
                        mb.i_img.Source = new BitmapImage(new Uri(".\\Image\\info.png", UriKind.RelativeOrAbsolute));
                        mb.i_img.Visibility = Visibility.Visible;
                        mb.tb_msg.Width = mb.Width - mb.i_img.Width - mb.i_img.Margin.Left - mb.i_img.Margin.Right - mb.tb_msg.Margin.Left - mb.tb_msg.Margin.Right;
                        break;

                    case MessageBoxImage.Question:
                        mb.i_img.Source = new BitmapImage(new Uri(".\\Image\\question.png", UriKind.RelativeOrAbsolute));
                        mb.i_img.Visibility = Visibility.Visible;
                        mb.tb_msg.Width = mb.Width - mb.i_img.Width - mb.i_img.Margin.Left - mb.i_img.Margin.Right - mb.tb_msg.Margin.Left - mb.tb_msg.Margin.Right;
                        break;

                    case MessageBoxImage.None:
                        mb.tb_msg.Width = mb.Width - mb.tb_msg.Margin.Left - mb.tb_msg.Margin.Right;
                        break;

                    default:
                        mb.tb_msg.Width = mb.Width - mb.tb_msg.Margin.Left - mb.tb_msg.Margin.Right;
                        break;
                }

                // 显示消息内容
                mb.tb_msg.Text = mb.tb_msg.Text + msg;

                // 根据按钮字体计算自定按钮文本中最长的文本的宽度和字符高度
                double maxContentWidth = 0;
                double contentHeight = 0;

                // 判断按钮类型并显示相应的按钮
                for (int i = 0; i < btnList.Count; ++i)
                {
                    // 当有多于两个选项时, 增加Grid的列数
                    if (i >= 2)
                    {
                        mb.g_buttongrid.ColumnDefinitions.Add(new ColumnDefinition());
                    }
                    // 实例化一个新的按钮
                    Button newBtn = new Button();
                    // 将按钮加入Grid中
                    mb.g_buttongrid.Children.Add(newBtn);
                    // 设置按钮在Grid中的行列
                    newBtn.SetValue(Grid.RowProperty, 0);
                    newBtn.SetValue(Grid.ColumnProperty, i);
                    // 引入按钮的样式
                    var myResourceDictionary = new ResourceDictionary
                    {
                        // 指定样式文件的路径
                        Source = new Uri("pack://application:,,,/MessageBoxTouch;component/WndStyles.xaml")
                    };
                    // 通过key找到指定的样式并赋值给按钮
                    var myButtonStyle = myResourceDictionary["MessageBoxButtonStyle"] as Style;
                    newBtn.Style = myButtonStyle;
                    //设置按钮文本
                    newBtn.Content = btnList[i];
                    // 设置按钮外边距
                    newBtn.Margin = new Thickness(10, 0, 10, 0);
                    // 设置按钮可见
                    newBtn.Visibility = Visibility.Visible;
                    // 绑定按钮点击事件
                    newBtn.Click += BtnClicked;

                    // 设置各种属性
                    if (WindowWidth > 0)
                    {
                        mb.Width = WindowWidth;
                    }
                    if (WindowMinHeight > 0)
                    {
                        mb.MinHeight = WindowMinHeight;
                    }
                    if (TitleFontSize > 0)
                    {
                        mb.l_title.FontSize = TitleFontSize;
                    }
                    if (MessageFontSize > 0)
                    {
                        mb.tb_msg.FontSize = MessageFontSize;
                    }
                    if (ButtonFontSize > 0)
                    {
                        newBtn.FontSize = ButtonFontSize;
                    }
                    if (WindowOpacity > 0)
                    {
                        mb.Opacity = WindowOpacity;
                    }
                    if (TitleBarOpacity > 0)
                    {
                        mb.g_title.Opacity = TitleBarOpacity;
                    }
                    if (MessageBarOpacity > 0)
                    {
                        mb.g_message.Opacity = MessageBarOpacity;
                    }
                    if (ButtonBarOpacity > 0)
                    {
                        mb.g_buttongrid.Opacity = ButtonBarOpacity;
                    }
                    if (TitlePanelColor != null)
                    {
                        mb.g_title.Background = TitlePanelColor.GetSolidColorBrush();
                    }
                    if (MessagePanelColor != null)
                    {
                        mb.g_message.Background = MessagePanelColor.GetSolidColorBrush();
                    }
                    if (ButtonPanelColor != null)
                    {
                        mb.g_buttongrid.Background = ButtonPanelColor.GetSolidColorBrush();
                    }
                    if (WndBorderColor != null)
                    {
                        mb.b_wndborder.Background = WndBorderColor.GetSolidColorBrush();
                    }
                    if (TitleBorderColor != null)
                    {
                        mb.b_titleborder.Background = TitleBorderColor.GetSolidColorBrush();
                    }
                    if (MessageBorderColor != null)
                    {
                        mb.b_messageborder.Background = MessageBorderColor.GetSolidColorBrush();
                    }
                    if (ButtonBorderColor != null)
                    {
                        mb.b_buttonborder.Background = ButtonBorderColor.GetSolidColorBrush();
                    }
                    if (WndBorderThickness != null)
                    {
                        mb.b_wndborder.BorderThickness = WndBorderThickness;
                    }
                    if (TitleBorderThickness != null)
                    {
                        mb.b_titleborder.BorderThickness = TitleBorderThickness;
                    }
                    if (MessageBorderThickness != null)
                    {
                        mb.b_messageborder.BorderThickness = MessageBorderThickness;
                    }
                    if (ButtonBorderThickness != null)
                    {
                        mb.b_buttonborder.BorderThickness = ButtonBorderThickness;
                    }
                    mb.tb_msg.TextWrapping = TextWrappingMode;

                    // 根据字体计算标题字符串高度
                    double height = new FormattedText(" ", CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.l_title.FontFamily.ToString()), mb.l_title.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip).Height;
                    // 设置标题栏高度
                    mb.g_title.Height = height + 7;
                    mb.rd_title.Height = new GridLength(height + 14);

                    // 当该变量还没计算时
                    if (maxContentWidth == 0)
                    {
                        foreach (string s in btnList)
                        {
                            // 使用字符串和字体设置作为参数实例化FormattedText
                            FormattedText ft = new FormattedText(s, CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(newBtn.FontFamily.ToString()), newBtn.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip);
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

                    // 设置按钮的宽高
                    // newBtn.Width = maxContentWidth + 7;
                    newBtn.Height = contentHeight + 7;
                }

                // 设置按钮栏高度
                mb.g_buttongrid.Height = contentHeight + 20;
                mb.rd_button.Height = new GridLength(contentHeight + 20, GridUnitType.Pixel);

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

        public static int Show(PropertiesSetter propertiesSetter, List<string> btnList, string msg, string title = "", MessageBoxImage img = MessageBoxImage.None)
        {
            PropertiesSetter = propertiesSetter;
            return Show(btnList, msg, title, img);
        }

        public static MessageBoxResult Show(PropertiesSetter propertiesSetter, string msg, string title = "", MessageBoxButton selectStyle = MessageBoxButton.OK, MessageBoxImage img = MessageBoxImage.None)
        {
            PropertiesSetter = propertiesSetter;
            return Show(msg, title, selectStyle, img);
        }

        /// <summary>
        /// 选择后保存选择结果并关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void BtnClicked(Object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            // 遍历按钮列表寻找和按钮文本相符的元素的索引
            currentClickIndex = btnList.IndexOf(btn.Content.ToString());

            // 关闭窗口
            mb.Hide();
            mb.Close();
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
            WindowOpacity = -1;
            TitleBarOpacity = -1;
            MessageBarOpacity = -1;
            ButtonBarOpacity = -1;
            TitlePanelColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
            MessagePanelColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
            ButtonPanelColor = new MessageBoxColor("#DDDDDD", ColorType.HEX);
            WndBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
            TitleBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
            MessageBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
            ButtonBorderColor = new MessageBoxColor(Colors.White, ColorType.COLORNAME);
            WndBorderThickness = new Thickness(2);
            TitleBorderThickness = new Thickness(0, 0, 0, 1);
            MessageBorderThickness = new Thickness(0);
            ButtonBorderThickness = new Thickness(0);
            propertiesSetter = new PropertiesSetter();

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
                            if (warpMode == WarpMode.NOWARP)
                            {
                                // DO NOTHING
                            }
                            else if (warpMode == WarpMode.WORDWITHOVERFLOW)
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
                            else if (warpMode == WarpMode.WORD)
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
                                        warpMode = WarpMode.TEXTINWORD;
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
                            else if (warpMode == WarpMode.TEXTINWORD)
                            {
                                if (!Regex.IsMatch(mb.tb_msg.Text[i].ToString(), "[a-zA-Z0-9]"))
                                {
                                    warpMode = WarpMode.WORD;
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
            mb.Hide();
            mb.Close();
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
    }
}