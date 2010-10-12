using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodePlex.CrystalWall.Logging;
using CodePlex.CrystalWall.Utils;

namespace QJSoft.ECBC.Message
{
    /// <summary>
    /// 包含一个自定义显示异常消息代理的使用<see cref="StringParser"/>
    /// 替换格式化中${res}占位符的静态消息服务类。如果没有设定自定义消息代理
    /// 对象，则使用ServiceManager.MessageService中的各种方法显示消息
    /// </summary>
    public static class MessageService
    {
        /// <summary>
        /// 自定义显示错误消息的委托
        /// </summary>
        public delegate void ShowErrorDelegate(Exception ex, string message);

        public static ShowErrorDelegate CustomErrorReporter { get; set; }

        public static void ShowError(Exception ex)
        {
            ShowError(ex, null);
        }

        public static void ShowError(string message)
        {
            ShowError(null, message);
        }

        /// <summary>
        /// 显示错误消息，并可以使用
        /// <see cref="StringParser"/>格式化参数
        /// </summary>
        public static void ShowErrorFormatted(string formatstring, params string[] formatitems)
        {
            ShowError(null, Format(formatstring, formatitems));
        }

        /// <summary>
        /// 显示错误。如果ex参数为null，则消息显示在消息框内，否则使用自定义消息委托显示 
        /// </summary>
        public static void ShowError(Exception ex, string message)
        {
            if (message == null) message = string.Empty;

            if (ex != null)
            {
                LoggingService.Error(message, ex);
                LoggingService.Warn("最终错误日志跟踪栈:\n" + Environment.StackTrace);
                if (CustomErrorReporter != null)
                {
                    CustomErrorReporter(ex, message);
                    return;
                }
            }
            else
            {
                LoggingService.Error(message);
            }
            ServiceManager.MessageService.ShowError(ex, message);
        }

        public static void ShowWarning(string message)
        {
            LoggingService.Warn(message);
            ServiceManager.MessageService.ShowWarning(message);
        }

        public static void ShowWarningFormatted(string formatstring, params string[] formatitems)
        {
            ShowWarning(Format(formatstring, formatitems));
        }

        public static bool AskQuestion(string question, string caption)
        {
            return ServiceManager.MessageService.AskQuestion(question, caption);
        }

        public static bool AskQuestionFormatted(string caption, string formatstring, params string[] formatitems)
        {
            return AskQuestion(Format(formatstring, formatitems), caption);
        }

        public static bool AskQuestionFormatted(string formatstring, params string[] formatitems)
        {
            return AskQuestion(Format(formatstring, formatitems));
        }

        public static bool AskQuestion(string question)
        {
            return AskQuestion(StringParser.Parse(question), StringParser.Parse("${res:Global.QuestionText}"));
        }

        /// <summary>
        ///  显示自定义对话框
        /// </summary>
        /// <param name="acceptButtonIndex">
        /// 默认确定按钮的索引，如果为-1则没有确认按钮
        /// </param>
        /// <param name="buttontexts">按钮文本列表</param>
        /// <returns>点击按钮后的索引</returns>
        public static int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts)
        {
            return ServiceManager.MessageService.ShowCustomDialog(caption, dialogText, acceptButtonIndex, cancelButtonIndex, buttontexts);
        }

        public static int ShowCustomDialog(string caption, string dialogText, params string[] buttontexts)
        {
            return ShowCustomDialog(caption, dialogText, -1, -1, buttontexts);
        }

        public static string ShowInputBox(string caption, string dialogText, string defaultValue)
        {
            return ServiceManager.MessageService.ShowInputBox(caption, dialogText, defaultValue);
        }

        static string defaultMessageBoxTitle = "MessageBox";
        static string productName = "Application Name";

        /// <summary>
        /// ${ProductName}.产品标记
        /// </summary>
        public static string ProductName
        {
            get { return productName; }
            set { productName = value; }
        }

        /// <summary>
        /// 默认消息标题
        /// </summary>
        public static string DefaultMessageBoxTitle
        {
            get { return defaultMessageBoxTitle; }
            set { defaultMessageBoxTitle = value; }
        }

        public static void ShowMessage(string message)
        {
            ShowMessage(message, DefaultMessageBoxTitle);
        }

        public static void ShowMessageFormatted(string formatstring, params string[] formatitems)
        {
            ShowMessage(Format(formatstring, formatitems));
        }

        public static void ShowMessageFormatted(string caption, string formatstring, params string[] formatitems)
        {
            ShowMessage(Format(formatstring, formatitems), caption);
        }

        public static void ShowMessage(string message, string caption)
        {
            LoggingService.Info(message);
            ServiceManager.MessageService.ShowMessage(message, caption);
        }

        /// <summary>
        /// 使用StringParser.Parse替换${res}占位符之后进行格式化字符的方法
        /// </summary>
        /// <param name="formatstring">格式化参数，可以使用${res}占位符</param>
        /// <param name="formatitems">格式化数据</param>
        /// <returns></returns>
        static string Format(string formatstring, string[] formatitems)
        {
            try
            {
                return String.Format(StringParser.Parse(formatstring), formatitems);
            }
            catch (FormatException)
            {
                StringBuilder b = new StringBuilder(StringParser.Parse(formatstring));
                foreach (string formatitem in formatitems)
                {
                    b.Append("\nItem: ");
                    b.Append(formatitem);
                }
                return b.ToString();
            }
        }
    }
}
