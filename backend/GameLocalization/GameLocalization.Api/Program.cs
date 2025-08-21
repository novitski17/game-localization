using GameLocalization.Api.Extensions;

namespace GameLocalization.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.AddLogging();

            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);

            var app = builder.Build();

            startup.Configure(app, app.Environment);

            app.Run();
        }
    }
}