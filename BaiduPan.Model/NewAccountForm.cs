using BaiduPan.Model.NetDiskInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaiduPan.Model
{
    public partial class NewAccountForm : Form
    {

        public ClientCredential Authentication { get; private set; }

        const string baiduPanHomeStr = "pan.baidu.com/disk/home";
        public NewAccountForm()
        {
            InitializeComponent();

            CookieReader.SupressCookiePersist();
        }

        private void panWebBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {

            if (e.Url.ToString().Contains(baiduPanHomeStr))
            {
                string cookies = CookieReader.GetCookie(panWebBrowser.Url.ToString());
                Dictionary<string, string> dict = CookieToDicy(cookies);

                if (dict.Count <= 0 || !dict.ContainsKey("STOKEN")
                    || !dict.ContainsKey("BDUSS"))
                {
                    return;
                }
              

                string uk = Regex.Match(panWebBrowser.DocumentText.ToString(), "uk\":([0-9]*)").Groups[1].Value;

                if (uk != "")
                {
                    Authentication = new ClientCredential();
                    Authentication.UK = uk;
                    Authentication.STOKEN = dict["STOKEN"];
                    Authentication.BDUSS = dict["BDUSS"];
                    DialogResult = DialogResult.OK;
                }
            }

        }


        private Dictionary<string, string> CookieToDicy(string cookies)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] cookies_split = cookies.Split(';');
            foreach (var cookie in cookies_split)
            {
                string[] cookie_split = cookie.Split('=');
                if (cookie_split.Length != 2) continue;
                if (dict.ContainsKey(cookie_split[0].Trim())) continue;
                dict.Add(cookie_split[0].Trim(), cookie_split[1].Trim());
            }
            return dict;
        }


        internal static class CookieReader
        {

            /// <summary>
            /// Enables the retrieval of cookies that are marked as "HTTPOnly". 
            /// Do not use this flag if you expose a scriptable interface, 
            /// because this has security implications. It is imperative that 
            /// you use this flag only if you can guarantee that you will never 
            /// expose the cookie to third-party code by way of an 
            /// extensibility mechanism you provide. 
            /// Version:  Requires Internet Explorer 8.0 or later.
            /// </summary>
            private const int INTERNET_COOKIE_HTTPONLY = 0x00002000;


            [DllImport("wininet.dll", SetLastError = true)]
            private static extern bool InternetGetCookieEx(
                string url,
                string cookieName,
                StringBuilder cookieData,
                ref int size,
                int flags,
                IntPtr pReserved);

            [DllImport("wininet.dll", SetLastError = true)]
            private static extern bool InternetSetOption(
                int hInternet,
                int dwOption,
                IntPtr lpBuffer,
                int dwBufferLength);

            /// <summary>
            /// Returns cookie contents as a string
            /// </summary>
            /// <param name="url"></param>
            /// <returns></returns>
            public static string GetCookie(string url)
            {
                int size = 512;
                StringBuilder sb = new StringBuilder(size);
                if (!InternetGetCookieEx(url, null, sb, ref size, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero))
                {
                    if (size < 0)
                    {
                        return null;
                    }
                    sb = new StringBuilder(size);
                    if (!InternetGetCookieEx(url, null, sb, ref size, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero))
                    {
                        return null;
                    }
                }

                return sb.ToString();

            }


            public static bool SupressCookiePersist()
            {

                // 3 = INTERNET_SUPPRESS_COOKIE_PERSIST 
                // 81 = INTERNET_OPTION_SUPPRESS_BEHAVIOR
                return SetOption(81, 3);
            }



            public static bool EndBroswerSession()
            {
                // 42 = INTERNET_OPTION_END_BROWSER_SESSION 
                return SetOption(42, null);
            }
            static bool SetOption(int settingCode, int? option)
            {
                IntPtr optionPtr = IntPtr.Zero;
                int size = 0;
                if (option.HasValue)
                {
                    size = sizeof(int);
                    optionPtr = Marshal.AllocCoTaskMem(size);
                    Marshal.WriteInt32(optionPtr, option.Value);
                }

                bool success = InternetSetOption(0, settingCode, optionPtr, size);

                if (optionPtr != IntPtr.Zero) Marshal.Release(optionPtr);

                return success;

            }


        }

        private void NewAccountForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            CookieReader.EndBroswerSession();
        }
    }
}
