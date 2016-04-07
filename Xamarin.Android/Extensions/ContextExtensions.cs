using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using MySIT.Mobile.Droid.Helper;

namespace MySIT.Mobile.Droid.Extensions
{
    /// <summary>
    /// Provides extensions for <see cref="Context" /> instances.
    /// </summary>
    public static class ContextExtensions
    {
        ///// <summary>
        ///// Gets the <see cref="Android.Support.V7.App.AppCompatDelegate" /> from a <see cref="Context" />.
        ///// </summary>
        ///// <param name="context">The context.</param>
        ///// <returns>Returns a <see cref="Android.Support.V7.App.AppCompatDelegate" />.</returns>
        //public static AppCompatDelegate GetAppCompatDelegate(this Context context)
        //{
        //    return context.GetAppCompatDelegateProvider().AppCompatDelegate;
        //}

        ///// <summary>
        ///// Gets a <see cref="IAppCompatDelegateProvider" /> from a <see cref="Context" />.
        ///// </summary>
        ///// <param name="context">The context.</param>
        ///// <returns>Returns a <see cref="IAppCompatDelegateProvider" />.</returns>
        ///// <exception cref="System.InvalidOperationException">Could not cast $IAppCompatDelegateProvider$ interface from the provided context.</exception>
        //public static IAppCompatDelegateProvider GetAppCompatDelegateProvider(this Context context)
        //{
        //    var provider = context as IAppCompatDelegateProvider;

        //    if (provider == null)
        //    {
        //        throw new InvalidOperationException("Could not cast IAppCompatDelegateProvider interface from the provided context.");
        //    }

        //    return provider;
        //}

        ///// <summary>
        ///// Gets a themed <see cref="Context" />.
        ///// </summary>
        ///// <param name="context">The context.</param>
        ///// <returns>Returns a <see cref="Context" />.</returns>
        //public static Context GetAppCompatThemedContext(this Context context)
        //{
        //    return context.GetAppCompatDelegate().SupportActionBar.ThemedContext;
        //}

        ///// <summary>
        ///// Gets the support action bar.
        ///// </summary>
        ///// <param name="context">The context.</param>
        ///// <returns>Returns a <see cref="Android.App.ActionBar" />.</returns>
        //public static ActionBar GetSupportActionBar(this Context context)
        //{
        //    return context.GetAppCompatDelegate().SupportActionBar;
        //}

        public static bool IsTablet(this Context context)
        {
            return (((int)context.Resources.Configuration.ScreenLayout) & ((int)ScreenLayout.SizeMask)) >= (int)ScreenLayout.SizeLarge;
        }

        public static int ConvertToDp(this Context context, int px)
        {
            return (int)Math.Ceiling(px / context.Resources.DisplayMetrics.Density);
        }

        public static int ConvertToPixel(this Context context, int dp)
        {
            return (int)Math.Ceiling(dp * context.Resources.DisplayMetrics.Density);
        }
    }
}