using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CodePlex.CrystalWall.Logging;

namespace QJSoft.ECBC.Logging
{
    /// <summary>
    /// 使用TextWriter记录日志的日志服务实现
    /// </summary>
    public class TextWriterLoggingService : ILoggingService
    {
        readonly TextWriter writer;

        public TextWriterLoggingService(TextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            this.writer = writer;
            this.IsFatalEnabled = true;
            this.IsErrorEnabled = true;
            this.IsWarnEnabled = true;
            this.IsInfoEnabled = true;
            this.IsDebugEnabled = true;
        }

        void Write(object message, Exception exception)
        {
            if (message != null)
            {
                writer.WriteLine(DateTime.Now.ToLongTimeString() + ": " +  message.ToString());
            }
            if (exception != null)
            {
                writer.WriteLine(DateTime.Now.ToLongTimeString() + ": " + exception.ToString());
            }
        }

        public bool IsDebugEnabled { get; set; }
        public bool IsInfoEnabled { get; set; }
        public bool IsWarnEnabled { get; set; }
        public bool IsErrorEnabled { get; set; }
        public bool IsFatalEnabled { get; set; }

        public void Debug(object message)
        {
            if (IsDebugEnabled)
            {
                Write(message, null);
            }
        }

        public void DebugFormatted(string format, params object[] args)
        {
            Debug(string.Format(format, args));
        }

        public void Info(object message)
        {
            if (IsInfoEnabled)
            {
                Write(message, null);
            }
        }

        public void InfoFormatted(string format, params object[] args)
        {
            Info(string.Format(format, args));
        }

        public void Warn(object message)
        {
            Warn(message, null);
        }

        public void Warn(object message, Exception exception)
        {
            if (IsWarnEnabled)
            {
                Write(message, exception);
            }
        }

        public void WarnFormatted(string format, params object[] args)
        {
            Warn(string.Format(format, args));
        }

        public void Error(object message)
        {
            Error(message, null);
        }

        public void Error(object message, Exception exception)
        {
            if (IsErrorEnabled)
            {
                Write(message, exception);
            }
        }

        public void ErrorFormatted(string format, params object[] args)
        {
            Error(string.Format(format, args));
        }

        public void Fatal(object message)
        {
            Fatal(message, null);
        }

        public void Fatal(object message, Exception exception)
        {
            if (IsFatalEnabled)
            {
                Write(message, exception);
            }
        }

        public void FatalFormatted(string format, params object[] args)
        {
            Fatal(string.Format(format, args));
        }
    }
}
