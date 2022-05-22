# Client所有函数参数接口

- 点击文件后再预览不用拖动
- `namespace`都是Client
- 参数空着的是变量
- 构造函数一般和私有变量、函数一般不列出
- 类名和文件名是一样的，所有文件都只有一个类
- 类型为 **鼠标点击事件** 的，参数一般是`object sender, RoutedEventArgs e`，一般是接收一个鼠标点击

|    类名、文件名    |    函数（变量）名    |                        功能                         |     类型/返回值     |                             参数                             |                             备注                             |
| :----------------: | :------------------: | :-------------------------------------------------: | :-----------------: | :----------------------------------------------------------: | :----------------------------------------------------------: |
|       `User`       |         `id`         |               区分每个用户客户端的ID                |       string        |                                                              |                     建议修改，以大写开头                     |
|                    |       `score`        |                        得分                         |         int         |                                                              |                   建议修改，并且似乎没引用                   |
|                    |    `LoginWindow`     |                      登录窗口                       |     LoginWindow     |                                                              |                   以下三个每个客户端都不同                   |
|                    |     `MainWindow`     |                  主窗口（画画的）                   |     MainWindow      |                                                              |                                                              |
|                    |     `RoomWindow`     |                      大厅窗口                       |     RoomWindow      |                                                              |                           建议删除                           |
| `VerifyCodeHelper` | `CreateVerifyCode()` |        验证码的，返回一个Bitmap，需要转一下         |       Bitmap        |                       out string code,                       |                                                              |
|  `RegisterWindow`  |    `Verification`    |                       验证码                        |       String        |                                                              |                           私有变量                           |
|                    |       `client`       |                 服务器接口，用户端                  | LoginServiceClient  |                                                              |          私有变量，在Service里定义(LoginReference)           |
|                    |     `GetImage()`     |             验证码bitmap转成imageSource             |       string        |                              ,                               |                      调用验证码那个函数                      |
|                    |   `Button_Click()`   |       三个按钮对应操作，提交、帮助、联系我们        |        void         |                         鼠标点击事件                         | 私有函数绑定按钮的，建议去掉帮助和联系我们。这里写了`try..catch` |
|   `StartWindow`    |   `StartWindow()`    |         构造函数，启动一个或者四个那个界面          |        void         |                              ,                               |       其他都是私有函数，分别是启动一个、启动四个、关闭       |
|   `LoginWindow`    |         `us`         |                   用户的所有信息                    | LoginReference.User |                                                              |                   在Service里面，私有变量                    |
|                    |   `Button_Click()`   |          三个按钮（登录、忘记密码、注册）           |        void         |                         鼠标点击事件                         |         私有函数，绑定按钮的。忘记密码这个功能扬了吧         |
|  `ForgetPwWindow`  |                      |                    忘记密码窗口                     |                     |                                                              |                      删了吧，好无聊的人                      |
|    `RoomWindow`    |                      |                  大厅窗口，去掉把                   |                     |                                                              |        这里和`MainWindow`里`ShowRoom()`应该是有bug的         |
|    `PlayerInfo`    |    `PlayerInfo()`    |                    玩家信息窗口                     |                     |                              NA                              |            感觉没用呢，其实也可以留着，但是没啥用            |
|    `MainWindow`    |       `client`       |                     服务器接口                      |    ServiceClient    |                                                              |              私有变量，用户端(ServiceReference)              |
|                    |    `loginclient`     |                     服务器接口                      | LoginServiceClient  |                                                              |           私有变量，登录界面传过来(LoginReference)           |
|                    |         `us`         |                     服务器接口                      | LoginReference.User |                                                              |                        用户的所有信息                        |
|                    |     `userdatas`      |                     服务器接口                      |     Userdata[]      |                                                              | 私有变量，这个应该是主界面显示的那几个信息(ServiceReference) |
|                    |       `roomId`       |                     不同房间ID                      |         int         |                                                              | 我觉得可以去掉，奇怪的是没看到赋值，可能是Service里面的EnterRoom函数或者StartGame函数 |
|                    |      `TipCheck`      |                       没用到                        |       string        |                                                              |                           建议去掉                           |
|                    |     `ShowInk()`      | 清空画板，把所有string的ink转换为stroke添加到canvas |        void         |                       string inkData,                        |                     ServiceReference引用                     |
|   以下是回调函数   |    `ShowLogin()`     |                聊天室：有人进入房间                 |        void         |                    string loginUserName,                     |                       ServiceReference                       |
|                    |    `ShowLogout()`    |                聊天室：有人退出房间                 |        void         |                       string userName,                       |                       ServiceReference                       |
|                    |     `ShowTalk()`     |                    聊天室：发言                     |        void         |               string userName, string message,               |                       ServiceReference                       |
|                    |     `ShowInfo()`     |                主界面显示的玩家信息                 |        void         |                      Userdata[] mydata,                      |     ServiceReference，他说有bug，点击只会显示自己的信息      |
|                    |    `EnterRoom()`     |                   进入不同的房间                    |        void         |                 string userName,int rooomId,                 |               调用了ServiceClient的`EnterRoom`               |
|                    |     `ShowRoom()`     |                     初始化房间                      |        void         |                              ,                               |                        这个函数有bug                         |
|                    |    `ShowStart()`     |       游戏开始后，对画图和猜词玩家的不同设置        |        void         |         string userName1, string answer, string tip,         |                       ServiceReference                       |
|                    |     `ShowWin()`      |                      你赢了吗                       |        void         |              string userName,string userName0,               |                       ServiceReference                       |
|                    |   `ShowNewTurn()`    |                        重开                         |        void         | string roommeg, string userName1, string answer, string tip, |             ServiceReference，这个函数应该去掉的             |
