using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Sitecore.Resources.Media;

namespace Dianoga.Png
{
	// uses PngOptimizer to crunch PNGs - this is the fastest optimizer by far, and quite effective: http://psydk.org/pngoptimizer
	public class PngOptimizer : ExtensionBasedImageOptimizer
	{
		protected override string[] SupportedExtensions
		{
			get { return new[] { "png" }; }
		}

		public override IOptimizerResult Optimize(MediaStream stream)
		{
			using (var memoryStream = new MemoryStream())
			{
				stream.Stream.CopyTo(memoryStream);
				byte[] imageBytes = memoryStream.ToArray();
				byte[] resultBytes = new byte[imageBytes.Length + 400000];
				int resultSize;

				bool success = OptimizeBytes(imageBytes, imageBytes.Length, resultBytes, resultBytes.Length, out resultSize);

				var result = new PngOptimizerResult();
				result.Success = success;
				result.SizeBefore = imageBytes.Length;
				result.SizeAfter = resultSize;
				result.OptimizedBytes = resultBytes.Take(resultSize).ToArray();

				if (!result.Success) result.ErrorMessage = GetLastErrorString();

				return result;
			}
		}

		[DllImport(@"../Dianoga Tools/PNGOptimizer/PngOptimizerDll.dll", EntryPoint = "PO_OptimizeFileMem")]
		private static extern bool OptimizeBytes(byte[] image, int imageSize, [Out] byte[] result, int resultCapacity, out int resultSize);

		[DllImport(@"../Dianoga Tools/PNGOptimizer/PngOptimizerDll.dll", EntryPoint = "PO_GetLastErrorString")]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private static extern string GetLastErrorString();
	}
}
