
# S7 Server 通讯模拟器

*S7SvrSim* 是一个西门子 `S7`系列的`PLC`通讯模拟器。

## 这个程序是做什么的？

之所以有这样一个小项目，主要是为了缓解上位机开发人员想随时测试和`S7`系列PLC通讯的尴尬：

- 手头没有特定型号的物理`PLC`
- 没钱购买博图，又不屑于使用盗版
- 有钱购买博图，但是这个软件超大，还会自动安装了一堆WinCC/SQL Server之类服务。偏偏自己对电脑还有洁癖。
- 有钱买了博图，然后废了老大劲安装好了，却拿不到电气部门的`PLC`程序。

回过头来想，上位机开发人员真的需要物理PLC或者博图吗？我们只需要一个`PLC`**通讯模拟器**而已！我们甚至不需要模拟完整的`PLC`程序，我们仅仅只需要根据和电气部门的通讯规约操作点表！对，只需要**模拟通讯**。

(当然，如果上面所列的场景不适合你，那么你没必要用这个程序 :))

## 这个程序不能干什么

我不打算、也没能力做一个完整的软`PLC`。像画各种梯形图、执行ST程序这种事，压根不是这个项目的目标。要是真有这种需求，还是去购买倍福之类的商用产品吧。

## 操作方式

*S7SvcSim* 支持单步操作和基于`Python`脚本的批量操作。其中，单步操作主要是为了方便手动调试；而基于`Python`脚本，我们可以为上位机程序编写自动化测试。

### Python 脚本工作方式

用户可以自定义一个`Python`脚本，然后在程序运行后导入。一旦导入，该脚本就会被立刻执行。为了操作模拟器，我向`Python`脚本暴露了一个预定义的`s7_server_svc`对象(类型为`S7ServerService`)，用于对当前正在运行的`S7` PLC模拟器进行操作。:
- 启动服务器
- 关闭服务器
- 读/写单个String
- 读/写单个Bit
- 读/写单个Byte
- 读/写单个Short
- 读/写单个浮点数
- 其它

通过这些API方法，我们可以通过编写`Python`脚本来动态执行一系列操作，比如：

```python
s7_server_svc.WriteString(6, 2780, "1908WC16V299F6+YSTC1100139+L2/L3:1757;L1/N:1762;")
s7_server_svc.WriteReal(6, 3036, 3545.2)
s7_server_svc.WriteReal(6, 3040, 68.3)
s7_server_svc.WriteReal(6, 3040, 4.8)
```

## 已知问题

- [ ] 目前由于[`IronPython`](https://ironpython.net/)的限制，发布时无法生成单个文件。参见[IronPython #762](https://github.com/IronLanguages/ironpython2/issues/762)。故发布项目时请不要使用`-p:PublishSingleFile=true`
- [ ] 由于这个问题，目前没法使用`ClickOnce`发布，参见 [#44602](https://github.com/dotnet/runtime/issues/44602#issuecomment-726472185)