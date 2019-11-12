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
        public SampleWnd()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.WindowMinHeight = 300;
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
            lbl.Content = MessageBox.Show("啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊qqqqqqqq\n0efwfewfefqfqfaaaaaaaaaaaq\n1efwfewfefqfqfaaaaaaaaaaaq\n2efwfewfefqfqfaaaaaaaaaaaq\n3efwfewfefqfqfaaaaaaaaaaaaaaiiiiiiiiiiiiiiiiiiiiiiiiiiiiii\nsaaaaaaaaaan siiiiiaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaan saaaaaaaan\naaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaawqwqedwqew\nfewsdddddddddddddddddddddddddddddddddddddd\ndwddddddddddddddddddddddddddddddddfefeeeeeeeeeeeeeeeeeeeeeeeeeeeeeef\nhnujyyyyyyyyyyyyy\nutjjjjjjjjjeeeeeeeeeeeeeeeef\nhnujyyyyyyyyyyyyy\nutjjjjjjjjjeeeeeeeeeeeeeeeeeeeeeeeeef\nhnujyyyyyyyyyyyyy\nutjjjjjjjjjjjjjjjjjjjjjjjjjjjj\nyrhhhhtr\nyhhhhhhhhhhh\nrythr\nrthyrhr\nryhrhr", "123123", MessageBoxButton.OK, MessageBoxImage.None);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            lbl.Content = MessageBox.Show("开啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊结\ns啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊n\naaaaaaaaaaaaa\naaaaaaaaaaaaaaa啊啊啊啊啊啊aaaaaaaaaaaaaaaaaaaaa啊aaaaaaaaaaaaaaaaaaaa\n1514564464\nrgeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee\nerfewfwf\nerwfwfewfewfef\nefewfewgfewgewrewqrwqedwqwqqqqqqqqqjj\nyrhhhhtr\nyhhhhhhhhhhh\nrythr\nrthyrhr\nryhrhrqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqjj\nyrhhhhtr\nyhhhhhhhhhhh\nrythr\nrthyrhr\nryhrhrqq\nefwfewfefqfqfwqwqedwqew\nfewf", "123123", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            MessageBox.WindowMinHeight = 300;
            lbl.Content = MessageBox.Show(new List<string> { "hello" }, "56145456454564544514", "title");
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            MessageBox.WindowOpacity = 0.4;
            lbl.Content = MessageBox.Show(new List<string> { "hello", "apple" }, "56145456454564544514\n51515615", "title", MessageBoxImage.Asterisk);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            MessageBox.TitleFontSize = 25;
            MessageBox.ButtonFontSize = 25;
            MessageBox.MessageFontSize = 22;
            lbl.Content = MessageBox.Show(new List<string> { "hello", "awsl", "banana", "艾维斯了" }, "啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊qqqqqqqq\nefwfewfefqfqfaaaaaaaaaaaq\nefwfewfefqfqfaaaaaaaaaaaq\nefwfewfefqfqfaaaaaaaaaaaq\nefwfewfefqfqfaaaaaaaaa aaaaaiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiaaaaaaa aaaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaaa aaaaaaaaaaa aaaaawqwqedwqewfewsddddddd dddddddddddd ddddddddddddd dddddd\ndwddddddddddddddddddddddddddddddddfddd\ndwdddddddddddddddddddd\ndwdddddddddddddddddddd\ndwdddddddddddddddddefeeeeeeeeeeeeeeeeeeeeeeeeeeeeeef\nhnujyyyyyyyyyyyyy\nutjjjjjjjjjeeeeeeeeeeeeeeeef\nhnujyyyyyyyyyyyyy\nutjjjjjjjjjeeeeeeeeeeeeeeeeeeeeeeeeef\nhnujyyyyyyyyyyyyy\nutjjjjjjjjjjjjjjjjjjjjjjjjjjjj\nyrhhhhtr\nyhhhhhhhhhhh\nrythr\nrthyrhr\nryhrhr", "123123", MessageBoxImage.Error);
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            MessageBox.ButtonFontSize = 25;
            lbl.Content = MessageBox.Show(new List<string> { "洋葱", "awsl", "fbk", "艾维斯了", "DD" }, "啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊\n啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊\naaaaaaaaaaaaa\naaaaaaaaaaaaaaa啊啊啊啊啊啊aaaaaaaaaaaaaaaaaaaaa啊aaaaaaaaaaaaaaaaaaaa\n1514564464\nrgeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee\nerfewfwf\nerwfwfewfewfef\nefewfewgfewgewrewqrwqedwqwqqqqqqqqqjj\nyrhhhhtr\nyhhhhhhhhhhh\nrythr\nrthyrhr\nryhrhrqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqjj\nyrhhhhtr\nyhhhhhhhhhhh\nrythr\nrthyrhr\nryhrhrqq\nefwfewfefqfqfwqwqedwqew\nfewf", "123123", MessageBoxImage.Warning);
        }
    }
}
