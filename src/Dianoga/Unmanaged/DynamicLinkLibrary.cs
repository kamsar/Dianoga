using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Web.Hosting;
using Sitecore.StringExtensions;

namespace Dianoga.Unmanaged
{
	public class DynamicLinkLibrary : IDisposable
	{
		private readonly string _tempPath;
		private readonly string _originalPathToDll;
		private readonly object _loaderLock = new object();
		private string _tempPathToDll;
		private IntPtr _pointerLibrary;

		public DynamicLinkLibrary(string pathToDll, string tempPath)
		{
			_tempPath = tempPath;
			_originalPathToDll = GetFilePath(pathToDll);

			_pointerLibrary = LoadModule();
		}

		private IntPtr LoadModule()
		{
			lock (_loaderLock)
			{
				if (_pointerLibrary != IntPtr.Zero) return _pointerLibrary;

				if (!File.Exists(_originalPathToDll))
				{
					throw new FileNotFoundException($"Unable to load DLL from {_originalPathToDll}");
				}

				_tempPathToDll = GetTempPathByConfig();

				CopyFile();

				_pointerLibrary = NativeMethods.LoadLibrary(_tempPathToDll);

				if (_pointerLibrary == IntPtr.Zero)
				{
					throw new Exception($"Unable to load DLL from {_originalPathToDll}");
				}

				return _pointerLibrary;
			}
		}

		private static string GetFilePath(string pathToDll)
		{
			return pathToDll.StartsWith("~") || pathToDll.StartsWith("/")
				? HostingEnvironment.MapPath(pathToDll)
				: pathToDll;
		}

		private string GetTempPathByConfig()
		{
			if (_tempPath.IsNullOrEmpty())
				return Path.GetTempFileName();

			var tempPathFixed = !_tempPath.EndsWith(@"\") ? _tempPath + @"\" : _tempPath;
			tempPathFixed = GetFilePath(tempPathFixed);

			return $@"{tempPathFixed}\{Path.GetRandomFileName()}";
		}

		private void CopyFile()
		{
			using (var stream = File.OpenRead(_originalPathToDll))
			using (var writeStream = new FileStream(_tempPathToDll, FileMode.Create, FileAccess.Write))
			{
				stream.Seek(0, SeekOrigin.Begin);
				stream.CopyTo(writeStream);
			}
		}

		private IntPtr LoadMethodOrFunction(string name)
		{
			var addressOfMethod = NativeMethods.GetProcAddress(_pointerLibrary, name);
			if (addressOfMethod == IntPtr.Zero)
			{
				throw new Exception("Can't find {functionName} funtion in {originalPathToDll}");
			}

			return addressOfMethod;
		}

		public object GetDelegateFunction(string functionName, Type type)
		{
			var addressOfMethod = LoadMethodOrFunction(functionName);
			var method = Marshal.GetDelegateForFunctionPointer(addressOfMethod, type);
			return method;
		}

		public void Dispose()
		{
			//Dispose the library
			if (_pointerLibrary != IntPtr.Zero)
			{
				NativeMethods.FreeLibrary(_pointerLibrary);
			}

			//Delete temporary File
			if (File.Exists(_tempPathToDll))
			{
				File.Delete(_tempPathToDll);
			}
		}
	}
}
