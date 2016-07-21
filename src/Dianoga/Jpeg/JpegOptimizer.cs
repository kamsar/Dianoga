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
		private string _pathToExe;

		public string ExePath
		{
			get { return _pathToExe; }
			set
			{
				if (value.StartsWith("~") || value.StartsWith("/")) _pathToExe = HostingEnvironment.MapPath(value);
				else _pathToExe = value;
			}
		}

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

			result.SizeBefore = (int)stream.Length;

			var jpegtran = Process.Start(ExePath, "-optimize -copy none -progressive -outfile \"{0}\" \"{0}\"".FormatWith(tempFilePath));
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
					result.ResultStream.Seek(0, SeekOrigin.Begin);
				}

				File.Delete(tempFilePath);

				return OptimizationSuccessful(result);
			}

			result.Success = false;
			result.ErrorMessage = "jpegtran took longer than {0} to execute, which we consider a failure.".FormatWith(ToolTimeout);
            // kill the process and clean up the tempfile as we have discarded the optimizer
            jpegtran.Kill();
            try
            {
                File.Delete(tempFilePath);
            } catch(IOException)
            {
                // Silently discard any io errors in the file deletion...
            }

			return result;
		}

		protected virtual string GetTempFilePath()
		{
			return Path.GetTempFileName();
		}

		protected virtual int ToolTimeout { get { return 60000; } }
	}
}
