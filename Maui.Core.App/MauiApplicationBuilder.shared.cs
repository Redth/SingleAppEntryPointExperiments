using System;
using System.Collections.Generic;
using System.Text;

namespace Maui.Core.App
{
	public class MauiApplicationBuilder : IApplicationBuilder
	{
		public IApplication Build()
		{
			var app = new MauiApplication();

#if __IOS__
			app.PlatformLifecycleHandlers = platformLifecycleHandlers;
#elif __ANDROID__
			app.PlatformLifecycleHandlers = platformLifecycleHandlers;
#endif

			app.CommonLifecycleHandlers = commonLifecycleHandlers;

			return app;
		}

#if __IOS__
		List<IiOSApplicationDelegateHandler> platformLifecycleHandlers { get; set; } = new List<IiOSApplicationDelegateHandler>();

		public void RegisterPlatformLifecycleHandler<TApplicationLifecycleHandler>() where TApplicationLifecycleHandler : IPlatformLifecycleHandler
		{
			var handler = (IiOSApplicationDelegateHandler)Activator.CreateInstance(typeof(TApplicationLifecycleHandler));
			platformLifecycleHandlers.Add(handler);
		}
#elif __ANDROID__
		List<IAndroidLifecycleHandler> platformLifecycleHandlers { get; set; } = new List<IAndroidLifecycleHandler>();

		public void RegisterPlatformLifecycleHandler<TAndroidLifecycleHandler>() where TAndroidLifecycleHandler : IPlatformLifecycleHandler
		{
			var handler = (IAndroidLifecycleHandler)Activator.CreateInstance(typeof(TAndroidLifecycleHandler));
			platformLifecycleHandlers.Add(handler);
		}
#else
		public void RegisterPlatformLifecycleHandler<TAndroidLifecycleHandler>() where TAndroidLifecycleHandler : IPlatformLifecycleHandler
		{
		}
#endif

			List<ICommonLifecycleHandler> commonLifecycleHandlers { get; set; } = new List<ICommonLifecycleHandler>();

		public void RegisterCommonLifecycleHandler<TCommonLifecycleHandler>() where TCommonLifecycleHandler : ICommonLifecycleHandler
		{
			var handler = (ICommonLifecycleHandler)Activator.CreateInstance(typeof(TCommonLifecycleHandler));
			commonLifecycleHandlers.Add(handler);
		}
	}
}
