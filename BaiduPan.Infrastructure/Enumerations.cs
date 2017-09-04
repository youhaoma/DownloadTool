using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure
{
    public enum FileTypeEnum
    {

        OtherType,

        ApkType,

        DocType,

        ExeType,

        FolderType,

        ImgType,

        MixFileType,

        MusicType,

        PDFType,

        PPTType,

        RarType,

        TorrentType,

        TxtType,

        VideoType,

        XlsType,
        
    }

    [Flags]

    public enum DownloadStateEnum
    {
        Created = 0,

        Waiting = 1,
        
        Downloading = 2,

        Paused = 4,

        Completed = 8,

        Canceld = 16,

        Faulted = 32,


    }


    public enum SizeUnitEnum
    {
        // Byte
        B,


        // K Byte
        K,

        // M Byte
        M,


        // G Byte
        G,

        // T Byte
        T,

        // P Byte
        P,

    }

    public enum ClientLoginStateEnum
    {
        /// <summary>
        /// 登陆成功
        /// </summary>
        Success = 0,

        /// <summary>
        /// 用户不存在
        /// </summary>
        NonUser = 1,

        /// <summary>
        /// 密码错误
        /// </summary>
        PasswordError = 2,

        /// <summary>
        /// 被封禁
        /// </summary>
        Baned  = 5, 


        /// <summary>
        /// 未知错误
        /// </summary>
        OtherError = 4,

    }


    public enum DownloadMethodEnum
    {
        /// <summary>
        ///  直接下载
        /// </summary>
        DirectDownload = 1,


        /// <summary>
        /// 中转下载
        /// </summary>
        JumpDownload = 2,



        /// <summary>
        /// Appid 下载
        /// </summary>
        AppidDownload = 3,
    }



    public enum LoginStateEnum
    {
        /// <summary>
        /// 成功登陆
        /// </summary>
        Success = 0,

        /// <summary>
        /// 系统错误
        /// </summary>
        SystemError = -1,


        /// <summary>
        /// 账号格式不正确
        /// </summary>
        UserNameError = 1,


        /// <summary>
        /// 账号不存在
        /// </summary>
        UserNotExists = 2,

        /// <summary>
        /// 验证码不存在，或者过期
        /// </summary>
        VerifyCodeNotExistsOrTimeout = 3,


        /// <summary>
        /// 用户名或者密码错误
        /// </summary>
        UserNameOrPasswordError = 4,


        /// <summary>
        /// 验证码错误
        /// </summary>
        VerifyCodeError = 6,

        /// <summary>
        /// 密码错误
        /// </summary>
        PasswordError = 7,


        /// <summary>
        /// 账号因安全问题被锁定
        /// </summary>
        SafeError   = 16,


        /// <summary>
        /// 未输入验证码
        /// </summary>
        NoInputVerifyCode = 257,


        /// <summary>
        ///  系统升级，无法登陆
        /// </summary>
        SystemUpdate = 100027,


        /// <summary>
        ///  没有权限登陆
        /// </summary>
        NoPermission = 21,
        

    }



    public enum RegisterStateEnum
    {
        /// <summary>
        /// 注册完成
        /// </summary>
        Completed, 

        /// <summary>
        /// 账户已经存在
        /// </summary>
        AccountExisted,

        /// <summary>
        /// 位置错误
        /// </summary>
        OhterError,


        /// <summary>
        /// 最大注册数
        /// </summary>
        Maximum
    }


    public enum ShareStateEnum
    {

        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,


        /// <summary>
        /// 文件已经被删除
        /// </summary>
        Deleted = 9,

        /// <summary>
        /// 分享失败
        /// </summary>
        Failure = 1,

        /// <summary>
        /// 审核未通过
        /// </summary>
        Notpassed = 4,
    }


    public enum LanguageEnum
    {
        Chinese,
        English,
        Japanese,
    }
}
