using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using static CustomizableMessageBox.MessageBoxColor;

namespace CustomizableMessageBox
{
    public class PropertiesSetter
    {
        public PropertiesSetter()
        {

        }

        public PropertiesSetter(PropertiesSetter propertiesSetter)
        {
            LockHeight = propertiesSetter.LockHeight;
            TextWrappingMode = propertiesSetter.TextWrappingMode;
            WindowWidth = propertiesSetter.WindowWidth;
            WindowMinHeight = propertiesSetter.WindowMinHeight;
            TitleFontSize = propertiesSetter.TitleFontSize;
            MessageFontSize = propertiesSetter.MessageFontSize;
            ButtonFontSize = propertiesSetter.ButtonFontSize;
            TitleFontColor = propertiesSetter.TitleFontColor;
            MessageFontColor = propertiesSetter.MessageFontColor;
            ButtonFontColor = propertiesSetter.ButtonFontColor;
            WindowOpacity = propertiesSetter.WindowOpacity;
            TitleBarOpacity = propertiesSetter.TitleBarOpacity;
            MessageBarOpacity = propertiesSetter.MessageBarOpacity;
            ButtonBarOpacity = propertiesSetter.ButtonBarOpacity;
            TitlePanelColor = propertiesSetter.TitlePanelColor;
            MessagePanelColor = propertiesSetter.MessagePanelColor;
            ButtonPanelColor = propertiesSetter.ButtonPanelColor;
            WndBorderColor = propertiesSetter.WndBorderColor;
            TitlePanelBorderColor = propertiesSetter.TitlePanelBorderColor;
            MessagePanelBorderColor = propertiesSetter.MessagePanelBorderColor;
            ButtonPanelBorderColor = propertiesSetter.ButtonPanelBorderColor;
            ButtonBorderColor = propertiesSetter.ButtonBorderColor;
            WndBorderThickness = propertiesSetter.WndBorderThickness;
            TitlePanelBorderThickness = propertiesSetter.TitlePanelBorderThickness;
            MessagePanelBorderThickness = propertiesSetter.MessagePanelBorderThickness;
            ButtonPanelBorderThickness = propertiesSetter.ButtonPanelBorderThickness;
            ButtonBorderThickness = propertiesSetter.ButtonBorderThickness;
            TitleFontFamily = propertiesSetter.TitleFontFamily;
            MessageFontFamily = propertiesSetter.MessageFontFamily;
            ButtonFontFamily = propertiesSetter.ButtonFontFamily;
            WindowShowDuration = propertiesSetter.WindowShowDuration;
            WindowShowAnimations = propertiesSetter.WindowShowAnimations;
            WindowCloseAnimations = propertiesSetter.WindowCloseAnimations;
            TitlePanelSpacing = propertiesSetter.TitlePanelSpacing;
            MessgagePanelSpacing = propertiesSetter.MessgagePanelSpacing;
            CloseIcon = propertiesSetter.CloseIcon;
            TitleIcon = propertiesSetter.TitleIcon;
            WarningIcon = propertiesSetter.WarningIcon;
            ErrorIcon = propertiesSetter.ErrorIcon;
            InfoIcon = propertiesSetter.InfoIcon;
            QuestionIcon = propertiesSetter.QuestionIcon;
            MessgaeIconHeight = propertiesSetter.MessgaeIconHeight;
            CloseIconHeight = propertiesSetter.CloseIconHeight;
            TitleIconHeight = propertiesSetter.TitleIconHeight;
            SetCloseIconHeightAsTitleFontHeight = propertiesSetter.SetCloseIconHeightAsTitleFontHeight;
            SetTitleIconHeightAsTitleFontHeight = propertiesSetter.SetTitleIconHeightAsTitleFontHeight;
            EnableCloseButton = propertiesSetter.EnableCloseButton;
            EnableTitleIcon = propertiesSetter.EnableTitleIcon;
            ButtonStyleList = propertiesSetter.ButtonStyleList;
            ButtonWidthList = propertiesSetter.ButtonWidthList;
            ButtonHeightList = propertiesSetter.ButtonHeightList;
            ButtonHorizontalAlignmentList = propertiesSetter.ButtonHorizontalAlignmentList;
            ButtonVerticalAlignmentList = propertiesSetter.ButtonVerticalAlignmentList;
            ButtonHorizontalContentAlignmentList = propertiesSetter.ButtonHorizontalContentAlignmentList;
            ButtonVerticalContentAlignmentList = propertiesSetter.ButtonVerticalContentAlignmentList;
            ButtonMarginList = propertiesSetter.ButtonMarginList;
            ButtonPaddingList = propertiesSetter.ButtonPaddingList;
            ButtonBackgroundList = propertiesSetter.ButtonBackgroundList;
            ButtonBorderBrushList = propertiesSetter.ButtonBorderBrushList;
            ButtonBorderThicknessList = propertiesSetter.ButtonBorderThicknessList;
            ButtonCursorList = propertiesSetter.ButtonCursorList;
            ButtonFontFamilyList = propertiesSetter.ButtonFontFamilyList;
            ButtonFontSizeList = propertiesSetter.ButtonFontSizeList;
            ButtonFontStretchList = propertiesSetter.ButtonFontStretchList;
            ButtonFontStyleList = propertiesSetter.ButtonFontStyleList;
            ButtonFontWeightList = propertiesSetter.ButtonFontWeightList;
            CloseTimer = propertiesSetter.CloseTimer;
            LoadedEventHandler = propertiesSetter.LoadedEventHandler;
            KeyDownEventHandler = propertiesSetter.KeyDownEventHandler;
            KeyUpEventHandler = propertiesSetter.KeyUpEventHandler;
        }

        // 锁定高度
        private bool lockHeight = Prefab.lockHeightDef;
        public bool LockHeight { get => lockHeight; set => lockHeight = value; }

        // 消息区域换行模式
        private TextWrapping textWrappingMode = Prefab.textWrappingModeDef;
        public TextWrapping TextWrappingMode { get => textWrappingMode; set => textWrappingMode = value; }

        // 窗口宽度
        private int windowWidth = Prefab.windowWidthDef;
        public int WindowWidth { get => windowWidth; set => windowWidth = value; }

        // 窗口最小高度
        private int windowMinHeight = Prefab.windowMinHeightDef;
        public int WindowMinHeight { get => windowMinHeight; set => windowMinHeight = value; }

        // 标题字体大小
        private int titleFontSize = Prefab.titleFontSizeDef;
        public int TitleFontSize { get => titleFontSize; set => titleFontSize = value; }

        // 消息文本字体大小
        private int messageFontSize = Prefab.messageFontSizeDef;
        public int MessageFontSize { get => messageFontSize; set => messageFontSize = value; }

        // 按钮字体大小
        private int buttonFontSize = Prefab.buttonFontSizeDef;
        public int ButtonFontSize { get => buttonFontSize; set => buttonFontSize = value; }

        // 标题字体颜色
        private MessageBoxColor titleFontColor = Prefab.titleFontColorDef;
        public MessageBoxColor TitleFontColor { get => titleFontColor; set => titleFontColor = value; }

        // 消息文本字体颜色
        private MessageBoxColor messageFontColor = Prefab.messageFontColorDef;
        public MessageBoxColor MessageFontColor { get => messageFontColor; set => messageFontColor = value; }

        // 按钮字体颜色
        private MessageBoxColor buttonFontColor = Prefab.buttonFontColorDef;
        public MessageBoxColor ButtonFontColor { get => buttonFontColor; set => buttonFontColor = value; }

        // 窗口透明度
        private double windowOpacity = Prefab.windowOpacityDef;
        public double WindowOpacity { get => windowOpacity; set => windowOpacity = value; }

        // 标题栏透明度
        private double titleBarOpacity = Prefab.titleBarOpacityDef;
        public double TitleBarOpacity { get => titleBarOpacity; set => titleBarOpacity = value; }

        // 消息栏透明度
        private double messageBarOpacity = Prefab.messageBarOpacityDef;
        public double MessageBarOpacity { get => messageBarOpacity; set => messageBarOpacity = value; }

        // 按钮栏透明度
        private double buttonBarOpacity = Prefab.buttonBarOpacityDef;
        public double ButtonBarOpacity { get => buttonBarOpacity; set => buttonBarOpacity = value; }

        // 标题区域背景颜色
        private MessageBoxColor titlePanelColor = Prefab.titlePanelColorDef;
        public MessageBoxColor TitlePanelColor { get => titlePanelColor; set => titlePanelColor = value; }

        // 消息区域背景颜色
        private MessageBoxColor messagePanelColor = Prefab.messagePanelColorDef;
        public MessageBoxColor MessagePanelColor { get => messagePanelColor; set => messagePanelColor = value; }

        // 按钮区域背景颜色
        private MessageBoxColor buttonPanelColor = Prefab.buttonPanelColorDef;
        public MessageBoxColor ButtonPanelColor { get => buttonPanelColor; set => buttonPanelColor = value; }

        // 窗口边框颜色
        private MessageBoxColor wndBorderColor = Prefab.wndBorderColorDef;
        public MessageBoxColor WndBorderColor { get => wndBorderColor; set => wndBorderColor = value; }

        // 标题区域边框颜色
        private MessageBoxColor titlePanelBorderColor = Prefab.titlePanelBorderColorDef;
        public MessageBoxColor TitlePanelBorderColor { get => titlePanelBorderColor; set => titlePanelBorderColor = value; }

        // 消息区域边框颜色
        private MessageBoxColor messagePanelBorderColor = Prefab.messagePanelBorderColorDef;
        public MessageBoxColor MessagePanelBorderColor { get => messagePanelBorderColor; set => messagePanelBorderColor = value; }

        // 按钮区域边框颜色
        private MessageBoxColor buttonPanelBorderColor = Prefab.buttonPanelBorderColorDef;
        public MessageBoxColor ButtonPanelBorderColor { get => buttonPanelBorderColor; set => buttonPanelBorderColor = value; }

        // 按钮边框颜色
        private MessageBoxColor buttonBorderColor = Prefab.buttonBorderColorDef;
        public MessageBoxColor ButtonBorderColor { get => buttonBorderColor; set => buttonBorderColor = value; }

        // 窗口边框宽度
        private Thickness wndBorderThickness = Prefab.wndBorderThicknessDef;
        public Thickness WndBorderThickness { get => wndBorderThickness; set => wndBorderThickness = value; }

        // 标题区域边框宽度
        private Thickness titlePanelBorderThickness = Prefab.titlePanelBorderThicknessDef;
        public Thickness TitlePanelBorderThickness { get => titlePanelBorderThickness; set => titlePanelBorderThickness = value; }

        // 消息区域边框宽度
        private Thickness messagePanelBorderThickness = Prefab.messagePanelBorderThicknessDef;
        public Thickness MessagePanelBorderThickness { get => messagePanelBorderThickness; set => messagePanelBorderThickness = value; }

        // 按钮区域边框宽度
        private Thickness buttonPanelBorderThickness = Prefab.buttonPanelBorderThicknessDef;
        public Thickness ButtonPanelBorderThickness { get => buttonPanelBorderThickness; set => buttonPanelBorderThickness = value; }

        // 按钮边框宽度
        private Thickness buttonBorderThickness = Prefab.buttonBorderThicknessDef;
        public Thickness ButtonBorderThickness { get => buttonBorderThickness; set => buttonBorderThickness = value; }

        // 标题文本字体
        private FontFamily titleFontFamily = Prefab.titleFontFamilyDef;
        public FontFamily TitleFontFamily { get => titleFontFamily; set => titleFontFamily = value; }

        // 消息文本字体
        private FontFamily messageFontFamily = Prefab.messageFontFamilyDef;
        public FontFamily MessageFontFamily { get => messageFontFamily; set => messageFontFamily = value; }

        // 按钮文本字体
        private FontFamily buttonFontFamily = Prefab.buttonFontFamilyDef;
        public FontFamily ButtonFontFamily { get => buttonFontFamily; set => buttonFontFamily = value; }

        // 窗口渐显时间
        private Duration windowShowDuration = Prefab.windowShowDurationDef;
        public Duration WindowShowDuration { get => windowShowDuration; set => windowShowDuration = value; }

        // 窗口显示动画
        private List<KeyValuePair<DependencyProperty, AnimationTimeline>> windowShowAnimations = null;
        public List<KeyValuePair<DependencyProperty, AnimationTimeline>> WindowShowAnimations { get => windowShowAnimations; set => windowShowAnimations = value; }

        // 窗口关闭动画
        private List<KeyValuePair<DependencyProperty, AnimationTimeline>> windowCloseAnimations = null;
        public List<KeyValuePair<DependencyProperty, AnimationTimeline>> WindowCloseAnimations { get => windowCloseAnimations; set => windowCloseAnimations = value; }

        // 标题区域间距
        private double titlePanelSpacing = Prefab.titlePanelSpacingDef;
        public double TitlePanelSpacing { get => titlePanelSpacing; set => titlePanelSpacing = value; }

        // Message区域间距
        private double messgagePanelSpacing = Prefab.messgagePanelSpacingDef;
        public double MessgagePanelSpacing { get => messgagePanelSpacing; set => messgagePanelSpacing = value; }

        // 自定义关闭图标
        private BitmapImage closeIcon = Prefab.closeIconDef;
        public BitmapImage CloseIcon { get => closeIcon; set => closeIcon = value; }

        // 自定义标题图标
        private BitmapImage titleIcon = Prefab.titleIconDef;
        public BitmapImage TitleIcon { get => titleIcon; set => titleIcon = value; }

        // 自定义警告图标
        private BitmapImage warningIcon = Prefab.warningIconDef;
        public BitmapImage WarningIcon { get => warningIcon; set => warningIcon = value; }

        // 自定义错误图标
        private BitmapImage errorIcon = Prefab.errorIconDef;
        public BitmapImage ErrorIcon { get => errorIcon; set => errorIcon = value; }

        // 自定义信息图标
        private BitmapImage infoIcon = Prefab.infoIconDef;
        public BitmapImage InfoIcon { get => infoIcon; set => infoIcon = value; }

        // 自定义问题图标
        private BitmapImage questionIcon = Prefab.questionIconDef;
        public BitmapImage QuestionIcon { get => questionIcon; set => questionIcon = value; }

        // Message图标的高度
        private double messgaeIconHeight = Prefab.messgaeIconHeightDef;
        public double MessgaeIconHeight { get => messgaeIconHeight; set => messgaeIconHeight = value; }

        // 关闭按钮图标的高度
        private double closeIconHeight = Prefab.closeIconHeightDef;
        public double CloseIconHeight { get => closeIconHeight; set => closeIconHeight = value; }

        // 标题图标的高度
        private double titleIconHeight = Prefab.titleIconHeightDef;
        public double TitleIconHeight { get => titleIconHeight; set => titleIconHeight = value; }

        // 设置关闭按钮图标高度为标题字体高度
        private bool setCloseIconHeightAsTitleFontHeight = Prefab.setCloseIconHeightAsTitleFontHeightDef;
        public bool SetCloseIconHeightAsTitleFontHeight { get => setCloseIconHeightAsTitleFontHeight; set => setCloseIconHeightAsTitleFontHeight = value; }

        // 设置标题图标高度为标题字体高度
        private bool setTitleIconHeightAsTitleFontHeight = Prefab.setTitleIconHeightAsTitleFontHeightDef;
        public bool SetTitleIconHeightAsTitleFontHeight { get => setTitleIconHeightAsTitleFontHeight; set => setTitleIconHeightAsTitleFontHeight = value; }

        // 应用窗口关闭按钮
        private bool enableCloseButton = Prefab.enableCloseButtonDef;
        public bool EnableCloseButton { get => enableCloseButton; set => enableCloseButton = value; }

        // 应用标题图标
        private bool enableTitleIcon = Prefab.enableTitleIconDef;
        public bool EnableTitleIcon { get => enableTitleIcon; set => enableTitleIcon = value; }

        // 按钮动作样式
        private List<Style> buttonStyleList = null;
        public List<Style> ButtonStyleList { get => buttonStyleList; set => buttonStyleList = value; }

        private List<double> buttonWidthList = null;
        public List<double> ButtonWidthList { get => buttonWidthList; set => buttonWidthList = value; }

        private List<double> buttonHeightList = null;
        public List<double> ButtonHeightList { get => buttonHeightList; set => buttonHeightList = value; }

        private List<System.Windows.HorizontalAlignment> buttonHorizontalAlignmentList = null;
        public List<HorizontalAlignment> ButtonHorizontalAlignmentList { get => buttonHorizontalAlignmentList; set => buttonHorizontalAlignmentList = value; }

        private List<VerticalAlignment> buttonVerticalAlignmentList = null;
        public List<VerticalAlignment> ButtonVerticalAlignmentList { get => buttonVerticalAlignmentList; set => buttonVerticalAlignmentList = value; }

        private List<System.Windows.HorizontalAlignment> buttonHorizontalContentAlignmentList = null;
        public List<HorizontalAlignment> ButtonHorizontalContentAlignmentList { get => buttonHorizontalContentAlignmentList; set => buttonHorizontalContentAlignmentList = value; }

        private List<VerticalAlignment> buttonVerticalContentAlignmentList = null;
        public List<VerticalAlignment> ButtonVerticalContentAlignmentList { get => buttonVerticalContentAlignmentList; set => buttonVerticalContentAlignmentList = value; }

        private List<Thickness> buttonMarginList = null;
        public List<Thickness> ButtonMarginList { get => buttonMarginList; set => buttonMarginList = value; }

        private List<Thickness> buttonPaddingList = null;
        public List<Thickness> ButtonPaddingList { get => buttonPaddingList; set => buttonPaddingList = value; }

        private List<Brush> buttonBackgroundList = null;
        public List<Brush> ButtonBackgroundList { get => buttonBackgroundList; set => buttonBackgroundList = value; }

        private List<Brush> buttonBorderBrushList = null;
        public List<Brush> ButtonBorderBrushList { get => buttonBorderBrushList; set => buttonBorderBrushList = value; }

        private List<Thickness> buttonBorderThicknessList = null;
        public List<Thickness> ButtonBorderThicknessList { get => buttonBorderThicknessList; set => buttonBorderThicknessList = value; }

        private List<System.Windows.Input.Cursor> buttonCursorList = null;
        public List<Cursor> ButtonCursorList { get => buttonCursorList; set => buttonCursorList = value; }

        private List<FontFamily> buttonFontFamilyList = null;
        public List<FontFamily> ButtonFontFamilyList { get => buttonFontFamilyList; set => buttonFontFamilyList = value; }

        private List<double> buttonFontSizeList = null;
        public List<double> ButtonFontSizeList { get => buttonFontSizeList; set => buttonFontSizeList = value; }

        private List<FontStretch> buttonFontStretchList = null;
        public List<FontStretch> ButtonFontStretchList { get => buttonFontStretchList; set => buttonFontStretchList = value; }

        private List<FontStyle> buttonFontStyleList = null;
        public List<FontStyle> ButtonFontStyleList { get => buttonFontStyleList; set => buttonFontStyleList = value; }

        private List<FontWeight> buttonFontWeightList = null;
        public List<FontWeight> ButtonFontWeightList { get => buttonFontWeightList; set => buttonFontWeightList = value; }

        // 窗口计时关闭
        private MessageBoxCloseTimer closeTimer = null;
        public MessageBoxCloseTimer CloseTimer { get => closeTimer; set => closeTimer = value; }

        // Loaded事件
        private RoutedEventHandler loadedEventHandler = null;
        public RoutedEventHandler LoadedEventHandler { get => loadedEventHandler; set => loadedEventHandler = value; }

        // KeyDown事件
        private KeyEventHandler keyDownEventHandler = null;
        public KeyEventHandler KeyDownEventHandler { get => keyDownEventHandler; set => keyDownEventHandler = value; }

        // KeyUp事件
        private KeyEventHandler keyUpEventHandler = null;
        public KeyEventHandler KeyUpEventHandler { get => keyUpEventHandler; set => keyUpEventHandler = value; }
    }
}
