# MessageBoxTouch
 ![WTFPL](http://www.wtfpl.net/wp-content/uploads/2012/12/wtfpl-badge-1.png)<br />
 因为系统MessageBox按钮和字体太小, 所以自己写了个方便更改外观的MessageBox. 有什么好玩的功能也可以扩展上去. 下有截图. <br />
### 特性
- 因为可以改变字体且支持触摸操作, 所以适合在平板上使用. 
- 消息框最初有一定高度, 但如果Message文本内容过多, 超出了高度限制, MessageBox显示不下时, 窗口高度会对应增加, 使消息能被完整显示出来. 
- 如果消息框窗口高度达到窗口工作区高度, 但还是容不下消息字符串显示时, 消息框高度不再增长, 但是可以通过滚动消息区域查看剩余消息. 
- 可以自由改变外观. 
- 可在按钮区域插入自定义控件 (如输入框, 另一个按钮, 等等). 调用后可获取用户对插入的自定控件的操作结果. 
### 用法
- 生成项目MessageBoxTouch, 得到对应动态链接库, 引用到自己的项目中, 并调用. 本工程自带有示例. 
- 工具的MessageBox的Show函数参数与System.Windows.MessageBox的参数兼容, 简单而言只要引入一下dll, 不用做过多改动即可使用工具提供的MessageBox. 
- 工具的MessageBox.Show函数还有与系统函数不同的重载, 可以实现更多功能. 例如自定义消息框按钮. 
- 可以通过设置属性改变MessageBox的外观, 例如窗口各部分的字体, 透明度, 背景, 边框, 窗口大小, 限制高度增长, 设置窗口宽度等等. 
### 示例
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
添加在Show函数之前
```csharp
MessageBox.ButtonPanelColor = new MessageBoxColor("red");
MessageBox.WindowMinHeight = 300;
MessageBox.MessageFontSize = 22;
MessageBox.Show(new List<object> { "btn1" }, "msg");
```
##### 批量修改
- **推荐** 在Show函数参数中设置
```csharp
PropertiesSetter propertiesSetter = new PropertiesSetter();
propertiesSetter.ButtonBorderThickness = new Thickness(10);
propertiesSetter.MessagePanelColor = new MessageBoxColor(Colors.black);
MessageBox.Show(propertiesSetter, "message", "title", MessageBoxButton.OKCancel, MessageBoxImage.Question);
MessageBox.Show(propertiesSetter, new List<object> { "btn1" }, "msg");
```
- 在Show函数之前设置
```csharp
PropertiesSetter propertiesSetter = new PropertiesSetter();
propertiesSetter.ButtonBorderThickness = new Thickness(10, 0, 0, 0);
propertiesSetter.MessagePanelColor = new MessageBoxColor("#222DDD");
MessageBox.PropertiesSetter = propertiesSetter;
MessageBox.Show(new List<object> { new TextBox(), "btn1" }, "msg");
```
#### 修改按钮区域
##### 插入空白
在传入的List<object>中对应位置插入一个ButtonSpacer实例. 构造函数参数可为空或指定宽度值. </br>
##### 插入自定义控件
在传入的List<object>中对应位置插入插入一个FrameworkElement派生类的实例, 即可在对应位置显示相应控件. </br>
列宽由插入的控件的宽度决定. </br>
- Show函数调用结束返回后可调用MessageBox.GetBtnList()获取该控件, 得到用户输入 / 操作结果.
```csharp
int result = MessageBox.Show(new List<object> { new TextBox(), "btn1", "btn2" }, "msg");
TextBox tb = (TextBox)MessageBox.GetBtnList()[0];
MessageBox.Show(tb.Text == string.Empty ? "用户未输入" : tb.Text, (string)MessageBox.GetBtnList()[result]);
```
 
### 成员函数与属性
|MessageBox属性|含义|状态|
|----|----|----|
|LockHeight|是否锁住窗口高度不允许自动增长||
|TextWrappingMode|消息段落换行风格|未实现自定义切换|
|WindowWidth|窗口宽度||
|WindowMinHeight|窗口最小 (初始) 高度||
|TitleFontSize|标题字体大小||
|MessageFontSize|消息字体大小||
|ButtonFontSize|按钮字体大小||
|TitleFontColor|标题字体颜色||
|MessageFontColor|消息字体颜色||
|ButtonFontColor|按钮字体颜色||
|WindowOpacity|窗口整体透明度|未实现自定义切换|
|TitleBarOpacity|标题区域透明度|未实现自定义切换|
|MessageBarOpacity|消息区域透明度|未实现自定义切换|
|ButtonBarOpacity|按钮区域透明度|未实现自定义切换|
|TitlePanelColor|标题区域背景色||
|MessagePanelColor|消息区域背景色||
|ButtonPanelColor|按钮区域背景色||
|WndBorderColor|窗口边框颜色||
|TitlePanelBorderColor|标题区域边框颜色||
|MessagePanelBorderColor|消息区域边框颜色||
|ButtonPanelBorderColor|按钮区域边框颜色||
|ButtonBorderColor|按钮边框颜色||
|WndBorderThickness|窗口边框宽度||
|TitlePanelBorderThickness|标题区域边框宽度||
|MessagePanelBorderThickness|消息区域边框宽度||
|ButtonPanelBorderThickness|按钮区域边框宽度||
|ButtonBorderThickness|按钮边框宽度||

|MessageBox函数|含义|
|----|----|
|public static MessageBoxResult Show(string msg, string title = "", MessageBoxButton selectStyle = MessageBoxButton.OK, MessageBoxImage img = MessageBoxImage.None)|兼容形式调出消息窗口|
|public static int Show(List<object> btnList, string msg, string title = "", MessageBoxImage img = MessageBoxImage.None)|自定义形式调出消息窗口|
|public static MessageBoxResult Show(PropertiesSetter propertiesSetter, string msg, string title = "", MessageBoxButton selectStyle = MessageBoxButton.OK, MessageBoxImage img = MessageBoxImage.None)|兼容形式调出消息窗口, 并使用既有样式|
|public static int Show(PropertiesSetter propertiesSetter, List<object> btnList, string msg, string title = "", MessageBoxImage img = MessageBoxImage.None)|自定义形式调出消息窗口, 并使用既有样式|
|public static List<object> GetBtnList()|获取按钮列表|
 
|MessageBoxColor属性|含义|
|----|----|
|color|颜色值|
|colorType|颜色类型|
 
|MessageBoxColor函数|含义|参数|
|----|----|----|
|public MessageBoxColor(object color)|构造函数|ColorType类实例, 或十六进制颜色码, 或颜色名字|
|public SolidColorBrush GetSolidColorBrush()|输出这个实例颜色实例对应的SolidColorBrush||

|PropertiesSetter属性|含义|
|----|----|
|略 (参考MessageBox属性)||

|PropertiesSetter函数|含义|参数|
|----|----|----|
|public PropertiesSetter()|构造函数||
|public PropertiesSetter(PropertiesSetter propertiesSetter)|构造函数|一个既有的PropertiesSetter实例|

|ButtonSpacer属性|含义|
|----|----|
|length|留白长度|

|ButtonSpacer函数|含义|
|----|----|
|public ButtonSpacer()|构造函数|
|public ButtonSpacer(double length)|构造函数|
|public double GetLength()|获取留白长度|

### 示例图片
![样式1](https://www.iaders.com/wp-content/uploads/2019/12/mb1.png "样式1")
![样式1](https://www.iaders.com/wp-content/uploads/2019/12/mb5.png "样式1")
![样式1](https://www.iaders.com/wp-content/uploads/2019/12/mb6.png "样式1")
![样式2](https://www.iaders.com/wp-content/uploads/2019/12/mb4.png "样式2")
![样式2](https://www.iaders.com/wp-content/uploads/2019/12/mb2.png "样式2")
![样式2](https://www.iaders.com/wp-content/uploads/2019/12/mb3.png "样式2")
