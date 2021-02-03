using System;
using System.Collections.Generic;
using System.Text;

namespace Maui.Core.App
{
	public static class Initializer
	{
		internal static MauiApplication Application { get; set; }

		static bool startupRan = false;

		public static void Startup<TStartup>(TStartup instance) where TStartup : IStartup
		{
			// Only run once especially with android
			if (startupRan)
				return;
			startupRan = true;

			var appBuilder = new MauiApplicationBuilder();

			instance.Configure(appBuilder);

			Application = (MauiApplication)appBuilder.Build();
		}
	}
}
