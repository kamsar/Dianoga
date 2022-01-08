﻿using System.Web;
using Sitecore.Resources.Media;

namespace Dianoga.NextGenFormats
{
	public static class Extensions
	{
		static Extensions()
		{
			Helpers = new Helpers();
		}

		private static readonly Helpers Helpers;
		public static void AddCustomOptions(this MediaRequest request, HttpContextBase context)
		{
			var customExtension = Helpers.GetCustomOptions(context);
			if (!string.IsNullOrEmpty(customExtension))
			{
				request.Options.CustomOptions["extension"] = customExtension;
			}
		}

		public static bool CheckSupportedFormat(this HttpContextBase context, string extension)
		{
			return Helpers.CheckSupportedFormat(context, extension);
		}
	}
}
