using MessageBoxTouch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = MessageBoxTouch.MessageBox;

namespace Sample
{
    /// <summary>
    /// SampleWnd.xaml 的交互逻辑
    /// </summary>
    public partial class SampleWnd : Window
    {
        PropertiesSetter ps1 = new PropertiesSetter();
        PropertiesSetter ps2 = null;

        public SampleWnd()
        {
            InitializeComponent();

            ps1.WndBorderThickness = new Thickness(0);
            ps1.ButtonPanelColor = new MessageBoxColor("#F03A3A3A");
            ps1.MessagePanelColor = new MessageBoxColor("#F03A3A3A");
            ps1.TitlePanelColor = new MessageBoxColor("#F03A3A3A");
            ps1.TitlePanelBorderThickness = new Thickness(0, 0, 0, 2);
            ps1.TitlePanelBorderColor = new MessageBoxColor("#FFEFE2E2");
            ps1.MessagePanelBorderThickness = new Thickness(0);
            ps1.ButtonPanelBorderThickness = new Thickness(0);
            ps1.WndBorderThickness = new Thickness(1);
            ps1.TitleFontSize = 14;
            ps1.TitleFontColor = new MessageBoxColor("#FFCBBEBE");
            ps1.MessageFontColor = new MessageBoxColor(Colors.White);
            ps1.MessageFontSize = 14;
            ps1.ButtonFontSize = 16;
            ps1.WindowMinHeight = 200;
            ps1.LockHeight = true;
            ps1.WindowWidth = 450;

            ps2 = new PropertiesSetter(ps1);
            ps2.MessageFontSize = 18;
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.ButtonPanelColor = new MessageBoxColor("red");
            MessageBox.WindowMinHeight = 150;
            lbl.Content = MessageBox.Show("123123", "123123", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.WindowOpacity = 0.1;
            lbl.Content = MessageBox.Show("啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊aaaaaaaa啊啊啊啊啊啊aaaaa\n1514564464", "123123", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBox.TitleFontSize = 25;
            MessageBox.ButtonFontSize = 25;
            MessageBox.MessageFontSize = 22;
            MessageBox.TextWrappingMode = TextWrapping.WrapWithOverflow;
            lbl.Content = MessageBox.Show("啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊qqqqqqqq\n0efwfewfefqfqfaaaaaaaaaaaq\n1efwfewfefqfqfaaaaaaaaaaaq\n2efwfewfefqfqfaaaaaaaaaaaq\n3efwfewfefqfqfaaaaaaaaaaaaaaiiiiiiiiiiiiiiiiiiiiiiiiiiiiii\nsaaaaaaaaaan siiiiiaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaan saaaaaaaan\nsaaaaaaaaaan siiiiiaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaan saaaaaaaan\nsaaaaaaaaaan siiiiiaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaan saaaaaaaan\naaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaawqwqedwqew\nfewsdddddddddddddddddddddddddddddddddddddd\ndwddddddddddddddddddddddddddddddddfefeeeeeeeeeeeeeeeeeeeeeeeeeeeeeef\nhnujyyyyyyyyyyyyy\nutjjjjjjjjjeeeeeeeeeeeeeeeef\nhnujyyyyyyyyyyyyy\nutjjjjjjjjjeeeeeeeeeeeeeeeeeeeeeeeeef\nhnujyyyyyyyyyyyyy\nutjjjjjjjjjjjjjjjjjjjjjjjjjjjj\nyrhhhhtr\nyhhhhhhhhhhh\nrythr\nrthyrhr\nryhrhr", "123123", MessageBoxButton.OK, MessageBoxImage.None);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MessageBox.TextWrappingMode = TextWrapping.WrapWithOverflow;
            lbl.Content = MessageBox.Show("开啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊结\ns啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊n\naaaaaaaaaaa啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊\n啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊\n啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊\n啊啊啊啊aa\naaaaaaaaaaaaaaa啊啊啊啊啊啊aaaaaaaaaaaaaaaaaaaaa啊aaaaaaaaaaaaaaaaaaaa\n1514564464\nrgeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee\nerfewfwf\nerwfwfewfewfef\nefewfewgfewgewrewqrwqedwqwqqqqqqqqqjj\nyrhhhhtr\nyhhhhhhhhhhh\nrythr\nrthyrhr\nryhrhrqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqjj\nyrhhhhtr\nyhhhhhhhhhhh\nrythr\nrthyrhr\nryhrhrqq\nefwfewfefqfqfwqwqedwqew\nfewf", "123123", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //MessageBox.WindowMinHeight = 300;
            TextBox newTb = new TextBox();
            newTb.Margin = new Thickness(10, 7, 10, 7);
            newTb.FontSize = 16;
            newTb.Width = 150;
            newTb.Text = "请输入...";
            lbl.Content = MessageBox.Show(ps1, new List<object> { new ButtonSpacer(30), newTb, new ButtonSpacer(90), "Yes", "No", new ButtonSpacer(30) }, "Question?啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊结\ns啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊结\ns啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊结\ns啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊结\ns啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊结\ns啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊结\ns啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊结\ns啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊结\ns啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊结\ns啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊结\ns啊啊啊啊啊", "Question", MessageBoxImage.Question);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            MessageBox.WindowOpacity = 0.4;
            lbl.Content = MessageBox.Show(new List<object> {new ButtonSpacer(500), "hello" }, "56145456454564544514\n51515615", "title", MessageBoxImage.Asterisk);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            MessageBox.TitleFontSize = 25;
            MessageBox.ButtonFontSize = 25;
            MessageBox.MessageFontSize = 22;
            MessageBox.TextWrappingMode = TextWrapping.WrapWithOverflow;
            TextBox newTb = new TextBox();
            newTb.Margin = new Thickness(10, 7, 10, 7);
            newTb.FontSize = 25;
            int result;
            lbl.Content = result = MessageBox.Show(new List<object> { newTb, "awsl", "banana", "艾维斯了" }, "啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊qqqqqqqq\nefwfewfefqfqfaaaaaaaaaaaq\nefwfewfefqfqfaaaaaaaaaaaq\nefwfewfefqfqfaaaaaaaaaaaq\nefwfewfefqfqfaaaaaaaaa aaaaaiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiaaaaaaa aaaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaaa aaaaaaaaaaa aaaaawqwqedwqewfewsddddddd dddddddddddd ddddddddddddd dddddd\ndwddddddddddddddddddddddddddddddddfddd\ndwdddddddddddddddddddd\ndwdddddddddddddddddddd\ndwdddddddddddddddddefeeeeeeeeeeeeeeeeeeeeeeeeeeeeeef\nhnujyyyyyyyyyyyyy\nutjjjjjjjjjeeeeeeeeeeeeeeeef\nhnujyyyyyyyyyyyyy\nutjjjjjjjjjeeeeeeeeeeeeeeeeeeeeeeeeef\nhnujyyyyyyyyyyyyy\nutjjjjjjjjjjjjjjjjjjjjjjjjjjjj\nyrhhhhtr\nyhhhhhhhhhhh\nrythr\nrthyrhr\nryhrhr", "123123", MessageBoxImage.Error);
            TextBox tb = (TextBox)MessageBox.GetBtnList()[0];
            MessageBox.Show(ps2, tb.Text == string.Empty ? "用户未输入" : tb.Text, (string)MessageBox.GetBtnList()[result]);
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Button btn = new Button();
            btn.Content = "改变主窗口显示文字";
            btn.Click += Fnc;
            MessageBox.ButtonFontSize = 25;
            lbl.Content = MessageBox.Show(new List<object> { btn, "洋葱", "awsl", "fbk", "艾维斯了", "DD" }, "啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊\n啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊\n啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊\n啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊\naaaaaaaaaaaaa\naaaaaaaaaaaaaaa啊啊啊啊啊啊aaaaaaaaaaaaaaaaaaaaa啊aaaaaaaaaaaaaaaaaaaa\n1514564464\nrgeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee\nerfewfwf\nerwfwfewfewfef\nefewfewgfewgewrewqrwqedwqwqqqqqqqqqjj\nyrhhhhtr\nyhhhhhhhhhhh\nrythr\nrthyrhr\nryhrhrqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqjj\nyrhhhhtr\nyhhhhhhhhhhh\nrythr\nrthyrhr\nryhrhrqq\nefwfewfefqfqfwqwqedwqew\nfewf", "123123", MessageBoxImage.Warning);
        }

        private void Fnc(object sender, RoutedEventArgs e)
        {
            Button sbtn = (Button)sender;
            sbtn.Content = "嘶哈嘶哈";
            lbl.Content = "aaawwwssslll";
        }
    }
}
