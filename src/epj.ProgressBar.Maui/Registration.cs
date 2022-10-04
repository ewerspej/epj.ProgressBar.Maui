namespace epj.ProgressBar.Maui;

public static class Registration
{
    public static MauiAppBuilder UseProgressBar(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(h =>
        {
            h.AddHandler<ProgressBar, ProgressBarHandler>();
        });

        return builder;
    }
}