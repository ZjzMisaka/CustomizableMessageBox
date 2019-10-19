# MessageBoxTouch
 ![WTFPL](http://www.wtfpl.net/wp-content/uploads/2012/12/wtfpl-badge-1.png)<br />
 触控友好, 兼容System.Windows.MessageBox调用参数的消息类)<br />
### 特性
- 因为可以改变字体且支持触摸操作, 所以适合在平板上使用. 
- 消息框最初有一定高度, 但如果Message文本内容过多, 超出了高度限制, MessageBox显示不下时, 窗口高度会对应增加, 使消息能被完整显示出来. 
- 如果消息框窗口高度达到窗口工作区高度, 但还是容不下消息字符串显示时, 消息框高度不再增长, 但是可以通过滚动消息区域查看剩余消息. 
### 用法
- 工具的MessageBox的Show函数参数与System.Windows.MessageBox的参数兼容, 简单而言只要引入一下dll, 不用做过多改动即可使用工具提供的MessageBox. 
- 工具的MessageBox.Show函数还有与系统函数不同的重载, 可以实现更多功能. 例如自定义消息框按钮. 
- 可以通过设置属性改变MessageBox的外观, 例如窗口各部分的字体, 透明度, 窗口大小等. 
### 示例
#### 兼容写法
```csharp
MessageBox.Show("message");
MessageBox.Show("message", "title", MessageBoxButton.OKCancel, MessageBoxImage.Question);
```
#### 自定义写法
```csharp
MessageBox.Show(new List<string> { "btn1" }, "msg");
MessageBox.Show(new List<string> { "btn1", "btn2", "btn3", "btn4", "btn5" }, "msg", "title", MessageBoxImage.Asterisk);
```
