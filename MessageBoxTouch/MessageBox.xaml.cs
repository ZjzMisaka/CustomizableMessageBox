using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

        // Messagebox实例
        private static MessageBox mb;

        // 用户选择的结果
        private static Object currentClick = null;

        private static readonly TaskScheduler _syncContextTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        private enum MessageBoxMode {
            // 兼容系统MessageBox参数模式
            COMPATIBLE,
            // 自定义MessageBox模式
            CUSTOMIZED
        }

        // 自定按钮列表
        private static List<string> btnList = null;

        // 模式选择
        private static MessageBoxMode messageBoxMode;

        // 窗口宽度
        private static int windowWidth = -1;
        public static int WindowWidth { get => windowWidth; set => windowWidth = value; }

        // 窗口最小高度
        private static int windowMinHeight = -1;
        public static int WindowMinHeight { get => windowMinHeight; set => windowMinHeight = value; }

        // 标题字体大小
        private static int titleFontSize = -1;
        public static int TitleFontSize { get => titleFontSize; set => titleFontSize = value; }

        // 消息文本字体大小
        private static int messageFontSize = -1;
        public static int MessageFontSize { get => messageFontSize; set => messageFontSize = value; }

        // 按钮字体大小
        private static int buttonFontSize = -1;
        public static int ButtonFontSize { get => buttonFontSize; set => buttonFontSize = value; }
        
        // 窗口透明度
        private static double windowOpacity = -1;
        public static double WindowOpacity { get => windowOpacity; set => windowOpacity = value; }

        // 标题栏透明度
        private static double titleBarOpacity = -1;
        public static double TitleBarOpacity { get => titleBarOpacity; set => titleBarOpacity = value; }

        // 消息栏透明度
        private static double messageBarOpacity = -1;
        public static double MessageBarOpacity { get => messageBarOpacity; set => messageBarOpacity = value; }

        // 按钮栏透明度
        private static double buttonBarOpacity = -1;
        public static double ButtonBarOpacity { get => buttonBarOpacity; set => buttonBarOpacity = value; }
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
            // 兼容模式
            messageBoxMode = MessageBoxMode.COMPATIBLE;

            try
            {
                if (mb != null)
                    return MessageBoxResult.Cancel;

                // 初始化窗口
                mb = new MessageBox();

                // 计算像素密度
                pixelsPerDip = VisualTreeHelper.GetDpi(mb).PixelsPerDip;

                //设定窗口最大高度为窗口工作区高度
                mb.MaxHeight = SystemInformation.WorkingArea.Height;

                //绑定按钮点击事件
                mb.btn_cancel.Click += BtnClicked;
                mb.btn_no.Click += BtnClicked;
                mb.btn_ok.Click += BtnClicked;
                mb.btn_single_ok.Click += BtnClicked;
                mb.btn_yes.Click += BtnClicked;

                //绑定关闭按钮点击事件
                mb.i_close.MouseLeftButtonUp += I_close_TouchDown;

                // 绑定标题栏鼠标事件, 拖动窗口
                mb.l_title.MouseLeftButtonDown += L_title_MouseLeftButtonDown;

                // 设置各种属性
                if (WindowWidth != -1)
                {
                    mb.Width = WindowWidth;
                }
                if(WindowMinHeight != -1)
                {
                    mb.MinHeight = WindowMinHeight;
                }
                if (TitleFontSize != -1)
                {
                    mb.l_title.FontSize = TitleFontSize;
                    // 根据字体计算标题字符串高度
                    double height = new FormattedText(" ", CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.l_title.FontFamily.ToString()), mb.l_title.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip).Height;
                    // 设置标题栏高度
                    mb.g_title.Height = height + 7;
                    mb.rd_title.Height = new GridLength(height + 14);
                }
                if (MessageFontSize != -1)
                {
                    mb.tb_msg.FontSize = MessageFontSize;
                }
                if (ButtonFontSize != -1)
                {
                    mb.btn_cancel.FontSize = MessageFontSize;
                    mb.btn_no.FontSize = MessageFontSize;
                    mb.btn_ok.FontSize = MessageFontSize;
                    mb.btn_single_ok.FontSize = MessageFontSize;
                    mb.btn_yes.FontSize = MessageFontSize;
                    // 根据字体计算按钮字符串高度
                    double height = new FormattedText(" ", CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.tb_msg.FontFamily.ToString()), mb.tb_msg.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip).Height;
                    // 设置按钮高度
                    mb.btn_cancel.Height = height + 7;
                    mb.btn_no.Height = height + 7;
                    mb.btn_ok.Height = height + 7;
                    mb.btn_single_ok.Height = height + 7;
                    mb.btn_yes.Height = height + 7;
                    // 设置按钮外边距
                    mb.btn_cancel.Margin = new Thickness(10, 0, 10, 0);
                    mb.btn_no.Margin = new Thickness(10, 0, 10, 0);
                    mb.btn_ok.Margin = new Thickness(10, 0, 10, 0);
                    mb.btn_single_ok.Margin = new Thickness(10, 0, 10, 0);
                    mb.btn_yes.Margin = new Thickness(10, 0, 10, 0);
                    // 设置按钮栏高度
                    mb.g_buttongrid.Height = height + 20;
                    mb.rd_button.Height = new GridLength(height + 20, GridUnitType.Pixel);
                }
                if (WindowOpacity != -1)
                {
                    mb.Opacity = WindowOpacity;
                }
                if (TitleBarOpacity != -1)
                {
                    mb.g_title.Opacity = TitleBarOpacity;
                }
                if (MessageBarOpacity != -1)
                {
                    mb.g_message.Opacity = MessageBarOpacity;
                }
                if (ButtonBarOpacity != -1)
                {
                    mb.g_buttongrid.Opacity = ButtonBarOpacity;
                }

                // 重置选择结果
                currentClick = MessageBoxResult.Cancel;

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

                // 判断按钮类型并显示相应的按钮
                switch (selectStyle)
                {
                    case MessageBoxButton.OK:
                        mb.btn_single_ok.Visibility = Visibility.Visible;
                        break;

                    case MessageBoxButton.OKCancel:
                        mb.btn_ok.Visibility = Visibility.Visible;
                        mb.btn_cancel.Visibility = Visibility.Visible;
                        break;

                    case MessageBoxButton.YesNo:
                        mb.btn_yes.Visibility = Visibility.Visible;
                        mb.btn_no.Visibility = Visibility.Visible;
                        break;

                    case MessageBoxButton.YesNoCancel:
                        mb.btn_yes.Visibility = Visibility.Visible;
                        mb.btn_no.Visibility = Visibility.Visible;
                        mb.i_close.Visibility = Visibility.Visible;
                        break;

                    default:
                        mb.btn_single_ok.Visibility = Visibility.Visible;
                        break;
                }

                // 设置所属的窗口
                mb.Owner = Application.Current.MainWindow;

                // 显示窗口
                mb.ShowDialog();
            }
            catch
            {
                // 调用系统MessageBox
                currentClick = System.Windows.MessageBox.Show(msg, title, selectStyle, img);
            }

            // 返回用户选择的结果
            return (MessageBoxResult)currentClick;
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
            // 自定模式
            messageBoxMode = MessageBoxMode.CUSTOMIZED;

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
                currentClick = -1;

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
                for (int i = 0;  i < btnList.Count; ++i)
                {
                    // 当有多于两个选项时, 增加Grid的列数
                    if(i >= 2)
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
                    if (WindowWidth != -1)
                    {
                        mb.Width = WindowWidth;
                    }
                    if (WindowMinHeight != -1)
                    {
                        mb.MinHeight = WindowMinHeight;
                    }
                    if (TitleFontSize != -1)
                    {
                        mb.l_title.FontSize = TitleFontSize;
                        // 根据字体计算标题字符串高度
                        double height = new FormattedText(" ", CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.l_title.FontFamily.ToString()), mb.l_title.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip).Height;
                        // 设置标题栏高度
                        mb.g_title.Height = height + 7;
                        mb.rd_title.Height = new GridLength(height + 14);
                    }
                    if (MessageFontSize != -1)
                    {
                        mb.tb_msg.FontSize = MessageFontSize;
                    }
                    if (ButtonFontSize != -1)
                    {
                        newBtn.FontSize = ButtonFontSize;
                    }
                    if (WindowOpacity != -1)
                    {
                        mb.Opacity = WindowOpacity;
                    }
                    if (TitleBarOpacity != -1)
                    {
                        mb.g_title.Opacity = TitleBarOpacity;
                    }
                    if (MessageBarOpacity != -1)
                    {
                        mb.g_message.Opacity = MessageBarOpacity;
                    }
                    if (ButtonBarOpacity != -1)
                    {
                        mb.g_buttongrid.Opacity = ButtonBarOpacity;
                    }

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
                currentClick = System.Windows.MessageBox.Show(msg, title, MessageBoxButton.OK, img);
                return -1;
            }

            // 返回用户选择的结果
            return (int)currentClick;
        }


        /// <summary>
        /// 选择后保存选择结果并关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void BtnClicked(Object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            // 与系统MessageBox兼容的情况
            if (messageBoxMode == MessageBoxMode.COMPATIBLE)
            {
                if (btn == mb.btn_cancel)
                {
                    currentClick = MessageBoxResult.Cancel;
                }
                else if (btn == mb.btn_no)
                {
                    currentClick = MessageBoxResult.No;
                }
                else if (btn == mb.btn_ok || btn == mb.btn_single_ok)
                {
                    currentClick = MessageBoxResult.OK;
                }
                else if (btn == mb.btn_yes)
                {
                    currentClick = MessageBoxResult.Yes;
                }
            }
            // 自定MessageBox的情况
            else
            {
                // 遍历按钮列表寻找和按钮文本相符的元素的索引
                currentClick = btnList.IndexOf(btn.Content.ToString());
            }

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
            WindowWidth = -1;
            WindowMinHeight = -1;
            TitleFontSize = -1;
            MessageFontSize = -1;
            ButtonFontSize = -1;
            WindowOpacity = -1;
            TitleBarOpacity = -1;
            MessageBarOpacity = -1;
            ButtonBarOpacity = -1;
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
                // 消息字符串的总长度
                double totalWidth = 0;
                // 单字符高度
                double height = 0;
                // 自从换行后字符串的累积宽度
                double widthSinceCr = 0;
                // 自从自动换行后字符串的累积宽度
                double widthSinceAutoCr = 0;
                // 手动换行后的前一行的剩余空白宽度
                double redundantWidth = 0;
                // 格式化的字符串
                FormattedText ft;
                // 遍历消息字符串中的每个字符
                foreach (char c in mb.tb_msg.Text)
                {
                    if (c == '\n')
                    {
                        // 计算手动换行后上一行的多余空白的宽度
                        double TextblockWidth = mb.tb_msg.Width;
                        double TextblockWidthTemp = TextblockWidth;
                        while (TextblockWidth < widthSinceCr)
                        {
                            TextblockWidth += TextblockWidthTemp;
                        }
                        redundantWidth += TextblockWidth % widthSinceCr;

                        // 换行后的字符串宽度被重置为零
                        widthSinceCr = 0;
                    }
                    else
                    {
                        // 使用字符串和字体设置作为参数实例化FormattedText
                        ft = new FormattedText(c.ToString(), CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.tb_msg.FontFamily.ToString()), mb.tb_msg.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip);

                        // 计算自动换行后上一行的剩余空白的宽度
                        if (widthSinceAutoCr + ft.Width > mb.tb_msg.Width)
                        {
                            // 将行内空白宽度附加到字符串宽度
                            widthSinceCr += mb.tb_msg.Width % widthSinceAutoCr;
                            widthSinceAutoCr = 0;
                        }

                        // 累加手动换行后的字符串的宽度
                        widthSinceCr += ft.Width;
                        widthSinceAutoCr += ft.Width;
                        // 累加字符的总宽度和高度
                        totalWidth += ft.Width;
                        height = ft.Height;
                    }
                }

                // 计算窗口高度
                mb.Height = height * ((totalWidth + redundantWidth) / (mb.tb_msg.Width - (mb.tb_msg.Margin.Left + mb.tb_msg.Margin.Right))) + rd_title.Height.Value + rd_button.Height.Value + mb.tb_msg.Margin.Top + mb.tb_msg.Margin.Bottom + 100;

                // 将窗口初始位置设置在屏幕中心
                SetWindowPos(new WindowInteropHelper(mb).Handle, new IntPtr(0), (int)(SystemInformation.WorkingArea.Width / 2 - mb.Width / 2), (int)(SystemInformation.WorkingArea.Height / 2 - mb.Height / 2), (int)(mb.Width), (int)(mb.Height), 0x1);
            }
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