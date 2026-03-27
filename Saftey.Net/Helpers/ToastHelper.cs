using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace WatchMe.Helpers
{
    public static class ToastHelper
    {

        public static Task CreateToast(string message, ToastDuration duration = ToastDuration.Long)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            double fontSize = 14;

            var toast = Toast.Make(message, duration, fontSize);

            return toast.Show(cancellationTokenSource.Token);
        }
    }
}
