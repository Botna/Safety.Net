namespace WatchMe.Helpers
{
    public static class CurrentServiceProvider
    {
        public static IServiceProvider Services
        {
            get
            {
                IPlatformApplication? app = IPlatformApplication.Current;
                if (app == null)
                    throw new InvalidOperationException("Cannot resolve current application. Services should be accessed after MauiProgram initialization.");
                return app.Services;
            }
        }
    }
}
