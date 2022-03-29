using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Constants
{
    public static class CS
    {

        /** 通用 可用 / 禁用 **/
        public const int PUB_USABLE = 1;
        public const int PUB_DISABLE = 0;
        public static Dictionary<int, string> PUB_USABLE_MAP = new Dictionary<int, string>();

        /** 系统类型定义 **/
        public interface SYS_TYPE
        {
            const string MCH = "MCH";
            const string MGR = "MGR";
            static Dictionary<string, string> SYS_TYPE_MAP = new Dictionary<string, string>();
        }

        /// <summary>
        /// 允许上传的的图片文件格式
        /// </summary>
        public static HashSet<string> ALLOW_UPLOAD_IMG_SUFFIX = new HashSet<string>();

        static CS()
        {
            ALLOW_UPLOAD_IMG_SUFFIX.Add("jpg");
            ALLOW_UPLOAD_IMG_SUFFIX.Add("png");
            ALLOW_UPLOAD_IMG_SUFFIX.Add("jpeg");
            ALLOW_UPLOAD_IMG_SUFFIX.Add("gif");
            ALLOW_UPLOAD_IMG_SUFFIX.Add("mp4");

            SYS_TYPE.SYS_TYPE_MAP.Add(SYS_TYPE.MCH, "商户系统");
            SYS_TYPE.SYS_TYPE_MAP.Add(SYS_TYPE.MGR, "运营平台");

            PUB_USABLE_MAP.Add(PUB_USABLE, "正常");
            PUB_USABLE_MAP.Add(PUB_DISABLE, "停用");
        }

        //登录图形验证码缓存时间，单位：s
        public const int VERCODE_CACHE_TIME = 60;

        #region yes or no
        public const byte NO = 0;
        public const byte YES = 1;
        #endregion

        #region 账号类型:1-服务商 2-商户 3-商户应用
        /// <summary>
        /// 服务商
        /// </summary>
        public const byte INFO_TYPE_ISV = 1;
        /// <summary>
        /// 商户
        /// </summary>
        public const byte INFO_TYPE_MCH = 2;
        /// <summary>
        /// 商户应用
        /// </summary>
        public const byte INFO_TYPE_MCH_APP = 3;
        #endregion

        #region 商户类型:1-普通商户 2-特约商户
        /// <summary>
        /// 普通商户
        /// </summary>
        public const byte MCH_TYPE_NORMAL = 1;
        /// <summary>
        /// 特约商户
        /// </summary>
        public const byte MCH_TYPE_ISVSUB = 2;
        #endregion

        #region 性别 1- 男， 2-女
        /// <summary>
        /// 未知
        /// </summary>
        public const byte SEX_UNKNOWN = 0;
        /// <summary>
        /// 男
        /// </summary>
        public const byte SEX_MALE = 1;
        /// <summary>
        /// 女
        /// </summary>
        public const byte SEX_FEMALE = 2;
        #endregion

        /// <summary>
        /// 默认密码
        /// </summary>
        public const string DEFAULT_PWD = "jeepay666";

        public const long TOKEN_TIME = 60 * 60 * 2; //单位：s,  两小时

        /// <summary>
        /// access_token 名称
        /// </summary>
        public const string ACCESS_TOKEN_NAME = "iToken";

        private const string CACHE_KEY_TOKEN = "TOKEN_{0}_{1}";
        /// <summary>
        /// ！！不同系统请放置不同的redis库 ！！<br/>
        /// 缓存key: 当前用户所有用户的token集合 <br/>
        /// <example>例子：TOKEN_1001_HcNheNDqHzhTIrT0lUXikm7xU5XY4Q</example>
        /// </summary>
        /// <param name="sysUserId">1001</param>
        /// <example>1001</example>
        /// <param name="uuid">
        /// <example>HcNheNDqHzhTIrT0lUXikm7xU5XY4Q</example>
        /// </param>
        /// <returns></returns>
        public static string GetCacheKeyToken(long sysUserId, string uuid)
        {
            return string.Format(CACHE_KEY_TOKEN, sysUserId, uuid);
        }

        private const string CACHE_KEY_IMG_CODE = "img_code_{0}";
        /// <summary>
        /// 图片验证码 缓存key
        /// </summary>
        /// <param name="imgToken"></param>
        /// <returns></returns>
        public static string GetCacheKeyImgCode(string imgToken)
        {
            return string.Format(CACHE_KEY_IMG_CODE, imgToken);
        }

        /// <summary>
        /// 登录认证类型
        /// </summary>
        public interface AUTH_TYPE
        {
            /// <summary>
            /// 登录用户名
            /// </summary>
            const byte LOGIN_USER_NAME = 1;
            /// <summary>
            /// 手机号
            /// </summary>
            const byte TELPHONE = 2;
            /// <summary>
            /// 邮箱
            /// </summary>
            const byte EMAIL = 3;

            /// <summary>
            /// 微信unionId
            /// </summary>
            const byte WX_UNION_ID = 10;
            /// <summary>
            /// 微信小程序
            /// </summary>
            const byte WX_MINI = 11;
            /// <summary>
            /// 微信公众号
            /// </summary>
            const byte WX_MP = 12;

            /// <summary>
            /// QQ
            /// </summary>
            const byte QQ = 20;
        }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public interface ENT_TYPE
        {
            /// <summary>
            /// 左侧显示菜单
            /// </summary>
            const string MENU_LEFT = "ML";
            /// <summary>
            /// 其他菜单
            /// </summary>
            const string MENU_OTHER = "MO";
            /// <summary>
            /// 页面 or 按钮
            /// </summary>
            const string PAGE_OR_BTN = "PB";
        }

        /// <summary>
        /// 接口类型
        /// </summary>
        public interface IF_CODE
        {
            /// <summary>
            /// 支付宝官方支付
            /// </summary>
            const string ALIPAY = "alipay";
            /// <summary>
            /// 微信官方支付
            /// </summary>
            const string WXPAY = "wxpay";
            /// <summary>
            /// 云闪付开放平台
            /// </summary>
            const string YSFPAY = "ysfpay";
            /// <summary>
            /// 小新支付
            /// </summary>
            const string XXPAY = "xxpay";
            /// <summary>
            /// Paypal 支付
            /// </summary>
            const string PPPAY = "pppay";
        }

        /// <summary>
        /// 支付方式代码
        /// </summary>
        public interface PAY_WAY_CODE
        {
            #region 特殊支付方式
            /// <summary>
            /// ( 通过二维码跳转到收银台完成支付， 已集成获取用户ID的实现。  )
            /// </summary>
            const string QR_CASHIER = "QR_CASHIER"; 
            /// <summary>
            /// 条码聚合支付（自动分类条码类型）
            /// </summary>
            const string AUTO_BAR = "AUTO_BAR";  
            #endregion

            /// <summary>
            /// 支付宝条码支付
            /// </summary>
            const string ALI_BAR = "ALI_BAR";
            /// <summary>
            /// 支付宝服务窗支付
            /// </summary>
            const string ALI_JSAPI = "ALI_JSAPI";
            /// <summary>
            /// 支付宝 app支付
            /// </summary>
            const string ALI_APP = "ALI_APP";
            /// <summary>
            /// 支付宝 电脑网站支付
            /// </summary>
            const string ALI_PC = "ALI_PC";
            /// <summary>
            /// 支付宝 wap支付
            /// </summary>
            const string ALI_WAP = "ALI_WAP";
            /// <summary>
            /// 支付宝 二维码付款
            /// </summary>
            const string ALI_QR = "ALI_QR";

            /// <summary>
            /// 云闪付条码支付
            /// </summary>
            const string YSF_BAR = "YSF_BAR";
            /// <summary>
            /// 云闪付服务窗支付
            /// </summary>
            const string YSF_JSAPI = "YSF_JSAPI";

            /// <summary>
            /// 微信jsapi支付
            /// </summary>
            const string WX_JSAPI = "WX_JSAPI";
            /// <summary>
            /// 微信小程序支付
            /// </summary>
            const string WX_LITE = "WX_LITE";
            /// <summary>
            /// 微信条码支付
            /// </summary>
            const string WX_BAR = "WX_BAR";
            /// <summary>
            /// 微信H5支付
            /// </summary>
            const string WX_H5 = "WX_H5";
            /// <summary>
            /// 微信扫码支付
            /// </summary>
            const string WX_NATIVE = "WX_NATIVE";

            /// <summary>
            /// Paypal 支付
            /// </summary>
            const string PP_PC = "PP_PC";
        }

        //支付数据包 类型
        public interface PAY_DATA_TYPE
        {
            /// <summary>
            /// 跳转链接的方式  redirectUrl
            /// </summary>
            const string PAY_URL = "payurl";  
            /// <summary>
            /// 表单提交
            /// </summary>
            const string FORM = "form";  
            /// <summary>
            /// 微信app参数
            /// </summary>
            const string WX_APP = "wxapp";  
            /// <summary>
            /// 支付宝app参数
            /// </summary>
            const string ALI_APP = "aliapp";  
            /// <summary>
            /// 云闪付app参数
            /// </summary>
            const string YSF_APP = "ysfapp";  
            /// <summary>
            /// 二维码URL
            /// </summary>
            const string CODE_URL = "codeUrl";  
            /// <summary>
            /// 二维码图片显示URL
            /// </summary>
            const string CODE_IMG_URL = "codeImgUrl";  
            /// <summary>
            /// 无参数
            /// </summary>
            const string NONE = "none";
            ///// <summary>
            ///// 二维码实际内容
            ///// </summary>
            //const string QR_CONTENT = "qrContent";  
        }

        //接口版本
        public interface PAY_IF_VERSION
        {
            /// <summary>
            /// 微信接口版本V2
            /// </summary>
            const string WX_V2 = "V2";  
            /// <summary>
            /// 微信接口版本V3
            /// </summary>
            const string WX_V3 = "V3";  
        }
    }
}
