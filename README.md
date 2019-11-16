# MessageBoxTouch
 ![WTFPL](http://www.wtfpl.net/wp-content/uploads/2012/12/wtfpl-badge-1.png)<br />
 因为系统MessageBox按钮和字体太小, 所以自己写了个方便更改外观的MessageBox. 有什么好玩的功能也可以扩展上去. <br />
### 特性
- 因为可以改变字体且支持触摸操作, 所以适合在平板上使用. 
- 消息框最初有一定高度, 但如果Message文本内容过多, 超出了高度限制, MessageBox显示不下时, 窗口高度会对应增加, 使消息能被完整显示出来. 
- 如果消息框窗口高度达到窗口工作区高度, 但还是容不下消息字符串显示时, 消息框高度不再增长, 但是可以通过滚动消息区域查看剩余消息. 
### 用法
- 生成项目MessageBoxTouch, 得到对应动态链接库, 引用到自己的项目中, 并调用. 本工程自带有示例. 
- 工具的MessageBox的Show函数参数与System.Windows.MessageBox的参数兼容, 简单而言只要引入一下dll, 不用做过多改动即可使用工具提供的MessageBox. 
- 工具的MessageBox.Show函数还有与系统函数不同的重载, 可以实现更多功能. 例如自定义消息框按钮. 
- 可以通过设置属性改变MessageBox的外观, 例如窗口各部分的字体, 透明度, 窗口大小等. 
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
MessageBox.Show(new List<string> { "btn1" }, "msg");
MessageBox.Show(new List<string> { "btn1", "btn2", "btn3", "btn4", "btn5" }, "msg", "title", MessageBoxImage.Asterisk);
```
#### 修改式样
##### 单独修改
添加在Show函数之前
```csharp
MessageBox.ButtonPanelColor = new MessageBoxColor("red");
MessageBox.WindowMinHeight = 300;
MessageBox.MessageFontSize = 22;
MessageBox.Show(new List<string> { "btn1" }, "msg");
```
##### 批量修改
- **推荐** 在Show函数参数中设置
```csharp
PropertiesSetter propertiesSetter = new PropertiesSetter();
propertiesSetter.ButtonBorderThickness = new Thickness(10);
propertiesSetter.MessagePanelColor = new MessageBoxColor(Colors.black);
MessageBox.Show(propertiesSetter, "message", "title", MessageBoxButton.OKCancel, MessageBoxImage.Question);
MessageBox.Show(propertiesSetter, new List<string> { "btn1" }, "msg");
```
- 在Show函数之前设置
```csharp
PropertiesSetter propertiesSetter = new PropertiesSetter();
propertiesSetter.ButtonBorderThickness = new Thickness(10, 0, 0, 0);
propertiesSetter.MessagePanelColor = new MessageBoxColor("#222DDD");
MessageBox.PropertiesSetter = propertiesSetter;
MessageBox.Show(new List<string> { "btn1" }, "msg");
```
