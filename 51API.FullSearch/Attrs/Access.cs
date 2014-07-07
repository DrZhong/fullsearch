using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace _51API.FullSearch.Attrs
{
    public class Access
    {
        /// <summary>
        /// 默认密钥
        /// </summary>
        private const string KEY = "!@#$Q";

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="key">密钥</param>
        /// <returns>验证码</returns>
        private static string GenCookieValStr(string userID, string key)
        {
            string source = key + "!!?" + userID + "USB";
            string valStr = Encrypt(source);
            return valStr;
        }

        /// <summary>
        /// 验证cookie
        /// </summary>
        /// <param name="cookies">传入cookie</param>
        /// <param name="key">密钥</param>
        /// <returns>是否通过验证</returns>
        public static bool CheckCookiesVal(HttpCookieCollection cookies, string key = KEY)
        {
            try
            {
                return (cookies["val"].Value == GenCookieValStr(cookies["admin_id"].Value, key));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 设置cookie（登陆调用）
        /// </summary>
        /// <param name="cookies">传入cookie</param>
        /// <param name="userID">用户id</param>
        /// <param name="key">密钥</param>
        public static void SetCookies(HttpCookieCollection cookies, string userID, string key = KEY)
        {
            HttpCookie cookie = new HttpCookie("admin_id", userID);
            cookie.Expires = DateTime.Now.AddDays(1);
            cookies.Add(cookie);

            cookie = new HttpCookie("val", GenCookieValStr(userID, key));
            cookie.Expires = DateTime.Now.AddDays(1);
            cookies.Add(cookie);
        }

        /// <summary>
        /// MD5编码
        /// </summary>
        /// <param name="strPwd">待编码字串</param>
        /// <returns>编码结果</returns>
        static public string Encrypt(string strPwd)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(strPwd);//将字符编码为一个字节序列 
            byte[] md5data = md5.ComputeHash(data);//计算data字节数组的哈希值 
            md5.Clear();
            //输出为字符串
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < md5data.Length; i++)
            {
                sb.Append(md5data[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
    }
}