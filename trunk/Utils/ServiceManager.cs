using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrystalWall.Message;
using CrystalWall.Logging;
using CrystalWall.Debugging;

///<remarks>this file is from Sharpdevelop</remarks>
namespace CrystalWall.Utils
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
