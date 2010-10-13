using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///<remarks>this file is from Sharpdevelop</remarks>
namespace CrystalWall.Message
{
    /// <summary>
    /// 消息服务器接口，他提供显示异常/警告/询问/显示自定义对话框等服务并将其抽象。
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// 显示一个错误，如果异常为null，则消息显示在消息框内，否则自定义错误报表器将被使用显示异常错误
        /// </summary>
        void ShowError(Exception ex, string message);

        /// <summary>
        /// 显示警告消息
        /// </summary>
        void ShowWarning(string message);

        /// <summary>
        /// 显示询问
        /// </summary>
        bool AskQuestion(string question, string caption);

        /// <summary>
        /// 显示自定义对话框
        /// </summary>
        /// <param name="caption">对话框标题</param>
        /// <param name="dialogText">对话框文本</param>
        /// <param name="acceptButtonIndex">
        /// 确定按钮的默认数字，如果不想使用确定按钮，则为-1
        /// </param>
        /// <param name="cancelButtonIndex">
        /// 取消按钮的默认数字，如果不想使用取消按钮，则为-1
        /// </param>
        /// <returns>点击了“确定”或“取消”按钮的数字，返回-1表示没有点击任何按钮关闭对话框</returns>
        int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts);
        
        /// <summary>
        /// 显示输入框
        /// </summary>
        /// <param name="caption">标题</param>
        /// <param name="dialogText">文本</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>输入的文本</returns>
        string ShowInputBox(string caption, string dialogText, string defaultValue);

        /// <summary>
        /// 小时消息框
        /// </summary>
        /// <param name="message">内容</param>
        /// <param name="caption">标题</param>
        void ShowMessage(string message, string caption);

        /// <summary>
        /// 显示保存错误的消息通知
        /// </summary>
        void InformSaveError(string fileName, string message, string dialogName, Exception exceptionGot);

        /// <summary>
        /// 显示保存错误的通知消息，并允许retry重试
        /// </summary>
        ChooseSaveErrorResult ChooseSaveError(string fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled);
    }

    /// <summary>
    /// 选择保存错误的消息服务的返回值，他指定是否可重试或忽略
    /// </summary>
    public sealed class ChooseSaveErrorResult
    {
        public bool IsRetry { get; private set; }
        public bool IsIgnore { get; private set; }
        public bool IsSaveAlternative { get { return AlternativeFileName != null; } }
        public string AlternativeFileName { get; private set; }

        private ChooseSaveErrorResult() { }

        public readonly static ChooseSaveErrorResult Retry = new ChooseSaveErrorResult { IsRetry = true };
        public readonly static ChooseSaveErrorResult Ignore = new ChooseSaveErrorResult { IsIgnore = true };
        public static ChooseSaveErrorResult SaveAlternative(string alternativeFileName)
        {
            return new ChooseSaveErrorResult { AlternativeFileName = alternativeFileName };
        }
    }
}
