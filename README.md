# MessageBoxTouch
 ![WTFPL](http://www.wtfpl.net/wp-content/uploads/2012/12/wtfpl-badge-1.png)<br />
 因为系统MessageBox按钮和字体太小, 所以自己写了个方便更改外观的MessageBox. 有什么好玩的功能也可以扩展上去. 下有演示图. <br />
### 特性
- 单例模式运行, 运行时无法操作父窗口. 
- 因为可以改变字体且支持触摸操作, 所以适合在平板上使用. 
- 消息框最初有一定高度, 但如果Message文本内容过多, 超出了高度限制, MessageBox显示不下时, 窗口高度会对应增加, 使消息能被完整显示出来. 
- 如果消息框窗口高度达到窗口工作区高度, 但还是容不下消息字符串显示时, 消息框高度不再增长, 但是可以通过滚动消息区域查看剩余消息. 
- 可以自由改变外观. 
- 可在按钮区域插入自定义控件 (如输入框, 另一个按钮, 进度条, 等等). 调用后可获取用户对插入的自定控件的操作结果. 
- 可以在运行时改变消息框内容与样式. 
### 用法
- 生成项目MessageBoxTouch, 得到对应动态链接库, 引用到自己的项目中, 并调用. 本工程自带有示例. 
- 工具的MessageBox的Show函数参数与System.Windows.MessageBox的参数兼容, 简单而言只要引入一下dll, 不用做过多改动即可使用工具提供的MessageBox. 
- 工具的MessageBox.Show函数还有与系统函数不同的重载, 可以实现更多功能. 例如自定义消息框按钮. 
- 可以通过设置属性改变MessageBox的外观, 例如窗口各部分的字体, 透明度, 背景, 边框, 窗口大小, 限制高度增长, 设置窗口宽度等等. 
### 图片示例
![示例](https://www.iaders.com/wp-content/uploads/2019/12/mb-1.gif "粗略做了两种样式")
### 代码示例
#### 兼容写法
返回值为MessageBoxResult型. 
```csharp
MessageBox.Show("message");
MessageBox.Show("message", "title", MessageBoxButton.OKCancel, MessageBoxImage.Question);
```
#### 自定义写法
返回值为int型, 值为参数按钮列表中的索引. 
```csharp
MessageBox.Show(new List<object> { "btn1" }, "msg");
MessageBox.Show(new List<object> { new ButtonSpacer(250), "btn1", "btn2", "btn3", "btn4", "btn5", new ButtonSpacer(30) }, "msg", "title", MessageBoxImage.Asterisk);
```
#### 修改式样属性
##### 单独修改
- 在调用Show函数前设置
```csharp
MessageBox.ButtonPanelColor = new MessageBoxColor("red");
MessageBox.WindowMinHeight = 300;
MessageBox.MessageFontSize = 22;
MessageBox.Show(new List<object> { "btn1" }, "msg");
```
##### 批量修改
- 事先设定PropertiesSetter
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
- 使用PropertiesSetter
1. **推荐** 在Show函数参数中设置
```csharp
MessageBox.Show(ps0, "message", "title", MessageBoxButton.OKCancel, MessageBoxImage.Question);
MessageBox.Show(ps1, new List<object> { "btn1" }, "msg");
```
2. 在调用Show函数前设置
```csharp
MessageBox.PropertiesSetter = ps1;
MessageBox.Show(new List<object> { new TextBox(), "btn1" }, "msg");
```
#### 修改按钮区域
##### 插入空白
在传入的List&lt;object&gt;中对应位置插入一个ButtonSpacer实例. 构造函数参数可为空或指定宽度值. </br>
##### 插入自定义控件
在传入的List&lt;object&gt;中对应位置插入插入一个FrameworkElement派生类的实例, 即可在对应位置显示相应控件. </br>
列宽由插入的控件的宽度决定. </br>
- Show函数调用结束返回后可再次获取该控件, 得到用户输入 / 操作结果.
```csharp
int result = MessageBox.Show(new List<object> { new TextBox(), "btn1", "btn2" }, "msg");
TextBox tb = (TextBox)MessageBox.ButtonList[0];
MessageBox.Show(tb.Text == string.Empty ? "用户未输入" : tb.Text, (string)MessageBox.ButtonList[result]);
```
### 可操作的属性
|属性|状态|
|----|----|
|窗口标题 / 消息 / 按钮|已实装|
|窗口锁高|已实装|
|窗口初始高度与宽度|已实装|
|消息区域换行风格|已实装|
|各区域文本字体|已实装|
|各区域文本大小|已实装|
|各区域文本颜色|已实装|
|各区域透明度|已实装|
|各区域背景色|已实装|
|各区域边框颜色|已实装|
|各区域边框宽度|已实装|
|窗口渐显时间|已实装|
|自定窗口打开与关闭动画|已实装|
|自定义显示图标类型|已实装|
|自定义图标|已实装|
|是否应用窗口关闭按钮|已实装|
|按钮动作样式|已实装|
|窗口计时 / 立即关闭|已实装|
### 成员函数与属性
|MessageBox属性|类型|含义|静态|状态|
|----|----|----|----|----|
|TitleText|string|设置 / 获取标题文字|是|已实装|
|MessageText|string|设置 / 获取消息文字|是|已实装|
|ButtonList|List&lt;object&gt;|设置 / 获取按钮列表|是|已实装|
|LockHeight|bool|是否锁住窗口高度不允许自动增长|是|已实装|
|TextWrappingMode|TextWrapping|消息段落换行风格|是|已实装|
|WindowWidth|double|窗口宽度|是|已实装|
|WindowMinHeight|double|窗口最小 (初始) 高度|是|已实装|
|TitleFontFamily|FontFamily|标题文本字体|是|已实装|
|MessageFontFamily|FontFamily|消息文本字体|是|已实装|
|ButtonFontFamily|FontFamily|按钮文本字体|是|已实装|
|TitleFontSize|int|标题文本大小|是|已实装|
|MessageFontSize|int|消息文本大小|是|已实装|
|ButtonFontSize|int|按钮文本大小|是|已实装|
|TitleFontColor|MessageBoxColor|标题文本颜色|是|已实装|
|MessageFontColor|MessageBoxColor|消息文本颜色|是|已实装|
|ButtonFontColor|MessageBoxColor|按钮文本颜色|是|已实装|
|WindowOpacity|double|窗口整体透明度|是|已实装|
|TitleBarOpacity|double|标题区域透明度|是|已实装|
|MessageBarOpacity|double|消息区域透明度|是|已实装|
|ButtonBarOpacity|double|按钮区域透明度|是|已实装|
|TitlePanelColor|MessageBoxColor|标题区域背景色|是|已实装|
|MessagePanelColor|MessageBoxColor|消息区域背景色|是|已实装|
|ButtonPanelColor|MessageBoxColor|按钮区域背景色|是|已实装|
|WndBorderColor|MessageBoxColor|窗口边框颜色|是|已实装|
|TitlePanelBorderColor|MessageBoxColor|标题区域边框颜色|是|已实装|
|MessagePanelBorderColor|MessageBoxColor|消息区域边框颜色|是|已实装|
|ButtonPanelBorderColor|MessageBoxColor|按钮区域边框颜色|是|已实装|
|ButtonBorderColor|MessageBoxColor|按钮边框颜色|是|已实装|
|WndBorderThickness|MessageBoxColor|窗口边框宽度|是|已实装|
|TitlePanelBorderThickness|Thickness|标题区域边框宽度|是|已实装|
|MessagePanelBorderThickness|Thickness|消息区域边框宽度|是|已实装|
|ButtonPanelBorderThickness|Thickness|按钮区域边框宽度|是|已实装|
|ButtonBorderThickness|Thickness|按钮边框宽度|是|已实装|
|WindowShowDuration|Duration|窗口渐显时间|是|已实装|
|WindowShowAnimations|List&lt;KeyValuePair&lt;DependencyProperty, AnimationTimeline&gt;&gt;|窗口显示动画|是|已实装|
|WindowCloseAnimations|List&lt;KeyValuePair&lt;DependencyProperty, AnimationTimeline&gt;&gt;|窗口关闭动画|是|已实装|
|CloseIcon|BitmapImage|自定义关闭图标|是|已实装|
|WarningIcon|BitmapImage|自定义警告图标|是|已实装|
|ErrorIcon|BitmapImage|自定义错误图标|是|已实装|
|InfoIcon|BitmapImage|自定义信息图标|是|已实装|
|QuestionIcon|BitmapImage|自定义问题图标|是|已实装|
|EnableCloseButton|bool|应用窗口关闭按钮|是|已实装|
|ButtonStyleList|List&lt;Style&gt;|按钮动作样式|是|已实装|
|CloseTimer|MessageBoxCloseTimer|窗口计时 / 立即关闭|是|已实装|
|MessageBoxImageType|MessageBoxImage|设定显示的图标类型|是|已实装|

|MessageBox函数|含义|参数|返回值|静态|
|----|----|----|----|----|
|Show(string, string, MessageBoxButton, MessageBoxImage)|兼容形式调出消息窗口|消息, 标题 (选), 按钮类型 (选), 图标类型 (选)|MessageBoxResult|是|
|Show(List&lt;object&gt;, string, string, MessageBoxImage)|自定义形式调出消息窗口|按钮列表, 消息, 标题 (选), 图标类型 (选)|int|是|
|Show(PropertiesSetter, string, string, MessageBoxButton, MessageBoxImage)|兼容形式调出消息窗口, 并使用既有样式|样式, 消息, 标题 (选), 按钮类型 (选), 图标类型 (选)|MessageBoxResult|是|
|Show(PropertiesSetter, List&lt;object&gt;, string, string, MessageBoxImage)|自定义形式调出消息窗口, 并使用既有样式|样式, 按钮列表, 消息, 标题 (选), 图标类型 (选)|int|是|
 
|MessageBoxColor属性|含义|类型|
|----|----|----|
|color|颜色值|object|
|colorType|颜色类型|ColorType|
 
|MessageBoxColor函数|含义|参数|返回值|静态|
|----|----|----|----|----|
|MessageBoxColor(object)|构造函数|十六进制颜色码字符串或者Color类的实例或颜色名字符串||否|
|MessageBoxColor(object, ColorType)|构造函数|十六进制颜色码字符串或者Color类的实例或颜色名字符串, ColorType枚举值||否|
|GetSolidColorBrush()|输出这个实例颜色实例对应的SolidColorBrush||SolidColorBrush|否|

|MessageBoxCloseTimer属性|含义|类型|
|----|----|----|
|timeSpan|距窗口关闭的时间|TimeSpan|
|result|int|窗口关闭后返回的返回值|

|MessageBoxCloseTimer函数|含义|参数|返回值|静态|
|----|----|----|----|----|
|MessageBoxCloseTimer(TimeSpan, int)|构造函数|TimeSpan实例 (距关闭的时间), 窗口关闭后返回的返回值||否|
|MessageBoxCloseTimer(int, int)|构造函数|距关闭的秒数, 窗口关闭后返回的返回值||否|
|CloseNow()|立即关闭窗口|||否|

|PropertiesSetter属性|含义|
|----|----|
|略 (参考MessageBox属性)||

|PropertiesSetter函数|含义|参数|返回值|静态|
|----|----|----|----|----|
|PropertiesSetter()|构造函数|||否|
|PropertiesSetter(PropertiesSetter)|构造函数|一个既有的PropertiesSetter实例||否|

|ButtonSpacer属性|含义|类型|
|----|----|----|
|length|留白长度|double|

|ButtonSpacer函数|含义|参数|返回值|静态|
|----|----|----|----|----|
|ButtonSpacer()|构造函数|||否|
|ButtonSpacer(double)|构造函数|留白长度||否|
|GetLength()|获取留白长度||double|否|
