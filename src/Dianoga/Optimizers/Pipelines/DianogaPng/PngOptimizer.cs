using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Dianoga.Unmanaged;

namespace Dianoga.Optimizers.Pipelines.DianogaPng
{
	// uses PngOptimizer to crunch PNGs - this is the fastest optimizer by far, and quite effective: http://psydk.org/pngoptimizer
	public class PngOptimizer : OptimizerProcessor
	{
		public string DllPath { private get; set; }

		protected override void ProcessOptimizer(OptimizerArgs args)
		{
			using (var pngOptimizer = new DynamicLinkLibrary(DllPath))
			{
				using (var memoryStream = new MemoryStream())
				{
					// buffer to a memory stream to operate on
					args.Stream.CopyTo(memoryStream);

					var imageBytes = memoryStream.ToArray();
					var resultBytes = new byte[imageBytes.Length + 400000]; // + 400kb in case optimization goes south and gets LARGER
					int resultSize;

					var optimizeMethod = (OptimizeBytes)pngOptimizer.GetDelegateFunction("PO_OptimizeFileMem", typeof(OptimizeBytes));

					var success = optimizeMethod(imageBytes, imageBytes.Length, resultBytes, resultBytes.Length, out resultSize);

					if (!success)
					{
						var errorMethod = (GetLastErrorString)pngOptimizer.GetDelegateFunction("PO_GetLastErrorString", typeof(GetLastErrorString));
						throw new InvalidOperationException($"PngOptimizerDll threw an error: {errorMethod()}");
					}

					args.IsOptimized = true;
					args.Stream = new MemoryStream(resultBytes.Take(resultSize).ToArray());
				}
			}
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate bool OptimizeBytes(byte[] image, int imageSize, [Out] byte[] result, int resultCapacity, out int resultSize);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private delegate string GetLastErrorString();

	}
}
