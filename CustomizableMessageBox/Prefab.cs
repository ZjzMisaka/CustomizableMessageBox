using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CustomizableMessageBox
{
    public enum PropertiesSetterName { Black, Gray };
    public enum ButtonStyleName { White };
    public static class Prefab
    {
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


        public static Style GetButonStyle(ButtonStyleName buttonStyleName)
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
