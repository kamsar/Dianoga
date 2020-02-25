using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using nQuant;
using Sitecore.StringExtensions;

namespace Dianoga.Optimizers.Pipelines.DianogaPng
{
	// uses nQuant to quantize the PNG (reduces its color palette)
	// this method is lossy, unlike PngOptimizer, but results in additional file size savings
	// implementation is loosely based on Oliver Picton's fork: https://github.com/EnjoyDigital/Dianoga/commit/716eb6ad3c62bd7b1e6b672b2c22d363cbd25457
	public class PngQuantOptimizer : OptimizerProcessor
	{
		protected override void ProcessOptimizer(OptimizerArgs args)
		{
			var quantizer = new WuQuantizer();

			var memoryStream = new MemoryStream();

			using (var bitmap = new Bitmap(args.Stream))
			{
				var bitDepth = Image.GetPixelFormatSize(bitmap.PixelFormat);
				if (bitDepth != 32)
				{
					args.AddMessage("The PNG image you are attempting to quantize does not contain a 32 bit ARGB palette. This image has a bit depth of {0} with {1} colors. Skipping quantization.".FormatWith(bitDepth, bitmap.Palette.Entries.Length));
				}
				else
				{
					using (var quantized = quantizer.QuantizeImage(bitmap))
					{
						quantized.Save(memoryStream, ImageFormat.Png);
					}

					args.Stream.Dispose();
					args.Stream = memoryStream;
					args.IsOptimized = true;
				}
			}
		}
	}
}