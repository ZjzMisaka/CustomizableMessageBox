# MessageBoxTouch
 ![WTFPL](http://www.wtfpl.net/wp-content/uploads/2012/12/wtfpl-badge-1.png) <br />
 [中文ReadMe](https://github.com/ZjzMisaka/MessageBoxTouch/blob/master/README_CH.md) <br />
 The system MessageBox button and font are too small, so I wrote a MessageBox that is convenient for changing the appearance. What fun features can also be extended. There are demo pictures below. <br />
### Features
- Running in Singleton Pattern, it is not suitable for use on a tablet because it can change the font and supports touch operation at runtime.
- The font can be changed and touch operation is supported, so it is suitable for use on a tablet.
- The MessageBox initially has a certain height, but if the Message text content is too much and exceeds the height limit, so that the MessageBox cannot be displayed, the window height will increase accordingly, so that the message can be completely displayed.
- If the height of the message box window reaches the height of the working area of the display, but the message string cannot be displayed, the height of the message box no longer increases, but you can view the remaining messages by scrolling the message area.
- You can change the appearance at will.
- 可You can insert custom controls (such as an input box, another button, a progress bar, etc.) in the button area. You can get the user's operation results after calling.
- You can change the content and style of the MessageBox at runtime.
### Usage
- Generate the project MessageBoxTouch, get the corresponding dynamic link library, refer to your own project, and call it. This project comes with examples.
- The Show function parameters of the tool's MessageBox are compatible with the parameters of System.Windows.MessageBox. Simply put in the dll, you can use the MessageBox provided by the tool without making too many changes.
- The MessageBox.Show function of the tool also has some overloads different from the system functions, which can achieve more functions. For example, you can customize the message box button.
- You can change the appearance of the MessageBox by setting properties, such as the font of each part of the window, transparency, background, border, window size, limit height growth, set window width, etc.
### Demo pictures
![Demo](https://www.iaders.com/wp-content/uploads/2019/12/mb-1.gif "I made two styles roughly")
### Code example
#### Compatible calling methods
The return value is MessageBoxResult.
```csharp
MessageBox.Show("message");
MessageBox.Show("message", "title", MessageBoxButton.OKCancel, MessageBoxImage.Question);
```
#### Custom calling method
The return value is int, and the value is the index in the parameter button list.
```csharp
MessageBox.Show(new List<object> { "btn1" }, "msg");
MessageBox.Show(new List<object> { new ButtonSpacer(250), "btn1", "btn2", "btn3", "btn4", "btn5", new ButtonSpacer(30) }, "msg", "title", MessageBoxImage.Asterisk);
```
#### Modify style attributes
##### Modify individually
- Set before calling Show function
```csharp
MessageBox.ButtonPanelColor = new MessageBoxColor("red");
MessageBox.WindowMinHeight = 300;
MessageBox.MessageFontSize = 22;
MessageBox.Show(new List<object> { "btn1" }, "msg");
```
##### Modify in batches
- Set PropertiesSetter in advance
```csharp
PropertiesSetter ps0 = new PropertiesSetter();
ps0.ButtonBorderThickness = new Thickness(10);
ps0.MessagePanelColor = new MessageBoxColor(Colors.Black);

PropertiesSetter ps1 = new PropertiesSetter(ps0);
ps1.MessagePanelBorderThickness = new Thickness(10, 0, 0, 0);
ps1.ButtonBorderColor = new MessageBoxColor("#222DDD");
ps1.MessageFontFamily = new FontFamily("宋体");
ps1.CloseTimer = new MessageBoxCloseTimer(10, -1);
```
- Use PropertiesSetter
1. **Recommended** Set in Show function parameters
```csharp
MessageBox.Show(ps0, "message", "title", MessageBoxButton.OKCancel, MessageBoxImage.Question);
MessageBox.Show(ps1, new List<object> { "btn1" }, "msg");
```
2. Set before calling Show function
```csharp
MessageBox.PropertiesSetter = ps1;
MessageBox.Show(new List<object> { new TextBox(), "btn1" }, "msg");
```
#### Modify button area
##### Insert blank
Insert a ButtonSpacer instance at the corresponding position in the passed in List&lt;object&gt;. The constructor parameter can be empty or specify a width value.
##### Insert custom control
Insert an instance of a FrameworkElement-derived class at the corresponding position in the passed-in List&lt;object&gt; to display the corresponding control at the corresponding position. </br>
The column width is determined by the width of the inserted control. </br>
- You can get the control again after the Show function call returns and get the user input / operation result.
```csharp
int result = MessageBox.Show(new List<object> { new TextBox(), "btn1", "btn2" }, "msg");
TextBox tb = (TextBox)MessageBox.ButtonList[0];
MessageBox.Show(tb.Text == string.Empty ? "用户未输入" : tb.Text, (string)MessageBox.ButtonList[result]);
```
|Properties|Status|
|----|----|
|Window Title / Message / Button|√|
|Window lock height|√|
|Window initial height and width|√|
|Wrap style in message area|√|
|Text font for each area|√|
|Text size of each area|√|
|Text color of each area|√|
|Transparency of each area|√|
|Background color of each area|√|
|Border color of each area|√|
|Border width of each area|√|
|Window fade time|√|
|Custom window opening and closing animation|√|
|Custom display icon type|√|
|Custom Icon|√|
|Whether the window close button is applied|√|
|Button Action Style|√|
|Window Timing / Close Now|√|
### Member Functions and Properties
|MessageBox Properties|Type|Meaning|Is Static|Status|
|----|----|----|----|----|
|TitleText|string|Set / Get Title Text|√|√|
|MessageText|string|Set / Get Message Text|√|√|
|ButtonList|List&lt;object&gt;|Set / Get Button List|√|√|
|LockHeight|bool|Whether the height of the locked window is not allowed to grow automatically|√|√|
|TextWrappingMode|TextWrapping|Wrap style of message paragraph|√|√|
|WindowWidth|double|Window width|√|√|
|WindowMinHeight|double|Window minimum (initial) height|√|√|
|TitleFontFamily|FontFamily|Title text font|√|√|
|MessageFontFamily|FontFamily|Message text font|√|√|
|ButtonFontFamily|FontFamily|Button text font|√|√|
|TitleFontSize|int|Title text size|√|√|
|MessageFontSize|int|Message text size|√|√|
|ButtonFontSize|int|Button text size|√|√|
|TitleFontColor|MessageBoxColor|Title Text Color|√|√|
|MessageFontColor|MessageBoxColor|Message text color|√|√|
|ButtonFontColor|MessageBoxColor|Button text color|√|√|
|WindowOpacity|double|The overall transparency of the window|√|√|
|TitleBarOpacity|double|Title area transparency|√|√|
|MessageBarOpacity|double|Message area transparency|√|√|
|ButtonBarOpacity|double|Button area transparency|√|√|
|TitlePanelColor|MessageBoxColor|Title area background color|√|√|
|MessagePanelColor|MessageBoxColor|Message area background color|√|√|
|ButtonPanelColor|MessageBoxColor|Button area background color|√|√|
|WndBorderColor|MessageBoxColor|Window border color|√|√|
|TitlePanelBorderColor|MessageBoxColor|Title Area Border Color|√|√|
|MessagePanelBorderColor|MessageBoxColor|Message area border color|√|√|
|ButtonPanelBorderColor|MessageBoxColor|Button area border color|√|√|
|ButtonBorderColor|MessageBoxColor|Button Border Color|√|√|
|WndBorderThickness|MessageBoxColor|Window border width|√|√|
|TitlePanelBorderThickness|Thickness|Title Area Border Width|√|√|
|MessagePanelBorderThickness|Thickness|Message area border width|√|√|
|ButtonPanelBorderThickness|Thickness|Button area border width|√|√|
|ButtonBorderThickness|Thickness|Button Border Width|√|√|
|WindowShowDuration|Duration|Window fade time|√|√|
|WindowShowAnimations|List&lt;KeyValuePair&lt;DependencyProperty, AnimationTimeline&gt;&gt;|Window display animation |√|√|
|WindowCloseAnimations|List&lt;KeyValuePair&lt;DependencyProperty, AnimationTimeline&gt;&gt;|Window Close Animation|√|√|
|CloseIcon|BitmapImage|Custom Close Icon|√|√|
|WarningIcon|BitmapImage|Custom warning icon|√|√|
|ErrorIcon|BitmapImage|Custom error icon|√|√|
|InfoIcon|BitmapImage|Custom information icon|√|√|
|QuestionIcon|BitmapImage|Custom Question Icon|√|√|
|EnableCloseButton|bool|Application window close button|√|√|
|ButtonStyleList|List&lt;Style&gt;|Button Action Style|√|√|
|CloseTimer|MessageBoxCloseTimer|Window timing / Close now|√|√|
|MessageBoxImageType|MessageBoxImage|Set the type of icon displayed|√|√|

|MessageBox function|Meaning|Parameter|Return value|Is Static|
|----|----|----|----|----|
|Show (string, string, MessageBoxButton, MessageBoxImage)|Call up the message window in compatible form|Message, Title (optional), Button type (optional), Icon type (optional)|MessageBoxResult|√|
|Show(List&lt;object&gt;, string, string, MessageBoxImage)|Customize the message window|Button list, message, title (optional), icon type (optional)|int|√|
|Show (PropertiesSetter, string, string, MessageBoxButton, MessageBoxImage)|Call the message window in a compatible form and use the existing style|style, message, title (optional), button type (optional), icon type (optional)|MessageBoxResult|√|
|Show(PropertiesSetter, List&lt;object&gt;, string, string, MessageBoxImage)|Customize the message window, and use the existing style|style, style, button list, message, title (optional), icon type (optional)|int|√|
 
|MessageBoxColor Property|Meaning|Type|
|----|----|----|
|color|Color value|object|
|colorType|Color Type|ColorType|

|MessageBoxColor function|Meaning|Parameter|Return value|Is Static|
|----|----|----|----|----|
|MessageBoxColor (object)|Constructor|Hex color code string or instance of Color class or color name string||×|
|MessageBoxColor (object, ColorType)|Constructor|Hexadecimal color code string or instance of Color class or color name string, ColorType enumeration value||×|
|GetSolidColorBrush ()|Output the SolidColorBrush corresponding to this instance color instance||SolidColorBrush|×|

|MessageBoxCloseTimer Property|Meaning|Type|
|----|----|----|
|timeSpan|Time to close window|TimeSpan|
|result|int|The return value returned after the window is closed|

|MessageBoxCloseTimer function|Meaning|Parameter|Return Value|Is Static|
|----|----|----|----|----|
|MessageBoxCloseTimer (TimeSpan, int)|Constructor|TimeSpan instance (time to close), return value returned after window close||×|
|MessageBoxCloseTimer (int, int)|Constructor|seconds to close, return value after window close||×|
|CloseNow ()|Close window immediately|||×|

|PropertiesSetter Properties|Meaning|
|----|----|
|Omitted (Refer to the MessageBox property)||

|PropertiesSetter Function|Meaning|Parameter|Return Value|Is Static|
|----|----|----|----|----|
|PropertiesSetter ()|Constructor|||×|
|PropertiesSetter (PropertiesSetter)|Constructor|An existing PropertiesSetter instance||×|

|ButtonSpacer Properties|Meaning|Type|
|----|----|----|
|length|Leave blank length|double|

|ButtonSpacer function|Meaning|Parameter|Return value|Is Static|
|----|----|----|----|----|
|ButtonSpacer ()|Constructor|||×|
|ButtonSpacer (double)|Constructor|Leave length||×|
|GetLength ()|Get the blank length||double|×|
