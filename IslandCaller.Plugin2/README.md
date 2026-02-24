# 插件自述文件

本文件会在插件市场上显示。在安装插件后，本自述文件也会在【应用设置】->【插件】页面中显示。因此，本文件也将会是用户了解你的插件功能的重要途径，建议好好写 README。

**注意事项：**

- 嵌入图片时请使用网络图床。
- 支持在这里直接调用 ClassIsland 内部的 Uri，例如[classisland://app/test/](classisland://app/test/)。
- 本文件一般会在 ClassIsland 内置的 Markdown 渲染器（基于 [MdXaml](https://github.com/whistyun/MdXaml)）中渲染，仅支持部分 Markdown 语法。

***

**支持的 Markdown 语法：**

> 本示例魔改自 [Leanote 博客](http://leanote.leanote.com/post/markdown-source-code)。

# Welcome to ClassIsland! 欢迎来到ClassIsland!
 
## 1. 排版
 
**粗体** *斜体* 
 
~~这是一段错误的文本。~~
 
引用:
 
> 123123123123
 
有充列表:
 1. 支持Vim
 2. 支持Emacs
 
无序列表:
 
 - 项目1
 - 项目2
 
 
## 2. 图片与链接
 
网络图片:
![Banner](https://github.com/user-attachments/assets/a815dd7d-8343-4da5-aee4-3f754aa297e4)

WPF 资源图片：

![1690356161339](pack://application:,,,/ClassIsland;component/Assets/AppLogo.png)

链接:
 
[ClassIsland 官网](http://classisland.tech)
 
## 3. 标题
 
以下是各级标题, 最多支持5级标题
 
```
# h1
## h2
### h3
#### h4
##### h4
###### h5
```
 
## 4. 代码
 
示例:
 
    function get(key) {
        return m[key];
    }
    
代码高亮示例:
 
``` javascript
/**
* nth element in the fibonacci series.
* @param n >= 0
* @return the nth element, >= 0.
*/
function fib(n) {
  var a = 1, b = 1;
  var tmp;
  while (--n >= 0) {
    tmp = a;
    a += b;
    b = tmp;
  }
  return a;
}
 
document.write(fib(10));
```
 
```python
class Employee:
   empCount = 0
 
   def __init__(self, name, salary):
        self.name = name
        self.salary = salary
        Employee.empCount += 1
```
 
# 5. Markdown 扩展
 
Markdown 扩展支持:
 
* 表格
 
## 5.1 表格
 
Item     | Value
-------- | ---
Computer | \$1600
Phone    | \$12
Pipe     | \$1
 
