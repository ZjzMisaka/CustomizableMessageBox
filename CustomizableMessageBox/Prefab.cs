using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CustomizableMessageBox
{
    public enum PropertiesSetterName { Black, Gray };
    public enum ButtonStyleName { White };
    public static class Prefab
    {
        internal static bool lockHeightDef = false;
        internal static TextWrapping textWrappingModeDef = TextWrapping.Wrap;
        internal static int windowWidthDef = 800;
        internal static int windowMinHeightDef = 450;
        internal static int titleFontSizeDef = 30;
        internal static int messageFontSizeDef = 25;
        internal static int buttonFontSizeDef = 30;
        internal static MessageBoxColor titleFontColorDef = new MessageBoxColor(Colors.Black);
        internal static MessageBoxColor messageFontColorDef = new MessageBoxColor(Colors.Black);
        internal static MessageBoxColor buttonFontColorDef = new MessageBoxColor(Colors.Black);
        internal static double windowOpacityDef = 0.95;
        internal static double titleBarOpacityDef = 1;
        internal static double messageBarOpacityDef = 1;
        internal static double buttonBarOpacityDef = 1;
        internal static MessageBoxColor titlePanelColorDef = new MessageBoxColor(Colors.White);
        internal static MessageBoxColor messagePanelColorDef = new MessageBoxColor(Colors.White);
        internal static MessageBoxColor buttonPanelColorDef = new MessageBoxColor("#DDDDDD");
        internal static MessageBoxColor wndBorderColorDef = new MessageBoxColor(Colors.White);
        internal static MessageBoxColor titlePanelBorderColorDef = new MessageBoxColor(Colors.White);
        internal static MessageBoxColor messagePanelBorderColorDef = new MessageBoxColor(Colors.White);
        internal static MessageBoxColor buttonPanelBorderColorDef = new MessageBoxColor(Colors.LightGray);
        internal static MessageBoxColor buttonBorderColorDef = new MessageBoxColor(Colors.White);
        internal static Thickness wndBorderThicknessDef = new Thickness(2);
        internal static Thickness titlePanelBorderThicknessDef = new Thickness(0, 0, 0, 1);
        internal static Thickness messagePanelBorderThicknessDef = new Thickness(0);
        internal static Thickness buttonPanelBorderThicknessDef = new Thickness(2);
        internal static Thickness buttonBorderThicknessDef = new Thickness(0);
        internal static FontFamily titleFontFamilyDef = new FontFamily("Times New Roman");
        internal static FontFamily messageFontFamilyDef = new FontFamily("Times New Roman");
        internal static FontFamily buttonFontFamilyDef = new FontFamily("Times New Roman");
        internal static Duration windowShowDurationDef = new Duration(new TimeSpan(0, 0, 0, 0, 200));
        internal static double titlePanelSpacingDef = 7;
        internal static double messgagePanelSpacingDef = 15;
        internal static BitmapImage closeIconDef = new BitmapImage(new Uri(".\\Image\\close.png", UriKind.RelativeOrAbsolute));
        internal static BitmapImage titleIconDef = new BitmapImage(new Uri(".\\Image\\file.png", UriKind.RelativeOrAbsolute));
        internal static BitmapImage warningIconDef = new BitmapImage(new Uri(".\\Image\\warn.png", UriKind.RelativeOrAbsolute));
        internal static BitmapImage errorIconDef = new BitmapImage(new Uri(".\\Image\\error.png", UriKind.RelativeOrAbsolute));
        internal static BitmapImage infoIconDef = new BitmapImage(new Uri(".\\Image\\info.png", UriKind.RelativeOrAbsolute));
        internal static BitmapImage questionIconDef = new BitmapImage(new Uri(".\\Image\\question.png", UriKind.RelativeOrAbsolute));
        internal static double messgaeIconHeightDef = 32;
        internal static double closeIconHeightDef = 0;
        internal static double titleIconHeightDef = 0;
        internal static bool setCloseIconHeightAsTitleFontHeightDef = true;
        internal static bool setTitleIconHeightAsTitleFontHeightDef = true;
        internal static bool enableCloseButtonDef = false;
        internal static bool enableTitleIconDef = false;
        public static PropertiesSetter GetPropertiesSetter(PropertiesSetterName propertiesSetterName)
        {
            PropertiesSetter propertiesSetter = null;
            if (propertiesSetterName == PropertiesSetterName.Black)
            {
                propertiesSetter = new PropertiesSetter()
                {
                    ButtonFontSize = 20,
                    MessageFontSize = 20,
                    TitleFontSize = 20,
                    WindowWidth = 500,
                    WindowMinHeight = 250,
                    ButtonPanelColor = new MessageBoxColor("#F03A3A3A"),
                    MessagePanelColor = new MessageBoxColor("#F03A3A3A"),
                    TitlePanelColor = new MessageBoxColor("#F03A3A3A"),
                    TitlePanelBorderThickness = new Thickness(0, 0, 0, 2),
                    TitlePanelBorderColor = new MessageBoxColor("#FFEFE2E2"),
                    MessagePanelBorderThickness = new Thickness(0),
                    ButtonPanelBorderThickness = new Thickness(0, 1, 0, 0),
                    WndBorderThickness = new Thickness(1),
                    TitleFontColor = new MessageBoxColor("#FFCBBEBE"),
                    MessageFontColor = new MessageBoxColor(Colors.White),
                };
            }
            else if (propertiesSetterName == PropertiesSetterName.Gray)
            {
                propertiesSetter = new PropertiesSetter()
                {
                    WndBorderThickness = new Thickness(1),
                    WndBorderColor = new MessageBoxColor(Colors.LightGray),
                    ButtonPanelColor = new MessageBoxColor("White"),
                    MessagePanelColor = new MessageBoxColor("White"),
                    TitlePanelColor = new MessageBoxColor(Colors.LightGray),
                    TitlePanelBorderThickness = new Thickness(0, 0, 0, 2),
                    TitlePanelBorderColor = new MessageBoxColor("#FFEFE2E2"),
                    MessagePanelBorderThickness = new Thickness(0),
                    ButtonPanelBorderThickness = new Thickness(0),
                    TitleFontSize = 14,
                    TitleFontColor = new MessageBoxColor(Colors.Black),
                    MessageFontColor = new MessageBoxColor(Colors.Black),
                    MessageFontSize = 14,
                    ButtonFontSize = 16,
                    ButtonBorderThickness = new Thickness(1),
                    ButtonBorderColor = new MessageBoxColor(Colors.LightGray),
                    WindowMinHeight = 200,
                    LockHeight = false,
                    WindowWidth = 450,
                    WindowShowDuration = new Duration(new TimeSpan(0, 0, 0, 0, 300)),
                };
            }
            else if (propertiesSetterName == PropertiesSetterName.Gray)
            {
                propertiesSetter = new PropertiesSetter()
                {
                    WndBorderThickness = new Thickness(1),
                    WndBorderColor = new MessageBoxColor(Colors.LightGray),
                    ButtonPanelColor = new MessageBoxColor("White"),
                    MessagePanelColor = new MessageBoxColor("White"),
                    TitlePanelColor = new MessageBoxColor(Colors.LightGray),
                    TitlePanelBorderThickness = new Thickness(0, 0, 0, 2),
                    TitlePanelBorderColor = new MessageBoxColor("#FFEFE2E2"),
                    MessagePanelBorderThickness = new Thickness(0),
                    ButtonPanelBorderThickness = new Thickness(0),
                    TitleFontSize = 14,
                    TitleFontColor = new MessageBoxColor(Colors.Black),
                    MessageFontColor = new MessageBoxColor(Colors.Black),
                    MessageFontSize = 14,
                    ButtonFontSize = 16,
                    ButtonBorderThickness = new Thickness(1),
                    ButtonBorderColor = new MessageBoxColor(Colors.LightGray),
                    WindowMinHeight = 200,
                    LockHeight = false,
                    WindowWidth = 450,
                    WindowShowDuration = new Duration(new TimeSpan(0, 0, 0, 0, 300)),
                };
            }

            return propertiesSetter;
        }


        public static Style GetButtonStyle(ButtonStyleName buttonStyleName)
        {
            Style style = null;
            if (buttonStyleName == ButtonStyleName.White)
            {
                style = new Style();
                style.TargetType = typeof(Button);
                style.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Colors.White)));
                style.Setters.Add(new Setter(Button.BorderBrushProperty, new SolidColorBrush(Colors.LightGray)));
                style.Setters.Add(new Setter(Button.ForegroundProperty, new SolidColorBrush(Colors.Black)));
                style.Setters.Add(new Setter(Button.FontSizeProperty, (double)12));
                style.Setters.Add(new Setter(Button.BorderThicknessProperty, new Thickness(1)));
                style.Setters.Add(new Setter(Button.MarginProperty, new Thickness(5)));
                style.Setters.Add(new Setter(Button.HeightProperty, 25d));
            }
            return style;
        }

    }
}
