namespace GameLocalization.Api.Extensions
{
    public static class LoggingExtensions
    {
        public static ILoggingBuilder AddLogging(this ILoggingBuilder logging)
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.AddDebug();

            return logging;
        }
    }
}
