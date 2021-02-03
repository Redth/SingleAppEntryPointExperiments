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

using Sample.Plugin;

[assembly: MauiStartup(typeof(Sample.Shared.Startup))]

namespace Sample.Shared
{
	public class Startup : IStartup
	{
		public void Configure(IApplicationBuilder app)
		{
			// Gives the referenced plugin/nuget/library a hook
			// where it can wire up its own lifecycle handlers
			app.UseCoolPlugin();

			app.RegisterCommonLifecycleHandler<MyCommonLifecycleHandler>();

			#region You can also manually handle platform specifics in your app
#if __IOS__
			app.RegisterPlatformLifecycleHandler<MyAppDelegateHandler>();
#elif __ANDROID__
			app.RegisterPlatformLifecycleHandler<MyAndroidLifecycleHandler>();
#endif
			#endregion
		}
	}


	public class MyCommonLifecycleHandler : CommonLifecycleHandler
	{
		public override void OnCreate(MauiApplication app)
		{
			base.OnCreate(app);
		}
	}

	#region Manual platform specific lifecycle handlers
#if __IOS__
	public class MyAppDelegateHandler : iOSApplicationDelegateHandler
	{
		public UIWindow Window { get; set; }

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			Console.WriteLine("Done launching");
			// create a new window instance based on the screen size
			Window = new UIWindow(UIScreen.MainScreen.Bounds);
			Window.RootViewController = new UIViewController();

			// make the window visible
			Window.MakeKeyAndVisible();

			return true;
		}
	}
#elif __ANDROID__
	public class MyAndroidLifecycleHandler : AndroidLifecycleHandler
	{
		public override void ActivityOnCreate(global::Android.App.Activity activity, Bundle savedInstanceState)
		{
			base.ActivityOnCreate(activity, savedInstanceState);

			Console.WriteLine("Hello!");
		}
	}
#endif
	#endregion
}
