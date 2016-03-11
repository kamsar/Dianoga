using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.Hosting;

namespace Dianoga.Optimizers.Pipelines.DianogaPng
{
	// uses PngOptimizer to crunch PNGs - this is the fastest optimizer by far, and quite effective: http://psydk.org/pngoptimizer
	public class PngOptimizer : OptimizerProcessor
	{
		private readonly object _loaderLock = new object();

		private string _pathToDll;

		public string DllPath
		{
			get { return _pathToDll; }
			set
			{
				if (value.StartsWith("~") || value.StartsWith("/")) _pathToDll = HostingEnvironment.MapPath(value);
				else _pathToDll = value;
			}
		}

		protected override void ProcessOptimizer(OptimizerArgs args)
		{
			IntPtr pngOptimizer = GetOrLoadPngOptimizer();

			using (var memoryStream = new MemoryStream())
			{
				// buffer to a memory stream to operate on
				args.Stream.CopyTo(memoryStream);

				byte[] imageBytes = memoryStream.ToArray();
				byte[] resultBytes = new byte[imageBytes.Length + 400000]; // + 400kb in case optimization goes south and gets LARGER
				int resultSize;

				IntPtr addressOfOptimize = NativeMethods.GetProcAddress(pngOptimizer, "PO_OptimizeFileMem");

				if (addressOfOptimize == IntPtr.Zero) throw new Exception("Can't find optimize funtion in PngOptimizerDll.dll!");

				OptimizeBytes optimizeMethod = (OptimizeBytes)Marshal.GetDelegateForFunctionPointer(addressOfOptimize, typeof(OptimizeBytes));

				bool success = optimizeMethod(imageBytes, imageBytes.Length, resultBytes, resultBytes.Length, out resultSize);

				if (!success)
				{
					IntPtr addressOfGetError = NativeMethods.GetProcAddress(pngOptimizer, "PO_GetLastErrorString");

					if (addressOfGetError == IntPtr.Zero) throw new Exception("Can't find get last error funtion in PngOptimizerDll.dll!");

					GetLastErrorString errorMethod = (GetLastErrorString)Marshal.GetDelegateForFunctionPointer(addressOfGetError, typeof(GetLastErrorString));

					throw new InvalidOperationException($"PngOptimizerDll threw an error: {errorMethod()}");
				}

				args.IsOptimized = true;
				args.Stream = new MemoryStream(resultBytes.Take(resultSize).ToArray());
			}
		}

		private IntPtr _pngo;
		protected virtual IntPtr GetOrLoadPngOptimizer()
		{
			if (_pngo != IntPtr.Zero) return _pngo;

			lock (_loaderLock)
			{
				if (_pngo != IntPtr.Zero) return _pngo;

				if (!File.Exists(DllPath)) throw new FileNotFoundException("Unable to load PngOptimizerDll.dll from " + DllPath);

				var tempPath = Path.GetTempFileName();

				File.Copy(DllPath, tempPath, true);

				_pngo = NativeMethods.LoadLibrary(tempPath);

				if (_pngo == IntPtr.Zero) throw new Exception("Unable to load PNGOptimizer from " + DllPath);

				return _pngo;
			}
		}

		static class NativeMethods
		{
			[DllImport("kernel32.dll")]
			public static extern IntPtr LoadLibrary(string dllToLoad);

			[DllImport("kernel32.dll")]
			public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate bool OptimizeBytes(byte[] image, int imageSize, [Out] byte[] result, int resultCapacity, out int resultSize);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private delegate string GetLastErrorString();

		
	}
}
