using System;
using System.Collections.Generic;
using System.Text;

namespace Maui.Core.App
{
	public partial class MauiApplication : IApplication
	{
		public MauiApplication()
		{
		}

#if __IOS__
		public List<IiOSApplicationDelegateHandler> PlatformLifecycleHandlers { get; set; } = new List<IiOSApplicationDelegateHandler>();
#elif __ANDROID__
		public List<IAndroidLifecycleHandler> PlatformLifecycleHandlers { get; set; } = new List<IAndroidLifecycleHandler>();
#endif
		public List<ICommonLifecycleHandler> CommonLifecycleHandlers {get;set; } = new List<ICommonLifecycleHandler>();
	}
}
