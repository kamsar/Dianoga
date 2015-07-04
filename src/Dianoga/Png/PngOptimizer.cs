using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.Hosting;
using Sitecore.Resources.Media;

namespace Dianoga.Png
{
	// uses PngOptimizer to crunch PNGs - this is the fastest optimizer by far, and quite effective: http://psydk.org/pngoptimizer
	public class PngOptimizer : ExtensionBasedImageOptimizer
	{
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

		protected override string[] SupportedExtensions
		{
			get { return new[] { "png" }; }
		}

		public override IOptimizerResult Optimize(MediaStream stream)
		{
			IntPtr pngOptimizer = IntPtr.Zero;
			try
			{
				pngOptimizer = NativeMethods.LoadLibrary(_pathToDll);

				if(pngOptimizer == IntPtr.Zero) throw new Exception("Unable to load PNGOptimizer from " + _pathToDll);

				using (var memoryStream = new MemoryStream())
				{
					stream.Stream.CopyTo(memoryStream);
					byte[] imageBytes = memoryStream.ToArray();
					byte[] resultBytes = new byte[imageBytes.Length + 400000];
					int resultSize;

					IntPtr addressOfOptimize = NativeMethods.GetProcAddress(pngOptimizer, "PO_OptimizeFileMem");

					if(addressOfOptimize == IntPtr.Zero) throw new Exception("Can't find optimize funtion in PngOptimizerDll.dll!");

					OptimizeBytes optimizeMethod = (OptimizeBytes) Marshal.GetDelegateForFunctionPointer(addressOfOptimize, typeof (OptimizeBytes));

					bool success = optimizeMethod(imageBytes, imageBytes.Length, resultBytes, resultBytes.Length, out resultSize);

					var result = new PngOptimizerResult();
					result.Success = success;
					result.SizeBefore = imageBytes.Length;
					result.SizeAfter = resultSize;
					result.OptimizedBytes = resultBytes.Take(resultSize).ToArray();

					if (!result.Success)
					{
						IntPtr addressOfGetError = NativeMethods.GetProcAddress(pngOptimizer, "PO_GetLastErrorString");

						if (addressOfGetError == IntPtr.Zero) throw new Exception("Can't find get last error funtion in PngOptimizerDll.dll!");

						GetLastErrorString errorMethod = (GetLastErrorString) Marshal.GetDelegateForFunctionPointer(addressOfGetError, typeof (GetLastErrorString));

						result.ErrorMessage = errorMethod();
					}

					return result;
				}
			}
			finally
			{
				if (pngOptimizer != IntPtr.Zero) NativeMethods.FreeLibrary(pngOptimizer);
			}
		}

		static class NativeMethods
		{
			[DllImport("kernel32.dll")]
			public static extern IntPtr LoadLibrary(string dllToLoad);

			[DllImport("kernel32.dll")]
			public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);


			[DllImport("kernel32.dll")]
			public static extern bool FreeLibrary(IntPtr hModule);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate bool OptimizeBytes(byte[] image, int imageSize, [Out] byte[] result, int resultCapacity, out int resultSize);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private delegate string GetLastErrorString();
	}
}
