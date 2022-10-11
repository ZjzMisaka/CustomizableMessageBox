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
using KeyEventHandler = System.Windows.Input.KeyEventHandler;

namespace CustomizableMessageBox
{
    /// <summary>
    /// MessageBox.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class MessageBox : Window
    {
        public class RefreshList : List<object>
        {
            public new void RemoveAt(int index)
            {
                base.RemoveAt(index);
                if (mb == null)
                {
                    return;
                }
                LoadButtonPanel();
            }

            public new void Remove(object item)
            {
                base.Remove(item);
                if (mb == null)
                {
                    return;
                }
                LoadButtonPanel();
            }

            public new int RemoveAll(Predicate<object> match)
            {
                int res = base.RemoveAll(match);
                if (mb == null)
                {
                    return res;
                }
                LoadButtonPanel();
                return res;
            }

            public new void RemoveRange(int index, int count)
            {
                base.RemoveRange(index, count);
                if (mb == null)
                {
                    return;
                }
                LoadButtonPanel();
            }

            public new void Add(object obj)
            {
                base.Add(obj);
                if (mb == null)
                {
                    return;
                }
                LoadButtonPanel();
            }

            public new void AddRange(object obj)
            {
                base.Add(obj);
                if (mb == null)
                {
                    return;
                }
                LoadButtonPanel();
            }

            public new void AddRange(IEnumerable<object> collection)
            {
                base.AddRange(collection);
                if (mb == null)
                {
                    return;
                }
                LoadButtonPanel();
            }

            public new void Insert(int index, object item)
            {
                base.Insert(index, item);
                if (mb == null)
                {
                    return;
                }
                LoadButtonPanel();
            }

            public new void InsertRange(int index, IEnumerable<object> collection)
            {
                base.InsertRange(index, collection);
                if (mb == null)
                {
                    return;
                }
                LoadButtonPanel();
            }
        }

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

        // 默认样式属性
        private static PropertiesSetter defaultProperties = null;
        public static PropertiesSetter DefaultProperties
        {
            get
            {
                return defaultProperties;
            }
            set
            {
                defaultProperties = value;
            }
        }

        // 设置 / 取得标题文字
        public static string TitleText
        {
            get
            {
                return mb.i_title.Content == null ? "" : mb.i_title.Content.ToString();
            }
            set
            {
                if (mb != null)
                {
                    mb.i_title.Content = value;
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
        private static RefreshList buttonList = null;
        public static RefreshList ButtonList
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

                    foreach (object obj in buttonList)
                    {
                        if (obj is FrameworkElement)
                        {
                            FrameworkElement fe = (FrameworkElement)obj;
                        }
                    }

                    LoadButtonPanel();
                }
            }
        }

        static WrapMode wrapMode = WrapMode.WORD;

        // Messagebox实例
        private static MessageBox mb;

        // 用户选择的结果
        private static int currentClickIndex = -1;

        // 是否定时关闭
        private static bool isClosedByTimer = false;

        // 锁定高度
        private static bool lockHeight = Prefab.lockHeightDef;
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
        private static TextWrapping textWrappingMode = Prefab.textWrappingModeDef;
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
        private static double windowWidth = Prefab.windowWidthDef;
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
        private static double windowMinHeight = Prefab.windowMinHeightDef;
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
        private static int titleFontSize = Prefab.titleFontSizeDef;
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
        private static int messageFontSize = Prefab.messageFontSizeDef;
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
        private static int buttonFontSize = Prefab.buttonFontSizeDef;
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
        private static MessageBoxColor titleFontColor = Prefab.titleFontColorDef;
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
        private static MessageBoxColor messageFontColor = Prefab.messageFontColorDef;
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
        private static MessageBoxColor buttonFontColor = Prefab.buttonFontColorDef;
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
        private static double windowOpacity = Prefab.windowOpacityDef;
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
        private static double titleBarOpacity = Prefab.titleBarOpacityDef;
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
        private static double messageBarOpacity = Prefab.messageBarOpacityDef;
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
        private static double buttonBarOpacity = Prefab.buttonBarOpacityDef;
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
        private static MessageBoxColor titlePanelColor = Prefab.titlePanelColorDef;
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
        private static MessageBoxColor messagePanelColor = Prefab.messagePanelColorDef;
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
        private static MessageBoxColor buttonPanelColor = Prefab.buttonPanelColorDef;
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
        private static MessageBoxColor wndBorderColor = Prefab.wndBorderColorDef;
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
        private static MessageBoxColor titlePanelBorderColor = Prefab.titlePanelBorderColorDef;
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
        private static MessageBoxColor messagePanelBorderColor = Prefab.messagePanelBorderColorDef;
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
        private static MessageBoxColor buttonPanelBorderColor = Prefab.buttonPanelBorderColorDef;
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
        private static MessageBoxColor buttonBorderColor = Prefab.buttonBorderColorDef;
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
        private static Thickness wndBorderThickness = Prefab.wndBorderThicknessDef;
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
        private static Thickness titlePanelBorderThickness = Prefab.titlePanelBorderThicknessDef;
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
        private static Thickness messagePanelBorderThickness = Prefab.messagePanelBorderThicknessDef;
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
        private static Thickness buttonPanelBorderThickness = Prefab.buttonPanelBorderThicknessDef;
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
        private static Thickness buttonBorderThickness = Prefab.buttonBorderThicknessDef;
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
        private static FontFamily titleFontFamily = Prefab.titleFontFamilyDef;
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
        private static FontFamily messageFontFamily = Prefab.messageFontFamilyDef;
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
        private static FontFamily buttonFontFamily = Prefab.buttonFontFamilyDef;
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
        private static Duration windowShowDuration = Prefab.windowShowDurationDef;
        public static Duration WindowShowDuration { get => windowShowDuration; set => windowShowDuration = value; }

        // 窗口显示动画
        private static List<KeyValuePair<DependencyProperty, AnimationTimeline>> windowShowAnimations = null;
        public static List<KeyValuePair<DependencyProperty, AnimationTimeline>> WindowShowAnimations { get => windowShowAnimations; set => windowShowAnimations = value; }

        // 窗口关闭动画
        private static List<KeyValuePair<DependencyProperty, AnimationTimeline>> windowCloseAnimations = null;
        public static List<KeyValuePair<DependencyProperty, AnimationTimeline>> WindowCloseAnimations { get => windowCloseAnimations; set => windowCloseAnimations = value; }

        // 标题区域间距
        private static double titlePanelSpacing = Prefab.titlePanelSpacingDef;
        public static double TitlePanelSpacing
        {
            get => titlePanelSpacing;
            set
            {
                titlePanelSpacing = value;
                if (mb == null)
                {
                    return;
                }
                LoadTitlePanel();
                SetWindowSize();
            }
        }

        // Message区域间距
        private static double messgagePanelSpacing = Prefab.messgagePanelSpacingDef;
        public static double MessgagePanelSpacing
        {
            get => messgagePanelSpacing;
            set
            {
                messgagePanelSpacing = value;
                if (mb == null)
                {
                    return;
                }
                LoadMessagePanel();
                SetWindowSize();
            }
        }


        // 自定义关闭图标
        private static BitmapImage closeIcon = Prefab.closeIconDef;
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

        // 自定义标题图标
        private static BitmapImage titleIcon = Prefab.titleIconDef;
        public static BitmapImage TitleIcon
        {
            get => titleIcon;
            set
            {
                titleIcon = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 自定义警告图标
        private static BitmapImage warningIcon = Prefab.warningIconDef;
        public static BitmapImage WarningIcon
        {
            get => warningIcon;
            set
            {
                warningIcon = value;
                if (mb == null)
                {
                    return;
                }
                if (MessageBoxImageType != MessageBoxImage.None)
                {
                    SetIconType(MessageBoxImageType);
                }
            }
        }

        // 自定义错误图标
        private static BitmapImage errorIcon = Prefab.errorIconDef;
        public static BitmapImage ErrorIcon
        {
            get => errorIcon;
            set
            {
                errorIcon = value;
                if (mb == null)
                {
                    return;
                }
                if (MessageBoxImageType != MessageBoxImage.None)
                {
                    SetIconType(MessageBoxImageType);
                }
            }
        }

        // 自定义信息图标
        private static BitmapImage infoIcon = Prefab.infoIconDef;
        public static BitmapImage InfoIcon
        {
            get => infoIcon;
            set
            {
                infoIcon = value;
                if (mb == null)
                {
                    return;
                }
                if (MessageBoxImageType != MessageBoxImage.None)
                {
                    SetIconType(MessageBoxImageType);
                }
            }
        }

        // 自定义问题图标
        private static BitmapImage questionIcon = Prefab.questionIconDef;
        public static BitmapImage QuestionIcon
        {
            get => questionIcon;
            set
            {
                questionIcon = value;
                if (mb == null)
                {
                    return;
                }
                if (MessageBoxImageType != MessageBoxImage.None)
                {
                    SetIconType(MessageBoxImageType);
                }
            }
        }

        // Message图标的高度
        private static double messgaeIconHeight = Prefab.messgaeIconHeightDef;
        public static double MessgaeIconHeight
        {
            get => messgaeIconHeight;
            set
            {
                messgaeIconHeight = value;
                if (mb == null)
                {
                    return;
                }
                LoadMessagePanel();
                SetWindowSize();
            }
        }

        // 关闭按钮图标的高度
        private static double closeIconHeight = Prefab.closeIconHeightDef;
        public static double CloseIconHeight
        {
            get => closeIconHeight;
            set
            {
                closeIconHeight = value;
                if (mb == null)
                {
                    return;
                }
                LoadTitlePanel();
                SetWindowSize();
            }
        }

        // 标题图标的高度
        private static double titleIconHeight = Prefab.titleIconHeightDef;
        public static double TitleIconHeight
        {
            get => titleIconHeight;
            set
            {
                titleIconHeight = value;
                if (mb == null)
                {
                    return;
                }
                LoadTitlePanel();
            }
        }

        // 设置关闭按钮图标高度为标题字体高度
        private static bool setCloseIconHeightAsTitleFontHeight = Prefab.setCloseIconHeightAsTitleFontHeightDef;
        public static bool SetCloseIconHeightAsTitleFontHeight
        {
            get => setCloseIconHeightAsTitleFontHeight;
            set
            {
                setCloseIconHeightAsTitleFontHeight = value;
                if (mb == null)
                {
                    return;
                }
                LoadTitlePanel();
            }
        }

        // 设置标题图标高度为标题字体高度
        private static bool setTitleIconHeightAsTitleFontHeight = Prefab.setTitleIconHeightAsTitleFontHeightDef;
        public static bool SetTitleIconHeightAsTitleFontHeight
        {
            get => setTitleIconHeightAsTitleFontHeight;
            set
            {
                setTitleIconHeightAsTitleFontHeight = value;
                if (mb == null)
                {
                    return;
                }
                LoadTitlePanel();
            }
        }

        // 应用窗口关闭按钮
        private static bool enableCloseButton = Prefab.enableCloseButtonDef;
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

        // 应用标题图标
        private static bool enableTitleIcon = Prefab.enableTitleIconDef;
        public static bool EnableTitleIcon
        {
            get => enableTitleIcon;
            set
            {
                enableTitleIcon = value;
                if (mb == null)
                {
                    return;
                }
                InitProperties();
            }
        }

        // 按钮动作样式
        private static List<Style> buttonStyleList = null;
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


        private static List<double> buttonWidthList = null;
        public static List<double> ButtonWidthList
        {
            get => buttonWidthList;
            set
            {
                buttonWidthList = value;
                if (mb == null)
                {
                    return;
                }
                SetButtonStyle();
            }
        }

        private static List<double> buttonHeightList = null;
        public static List<double> ButtonHeightList
        {
            get => buttonHeightList;
            set
            {
                buttonHeightList = value;
                if (mb == null)
                {
                    return;
                }
                SetButtonStyle();
            }
        }

        private static List<System.Windows.HorizontalAlignment> buttonHorizontalAlignmentList = null;
        public static List<System.Windows.HorizontalAlignment> ButtonHorizontalAlignmentList
        {
            get => buttonHorizontalAlignmentList;
            set
            {
                buttonHorizontalAlignmentList = value;
                if (mb == null)
                {
                    return;
                }
                SetButtonStyle();
            }
        }

        private static List<VerticalAlignment> buttonVerticalAlignmentList = null;
        public static List<VerticalAlignment> ButtonVerticalAlignmentList
        {
            get => buttonVerticalAlignmentList;
            set
            {
                buttonVerticalAlignmentList = value;
                if (mb == null)
                {
                    return;
                }
                SetButtonStyle();
            }
        }

        private static List<System.Windows.HorizontalAlignment> buttonHorizontalContentAlignmentList = null;
        public static List<System.Windows.HorizontalAlignment> ButtonHorizontalContentAlignmentList
        {
            get => buttonHorizontalContentAlignmentList;
            set
            {
                buttonHorizontalContentAlignmentList = value;
                if (mb == null)
                {
                    return;
                }
                SetButtonStyle();
            }
        }

        private static List<VerticalAlignment> buttonVerticalContentAlignmentList = null;
        public static List<VerticalAlignment> ButtonVerticalContentAlignmentList
        {
            get => buttonVerticalContentAlignmentList;
            set
            {
                buttonVerticalContentAlignmentList = value;
                if (mb == null)
                {
                    return;
                }
                SetButtonStyle();
            }
        }

        private static List<Thickness> buttonMarginList = null;
        public static List<Thickness> ButtonMarginList
        {
            get => buttonMarginList;
            set
            {
                buttonMarginList = value;
                if (mb == null)
                {
                    return;
                }
                SetButtonStyle();
            }
        }

        private static List<Thickness> buttonPaddingList = null;
        public static List<Thickness> ButtonPaddingList
        {
            get => buttonPaddingList;
            set
            {
                buttonPaddingList = value;
                if (mb == null)
                {
                    return;
                }
                SetButtonStyle();
            }
        }

        private static List<Brush> buttonBackgroundList = null;
        public static List<Brush> ButtonBackgroundList
        {
            get => buttonBackgroundList;
            set
            {
                buttonBackgroundList = value;
                if (mb == null)
                {
                    return;
                }
                SetButtonStyle();
            }
        }

        private static List<Brush> buttonBorderBrushList = null;
        public static List<Brush> ButtonBorderBrushList
        {
            get => buttonBorderBrushList;
            set
            {
                buttonBorderBrushList = value;
                if (mb == null)
                {
                    return;
                }
                SetButtonStyle();
            }
        }

        private static List<Thickness> buttonBorderThicknessList = null;
        public static List<Thickness> ButtonBorderThicknessList
        {
            get => buttonBorderThicknessList;
            set
            {
                buttonBorderThicknessList = value;
                if (mb == null)
                {
                    return;
                }
                SetButtonStyle();
            }
        }

        private static List<System.Windows.Input.Cursor> buttonCursorList = null;
        public static List<System.Windows.Input.Cursor> ButtonCursorList
        {
            get => buttonCursorList;
            set
            {
                buttonCursorList = value;
                if (mb == null)
                {
                    return;
                }
                SetButtonStyle();
            }
        }

        private static List<FontFamily> buttonFontFamilyList = null;
        public static List<FontFamily> ButtonFontFamilyList
        {
            get => buttonFontFamilyList;
            set
            {
                buttonFontFamilyList = value;
                if (mb == null)
                {
                    return;
                }
                SetButtonStyle();
            }
        }

        private static List<double> buttonFontSizeList = null;
        public static List<double> ButtonFontSizeList
        {
            get => buttonFontSizeList;
            set
            {
                buttonFontSizeList = value;
                if (mb == null)
                {
                    return;
                }
                SetButtonStyle();
            }
        }

        private static List<FontStretch> buttonFontStretchList = null;
        public static List<FontStretch> ButtonFontStretchList
        {
            get => buttonFontStretchList;
            set
            {
                buttonFontStretchList = value;
                if (mb == null)
                {
                    return;
                }
                SetButtonStyle();
            }
        }

        private static List<FontStyle> buttonFontStyleList = null;
        public static List<FontStyle> ButtonFontStyleList
        {
            get => buttonFontStyleList;
            set
            {
                buttonFontStyleList = value;
                if (mb == null)
                {
                    return;
                }
                SetButtonStyle();
            }
        }

        private static List<FontWeight> buttonFontWeightList = null;
        public static List<FontWeight> ButtonFontWeightList
        {
            get => buttonFontWeightList;
            set
            {
                buttonFontWeightList = value;
                if (mb == null)
                {
                    return;
                }
                SetButtonStyle();
            }
        }

        // 窗口计时关闭
        private static MessageBoxCloseTimer closeTimer = null;
        public static MessageBoxCloseTimer CloseTimer
        {
            get => closeTimer;
            set
            {
                closeTimer = value;
                if (mb != null)
                {
                    SetCloseTimer();
                }
            }
        }

        // Loaded事件
        private static RoutedEventHandler loadedEventHandler = null;
        public static RoutedEventHandler LoadedEventHandler
        {
            get => loadedEventHandler;
            set => loadedEventHandler = value;
        }

        // KeyDown事件
        private static KeyEventHandler keyDownEventHandler = null;
        public static KeyEventHandler KeyDownEventHandler
        {
            get => keyDownEventHandler;
            set => keyDownEventHandler = value;
        }

        // KeyUp事件
        private static KeyEventHandler keyUpEventHandler = null;
        public static KeyEventHandler KeyUpEventHandler
        {
            get => keyUpEventHandler;
            set => keyUpEventHandler = value;
        }

        // 属性集合
        private static PropertiesSetter propertiesSetter = null;
        public static PropertiesSetter PropertiesSetter
        {
            get => propertiesSetter;
            set
            {
                if (lockHeight == Prefab.lockHeightDef)
                {
                    LockHeight = value.LockHeight;
                }
                if (textWrappingMode == Prefab.textWrappingModeDef)
                {
                    TextWrappingMode = value.TextWrappingMode;
                }
                if (windowWidth == Prefab.windowWidthDef)
                {
                    WindowWidth = value.WindowWidth;
                }
                if (windowMinHeight == Prefab.windowMinHeightDef)
                {
                    WindowMinHeight = value.WindowMinHeight;
                }
                if (titleFontSize == Prefab.titleFontSizeDef)
                {
                    TitleFontSize = value.TitleFontSize;
                }
                if (messageFontSize == Prefab.messageFontSizeDef)
                {
                    MessageFontSize = value.MessageFontSize;
                }
                if (buttonFontSize == Prefab.buttonFontSizeDef)
                {
                    ButtonFontSize = value.ButtonFontSize;
                }
                if (titleFontColor == Prefab.titleFontColorDef)
                {
                    TitleFontColor = value.TitleFontColor;
                }
                if (messageFontColor == Prefab.messageFontColorDef)
                {
                    MessageFontColor = value.MessageFontColor;
                }
                if (buttonFontColor == Prefab.buttonFontColorDef)
                {
                    ButtonFontColor = value.ButtonFontColor;
                }
                if (windowOpacity == Prefab.windowOpacityDef)
                {
                    WindowOpacity = value.WindowOpacity;
                }
                if (titleBarOpacity == Prefab.titleBarOpacityDef)
                {
                    TitleBarOpacity = value.TitleBarOpacity;
                }
                if (messageBarOpacity == Prefab.messageBarOpacityDef)
                {
                    MessageBarOpacity = value.MessageBarOpacity;
                }
                if (buttonBarOpacity == Prefab.buttonBarOpacityDef)
                {
                    ButtonBarOpacity = value.ButtonBarOpacity;
                }
                if (titlePanelColor == Prefab.titlePanelColorDef)
                {
                    TitlePanelColor = value.TitlePanelColor;
                }
                if (messagePanelColor == Prefab.messagePanelColorDef)
                {
                    MessagePanelColor = value.MessagePanelColor;
                }
                if (buttonPanelColor == Prefab.buttonPanelColorDef)
                {
                    ButtonPanelColor = value.ButtonPanelColor;
                }
                if (wndBorderColor == Prefab.wndBorderColorDef)
                {
                    WndBorderColor = value.WndBorderColor;
                }
                if (titlePanelBorderColor == Prefab.titlePanelBorderColorDef)
                {
                    TitlePanelBorderColor = value.TitlePanelBorderColor;
                }
                if (messagePanelBorderColor == Prefab.messagePanelBorderColorDef)
                {
                    MessagePanelBorderColor = value.MessagePanelBorderColor;
                }
                if (buttonPanelBorderColor == Prefab.buttonPanelBorderColorDef)
                {
                    ButtonPanelBorderColor = value.ButtonPanelBorderColor;
                }
                if (buttonBorderColor == Prefab.buttonBorderColorDef)
                {
                    ButtonBorderColor = value.ButtonBorderColor;
                }
                if (wndBorderThickness == Prefab.wndBorderThicknessDef)
                {
                    WndBorderThickness = value.WndBorderThickness;
                }
                if (titlePanelBorderThickness == Prefab.titlePanelBorderThicknessDef)
                {
                    TitlePanelBorderThickness = value.TitlePanelBorderThickness;
                }
                if (messagePanelBorderThickness == Prefab.messagePanelBorderThicknessDef)
                {
                    MessagePanelBorderThickness = value.MessagePanelBorderThickness;
                }
                if (buttonPanelBorderThickness == Prefab.buttonPanelBorderThicknessDef)
                {
                    ButtonPanelBorderThickness = value.ButtonPanelBorderThickness;
                }
                if (buttonBorderThickness == Prefab.buttonBorderThicknessDef)
                {
                    ButtonBorderThickness = value.ButtonBorderThickness;
                }
                if (titleFontFamily == Prefab.titleFontFamilyDef)
                {
                    TitleFontFamily = value.TitleFontFamily;
                }
                if (messageFontFamily == Prefab.messageFontFamilyDef)
                {
                    MessageFontFamily = value.MessageFontFamily;
                }
                if (buttonFontFamily == Prefab.buttonFontFamilyDef)
                {
                    ButtonFontFamily = value.ButtonFontFamily;
                }
                if (windowShowDuration == Prefab.windowShowDurationDef)
                {
                    WindowShowDuration = value.WindowShowDuration;
                }
                if (windowShowAnimations == null)
                {
                    WindowShowAnimations = value.WindowShowAnimations;
                }
                if (windowCloseAnimations == null)
                {
                    WindowCloseAnimations = value.WindowCloseAnimations;
                }
                if (titlePanelSpacing == Prefab.titlePanelSpacingDef)
                {
                    TitlePanelSpacing = value.TitlePanelSpacing;
                }
                if (messgagePanelSpacing == Prefab.messgagePanelSpacingDef)
                {
                    MessgagePanelSpacing = value.MessgagePanelSpacing;
                }
                if (closeIcon == Prefab.closeIconDef)
                {
                    CloseIcon = value.CloseIcon;
                }
                if (titleIcon == Prefab.titleIconDef)
                {
                    TitleIcon = value.TitleIcon;
                }
                if (warningIcon == Prefab.warningIconDef)
                {
                    WarningIcon = value.WarningIcon;
                }
                if (errorIcon == Prefab.errorIconDef)
                {
                    ErrorIcon = value.ErrorIcon;
                }
                if (infoIcon == Prefab.infoIconDef)
                {
                    InfoIcon = value.InfoIcon;
                }
                if (questionIcon == Prefab.questionIconDef)
                {
                    QuestionIcon = value.QuestionIcon;
                }
                if (messgaeIconHeight == Prefab.messgaeIconHeightDef)
                {
                    MessgaeIconHeight = value.MessgaeIconHeight;
                }
                if (closeIconHeight == Prefab.closeIconHeightDef)
                {
                    CloseIconHeight = value.CloseIconHeight;
                }
                if (titleIconHeight == Prefab.titleIconHeightDef)
                {
                    TitleIconHeight = value.TitleIconHeight;
                }
                if (setCloseIconHeightAsTitleFontHeight == Prefab.setCloseIconHeightAsTitleFontHeightDef)
                {
                    SetCloseIconHeightAsTitleFontHeight = value.SetCloseIconHeightAsTitleFontHeight;
                }
                if (setTitleIconHeightAsTitleFontHeight == Prefab.setTitleIconHeightAsTitleFontHeightDef)
                {
                    SetTitleIconHeightAsTitleFontHeight = value.SetTitleIconHeightAsTitleFontHeight;
                }
                if (enableCloseButton == Prefab.enableCloseButtonDef)
                {
                    EnableCloseButton = value.EnableCloseButton;
                }
                if (enableTitleIcon == Prefab.enableTitleIconDef)
                {
                    EnableTitleIcon = value.EnableTitleIcon;
                }
                if (buttonStyleList == null)
                {
                    ButtonStyleList = value.ButtonStyleList;
                }
                if (buttonWidthList == null)
                {
                    ButtonWidthList = value.ButtonWidthList;
                }
                if (buttonHeightList == null)
                {
                    ButtonHeightList = value.ButtonHeightList;
                }
                if (buttonHorizontalAlignmentList == null)
                {
                    ButtonHorizontalAlignmentList = value.ButtonHorizontalAlignmentList;
                }
                if (buttonVerticalAlignmentList == null)
                {
                    ButtonVerticalAlignmentList = value.ButtonVerticalAlignmentList;
                }
                if (buttonMarginList == null)
                {
                    ButtonMarginList = value.ButtonMarginList;
                }
                if (buttonBackgroundList == null)
                {
                    ButtonBackgroundList = value.ButtonBackgroundList;
                }
                if (buttonBorderBrushList == null)
                {
                    ButtonBorderBrushList = value.ButtonBorderBrushList;
                }
                if (buttonBorderThicknessList == null)
                {
                    ButtonBorderThicknessList = value.ButtonBorderThicknessList;
                }
                if (buttonCursorList == null)
                {
                    ButtonCursorList = value.ButtonCursorList;
                }
                if (closeTimer == null)
                {
                    CloseTimer = value.CloseTimer;
                }
                if (loadedEventHandler == null)
                {
                    LoadedEventHandler = value.LoadedEventHandler;
                }
                if (keyDownEventHandler == null)
                {
                    KeyDownEventHandler = value.KeyDownEventHandler;
                }
                if (keyUpEventHandler == null)
                {
                    KeyUpEventHandler = value.KeyUpEventHandler;
                }
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
                mb.i_title.FontSize = TitleFontSize;
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
                mb.i_title.Foreground = TitleFontColor.GetSolidColorBrush();
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
            mb.i_title.FontFamily = TitleFontFamily;
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
            mb.i_icon.Source = TitleIcon;
            if (enableTitleIcon)
            {
                mb.i_icon.Visibility = Visibility.Visible;
            }
            else
            {
                mb.i_icon.Visibility = Visibility.Collapsed;
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
            RefreshList btnList;

            // 判断按钮类型并显示相应的按钮
            switch (selectStyle)
            {
                case MessageBoxButton.OK:
                    btnList = new RefreshList { MessageBoxResult.OK.ToString() };
                    break;

                case MessageBoxButton.OKCancel:
                    btnList = new RefreshList { MessageBoxResult.OK.ToString(), MessageBoxResult.Cancel.ToString() };
                    break;

                case MessageBoxButton.YesNo:
                    btnList = new RefreshList { MessageBoxResult.Yes.ToString(), MessageBoxResult.No.ToString() };
                    break;

                case MessageBoxButton.YesNoCancel:
                    btnList = new RefreshList { MessageBoxResult.Yes.ToString(), MessageBoxResult.No.ToString(), MessageBoxResult.Cancel.ToString() };
                    break;

                default:
                    btnList = new RefreshList { MessageBoxResult.OK.ToString() };
                    break;
            }

            // 返回值
            MessageBoxResult result;

            int index = Show(btnList, msg, title, img);

            if (Info.IsLastShowSucceed)
            {
                if (isClosedByTimer)
                {
                    return MessageBoxResult.None;
                }
                if (index == -1)
                {
                    result = MessageBoxResult.None;
                }
                else
                {
                    // 获取返回值字符串
                    string resultStr = btnList[index].ToString();
                    // 找到对应的MessageBoxResult元素并返回
                    result = (MessageBoxResult)System.Enum.Parse(typeof(MessageBoxResult), resultStr);
                }
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
        public static int Show(RefreshList btnList, string msg, string title = "", MessageBoxImage img = MessageBoxImage.None)
        {
            try
            {
                if (mb != null)
                    return -1;

                if (propertiesSetter == null && defaultProperties != null)
                {
                    PropertiesSetter = defaultProperties;
                }

                // 重置
                Info.IsLastShowSucceed = true;

                // 初始化窗口
                mb = new MessageBox();

                InitProperties();

                if (loadedEventHandler != null)
                {
                    mb.Loaded += loadedEventHandler;
                }
                if (keyDownEventHandler != null)
                {
                    mb.KeyDown += keyDownEventHandler;
                }
                if (keyUpEventHandler != null)
                {
                    mb.KeyUp += keyUpEventHandler;
                }

                mb.ContentRendered += (a, b) => { SetButtonAndButtonPanelHeight(); };

                // 计算像素密度
                pixelsPerDip = VisualTreeHelper.GetDpi(mb).PixelsPerDip;

                //设定窗口最大高度为窗口工作区高度
                mb.MaxHeight = SystemInformation.WorkingArea.Height;

                //绑定关闭按钮点击事件
                mb.i_close.MouseLeftButtonUp += I_close_TouchDown;

                // 绑定标题栏鼠标事件, 拖动窗口
                mb.i_title.MouseLeftButtonDown += i_title_MouseLeftButtonDown;

                // 重置选择结果
                currentClickIndex = -1;

                // 重置是否定时关闭
                isClosedByTimer = false;

                // 显示标题
                TitleText = title;
                mb.Title = title;

                // 判断消息类型并显示相应的图像
                MessageBoxImageType = img;

                // 显示消息内容
                MessageText += msg;

                ButtonList = btnList;

                LoadTitlePanel();

                LoadMessagePanel();

                if (CloseTimer != null)
                {
                    SetCloseTimer();
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
        /// 立即关闭消息框
        /// </summary>
        /// <param name="result">MessageBox的返回值</param>
        public static void CloseNow(int result)
        {
            currentClickIndex = result;

            mb.Hide();
            mb.Close();
        }

        /// <summary>
        /// 重新设定标题区域大小
        /// </summary>
        private static void LoadTitlePanel()
        {
            // 根据字体计算标题字符串高度
            double heightTitle = new FormattedText(" ", CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface(mb.i_title.FontFamily, mb.i_title.FontStyle, mb.i_title.FontWeight, mb.i_title.FontStretch), mb.i_title.FontSize, System.Windows.Media.Brushes.Black, pixelsPerDip).Height;
            if (setCloseIconHeightAsTitleFontHeight)
            {
                mb.i_close.Height = heightTitle;
            }
            else if (closeIconHeight > 0)
            {
                mb.i_close.Height = closeIconHeight;
            }
            if (setTitleIconHeightAsTitleFontHeight)
            {
                mb.i_icon.Height = heightTitle;
            }
            else if (titleIconHeight > 0)
            {
                mb.i_icon.Height = titleIconHeight;
            }
            double heightIcon = mb.i_icon.Height;
            double heightBtn = mb.i_close.Height;
            mb.i_icon.Margin = new Thickness(titlePanelSpacing, 0, 0, 0);
            mb.i_close.Margin = new Thickness(0, 0, titlePanelSpacing, 0);
            double height = 0;
            if (heightTitle > height)
            {
                height = heightTitle;
            }
            if (heightIcon > height)
            {
                height = heightIcon;
            }
            if (heightBtn > height)
            {
                height = heightBtn;
            }
            double titleAndBorderHeight = height + mb.b_titleborder.BorderThickness.Top + mb.b_titleborder.BorderThickness.Bottom;

            // 设置标题栏高度
            mb.g_titlegrid.Height = titleAndBorderHeight + titlePanelSpacing * 2;
            mb.rd_title.Height = new GridLength(titleAndBorderHeight + titlePanelSpacing * 2);
            mb.b_titleborder.Height = titleAndBorderHeight + titlePanelSpacing * 2;
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
            int elementCount = 0;
            int FrameworkElementCount = 0;
            for (int i = 0; i < buttonList.Count; ++i)
            {
                if (buttonList[i] is RoutedEventHandler)
                {
                    continue;
                }

                // 当有多于两个选项时, 增加Grid的列数
                if (elementCount >= 2)
                {
                    mb.g_buttongrid.ColumnDefinitions.Add(new ColumnDefinition());
                }


                if (buttonList[i] is string)
                {
                    // 实例化一个新的按钮
                    Button newBtn = new Button();

                    // 将按钮加入Grid中
                    mb.g_buttongrid.Children.Add(newBtn);
                    // 设置按钮在Grid中的行列
                    newBtn.SetValue(Grid.RowProperty, 0);
                    newBtn.SetValue(Grid.ColumnProperty, elementCount);
                    // 设置按钮样式
                    if (buttonStyleList != null && buttonStyleList.Count >= 1)
                    {
                        newBtn.Style = buttonStyleList.Count > styleIndex + 1 ? buttonStyleList[styleIndex++] : buttonStyleList[styleIndex];
                    }

                    //设置按钮文本
                    newBtn.Content = buttonList[i];
                    // 设置按钮可见
                    newBtn.Visibility = Visibility.Visible;
                    // 绑定按钮点击事件
                    if (buttonList.Count - 1 >= i + 1 && buttonList[i + 1] is RoutedEventHandler)
                    {
                        newBtn.Click += (RoutedEventHandler)buttonList[i + 1];
                    }
                    else
                    {
                        newBtn.Click += BtnClickedClose;
                    }

                    // 设置拉伸
                    newBtn.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    newBtn.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                    ++FrameworkElementCount;
                }
                else if (buttonList[i] is ButtonSpacer)
                {
                    ButtonSpacer buttonSpacer = (ButtonSpacer)buttonList[i];

                    if (buttonSpacer.IsForSpan && elementCount >= 1 && mb.g_buttongrid.Children[FrameworkElementCount - 1] is FrameworkElement)
                    {
                        Grid.SetColumnSpan((FrameworkElement)mb.g_buttongrid.Children[FrameworkElementCount - 1], 2);
                    }

                    if (buttonSpacer.GridUnitType == GridUnitType.Pixel && buttonSpacer.Value != -1)
                    {
                        // 修改列宽度
                        mb.g_buttongrid.ColumnDefinitions[elementCount].Width = new GridLength(buttonSpacer.Value);
                    }
                    else if (buttonSpacer.GridUnitType == GridUnitType.Star && buttonSpacer.Value != -1)
                    {
                        mb.g_buttongrid.ColumnDefinitions[elementCount].Width = new GridLength(buttonSpacer.Value, GridUnitType.Star);
                    }
                }
                else if (buttonList[i] is FrameworkElement)
                {
                    FrameworkElement fe = (FrameworkElement)buttonList[i];
                    // 将按钮加入Grid中
                    mb.g_buttongrid.Children.Add(fe);
                    // 修改列宽度
                    mb.g_buttongrid.ColumnDefinitions[elementCount].Width = new GridLength(!fe.Width.Equals(Double.NaN) ? fe.Width : 1, !fe.Width.Equals(Double.NaN) ? GridUnitType.Pixel : GridUnitType.Star);
                    // 设置按钮在Grid中的行列
                    fe.SetValue(Grid.RowProperty, 0);
                    fe.SetValue(Grid.ColumnProperty, elementCount);

                    fe.SizeChanged += ButtonObjectSizeChanged;

                    if (buttonList[i] is Button && buttonList.Count - 1 >= i + 1 && buttonList[i + 1] is RoutedEventHandler)
                    {
                        ((Button)fe).Click += (RoutedEventHandler)buttonList[i + 1];
                    }

                    ++FrameworkElementCount;
                }

                ++elementCount;
            }

            SetButtonStyle();
        }

        /// <summary>
        /// 设置消息区域宽度
        /// </summary>
        private static void LoadMessagePanel()
        {
            if (messgagePanelSpacing >= 0)
            {
                mb.i_img.Margin = new Thickness(messgagePanelSpacing, 0, messgagePanelSpacing, 0);
                if (mb.i_img.Visibility == Visibility.Visible)
                {
                    mb.tb_msg.Margin = new Thickness(0, messgagePanelSpacing, messgagePanelSpacing, messgagePanelSpacing);
                }
                else
                {
                    mb.tb_msg.Margin = new Thickness(messgagePanelSpacing);
                }
            }
            if (messgaeIconHeight > 0)
            {
                mb.i_img.Height = messgaeIconHeight;
            }
            if (mb.i_img.Visibility == Visibility.Visible)
            {
                mb.tb_msg.Width = mb.Width - (mb.i_img.Height * (mb.i_img.Source.Width / mb.i_img.Source.Height)) - mb.i_img.Margin.Left - mb.i_img.Margin.Right - mb.tb_msg.Margin.Left - mb.tb_msg.Margin.Right - mb.b_messageborder.BorderThickness.Left - mb.b_messageborder.BorderThickness.Right;
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
        public static int Show(PropertiesSetter propertiesSetter, RefreshList btnList, string msg, string title = "", MessageBoxImage img = MessageBoxImage.None)
        {
            PropertiesSetter = propertiesSetter;
            return Show(btnList, msg, title, img);
        }

        /// <summary>
        /// 选择后保存选择结果并关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void BtnClickedClose(Object sender, RoutedEventArgs e)
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
            lockHeight = Prefab.lockHeightDef;
            textWrappingMode = Prefab.textWrappingModeDef;
            windowWidth = Prefab.windowWidthDef;
            windowMinHeight = Prefab.windowMinHeightDef;
            titleFontSize = Prefab.titleFontSizeDef;
            messageFontSize = Prefab.messageFontSizeDef;
            buttonFontSize = Prefab.buttonFontSizeDef;
            titleFontColor = Prefab.titleFontColorDef;
            messageFontColor = Prefab.messageFontColorDef;
            buttonFontColor = Prefab.buttonFontColorDef;
            windowOpacity = Prefab.windowOpacityDef;
            titleBarOpacity = Prefab.titleBarOpacityDef;
            messageBarOpacity = Prefab.messageBarOpacityDef;
            buttonBarOpacity = Prefab.buttonBarOpacityDef;
            titlePanelColor = Prefab.titlePanelColorDef;
            messagePanelColor = Prefab.messagePanelColorDef;
            buttonPanelColor = Prefab.buttonPanelColorDef;
            wndBorderColor = Prefab.wndBorderColorDef;
            titlePanelBorderColor = Prefab.titlePanelBorderColorDef;
            messagePanelBorderColor = Prefab.messagePanelBorderColorDef;
            buttonPanelBorderColor = Prefab.buttonPanelBorderColorDef;
            buttonBorderColor = Prefab.buttonBorderColorDef;
            wndBorderThickness = Prefab.wndBorderThicknessDef;
            titlePanelBorderThickness = Prefab.titlePanelBorderThicknessDef;
            messagePanelBorderThickness = Prefab.messagePanelBorderThicknessDef;
            buttonPanelBorderThickness = Prefab.buttonPanelBorderThicknessDef;
            buttonBorderThickness = Prefab.buttonBorderThicknessDef;
            titleFontFamily = Prefab.titleFontFamilyDef;
            messageFontFamily = Prefab.messageFontFamilyDef;
            buttonFontFamily = Prefab.buttonFontFamilyDef;
            windowShowDuration = Prefab.windowShowDurationDef;
            windowShowAnimations = null;
            windowCloseAnimations = null;
            titlePanelSpacing = Prefab.titlePanelSpacingDef;
            messgagePanelSpacing = Prefab.messgagePanelSpacingDef;
            closeIcon = Prefab.closeIconDef;
            titleIcon = Prefab.titleIconDef;
            warningIcon = Prefab.warningIconDef;
            errorIcon = Prefab.errorIconDef;
            infoIcon = Prefab.infoIconDef;
            questionIcon = Prefab.questionIconDef;
            messgaeIconHeight = Prefab.messgaeIconHeightDef;
            closeIconHeight = Prefab.closeIconHeightDef;
            titleIconHeight = Prefab.titleIconHeightDef;
            setCloseIconHeightAsTitleFontHeight = Prefab.setCloseIconHeightAsTitleFontHeightDef;
            setTitleIconHeightAsTitleFontHeight = Prefab.setTitleIconHeightAsTitleFontHeightDef;
            enableCloseButton = Prefab.enableCloseButtonDef;
            enableTitleIcon = Prefab.enableTitleIconDef;
            buttonStyleList = null;
            buttonWidthList = null;
            buttonHeightList = null;
            buttonHorizontalAlignmentList = null;
            buttonVerticalAlignmentList = null;
            buttonHorizontalContentAlignmentList = null;
            buttonVerticalContentAlignmentList = null;
            buttonMarginList = null;
            buttonPaddingList = null;
            buttonBackgroundList = null;
            buttonBorderBrushList = null;
            buttonBorderThicknessList = null;
            buttonCursorList = null;
            buttonFontFamilyList = null;
            buttonFontSizeList = null;
            buttonFontStretchList = null;
            buttonFontStyleList = null;
            buttonFontWeightList = null;
            closeTimer = null;
            loadedEventHandler = null;
            keyDownEventHandler = null;
            keyUpEventHandler = null;

            propertiesSetter = null;

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

            double contentHeight = height * lineCount + mb.tb_msg.Margin.Top + mb.tb_msg.Margin.Bottom;
            if (mb.i_img.Visibility == Visibility.Visible)
            {
                if (mb.i_img.Height > contentHeight)
                {
                    contentHeight = mb.i_img.Height;
                }
            }

            // 计算窗口高度
            mb.Height = contentHeight + mb.rd_title.Height.Value + mb.rd_button.Height.Value + mb.b_messageborder.BorderThickness.Top + mb.b_messageborder.BorderThickness.Bottom + height;
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
        private static void i_title_MouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
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
                for (int i = 0; i < buttonList.Count && i < mb.g_buttongrid.ColumnDefinitions.Count; i++)
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
            isClosedByTimer = true;
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

            LoadMessagePanel();
        }

        /// <summary>
        /// 设置按钮样式
        /// </summary>
        private static void SetButtonStyle()
        {
            //double maxContentWidth = 0;
            double contentAndBorderAndMarginHeight = 0;
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

                    if (ButtonWidthList != null && ButtonWidthList.Count >= 1)
                    {
                        btn.Width = ButtonWidthList.Count > buttonIndex ? ButtonWidthList[buttonIndex] : ButtonWidthList[ButtonWidthList.Count - 1];
                    }
                    if (ButtonHeightList != null && ButtonHeightList.Count >= 1)
                    {
                        btn.Height = ButtonHeightList.Count > buttonIndex ? ButtonHeightList[buttonIndex] : ButtonHeightList[ButtonHeightList.Count - 1];
                    }
                    if (ButtonHorizontalAlignmentList != null && ButtonHorizontalAlignmentList.Count >= 1)
                    {
                        btn.HorizontalAlignment = ButtonHorizontalAlignmentList.Count > buttonIndex ? ButtonHorizontalAlignmentList[buttonIndex] : ButtonHorizontalAlignmentList[ButtonHorizontalAlignmentList.Count - 1];
                    }
                    if (ButtonVerticalAlignmentList != null && ButtonVerticalAlignmentList.Count >= 1)
                    {
                        btn.VerticalAlignment = ButtonVerticalAlignmentList.Count > buttonIndex ? ButtonVerticalAlignmentList[buttonIndex] : ButtonVerticalAlignmentList[ButtonVerticalAlignmentList.Count - 1];
                    }
                    if (ButtonHorizontalContentAlignmentList != null && ButtonHorizontalContentAlignmentList.Count >= 1)
                    {
                        btn.HorizontalContentAlignment = ButtonHorizontalContentAlignmentList.Count > buttonIndex ? ButtonHorizontalContentAlignmentList[buttonIndex] : ButtonHorizontalContentAlignmentList[ButtonHorizontalContentAlignmentList.Count - 1];
                    }
                    if (ButtonVerticalContentAlignmentList != null && ButtonVerticalContentAlignmentList.Count >= 1)
                    {
                        btn.VerticalContentAlignment = ButtonVerticalContentAlignmentList.Count > buttonIndex ? ButtonVerticalContentAlignmentList[buttonIndex] : ButtonVerticalContentAlignmentList[ButtonVerticalContentAlignmentList.Count - 1];
                    }
                    if (ButtonMarginList != null && ButtonMarginList.Count >= 1)
                    {
                        btn.Margin = ButtonMarginList.Count > buttonIndex ? ButtonMarginList[buttonIndex] : ButtonMarginList[ButtonMarginList.Count - 1];
                    }
                    if (ButtonBackgroundList != null && ButtonBackgroundList.Count >= 1)
                    {
                        btn.Background = ButtonBackgroundList.Count > buttonIndex ? ButtonBackgroundList[buttonIndex] : ButtonBackgroundList[ButtonBackgroundList.Count - 1];
                    }
                    if (ButtonBorderBrushList != null && ButtonBorderBrushList.Count >= 1)
                    {
                        btn.BorderBrush = ButtonBorderBrushList.Count > buttonIndex ? ButtonBorderBrushList[buttonIndex] : ButtonBorderBrushList[ButtonBorderBrushList.Count - 1];
                    }
                    if (ButtonBorderThicknessList != null && ButtonBorderThicknessList.Count >= 1)
                    {
                        btn.BorderThickness = ButtonBorderThicknessList.Count > buttonIndex ? ButtonBorderThicknessList[buttonIndex] : ButtonBorderThicknessList[ButtonBorderThicknessList.Count - 1];
                    }
                    if (ButtonCursorList != null && ButtonCursorList.Count >= 1)
                    {
                        btn.Cursor = ButtonCursorList.Count > buttonIndex ? ButtonCursorList[buttonIndex] : ButtonCursorList[ButtonCursorList.Count - 1];
                    }
                    if (ButtonFontFamilyList != null && ButtonFontFamilyList.Count >= 1)
                    {
                        btn.FontFamily = ButtonFontFamilyList.Count > buttonIndex ? ButtonFontFamilyList[buttonIndex] : ButtonFontFamilyList[ButtonFontFamilyList.Count - 1];
                    }
                    if (ButtonFontSizeList != null && ButtonFontSizeList.Count >= 1)
                    {
                        btn.FontSize = ButtonFontSizeList.Count > buttonIndex ? ButtonFontSizeList[buttonIndex] : ButtonFontSizeList[ButtonFontSizeList.Count - 1];
                    }
                    if (ButtonFontStretchList != null && ButtonFontStretchList.Count >= 1)
                    {
                        btn.FontStretch = ButtonFontStretchList.Count > buttonIndex ? ButtonFontStretchList[buttonIndex] : ButtonFontStretchList[ButtonFontStretchList.Count - 1];
                    }
                    if (ButtonFontStyleList != null && ButtonFontStyleList.Count >= 1)
                    {
                        btn.FontStyle = ButtonFontStyleList.Count > buttonIndex ? ButtonFontStyleList[buttonIndex] : ButtonFontStyleList[ButtonFontStyleList.Count - 1];
                    }
                    if (ButtonFontWeightList != null && ButtonFontWeightList.Count >= 1)
                    {
                        btn.FontWeight = ButtonFontWeightList.Count > buttonIndex ? ButtonFontWeightList[buttonIndex] : ButtonFontWeightList[ButtonFontWeightList.Count - 1];
                    }

                    Style style = null;

                    if (ButtonStyleList != null && ButtonStyleList.Count >= 1)
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
                    Thickness margin = btn.Margin;
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
                                margin = (Thickness)setter.Value;
                            }

                            if (needSetColumnDefinitionWidth && ColumnDefinitionWidth > 0)
                            {
                                mb.g_buttongrid.ColumnDefinitions[i].Width = new GridLength(ColumnDefinitionWidth, GridUnitType.Pixel);
                            }
                        }
                    }

                    double heightTemp = ft.Height + thickness.Top + thickness.Bottom + mb.b_buttonborder.BorderThickness.Top + mb.b_buttonborder.BorderThickness.Bottom + margin.Top + margin.Bottom;
                    if (heightTemp > contentAndBorderAndMarginHeight)
                    {
                        contentAndBorderAndMarginHeight = heightTemp;
                    }
                }
            }

            // 设置按钮栏高度
            mb.buttonHeight = contentAndBorderAndMarginHeight;

            SetButtonAndButtonPanelHeight();
        }

        /// <summary>
        /// 根据最大高度设置按钮和按钮区域高度
        /// </summary>
        private static void SetButtonAndButtonPanelHeight()
        {
            double highestItemHeight = -1;
            for (int i = 0; i < buttonList.Count; i++)
            {
                if (buttonList[i] is string)
                {
                    highestItemHeight = mb.buttonHeight;
                    break;
                }
            }


            if (mb.g_buttongrid.Children != null)
            {
                foreach (FrameworkElement fe in mb.g_buttongrid.Children)
                {
                    double heightTemp = (fe.Height < fe.ActualHeight ? fe.ActualHeight : fe.Height) + fe.Margin.Top + fe.Margin.Bottom;
                    if (heightTemp > highestItemHeight)
                    {
                        highestItemHeight = heightTemp;
                    }
                }
            }

            mb.g_buttongrid.Height = highestItemHeight;
            mb.rd_button.Height = new GridLength(highestItemHeight, GridUnitType.Pixel);
            mb.b_buttonborder.Height = highestItemHeight;
        }

        /// <summary>
        /// 设置定时关闭
        /// </summary>
        private static void SetCloseTimer()
        {
            CloseTimer.closeWindowByTimer = new MessageBoxCloseTimer.CloseWindowByTimer(CloseWindowByTimer);

            // 设置定时关闭窗口时间
            // TODO 换成Timer类 + 委托调用关闭函数的形式是不是更好
            mb.timer = new DispatcherTimer();
            mb.timer.Interval = CloseTimer.timeSpan;
            mb.timer.Tick += CloseWindowByTimer;
            mb.timer.Start();
        }
    }
}