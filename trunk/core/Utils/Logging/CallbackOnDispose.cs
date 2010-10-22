using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

///<remarks>this file is from Sharpdevelop</remarks>
namespace CrystalWall.Logging
{
    /// <summary>
    /// 当类销毁时执行一个ThreadStart线程回调且以线程安全的方式
    /// 将回调设置为null的IDisposable接口实现
    /// </summary>
    sealed class CallbackOnDispose : IDisposable
    {
        System.Threading.ThreadStart callback;

        public CallbackOnDispose(System.Threading.ThreadStart callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            this.callback = callback;
        }

        /// <summary>
        /// 将内部的线程启动委托对象设置为null，如果之前启动对象不为null，则销毁前执行此委托。
        /// 本方法是线程安全的，他使用了Interlocked原子变量锁对象设置null值
        /// </summary>
        public void Dispose()
        {
            System.Threading.ThreadStart action = Interlocked.Exchange(ref callback, null);
            if (action != null)
                action();
        }
    }
}
