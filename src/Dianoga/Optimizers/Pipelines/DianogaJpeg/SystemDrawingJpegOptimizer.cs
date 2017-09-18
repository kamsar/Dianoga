using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Dianoga.Optimizers.Pipelines.DianogaJpeg
{
	public class SystemDrawingJpegOptimizer : OptimizerProcessor
	{
		public virtual int StartQuality { get; set; }
		public virtual int QualityStep { get; set; }
		public virtual int SizeFactor { get; set; }

		protected override void ProcessOptimizer(OptimizerArgs args)
		{
			string finalPath = null;
			try
			{
				finalPath = DoProcessOptimizer(args);

				args.IsOptimized = true;
				using (var fileStream = File.OpenRead(finalPath))
				{
					args.Stream = new MemoryStream();
					fileStream.CopyTo(args.Stream);
				}
			}
			catch (Exception ex)
			{
				Sitecore.Diagnostics.Log.Error(ex.Message, ex, this);
			}
			finally
			{
				if (finalPath != null)
				{
					CleanUpTempFile(finalPath);
				}
			}
		}

		private string DoProcessOptimizer(OptimizerArgs args)
		{
			var tempInputPath = GetTempFilePath();
			var tempOutputPath = GetTempFilePath();
			long originalStreamLength = args.Stream.Length;
			SaveStreamToPath(args.Stream, tempInputPath);

			using (var img = Image.FromFile(tempInputPath))
			{
				int target = GetTargetSize(img);

				if (originalStreamLength <= target)
				{
					Sitecore.Diagnostics.Log.Info(string.Format("SystemDrawingJpegOptimizer: Skipping, {0} < {1}", originalStreamLength, target), this);
					return tempInputPath;
				}

				var encoder = GetEncoderInfo("image/jpeg");
				var encoderParams = new EncoderParameters();
				
				for (var quality = StartQuality; quality > 0; quality -= QualityStep)
				{
					encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);

					img.Save(tempOutputPath, encoder, encoderParams);
					var info = new FileInfo(tempOutputPath);
					if (info.Length <= target)
					{
						Sitecore.Diagnostics.Log.Info(string.Format("SystemDrawingJpegOptimizer: Optimized using quality {0}, {1} < {2}", quality, info.Length, target), this);
						break;
					}
				}
			}

			CleanUpTempFile(tempInputPath);

			return tempOutputPath;
		}

		private void SaveStreamToPath(Stream inputStream, string outputPath)
		{
			using (var outputStream = File.OpenWrite(outputPath))
			{
				inputStream.CopyTo(outputStream);
				inputStream.Dispose();
			}
		}

		private int GetTargetSize(Image img)
		{
			int max = Math.Max(img.Width, img.Height);

			return max * SizeFactor;
		}

		private void CleanUpTempFile(string path)
		{
			if (File.Exists(path)) File.Delete(path);
		}

		private static ImageCodecInfo GetEncoderInfo(string mimeType)
		{
			return ImageCodecInfo.GetImageEncoders()
				.Where(x => x.MimeType == mimeType)
				.FirstOrDefault();
		}

		protected virtual string GetTempFilePath()
		{
			return Path.GetTempFileName();
		}
	}
}
