using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ZXClient.util
{
    class HotKey
    {
        //如果函数执行成功，返回值不为0。
        //如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(
            IntPtr hWnd,                //要定义热键的窗口的句柄
            int id,                     //定义热键ID（不能与其它ID重复）           
            KeyModifiers fsModifiers,   //标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效
            Keys vk                     //定义热键的内容
            );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(
            IntPtr hWnd,                //要取消热键的窗口的句柄
            int id                      //要取消热键的ID
            );

        //定义了辅助键的名称（将数字转变为字符以便于记忆，也可去除此枚举而直接使用数值）
        [Flags()]
        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Ctrl = 2,
            Shift = 4,
            WindowsKey = 8
        }

        /// <summary>
        /// 依据KeyEventArgs生成组合键字符串
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetStringByKey(KeyEventArgs e)
        {
            if (e.KeyValue == 16)
            {
                return "Shift + ";
            }
            else if (e.KeyValue == 17)
            {
                return "Ctrl + ";
            }
            else if (e.KeyValue == 18)
            {
                return "Alt + ";
            }
            else
            {
                StringBuilder keyValue = new StringBuilder();
                if (e.Modifiers != 0)
                {
                    if (e.Control)
                    {
                        keyValue.Append("Ctrl + ");
                    }
                    if (e.Alt)
                    {
                        keyValue.Append("Alt + ");
                    }
                    if (e.Shift)
                    {
                        keyValue.Append("Shift + ");
                    }
                }
                if ((e.KeyValue >= 48 && e.KeyValue <= 57))    //0-9
                {
                    keyValue.Append(e.KeyCode.ToString().Substring(1));
                }
                else
                {
                    keyValue.Append(e.KeyCode);
                }

                return keyValue.ToString();
            }
        }

        /// <summary>
        ///  依据按键获得单一键值相应字符串
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetSingleStrByKey(KeyEventArgs e)
        {
            if (e.KeyValue == 16)
            {
                return "Shift";
            }
            else if (e.KeyValue == 17)
            {
                return "Ctrl";
            }
            else if (e.KeyValue == 18)
            {
                return "Alt";
            }
            else
            {
                return e.KeyCode.ToString();
            }
        }

        /// <summary>
        /// 依据string生成KeyEventArgs
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static KeyEventArgs GetKeyByString(string strKey)
        {
            Keys keyResult = new Keys();
            string[] strKeyCodes = strKey.Split('+');
            if (strKeyCodes.Length > 0)
            {
                int numberKey;
                foreach (string keyEach in strKeyCodes)
                {
                    if (keyEach.Trim().ToUpper() == "CTRL")
                    {
                        keyResult = keyResult | Keys.Control;
                    }
                    else if (keyEach.Trim().ToUpper() == "SHIFT")
                    {
                        keyResult = keyResult | Keys.Shift;
                    }
                    else if (keyEach.Trim().ToUpper() == "ALT")
                    {
                        keyResult = keyResult | Keys.Alt;
                    }
                    //数字
                    else if (int.TryParse(keyEach, out numberKey))
                    {
                        KeysConverter converter = new KeysConverter();
                        object o = converter.ConvertFromString('D' + keyEach.Trim());
                        if (o != null)
                        {
                            Keys getKey = (Keys)o;
                            keyResult = keyResult | getKey;
                        }
                    }
                    //其它（字母，F0-F12)
                    else
                    {
                        try
                        {
                            KeysConverter converter = new KeysConverter();
                            object o = converter.ConvertFromString(keyEach);
                            
                            if (o != null)
                            {
                                Keys getKey = (Keys)o;
                                keyResult = keyResult | getKey;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
            }
            KeyEventArgs newEventArgs = new KeyEventArgs(keyResult);
            return newEventArgs;
        }
    }
}
