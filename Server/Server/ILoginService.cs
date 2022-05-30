using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Server
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“ILoginService”。
    [ServiceContract]
    public interface ILoginService
    {
        #region 远程登录服务接口
        //登录
        [OperationContract]
        bool Login(string id, string pw);

        //注册
        [OperationContract]
        string Registered(string id, string pw, string sn, string name,string code);


        [OperationContract]
        bool sendEmail(string email);
        //修改密码
        [OperationContract]
        string ForgetPassword(string id, string ps, string code);

        [OperationContract]
        User Userinfo(string id);

        [OperationContract]
        int GetUserNum();
        #endregion
    }
}
