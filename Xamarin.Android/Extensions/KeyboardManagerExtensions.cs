using System;
using Android.OS;
using Android.Views.InputMethods;
using Android.Widget;
using Xamarin.Forms;

namespace MySIT.Mobile.Droid.Extensions
{
    public static class KeyboardManagerExtensions
    {
        public static void ShowKeyboard(this Android.Views.View inputView)
        {
            using (var inputMethodManager = (InputMethodManager)Forms.Context.GetSystemService("input_method"))
            {
                if (!(inputView is EditText) && !(inputView is TextView) && !(inputView is SearchView))
                    throw new ArgumentException("inputView should be of type EditText, SearchView, or TextView");
                inputMethodManager.ShowSoftInput(inputView, ShowFlags.Forced);
                inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
            }
        }

        public static void HideKeyboard(this Android.Views.View inputView)
        {
            using (var inputMethodManager = (InputMethodManager)Forms.Context.GetSystemService("input_method"))
            {
                var windowToken = (IBinder) null;
                if (inputView is EditText)
                    windowToken = inputView.WindowToken;
                else if (inputView is TextView)
                    windowToken = inputView.WindowToken;
                else if (inputView is SearchView)
                    windowToken = inputView.WindowToken;
                if (windowToken == null)
                    throw new ArgumentException("inputView should be of type EditText, SearchView, or TextView");
                inputMethodManager.HideSoftInputFromWindow(windowToken, HideSoftInputFlags.None);
            }
        }
    }
}