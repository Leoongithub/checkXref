# checkXref
检查dwg文件是否有外部参照，无需打开AutoCAD

## 背景
最近又因为发图的时候忘记绑定底图被喷了一顿，于是想做一个批量检查dwg文件是否有外部参照的工具。如果是在AutoCAD内，解决方法是比较多的，现成的方案有[DBXscanXref](https://www.cadforum.cz/en/qaID.asp?tip=6598)，它可以将指定目录下的所有dwg文件（包括子目录）的参照情况导出成txt文件；或[这个lsp脚本](https://forums.augi.com/showthread.php?152990-Listing-all-xrefs-attached-to-a-group-of-(unopened)-drawings-to-xls-file&p=1250384&viewfull=1#post1250384)，它会将同一个目录下所有的dwg文件（不含子目录）的参照情况导出为csv文件。

这些工具虽然不用打开dwg文件本身，但还是需要在AutoCAD里运行。那么有没有无需打开AutoCAD的方案呢？可以用Autodesk参照管理器（AdRefMan），它可以查看dwg文件的外部参照，此外也包括引用的打印配置、字体等；另外还有accoreconsole，可以当成一个命令行版的AutoCAD，写一个列出参照的scr脚本，用它在指定的dwg文件上运行也可以实现需求。这两个工具都是AutoCAD自带的，但个人感觉都不够好用。

搜索之下发现了这篇[博文](http://www.cppblog.com/mzty/archive/2006/06/21/8792.html)，看描述基本上能解决我的问题。虽然没写过c#，也没接触过AutoCAD二次开发，但该文中的代码已经非常接近我的需求，[稍微修改一下](https://github.com/Leoongithub/checkXref/commit/b51629479fd8a9cbb00cf0c7b6916f183f1b9714)代码就可以实现想要的效果了。

## 编译与运行
### 环境需求

- AutoCAD
  + 虽然只在2016版上测试过，但只要不是太旧的版本应该不会有问题
  + 即便无需运行AutoCAD，但还是得安装它
- .Net Framework 4.0以上
  + 在Win10或者更新的系统里应该是自带的

一般来说既然需要这个工具，那你应该无需特意配置，这些都是已经有的。

### 编译
1. 新建一个文件夹，把checkXref.cs放进去
2. 以我的环境为例，AutoCAD 2016的安装路径是`C:\Program Files\Autodesk\AutoCAD 2016`，把此路径下的`acdbmgd.dll`复制到刚才的文件夹
3. .Net Framework的路径是`C:\Windows\Microsoft.NET\Framework64\v4.0.30319`，该目录下有一个`csc.exe`是C#编译器，等下要用到
4. 在刚才新建的文件夹下打开cmd，运行`C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /r:acdbmgd.dll checkXref.cs`，没有提示error的话就编译成功了，会在该目录下得到`checkXref.exe`

### 运行
虽然编译成功了，但现在还不能直接运行，因为该程序运行时除了`acdbmgd.dll`，还需要许多其他AutoCAD所带的dll文件，把它们全部复制过来不太现实，也没必要。虽然尝试过从源代码下手，但都没有成功，所以只能在程序外做一些额外的设置。具体的解决方法有如下几种，选择其中之一使用即可：
1. 将AutoCAD的安装路径添加到path环境变量中，如果不确定环境变量修改后是否生效可重启计算机
2. 通过其他程序引导checkXref启动，并将工作目录（working directory）设置为AutoCAD的安装路径
3. 最简单粗暴的方法，直接把`checkXref.exe`放到AutoCAD的安装路径下

需要注意的是使用方法1或2时，`acdbmgd.dll`仍然需要复制一份放在`checkXref.exe`的同目录下。

## 使用
`checkXref filename1 filename2 ...`

路径中有空格的文件需要用双引号包裹，所有的文件都不能被打开，否则会导致程序出错。

例如`D:\example`下有三个文件，`Drawing1.dwg`、`Drawing2.dwg`、`Drawing3.dwg`，其中`Drawing2.dwg`参照了`Drawing3.dwg`，那么运行  
`checkXref.exe D:\example\Drawing1.dwg d:\example\Drawing2.dwg`  
就会得到如下输出
```
D:\example\Drawing1.dwg
        无外部参照

d:\example\Drawing2.dwg
        Drawing3        D:\example\Drawing3.dwg

```
程序的返回值为有外部参照的文件数量，此情况下是1。

输出的格式可以根据自己的需求修改代码，只要对任何程序语言稍微有一定的编程基础应该都不困难。
