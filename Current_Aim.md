# 将SmartTank项目需要实现的目标写在该文档中。如有人对其中的一个或多个部分感兴趣，并希望加入到编写之中来，可以联系我（wufei\_spring\_c@163.com QQ:298210841）。需要讨论之处可在Wiki中新建页面，或者在SmartTank的QQ群21597580中讨论。 #

## _2008-3-10：_ ##

SmartTank当前的主要任务是构建地图编辑器。


## _2007-12-6：_ ##

**本阶段实现了以下内容：**

一	对AI漫游场景的支持基本建成，为AI编写者提供了导航图、警戒线、物体边界线以及一个用A\*算法计算路径的通用方法的支持。

二	利用平台提供寻路辅助进行寻路的示例AI也同时完成。

三	场景物体编辑器构造完成，并将贴图预处理的功能包含在了场景物体编辑器中。

**接下来的目标：**

一	以前制定的任务中没完成的继续积累。

二	需要对场景管理进行重新的设计。

三	构建地图编辑器。

附地图编辑器用例图：
![http://photo14.yupoo.com/20071207/092457_1369072793_swfdyyck.jpg](http://photo14.yupoo.com/20071207/092457_1369072793_swfdyyck.jpg)

## _2007-11-25：近期目标如下_ ##

**一	逻辑部分：**

I	**_（已完成）_**继续完善对AI漫游场景的支持，当前已经可以根据坦克看到过的场景物体得到导航图。需要在构造一个寻路AI的过程中分析仍需要何种支持。

II	**_（已完成）_**写一个利用导航图进行寻路的AI。

III	为场景物体添加可被摧毁的接口。


**二	游戏界面部分，游戏界面需要得到效果上的提升，可能需要包括以下一些方面：**

I	整理GameBase.UI中的UI控件的代码。使这些控件的可调节性更佳，更易于使用。（注：可以根据实际情况决定是否放弃这些代码或者重写这些代码。如果这样，则与下一条合并。）

II	设计游戏的主界面、规则选择界面，以及具体游戏规则的界面。同时构造一些界面元素，如界面中的边框，界面中的导航控件，界面中的对话框等。这些界面元素的设计应生动，美观，并尽可能满足重用性的要求，能够在以后的界面编写中以及他方编写规则类的时候重用。界面元素放置在GameDraw.UIElement.Menu名称空间中。（注：重用不包括背景贴图以及对不同界面具有差异性的贴图的重用。）

III	实现一个能够显示方向和位置的罗盘，罗盘获取当前摄像机的方向和位置来更新自己的状态。该罗盘作为一个标准的游戏界面元素，放置在GameDraw.UIElement.GameScreen名称空间中。

IV	设计游戏中的鼠标，同样要求生动和美观，和游戏的界面风格保持一致。


**三	地图和场景物体编辑器：**

I	**_（已完成）_**贴图导入预处理工具：由于平台采用了像素碰撞检测技术，在创建一个场景物体的贴图的时候会对该贴图的边界进行提取，而这个提取过程对贴图的Alpha通道有一定的要求。为了方便贴图的导入，需要编写一个贴图导入预处理工具。工具可以利用Sprite类的“贴图导入错误报告功能”提供的支持，要求能够显示当前贴图不符合要求的区域，方便贴图的修改。作为进一步要求，希望工具可以自动修正贴图中不符合要求的地方。（注，应该需要对GameBase.Sprite类的贴图导入错误报告部分进行一定的修改。）

II	**_（已完成）_**场景物体创建工具：对每个场景物体而言，有一些信息只与贴图坐标相关联，如物体的中心位置，物体的关键可见点，（注：当物体是非遮挡物体时，VisiManger通过关键可视点判断坦克是否能看到并识别出这个物体。）同一个物体多个贴图之间的连接位置（如坦克炮塔轴心点）等。为了方面物体的创建，这些信息最好在代码之外以脚本的形式储存。（写在XML文件中。）而这个工具的作用，就是方便编辑并储存这些信息。由于物体的创建形式可能是多样的，故该工具需要充分考虑扩展性。并和平台内部的代码相配合。


**四	额外的一些可以实现的想法：**

I	构建表情支持系统，让坦克具备表情。不少游戏中使用了一些贴图表示游戏对象的状态，例如《盟军敢死队》2代和3代中在敌人的头顶上显示感叹号，问号等来表现其当前状态。这样的系统也可以在SmartTank中实现，能够凸现“Smart”这个主题。AI负责对表情系统的最终调用，方式应该为使用OrderServer来设置自己的表情。具体细节需要另外考虑。