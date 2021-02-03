using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Sample.iOSApp
{
	public partial class AppDelegate : Maui.Core.App.MauiApplicationDelegate
	{
		public override void PerformActionForShortcutItem(UIApplication application, UIApplicationShortcutItem shortcutItem, UIOperationHandler completionHandler)
		{
			base.PerformActionForShortcutItem(application, shortcutItem, completionHandler);

			Console.WriteLine("Action");
		}
	}
}