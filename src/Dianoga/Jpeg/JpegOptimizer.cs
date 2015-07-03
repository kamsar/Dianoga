using System.Diagnostics;
using System.IO;
using System.Web.Hosting;
using Sitecore.Resources.Media;
using Sitecore.StringExtensions;

namespace Dianoga.Jpeg
{
	// Squish JPEGs (strip exif, optimize coding) using jpegtran: http://jpegclub.org/jpegtran/
	public class JpegOptimizer : ExtensionBasedImageOptimizer
	{
		//jpegtran -optimize -progressive -copy none -outfile "<filename>" "<filename>"
		protected override string[] SupportedExtensions
		{
			get { return new[] {"jpg", "jpeg", "jfif", "jpe"}; }
		}

		public override IOptimizerResult Optimize(MediaStream stream)
		{
			var tempFilePath = GetTempFilePath();

			using (var fileStream = File.OpenWrite(tempFilePath))
			{
				stream.Stream.CopyTo(fileStream);
			}
			
			var result = new JpegOptimizerResult();

			result.SizeBefore = (int)new FileInfo(tempFilePath).Length;

			var jpegtran = Process.Start(ToolPath, "-optimize -copy none -progressive -outfile \"{0}\" \"{0}\"".FormatWith(tempFilePath));
			if (jpegtran != null && jpegtran.WaitForExit(ToolTimeout))
			{
				if (jpegtran.ExitCode != 0)
				{
					result.Success = false;
					result.ErrorMessage = "jpegtran exited with unexpected exit code " + jpegtran.ExitCode;
					return result;
				}

				result.Success = true;
				result.SizeAfter = (int)new FileInfo(tempFilePath).Length;

				// read the file to memory so we can nuke the temp file
				using (var fileStream = File.OpenRead(tempFilePath))
				{
					result.ResultStream = new MemoryStream();
					fileStream.CopyTo(result.ResultStream);
				}

				return result;
			}

			result.Success = false;
			result.ErrorMessage = "jpegtran took longer than {0} to execute, which we consider a failure.".FormatWith(ToolTimeout);

			return result;
		}

		protected virtual string GetTempFilePath()
		{
			return Path.GetTempFileName();
		}

		protected virtual string ToolPath { get { return HostingEnvironment.MapPath(@"~/Dianoga Tools/libjpeg/jpegtran.exe"); }}
		protected virtual int ToolTimeout { get { return 4000; } }
	}
}
