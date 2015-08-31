using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using nQuant;
using Sitecore.Resources.Media;
using Sitecore.StringExtensions;

namespace Dianoga.Png
{
	// uses nQuant to quantize the PNG (reduces its color palette)
	// this method is lossy, unlike PngOptimizer, but results in additional file size savings
	// implementation is loosely based on Oliver Picton's fork: https://github.com/EnjoyDigital/Dianoga/commit/716eb6ad3c62bd7b1e6b672b2c22d363cbd25457
	public class PngQuantOptimizer : ExtensionBasedImageOptimizer
	{
		protected override string[] SupportedExtensions
		{
			get { return new[] { "png" }; }
		}

		public override IOptimizerResult Optimize(MediaStream stream)
		{
			var quantizer = new WuQuantizer();

			var memoryStream = new MemoryStream();

			using (var bitmap = new Bitmap(stream.Stream))
			{
				var bitDepth = Image.GetPixelFormatSize(bitmap.PixelFormat);
				if (bitDepth != 32)
				{
					return OptimizerFailureResult("the image you are attempting to quantize does not contain a 32 bit ARGB palette. This image has a bit depth of {0} with {1} colors".FormatWith(bitDepth, bitmap.Palette.Entries.Length));
				}

				using (var quantized = quantizer.QuantizeImage(bitmap))
				{
					quantized.Save(memoryStream, ImageFormat.Png);
				}
				
				// rewind the stream
				memoryStream.Seek(0, SeekOrigin.Begin);

				var result = new PngQuantOptimizerResult();
				result.Success = true;
				result.SizeBefore = (int)stream.Length;
				result.SizeAfter = (int)memoryStream.Length;
				result.ResultStream = memoryStream;

				return OptimizationSuccessful(result);
			}
		}

		private IOptimizerResult OptimizerFailureResult(string message)
		{
			var result = new PngQuantOptimizerResult()
			{
				Success = false,
				ErrorMessage = message
			};
			return result;
		}
	}
}