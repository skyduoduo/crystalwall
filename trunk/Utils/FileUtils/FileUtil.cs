using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using CrystalWall.Utils;
using CrystalWall.FileUtils;
using CrystalWall.Message;

///<remarks>this file is from Sharpdevelop</remarks>
namespace CrystalWall.FileUtils
{
    /// <summary>
    /// 文件错误策略枚举
    /// </summary>
    public enum FileErrorPolicy
    {
        Inform,
        ProvideAlternative
    }

    public enum FileOperationResult
    {
        OK,
        Failed,
        SavedAlternatively
    }

    /// <summary>
    /// 文件操作委托
    /// </summary>
    public delegate void FileOperationDelegate();

    public delegate void NamedFileOperationDelegate(string fileName);

    public static class FileUtil
    {
        readonly static char[] separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, Path.VolumeSeparatorChar };
		/// <summary>
		/// 当前应用域的基路径
		/// </summary>
		static string applicationRootPath = AppDomain.CurrentDomain.BaseDirectory;
		const string fileNameRegEx = @"^([a-zA-Z]:)?[^:]+$";

		/// <summary>
		/// 应用程序根路径，默认为当前应用域的基目录
		/// </summary>
		public static string ApplicationRootPath {
			get {
				return applicationRootPath;
			}
			set {
				applicationRootPath = value;
			}
		}
		
		/// <summary>
		/// 从注册表LocalMachine中获取指定的子键中获取指定值返回
		/// </summary>
		static string GetPathFromRegistry(string key, string valueName)
		{
			using (RegistryKey installRootKey = Registry.LocalMachine.OpenSubKey(key)) {
				if (installRootKey != null) {
					object o = installRootKey.GetValue(valueName);
					if (o != null) {
						string r = o.ToString();
						if (!string.IsNullOrEmpty(r))
							return r;
					}
				}
			}
			return null;
		}
		
		//.net框架/sdk各版本的根路径只读属性
		#region InstallRoot Properties
		
		static string netFrameworkInstallRoot = null;

		/// <summary>
		/// 获取.net框架安装的根路径，从注册表的SOFTWARE\Microsoft\.NETFramework键中获取InstallRoot值
		/// </summary>
		public static string NetFrameworkInstallRoot {
			get {
				if (netFrameworkInstallRoot == null) {
					netFrameworkInstallRoot = GetPathFromRegistry(@"SOFTWARE\Microsoft\.NETFramework", "InstallRoot") ?? string.Empty;
				}
				return netFrameworkInstallRoot;
			}
		}
		
		static string netSdk20InstallRoot = null;

		/// <summary>
		/// 从注册表的SOFTWARE\Microsoft\.NETFramework键中获取sdkInstallRootv2.0表示的.net sdk2.0的根路径
		/// </summary>
		public static string NetSdk20InstallRoot {
			get {
				if (netSdk20InstallRoot == null) {
					netSdk20InstallRoot = GetPathFromRegistry(@"SOFTWARE\Microsoft\.NETFramework", "sdkInstallRootv2.0") ?? string.Empty;
				}
				return netSdk20InstallRoot;
			}
		}
		
		static string windowsSdk60InstallRoot = null;

		/// <summary>
		/// 从注册表的SOFTWARE\Microsoft\Microsoft SDKs\Windows\v6.0键中获取InstallationFolder表示的.net sdk3.0的根路径
		/// </summary>
		public static string WindowsSdk60InstallRoot {
			get {
				if (windowsSdk60InstallRoot == null) {
					windowsSdk60InstallRoot = GetPathFromRegistry(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v6.0", "InstallationFolder") ?? string.Empty;
				}
				return windowsSdk60InstallRoot;
			}
		}
		
		static string windowsSdk60aInstallRoot = null;

		/// <summary>
		/// 从注册表的SOFTWARE\Microsoft\Microsoft SDKs\Windows\v6.0a键中获取InstallationFolder表示的vs2008中.net sdk3.5的根路径
		/// </summary>
		public static string WindowsSdk60aInstallRoot {
			get {
				if (windowsSdk60aInstallRoot == null) {
					windowsSdk60aInstallRoot = GetPathFromRegistry(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v6.0a", "InstallationFolder") ?? string.Empty;
				}
				return windowsSdk60aInstallRoot;
			}
		}
		
		static string windowsSdk61InstallRoot = null;

		/// <summary>
		/// 从注册表的SOFTWARE\Microsoft\Microsoft SDKs\Windows\v6.1键中获取InstallationFolder表示的.net sdk3.5的根路径
		/// </summary>
		public static string WindowsSdk61InstallRoot {
			get {
				if (windowsSdk61InstallRoot == null) {
					windowsSdk61InstallRoot = GetPathFromRegistry(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v6.1", "InstallationFolder") ?? string.Empty;
				}
				return windowsSdk61InstallRoot;
			}
		}
		
		#endregion
		
		/// <summary>
		/// 使用Path.Combine合并指定路径
		/// </summary>
		public static string Combine(params string[] paths)
		{
			if (paths == null || paths.Length == 0) {
				return String.Empty;
			}
			
			string result = paths[0];
			for (int i = 1; i < paths.Length; i++) {
				result = Path.Combine(result, paths[i]);
			}
			return result;
		}
		
		/// <summary>
		/// 使用序号排序规则从path中获取"://"的索引位置，如果>0则为url
		/// </summary>
		public static bool IsUrl(string path)
		{
			return path.IndexOf("://", StringComparison.Ordinal) > 0;
		}
		
		/// <summary>
		/// 获取顺序下，dir1与dir2的公共部分
		/// </summary>
		public static string GetCommonBaseDirectory(string dir1, string dir2)
		{
			if (dir1 == null || dir2 == null) return null;
			if (IsUrl(dir1) || IsUrl(dir2)) return null;
			
			dir1 = NormalizePath(dir1);
			dir2 = NormalizePath(dir2);
			
			string[] aPath = dir1.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			string[] bPath = dir2.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			StringBuilder result = new StringBuilder();
			int indx = 0;
			for(; indx < Math.Min(bPath.Length, aPath.Length); ++indx) {
				if (bPath[indx].Equals(aPath[indx], StringComparison.OrdinalIgnoreCase)) {
					if (result.Length > 0) result.Append(Path.DirectorySeparatorChar);
					result.Append(aPath[indx]);
				} else {
					break;
				}
			}
			if (indx == 0)
				return null;
			else
				return result.ToString();
		}
		
		/// <summary>
		/// 依次从高到低版本的.net sdk/bin目录下查找指定的exe文件路径
		/// </summary>
		/// <returns>各版本.net都不存在则返回null</returns>
		public static string GetSdkPath(string exeName) {
			string execPath;
			if (!string.IsNullOrEmpty(WindowsSdk61InstallRoot)) {
				execPath = Path.Combine(WindowsSdk61InstallRoot, "bin\\" + exeName);
				if (File.Exists(execPath)) { return execPath; }
			}
			if (!string.IsNullOrEmpty(WindowsSdk60aInstallRoot)) {
				execPath = Path.Combine(WindowsSdk60aInstallRoot, "bin\\" + exeName);
				if (File.Exists(execPath)) { return execPath; }
			}
			if (!string.IsNullOrEmpty(WindowsSdk60InstallRoot)) {
				execPath = Path.Combine(WindowsSdk60InstallRoot, "bin\\" + exeName);
				if (File.Exists(execPath)) { return execPath; }
			}
			if (!string.IsNullOrEmpty(NetSdk20InstallRoot)) {
				execPath = Path.Combine(NetSdk20InstallRoot, "bin\\" + exeName);
				if (File.Exists(execPath)) { return execPath; }
			}
			return null;
		}
		
		/// <summary>
		/// 从绝对路径转换为相对于指定基路径的相对路径。在两者不同目录上替换为".."
		/// </summary>
		public static string GetRelativePath(string baseDirectoryPath, string absPath)
		{
			if (IsUrl(absPath) || IsUrl(baseDirectoryPath)){
				return absPath;
			}
			
			baseDirectoryPath = NormalizePath(baseDirectoryPath);
			absPath           = NormalizePath(absPath);
			
			string[] bPath = baseDirectoryPath.Split(separators);
			string[] aPath = absPath.Split(separators);
			int indx = 0;
			for(; indx < Math.Min(bPath.Length, aPath.Length); ++indx){
				if(!bPath[indx].Equals(aPath[indx], StringComparison.OrdinalIgnoreCase))
					break;
			}
			
			if (indx == 0) {//无公共部分
				return absPath;
			}
			
			StringBuilder erg = new StringBuilder();
			
			if(indx == bPath.Length) {
//				erg.Append('.');
//				erg.Append(Path.DirectorySeparatorChar);
			} else {
				for (int i = indx; i < bPath.Length; ++i) {//不同位置则为上一级目录
					erg.Append("..");
					erg.Append(Path.DirectorySeparatorChar);
				}
			}
			erg.Append(String.Join(Path.DirectorySeparatorChar.ToString(), aPath, indx, aPath.Length-indx));
			return erg.ToString();
		}

		public static string GetAbsolutePath(string baseDirectoryPath, string relPath)
		{
			return NormalizePath(Path.Combine(baseDirectoryPath, relPath));
		}
		
		public static bool IsBaseDirectory(string baseDirectory, string testDirectory)
		{
			if (baseDirectory == null || testDirectory == null)
				return false;
			baseDirectory = NormalizePath(baseDirectory) + Path.DirectorySeparatorChar;
			testDirectory = NormalizePath(testDirectory) + Path.DirectorySeparatorChar;
			
			return testDirectory.StartsWith(baseDirectory, StringComparison.OrdinalIgnoreCase);
		}
		
		/// <summary>
		/// 将指定文件的基目录重命名为新目录，他只是使用Path.Combine合并新目录，并不是实际的重命名移动
		/// </summary>
		public static string RenameBaseDirectory(string fileName, string oldDirectory, string newDirectory)
		{
			fileName     = NormalizePath(fileName);
			oldDirectory = NormalizePath(oldDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
			newDirectory = NormalizePath(newDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
			if (IsBaseDirectory(oldDirectory, fileName)) {
				if (fileName.Length == oldDirectory.Length) {
					return newDirectory;
				}
				return Path.Combine(newDirectory, fileName.Substring(oldDirectory.Length + 1));
			}
			return fileName;
		}
		
		/// <summary>
		/// 递归深度拷贝原目录到目标目录，子目录深度拷贝
		/// </summary>
		public static void DeepCopy(string sourceDirectory, string destinationDirectory, bool overwrite)
		{
			if (!Directory.Exists(destinationDirectory)) {
				Directory.CreateDirectory(destinationDirectory);
			}
			foreach (string fileName in Directory.GetFiles(sourceDirectory)) {
				//拷贝文件
				File.Copy(fileName, Path.Combine(destinationDirectory, Path.GetFileName(fileName)), overwrite);
			}
			foreach (string directoryName in Directory.GetDirectories(sourceDirectory)) {
				//递归深度拷贝子目录
				DeepCopy(directoryName, Path.Combine(destinationDirectory, Path.GetFileName(directoryName)), overwrite);
			}
		}
		
		public static List<string> SearchDirectory(string directory, string filemask, bool searchSubdirectories, bool ignoreHidden)
		{
			List<string> collection = new List<string>();
			SearchDirectory(directory, filemask, collection, searchSubdirectories, ignoreHidden);
			return collection;
		}
		
		public static List<string> SearchDirectory(string directory, string filemask, bool searchSubdirectories)
		{
			return SearchDirectory(directory, filemask, searchSubdirectories, true);
		}
		
		public static List<string> SearchDirectory(string directory, string filemask)
		{
			return SearchDirectory(directory, filemask, true, true);
		}
		
		/// <summary>
		/// 在指定目录下查找符合filemask的子目录并存入collection列表中
		/// </summary>
		static void SearchDirectory(string directory, string filemask, List<string> collection, bool searchSubdirectories, bool ignoreHidden)
		{
			try {
				bool isExtMatch = Regex.IsMatch(filemask, @"^\*\..{3}$");
				string ext = null;
				string[] file = Directory.GetFiles(directory, filemask);
				if (isExtMatch) ext = filemask.Remove(0,1);
				
				foreach (string f in file) {
					if (ignoreHidden && (File.GetAttributes(f) & FileAttributes.Hidden) == FileAttributes.Hidden) {
						continue;
					}
					if (isExtMatch && Path.GetExtension(f) != ext) continue;
					
					collection.Add(f);
				}
				
				if (searchSubdirectories) {
					string[] dir = Directory.GetDirectories(directory);
					foreach (string d in dir) {
						if (ignoreHidden && (File.GetAttributes(d) & FileAttributes.Hidden) == FileAttributes.Hidden) {
							continue;
						}
						SearchDirectory(d, filemask, collection, searchSubdirectories, ignoreHidden);
					}
				}
			} catch (UnauthorizedAccessException) {
			}
		}

		public static readonly int MaxPathLength = 260;
		
		/// <summary>
		/// 路径是否有效，260路径长度是硬编码，目前没有发现.net属性标识
		/// </summary>
		public static bool IsValidPath(string fileName)
		{
			if (fileName == null || fileName.Length == 0 || fileName.Length >= MaxPathLength) {
				return false;
			}
			
			if (fileName.IndexOfAny(Path.GetInvalidPathChars()) >= 0) {
				return false;
			}
			if (fileName.IndexOf('?') >= 0 || fileName.IndexOf('*') >= 0) {
				return false;
			}
			
			if (!Regex.IsMatch(fileName, fileNameRegEx)) {//匹配文件名模式
				return false;
			}
			
			if(fileName[fileName.Length-1] == ' ') {
				return false;
			}
			
			if(fileName[fileName.Length-1] == '.') {
				return false;
			}
			
			string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			if (nameWithoutExtension != null) {
				nameWithoutExtension = nameWithoutExtension.ToUpperInvariant();
			}
			
			if (nameWithoutExtension == "CON" ||
			    nameWithoutExtension == "PRN" ||
			    nameWithoutExtension == "AUX" ||
			    nameWithoutExtension == "NUL") {
				return false;
			}
			
			char ch = nameWithoutExtension.Length == 4 ? nameWithoutExtension[3] : '\0';
			
			return !((nameWithoutExtension.StartsWith("COM") ||
			          nameWithoutExtension.StartsWith("LPT")) &&
			         Char.IsDigit(ch));//是否十进制
		}
		
		/// <summary>
		/// 检测单独的目录（非完整目录）是否有效
		/// </summary>
		public static bool IsValidDirectoryName(string name)
		{
			if (!IsValidPath(name)) {
				return false;
			}
			if (name.IndexOfAny(new char[]{Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar}) >= 0) {
				return false;
			}
			if (name.Trim(' ').Length == 0) {
				return false;
			}
			return true;
		}
		
		public static bool TestFileExists(string filename)
		{
			if (!File.Exists(filename)) {
				MessageService.ShowWarning(StringParser.Parse("${res:Fileutil.CantFindFileError}", new string[,] { {"FILE",  filename} }));
				return false;
			}
			return true;
		}
		
		public static bool IsDirectory(string filename)
		{
			if (!Directory.Exists(filename)) {
				return false;
			}
			FileAttributes attr = File.GetAttributes(filename);
			return (attr & FileAttributes.Directory) != 0;
		}

		/// <summary>
		/// 匹配通配符”？","*"???
		/// </summary>
		static bool MatchN (string src, int srcidx, string pattern, int patidx)
		{
			int patlen = pattern.Length;
			int srclen = src.Length;
			char next_char;

			for (;;) {
				if (patidx == patlen)
					return (srcidx == srclen);
				next_char = pattern[patidx++];
				if (next_char == '?') {
					if (srcidx == src.Length)
						return false;
					srcidx++;
				}
				else if (next_char != '*') {
					if ((srcidx == src.Length) || (src[srcidx] != next_char))
						return false;
					srcidx++;
				}
				else {
					if (patidx == pattern.Length)
						return true;
					while (srcidx < srclen) {
						if (MatchN(src, srcidx, pattern, patidx))
							return true;
						srcidx++;
					}
					return false;
				}
			}
		}

		static bool Match(string src, string pattern)
		{
			if (pattern[0] == '*') {
				int i = pattern.Length;
				int j = src.Length;
				while (--i > 0) {
					if (pattern[i] == '*')
						return MatchN(src, 0, pattern, 0);
					if (j-- == 0)
						return false;
					if ((pattern[i] != src[j]) && (pattern[i] != '?'))
						return false;
				}
				return true;
			}
			return MatchN(src, 0, pattern, 0);
		}

		/// <summary>
		/// 指定文件名是否匹配pattern
		/// </summary>
		/// <param name="pattern">可以使用";"分隔</param>
		public static bool MatchesPattern(string filename, string pattern)
		{
			filename = filename.ToUpper();
			pattern = pattern.ToUpper();
			string[] patterns = pattern.Split(';');
			foreach (string p in patterns) {
				if (Match(filename, p)) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 执行保存方法，并进行监视。如果保存出现异常，则根据FileErrorPolicy
		/// 策略的值使用Message服务执行日志记录
		/// </summary>
		/// <param name="saveFile">保存文件委托</param>
		/// <param name="fileName">文件名</param>
		/// <returns>文件操作枚举值</returns>
		public static FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName, string message, FileErrorPolicy policy)
		{
			System.Diagnostics.Debug.Assert(IsValidPath(fileName));
			try {
				saveFile();
				RaiseFileSaved(new FileNameEventArgs(fileName));
				return FileOperationResult.OK;
			} catch (Exception e) {
				switch (policy) {
					case FileErrorPolicy.Inform:
						ServiceManager.MessageService.InformSaveError(fileName, message, "${res:FileUtil.ErrorWhileSaving}", e);
						break;
					case FileErrorPolicy.ProvideAlternative:
						ChooseSaveErrorResult r = ServiceManager.MessageService.ChooseSaveError(fileName, message, "${res:FileUtil.ErrorWhileSaving}", e, false);
						if (r.IsRetry) {
							return ObservedSave(saveFile, fileName, message, policy);
						} else if (r.IsIgnore) {
							return FileOperationResult.Failed;
						}
						break;
				}
			}
			return FileOperationResult.Failed;
		}
		
		public static FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName, FileErrorPolicy policy)
		{
			return ObservedSave(saveFile,
			                    fileName,
			                    ResourceService.GetString("ECBC.Services.FileUtil.CantSaveFileStandardText"),
			                    policy);
		}
		
		public static FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName)
		{
			return ObservedSave(saveFile, fileName, FileErrorPolicy.Inform);
		}
		
		/// <summary>
		/// 如果指定目录不存在，则创建
		/// </summary>
		public static FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName, string message, FileErrorPolicy policy)
		{
			System.Diagnostics.Debug.Assert(IsValidPath(fileName));
			try {
				string directory = Path.GetDirectoryName(fileName);
				if (!Directory.Exists(directory)) {
					Directory.CreateDirectory(directory);
				}
				saveFileAs(fileName);
				RaiseFileSaved(new FileNameEventArgs(fileName));
				return FileOperationResult.OK;
			} catch (Exception e) {
				switch (policy) {
					case FileErrorPolicy.Inform:
						ServiceManager.MessageService.InformSaveError(fileName, message, "${res:FileUtil.ErrorWhileSaving}", e);
						break;
					case FileErrorPolicy.ProvideAlternative:
						ChooseSaveErrorResult r = ServiceManager.MessageService.ChooseSaveError(fileName, message, "${res:FileUtil.ErrorWhileSaving}", e, true);
						if (r.IsRetry) {
							return ObservedSave(saveFileAs, fileName, message, policy);
						} else if (r.IsIgnore) {
							return FileOperationResult.Failed;
						} else if (r.IsSaveAlternative) {
							return ObservedSave(saveFileAs, r.AlternativeFileName, message, policy);
						}
						break;
				}
			}
			return FileOperationResult.Failed;
		}
		
		public static FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName, FileErrorPolicy policy)
		{
			return ObservedSave(saveFileAs,
			                    fileName,
			                    ResourceService.GetString("ECBC.Services.FileUtil.CantSaveFileStandardText"),
			                    policy);
		}
		
		public static FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName)
		{
			return ObservedSave(saveFileAs, fileName, FileErrorPolicy.Inform);
		}
		
		/// <summary>
		/// 调用加载文件委托并执行监视，出现异常将根据策略类型调用消息服务记录日志
		/// </summary>
		/// <param name="loadFile">加载文件委托</param>
		/// <param name="fileName">文件名</param>
		/// <param name="message">消息</param>
		/// <param name="policy">策略</param>
		public static FileOperationResult ObservedLoad(FileOperationDelegate loadFile, string fileName, string message, FileErrorPolicy policy)
		{
			try {
				loadFile();
				OnFileLoaded(new FileNameEventArgs(fileName));
				return FileOperationResult.OK;
			} catch (Exception e) {
				switch (policy) {
					case FileErrorPolicy.Inform:
						ServiceManager.MessageService.InformSaveError(fileName, message, "${res:FileUtil.ErrorWhileLoading}", e);
						break;
					case FileErrorPolicy.ProvideAlternative:
						ChooseSaveErrorResult r = ServiceManager.MessageService.ChooseSaveError(fileName, message, "${res:FileUtil.ErrorWhileLoading}", e, false);
						if (r.IsRetry)
							return ObservedLoad(loadFile, fileName, message, policy);
						else if (r.IsIgnore)
							return FileOperationResult.Failed;
						break;
				}
			}
			return FileOperationResult.Failed;
		}
		
		public static FileOperationResult ObservedLoad(FileOperationDelegate loadFile, string fileName, FileErrorPolicy policy)
		{
			return ObservedLoad(loadFile,
			                    fileName,
			                    ResourceService.GetString("ECBC.Services.FileUtil.CantLoadFileStandardText"),
			                    policy);
		}
		
		public static FileOperationResult ObservedLoad(FileOperationDelegate loadFile, string fileName)
		{
			return ObservedLoad(loadFile, fileName, FileErrorPolicy.Inform);
		}
		
		public static FileOperationResult ObservedLoad(NamedFileOperationDelegate saveFileAs, string fileName, string message, FileErrorPolicy policy)
		{
			return ObservedLoad(new FileOperationDelegate(delegate { saveFileAs(fileName); }), fileName, message, policy);
		}
		
		public static FileOperationResult ObservedLoad(NamedFileOperationDelegate saveFileAs, string fileName, FileErrorPolicy policy)
		{
			return ObservedLoad(saveFileAs,
			                    fileName,
			                    ResourceService.GetString("ECBC.Services.FileUtil.CantLoadFileStandardText"),
			                    policy);
		}
		
		public static FileOperationResult ObservedLoad(NamedFileOperationDelegate saveFileAs, string fileName)
		{
			return ObservedLoad(saveFileAs, fileName, FileErrorPolicy.Inform);
		}
		
		/// <summary>
		/// 重新出发额外的FileLoaded事件
		/// </summary>
		/// <param name="e"></param>
		static void OnFileLoaded(FileNameEventArgs e)
		{
			if (FileLoaded != null) {
				FileLoaded(null, e);
			}
		}
		
		/// <summary>
		/// 重新触发额外的FileSaved事件
		/// </summary>
		/// <param name="e"></param>
		public static void RaiseFileSaved(FileNameEventArgs e)
		{
			if (FileSaved != null) {
				FileSaved(null, e);
			}
		}

        /// <summary>
        /// 获取文件名的规范版本，分隔符都将被backsslashes替换，"."和".."也会被计算
        /// </summary>
        public static string NormalizePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return fileName;

            int i;

            bool isWeb = false;
            for (i = 0; i < fileName.Length; i++)
            {
                if (fileName[i] == '/' || fileName[i] == '\\')
                    break;
                if (fileName[i] == ':')
                {
                    if (i > 1)
                        isWeb = true;
                    break;
                }
            }

            char outputSeparator = isWeb ? '/' : System.IO.Path.DirectorySeparatorChar;

            StringBuilder result = new StringBuilder();
            if (isWeb == false && fileName.StartsWith(@"\\") || fileName.StartsWith("//"))
            {
                i = 2;
                result.Append(outputSeparator);
            }
            else
            {
                i = 0;
            }
            int segmentStartPos = i;
            for (; i <= fileName.Length; i++)
            {
                if (i == fileName.Length || fileName[i] == '/' || fileName[i] == '\\')
                {
                    int segmentLength = i - segmentStartPos;
                    switch (segmentLength)
                    {
                        case 0:
                            if (isWeb || (i == 0 && Environment.OSVersion.Platform == PlatformID.Unix))
                            {
                                result.Append(outputSeparator);
                            }
                            break;
                        case 1:
                            if (fileName[segmentStartPos] != '.')
                            {
                                if (result.Length > 0) result.Append(outputSeparator);
                                result.Append(fileName[segmentStartPos]);
                            }
                            break;
                        case 2:
                            if (fileName[segmentStartPos] == '.' && fileName[segmentStartPos + 1] == '.')
                            {
                                int j;
                                for (j = result.Length - 1; j >= 0 && result[j] != outputSeparator; j--) ;
                                if (j > 0)
                                {
                                    result.Length = j;
                                }
                                break;
                            }
                            else
                            {
                                goto default;
                            }
                        default:
                            if (result.Length > 0) result.Append(outputSeparator);
                            result.Append(fileName, segmentStartPos, segmentLength);
                            break;
                    }
                    segmentStartPos = i + 1; 
                }
            }
            if (isWeb == false)
            {
                if (result.Length > 0 && result[result.Length - 1] == outputSeparator)
                {
                    result.Length -= 1;
                }
                if (result.Length == 2 && result[1] == ':')
                {
                    result.Append(outputSeparator);
                }
            }
            return result.ToString();
        }

        public static bool IsEqualFileName(string fileName1, string fileName2)
        {
            return string.Equals(NormalizePath(fileName1),
                                 NormalizePath(fileName2),
                                 StringComparison.OrdinalIgnoreCase);
        }

		public static event FileNameEventHandler FileLoaded;
		public static event FileNameEventHandler FileSaved;
	}

}
