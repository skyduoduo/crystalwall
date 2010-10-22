using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Threading;
using CrystalWall.Logging;
using CrystalWall.Property;

///<remarks>this file is from Sharpdevelop</remarks>
namespace CrystalWall.Utils
{
    /// <summary>
    /// 此类包含两个ResourceManager资源管理器，他为应用处理字符和图形资源。在此层次上，他也处理本地资源。
    /// 他包含当前语言、图像/字符资源管理器缓存列表，使用一定的加载顺序搜索指定资源
    /// </summary>
    public static class ResourceService
    {
        const string uiLanguageProperty = "CoreProperties.UILanguage";

        const string stringResources = "StringResources";
        const string imageResources = "BitmapResources";

        /// <summary>
        /// 资源目录
        /// </summary>
        static string resourceDirectory;

        /// <summary>
        /// 将加载语言资源
        /// </summary>
        public static void InitializeService(string resourceDirectory)
        {
            if (ResourceService.resourceDirectory != null)
                throw new InvalidOperationException("Service is already initialized.");
            if (resourceDirectory == null)
                throw new ArgumentNullException("resourceDirectory");

            ResourceService.resourceDirectory = resourceDirectory;

            PropertyService.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChange);
            LoadLanguageResources(ResourceService.Language);
        }

        public static string Language
        {
            get
            {
                return PropertyService.Get(uiLanguageProperty, Thread.CurrentThread.CurrentUICulture.Name);
            }
            set
            {
                PropertyService.Set(uiLanguageProperty, value);
            }
        }

        /// <summary>E文资源列表</summary>
        static List<ResourceManager> strings = new List<ResourceManager>();
        /// <summary>E文图像列表</summary>
        static List<ResourceManager> icons = new List<ResourceManager>();

        /// 主应用程序的本地字符串
        /// </summary>
        static Hashtable localStrings = null;
        static Hashtable localIcons = null;

        /// <summary>当前语言的字符资源管理器</summary>
        static List<ResourceManager> localStringsResMgrs = new List<ResourceManager>();
        /// <summary>当前语言的图像资源管理器</summary>
        static List<ResourceManager> localIconsResMgrs = new List<ResourceManager>();

        static List<ResourceAssembly> resourceAssemblies = new List<ResourceAssembly>();

        /// <summary>
        /// 资源程序集类，用于从指定程序集中构造当前资源服务中的当前语言的指定基本资源名的资源管理器
        /// </summary>
        class ResourceAssembly
        {
            Assembly assembly;
            string baseResourceName;
            bool isIcons;

            public ResourceAssembly(Assembly assembly, string baseResourceName, bool isIcons)
            {
                this.assembly = assembly;
                this.baseResourceName = baseResourceName;
                this.isIcons = isIcons;
            }

            /// <summary>
            /// 获取指定程序集路径 + 语言 + 程序集资源文件（.resources.dll）路径，加载此资源程序集，并使用
            /// 其与基资源名构建ResourceManager
            /// </summary>
            ResourceManager TrySatellite(string language)
            {
                // 资源管理器可能自动使用附属程序集，但他不能工作，我们将手工建立
                string fileName = Path.GetFileNameWithoutExtension(assembly.Location) + ".resources.dll";
                // 程序集路径 + 语言 + 程序集资源文件路径
                fileName = Path.Combine(Path.Combine(Path.GetDirectoryName(assembly.Location), language), fileName);
                if (File.Exists(fileName))
                {
                    LoggingService.Info("Loging resources " + baseResourceName + " loading from satellite " + language);
                    return new ResourceManager(baseResourceName, Assembly.LoadFrom(fileName));
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// 按照一定层次构造语言资源管理器，并加入到localIconsResMgrs列表缓存中
            /// </summary>
            public void Load()
            {
                string logMessage = "Loading resources " + baseResourceName + "." + currentLanguage + ": ";
                ResourceManager manager = null;
                if (assembly.GetManifestResourceInfo(baseResourceName + "." + currentLanguage + ".resources") != null)
                {
                    //程序集中存在当前语言指定的resources资源文件，则使用当前语言构造资源管理器
                    LoggingService.Info(logMessage + " loading from main assembly");
                    manager = new ResourceManager(baseResourceName + "." + currentLanguage, assembly);
                }
                else if (currentLanguage.IndexOf('-') > 0
                         && assembly.GetManifestResourceInfo(baseResourceName + "." + currentLanguage.Split('-')[0] + ".resources") != null)
                {
                    //当前国家不存在，则使用"-"之前指定的无国家的资源
                    LoggingService.Info(logMessage + " loading from main assembly (no country match)");
                    manager = new ResourceManager(baseResourceName + "." + currentLanguage.Split('-')[0], assembly);
                }
                else
                {
                    // try satellite assembly
                    // 否则使用当前语言的附属资源文件（.resources.dll）构造资源管理器
                    manager = TrySatellite(currentLanguage);
                    if (manager == null && currentLanguage.IndexOf('-') > 0)
                    {
                        manager = TrySatellite(currentLanguage.Split('-')[0]);
                    }
                }
                if (manager == null)
                {
                    LoggingService.Warn(logMessage + "NOT FOUND");
                }
                else
                {
                    if (isIcons)
                        localIconsResMgrs.Add(manager);
                    else
                        localStringsResMgrs.Add(manager);
                }
            }
        }

        /// <summary>
        /// <br/>注册字符串资源：使用指定基本资源名和程序集构造ResourceAssembly，并调用其Load
        /// </summary>
        /// <param name="baseResourceName">嵌入在程序集中的资源文件名</param>
        /// <param name="assembly">包含资源文件的程序集</param>
        public static void RegisterStrings(string baseResourceName, Assembly assembly)
        {
            RegisterNeutralStrings(new ResourceManager(baseResourceName, assembly));
            ResourceAssembly ra = new ResourceAssembly(assembly, baseResourceName, false);
            resourceAssemblies.Add(ra);
            ra.Load();
        }

        public static void RegisterNeutralStrings(ResourceManager stringManager)
        {
            strings.Add(stringManager);
        }

        /// <summary>
        /// 参考RegisterStrings注册字符串资源
        /// </summary>
        public static void RegisterImages(string baseResourceName, Assembly assembly)
        {
            RegisterNeutralImages(new ResourceManager(baseResourceName, assembly));
            ResourceAssembly ra = new ResourceAssembly(assembly, baseResourceName, true);
            resourceAssemblies.Add(ra);
            ra.Load();
        }

        public static void RegisterNeutralImages(ResourceManager imageManager)
        {
            icons.Add(imageManager);
        }

        /// <summary>
        /// 如果改变的事件为uiLanguageProperty语言事件，则调用LoadLanguageResources重新加载新语言刷新
        /// 加载字符、图像等各资源管理器。并触发LanguageChanged语言改变事件
        /// </summary>
        static void OnPropertyChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.Key == uiLanguageProperty && e.NewValue != e.OldValue)
            {
                LoadLanguageResources((string)e.NewValue);
                if (LanguageChanged != null)
                    LanguageChanged(null, e);
            }
        }

        /// <summary>
        /// 清除缓存事件
        /// </summary>
        public static event EventHandler ClearCaches;

        /// <summary>
        /// 语言改变事件
        /// </summary>
        public static event EventHandler LanguageChanged;
        static string currentLanguage;

        /// <summary>
        /// 0、触发ClearCaches清除缓存事件
        /// 1、将当前线程的当前UI文化设置为指定语言的文化
        /// 2、Load方法加载指定语言的字符资源、图片资源
        /// 3、清空字符、图片资源管理器列表缓存，设置当前语言
        /// 4、遍历资源程序集列表，调用Load重新加载
        /// </summary>
        /// <param name="language"></param>
        static void LoadLanguageResources(string language)
        {
            if (ClearCaches != null)
                ClearCaches(null, EventArgs.Empty);

            try
            {
                //设置当前线程的文化
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
            }
            catch (Exception)
            {
                try
                {
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language.Split('-')[0]);
                }
                catch (Exception) { }
            }
            //加载指定语言的字符资源
            localStrings = Load(stringResources, language);
            if (localStrings == null && language.IndexOf('-') > 0)
            {
                localStrings = Load(stringResources, language.Split('-')[0]);
            }
            //加载指定语言的图像资源
            localIcons = Load(imageResources, language);
            if (localIcons == null && language.IndexOf('-') > 0)
            {
                localIcons = Load(imageResources, language.Split('-')[0]);
            }
            //清空列表缓存
            localStringsResMgrs.Clear();
            localIconsResMgrs.Clear();
            currentLanguage = language;
            foreach (ResourceAssembly ra in resourceAssemblies)
            {
                ra.Load();
            }
        }

        /// <summary>
        /// 将资源文件加载为hash表
        /// </summary>
        static Hashtable Load(string fileName)
        {
            if (File.Exists(fileName))
            {
                Hashtable resources = new Hashtable();
                //资源文件读取器
                ResourceReader rr = new ResourceReader(fileName);
                foreach (DictionaryEntry entry in rr)
                {
                    resources.Add(entry.Key, entry.Value);
                }
                rr.Close();
                return resources;
            }
            return null;
        }

        /// <summary>
        /// 在此服务的资源目录下啊加载指定资源文件名.语言.resources的键值
        /// </summary>
        static Hashtable Load(string name, string language)
        {
            return Load(resourceDirectory + Path.DirectorySeparatorChar + name + "." + language + ".resources");
        }

        /// <summary>
        /// localStrings缓存中获取 ==> localStringsResMgrs当前语言管理器列表缓存中获取 ==> strings E文管理器列表中获取
        /// </summary>
        /// <returns>
        /// 本地文字
        /// </returns>
        /// <param name="name">
        /// 请求的资源名
        /// </param>
        /// <exception cref="ResourceNotFoundException">
        /// 资源不存在
        /// </exception>
        public static string GetString(string name)
        {
            if (localStrings != null && localStrings[name] != null)
            {
                return localStrings[name].ToString();
            }

            string s = null;
            foreach (ResourceManager resourceManger in localStringsResMgrs)
            {
                try
                {
                    s = resourceManger.GetString(name);
                }
                catch (Exception) { }

                if (s != null)
                {
                    break;
                }
            }

            if (s == null)
            {
                foreach (ResourceManager resourceManger in strings)
                {
                    try
                    {
                        s = resourceManger.GetString(name);
                    }
                    catch (Exception) { }

                    if (s != null)
                    {
                        break;
                    }
                }
            }
            if (s == null)
            {
                throw new ResourceNotFoundException("string >" + name + "<");
            }

            return s;
        }

        /// <summary>
        /// localIcons缓存中获取 ==> localIconsResMgrs资源管理器列表缓存中获取 ==> icons E文图像列表中获取
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object GetImageResource(string name)
        {
            object iconobj = null;
            if (localIcons != null && localIcons[name] != null)
            {
                iconobj = localIcons[name];
            }
            else
            {
                foreach (ResourceManager resourceManger in localIconsResMgrs)
                {
                    iconobj = resourceManger.GetObject(name);
                    if (iconobj != null)
                    {
                        break;
                    }
                }

                if (iconobj == null)
                {
                    foreach (ResourceManager resourceManger in icons)
                    {
                        try
                        {
                            iconobj = resourceManger.GetObject(name);
                        }
                        catch (Exception) { }

                        if (iconobj != null)
                        {
                            break;
                        }
                    }
                }
            }
            return iconobj;
        }
    }
}
