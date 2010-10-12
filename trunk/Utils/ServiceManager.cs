using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodePlex.CrystalWall.Message;
using CodePlex.CrystalWall.Logging;
using CodePlex.CrystalWall.Debugging;
using QJSoft.ECBC.Logging;

namespace CodePlex.CrystalWall.Utils
{
    /// <summary>
    /// 维护核心服务接口的实现，包括日志服务ILoggingService与消息服务IMessageService
    /// </summary>
    public static class ServiceManager
    {
        static ILoggingService loggingService = new TextWriterLoggingService(new DebugTextWriter());

        public static ILoggingService LoggingService
        {
            get { return loggingService; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                loggingService = value;
            }
        }

        static IMessageService messageService = new TextWriterMessageService(Console.Out);

        public static IMessageService MessageService
        {
            get { return messageService; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                messageService = value;
            }
        }
    }
}
