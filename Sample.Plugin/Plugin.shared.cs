#if __ANDROID__
using Android.OS;
using AndroidX.AppCompat.App;
#endif
using Maui.Core.App;
using System;
using System.Collections.Generic;
using System.Text;
#if __IOS__
using Foundation;
using UIKit;
using ObjCRuntime;
#endif

namespace Sample.Plugin
{
	public static class CoolPluginExtensions
	{
		public static void UseCoolPlugin(this IApplicationBuilder app)
		{
#if __IOS__
			app.RegisterPlatformLifecycleHandler<CoolPluginDelegateHandler>();
#elif __ANDROID__
			app.RegisterPlatformLifecycleHandler<CoolPluginAndroidLifecycleHandler>();
#endif
		}
	}

#if __IOS__
	public class CoolPluginDelegateHandler : iOSApplicationDelegateHandler
	{
		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			Console.WriteLine("Hello from cool plugin!");
			return true;
		}
	}
#elif __ANDROID__
	public class CoolPluginAndroidLifecycleHandler : AndroidLifecycleHandler
	{
		public override void ActivityOnCreate(global::Android.App.Activity activity, Bundle savedInstanceState)
		{
			base.ActivityOnCreate(activity, savedInstanceState);

			Console.WriteLine("Hello from cool plugin!");
		}
	}
#endif
}
