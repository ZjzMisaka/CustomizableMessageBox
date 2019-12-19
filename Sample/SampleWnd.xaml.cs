using MessageBoxTouch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
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
        PropertiesSetter ps3 = null;
        PropertiesSetter ps4 = null;

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
            ps1.WindowShowDuration = new Duration(new TimeSpan(0, 0, 0, 0, 300));

            ps2 = new PropertiesSetter(ps1);
            ps2.MessageFontSize = 18;

            ps3 = new PropertiesSetter(ps1);
            ps3.CloseTimer = new MessageBoxCloseTimer(5, -100);

            ps4 = new PropertiesSetter(ps1);
            ps4.WarningIcon = new BitmapImage(new Uri("C:\\Users\\admin\\Pictures\\Global-Book-Close.png"));
            ps4.MessageFontSize = 20;
            ps4.TitleFontFamily = new FontFamily("黑体");
            ps4.ButtonFontFamily = new FontFamily("黑体");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.ButtonPanelColor = new MessageBoxColor("red");
            MessageBox.ButtonFontColor = new MessageBoxColor("#114514");
            MessageBox.ButtonPanelBorderColor = new MessageBoxColor(Colors.Blue);
            MessageBox.ButtonBorderColor = new MessageBoxColor("#19198100");
            MessageBox.WindowMinHeight = 150;
            MessageBox.EnableCloseButton = true;
            MessageBox.CloseTimer = new MessageBoxCloseTimer(2, 106);
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

        static ProgressBar pb;
        private delegate void TimerDispatcherDelegate();
        private static List<string> lys = new List<string>
        {
            "ルイズ！ルイズ！ルイズ！ルイズぅぅうううわぁああああああああああああああああああああああん！！！",
            "あぁああああ…ああ…あっあっー！あぁああああああ！！！ルイズルイズルイズぅううぁわぁああああ！！！",
            "あぁクンカクンカ！クンカクンカ！スーハースーハー！スーハースーハー！いい匂いだなぁ…くんくん",
            "んはぁっ！ルイズ・フランソワーズたんの桃色ブロンドの髪をクンカクンカしたいお！クンカクンカ！あぁあ！！",
            "間違えた！モフモフしたいお！モフモフ！モフモフ！髪髪モフモフ！カリカリモフモフ…きゅんきゅんきゅい！！",
            "小説12巻のルイズたんかわいかったよぅ！！あぁぁああ…あああ…あっあぁああああ！！ふぁぁあああんんっ！！",
            "アニメ2期放送されて良かったねルイズたん！あぁあああああ！かわいい！ルイズたん！かわいい！あっああぁああ！",
            "コミック2巻も発売されて嬉し…いやぁああああああ！！！にゃああああああああん！！ぎゃああああああああ！！",
            "ぐあああああああああああ！！！コミックなんて現実じゃない！！！！あ…小説もアニメもよく考えたら…",
            "ル イ ズ ち ゃ ん は 現実 じ ゃ な い？にゃあああああああああああああん！！うぁああああああああああ！！",
            "そんなぁああああああ！！いやぁぁぁあああああああああ！！はぁああああああん！！ハルケギニアぁああああ！！",
            "この！ちきしょー！やめてやる！！現実なんかやめ…て…え！？見…てる？表紙絵のルイズちゃんが僕を見てる？",
            "表紙絵のルイズちゃんが僕を見てるぞ！ルイズちゃんが僕を見てるぞ！挿絵のルイズちゃんが僕を見てるぞ！！",
            "アニメのルイズちゃんが僕に話しかけてるぞ！！！よかった…世の中まだまだ捨てたモンじゃないんだねっ！",
            "いやっほぉおおおおおおお！！！僕にはルイズちゃんがいる！！やったよケティ！！ひとりでできるもん！！！",
            "あ、コミックのルイズちゃああああああああああああああん！！いやぁあああああああああああああああ！！！！",
            "あっあんああっああんあアン様ぁあ！！シ、シエスター！！アンリエッタぁああああああ！！！タバサｧぁあああ！！",
            "ううっうぅうう！！俺の想いよルイズへ届け！！ハルケギニアのルイズへ届け！ "
        };
        static int lys_i = 0;
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //MessageBox.WindowMinHeight = 300;
            TextBox newTb = new TextBox();
            newTb.Margin = new Thickness(10, 7, 10, 7);
            newTb.FontSize = 16;
            newTb.Width = 150;

            pb = new ProgressBar();
            pb.Margin = new Thickness(10, 7, 10, 7);
            pb.Width = 150;
            pb.Minimum = 0;
            pb.Maximum = 10000;
            Timer timer = new Timer();
            timer.Interval = 100;
            timer.Elapsed += Tick;
            timer.Start();
            lbl.Content = MessageBox.Show(ps3, new List<object> { new ButtonSpacer(30), pb, new ButtonSpacer(90), "好的", "不明白", new ButtonSpacer(30) }, "露易丝定型文是流传于2ch VIP版上的一段定型文。主要内容是某阿宅对《零之使魔》作品女主角露易丝撕心裂肺的表白。 ", "露易丝定型文", MessageBoxImage.Information);
            timer.Stop();

            pb = new ProgressBar();
            pb.Margin = new Thickness(10, 7, 10, 7);
            pb.Width = 150;
            pb.Minimum = 0;
            pb.Maximum = 10000;
            timer.Start();
            lbl.Content = MessageBox.Show(ps3, new List<object> { new ButtonSpacer(30), pb, new ButtonSpacer(90), "好的", "不明白", new ButtonSpacer(30) }, "该篇定型文首次流行于2007年左右，具体发表的时间和出处已不可考。但就内容来看，该篇定型文提及《零之使魔》动画二期放送（2007年7月8日）、漫画第二卷发售（2007年5月23日）、小说第12卷发售（2007年8月24日），故推测成型于2007年8月左右。\n此后，该篇定型文被众多声优、up主配音。最早的配音是up主podcast于2008年4月17日投稿的sm3018556，截至2019年8月21日已有70万再生。最著名的女性声优配音则是电波 / 音游歌手ななひら于2008年11月2日投稿的成名作sm5123978，也是她的黑历史之一，截至2019年8月21日已有60万再生。 ", "露易丝定型文", MessageBoxImage.Information);
            timer.Stop();

            int result;
            lbl.Content = result = MessageBox.Show(ps2, new List<object> { new ButtonSpacer(30), newTb, new ButtonSpacer(90), "提交", "取消", new ButtonSpacer(30) }, "你对露易丝定型文有什么看法吗?", "问题", MessageBoxImage.Question);
            TextBox tb = (TextBox)MessageBox.GetBtnList()[1];
            MessageBox.Show(ps4, tb.Text == string.Empty ? "用户未输入" : "内容: \n" + tb.Text, "您点击了" + (string)MessageBox.GetBtnList()[result]);

            DDChallenge();

            MessageBox.WarningIcon = new BitmapImage(new Uri("C:\\Users\\admin\\Pictures\\lys1.jpg"));
            MessageBox.ErrorIcon = new BitmapImage(new Uri("C:\\Users\\admin\\Pictures\\lys2.png"));
            MessageBox.WindowShowDuration = new Duration(new TimeSpan(0, 0, 0, 1));
            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += Tick_AddStr;
            timer.Start();
            MessageBox.Show("ルイズ！ルイズ！ルイズ！ルイズぅぅうううわぁああああああああああああああああああああああん！！！", "露易丝定型文全文鉴赏", MessageBoxButton.OK);
            timer.Stop();
        }

        private void DDChallenge()
        {
            int result;
            Button btn = new Button();
            btn.Content = "推!";
            btn.Click += Fnc;
            btn.Height = 100;
            MessageBox.ButtonFontSize = 25;
            MessageBox.TextWrappingMode = TextWrapping.WrapWithOverflow;
            MessageBox.EnableCloseButton = true;
            MessageBox.CloseTimer = new MessageBoxCloseTimer(5, -100);
            lbl.Content = result = MessageBox.Show(new List<object> { btn, "洋葱", "fbk", "马自立", "狗妈", "新科娘" }, "在五秒内单推所有人", "DD挑战", MessageBoxImage.Exclamation);
            if (result == -100)
            {
                if (MessageBox.Show("人是有极限的. \n再试一次?", "单推失败", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    DDChallenge();
                }
            }
            else
            {
                MessageBox.Show("单推所有人是虚假的单推", "单推成功", MessageBoxButton.OKCancel);
            }
        }

        private static void Tick(object sender, EventArgs e)
        {
            pb.Dispatcher.Invoke(DispatcherPriority.Normal, new TimerDispatcherDelegate(updateUI));
        }

        private static void updateUI()
        {
            pb.Value = pb.Value + 200;
            //if(pb.Value == 5000)
            //{
            //    MessageBox.CloseTimer.CloseNow();
            //}
        }

        private static void Tick_AddStr(object sender, EventArgs e)
        {
            pb.Dispatcher.Invoke(DispatcherPriority.Normal, new TimerDispatcherDelegate(Tick_AddStr));
        }

        private static void Tick_AddStr()
        {
            if (MessageBox.MessageFontSize < 40)
            {
                MessageBox.SetIconType(MessageBoxImage.Warning);
                MessageBox.MessageFontSize += 5;
                ++MessageBox.ButtonFontSize;
                ++MessageBox.TitleFontSize;
            }
            else if (++lys_i < lys.Count)
            {
                MessageBox.SetIconType(MessageBoxImage.Error);
                MessageBox.MessageText += "\n" + lys[lys_i];
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            MessageBox.WindowOpacity = 1;
            MessageBox.TitleBarOpacity = 1;
            MessageBox.MessageBarOpacity = 0.1;
            MessageBox.ButtonBarOpacity = 1;
            MessageBox.WindowShowDuration = new Duration(new TimeSpan(0, 0, 0, 0, 1500));
            MessageBox.ButtonFontFamily = new FontFamily("黑体");
            lbl.Content = MessageBox.Show(new List<object> { new ButtonSpacer(500), "hello" }, "56145456454564544514\n51515615", "title", MessageBoxImage.Asterisk);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            MessageBox.TitleFontSize = 25;
            MessageBox.ButtonFontSize = 25;
            MessageBox.MessageFontSize = 22;
            MessageBox.TextWrappingMode = TextWrapping.WrapWithOverflow;
            TextBox newTb = new TextBox();
            //newTb.Margin = new Thickness(10, 7, 10, 7);
            newTb.FontSize = 25;
            newTb.Height = 100;
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
            btn.Height = 100;
            MessageBox.ButtonFontSize = 25;
            MessageBox.TextWrappingMode = TextWrapping.WrapWithOverflow;
            MessageBox.EnableCloseButton = true;

            //DoubleAnimation widthAnimation = new DoubleAnimation(200, 800, new Duration(TimeSpan.FromSeconds(100)));
            //DoubleAnimation heightAnimation = new DoubleAnimation(700, 1080, new Duration(TimeSpan.FromSeconds(100)));
            //List<KeyValuePair<DependencyProperty, AnimationTimeline>> lkv = new List<KeyValuePair<DependencyProperty, AnimationTimeline>>();
            //lkv.Add(new KeyValuePair<DependencyProperty, AnimationTimeline> ( Window.WidthProperty, widthAnimation ));
            //lkv.Add(new KeyValuePair<DependencyProperty, AnimationTimeline>(Window.HeightProperty, heightAnimation));
            //MessageBox.WindowShowAnimations = lkv;

            //DoubleAnimation widthAnimation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.5)));
            //DoubleAnimation heightAnimation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.5)));
            //Timeline.SetDesiredFrameRate(widthAnimation, 100);
            //Timeline.SetDesiredFrameRate(heightAnimation, 100);
            //List<KeyValuePair<DependencyProperty, AnimationTimeline>> lkv = new List<KeyValuePair<DependencyProperty, AnimationTimeline>>();
            //lkv.Add(new KeyValuePair<DependencyProperty, AnimationTimeline>(Window.WidthProperty, widthAnimation));
            //lkv.Add(new KeyValuePair<DependencyProperty, AnimationTimeline>(Window.HeightProperty, heightAnimation));
            //MessageBox.WindowCloseAnimations = lkv;

            MessageBox.CloseTimer = new MessageBoxCloseTimer(5, -100);

            MessageBox.WarningIcon = new BitmapImage(new Uri("C:\\Users\\admin\\Pictures\\ssccicon.png"));

            lbl.Content = MessageBox.Show(new List<object> { btn, "洋葱", "awsl", "fbk", "艾维斯了", "DD" }, "啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊\n啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊\n啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊\n啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊\naaaaaaaaaaaaa\naaaaaaaaaaaaaaa啊啊啊啊啊啊aaaaaaaaaaaaaaaaaaaaa啊aaaaaaaaaaaaaaaaaaaa\n1514564464\nrgeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee\nerfewfwf\nerwfwfewfewfef\nefewfewgfewgewrewqrwqedwqwqqqqqqqqqjj\nyrhhhhtr\nyhhhhhhhhhhh\nrythr\nrthyrhr\nryhrhrqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqjj\nyrhhhhtr\nyhhhhhhhhhhh\nrythr\nrthyrhr\nryhrhrqq\nefwfewfefqfqfwqwqedwqew\nfewf", "123123", MessageBoxImage.Warning);
        }

        private void Fnc(object sender, RoutedEventArgs e)
        {
            Button sbtn = (Button)sender;
            sbtn.Width = sbtn.ActualWidth + 20;
            if (sbtn.ActualHeight - 20 > 0)
                sbtn.Height = sbtn.ActualHeight - 20;

            sbtn.Content = "嘶哈嘶哈";
            lbl.Content = "awsl";

            MessageBox.MessageText = "1145141919810";
        }
    }
}
