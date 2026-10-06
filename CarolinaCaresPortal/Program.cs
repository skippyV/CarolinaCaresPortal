using CarolinaCaresPortal.Components;
using CarolinaCaresPortal.Data;
using CarolinaCaresPortal.Services;
using CarolinaCaresPortal.Shared;
using Radzen;
using Serilog;

namespace CarolinaCaresPortal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddRadzenComponents();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            Log.Logger.Warning("Carolina Cares Portal - Program.c - logging test with Warning level.");

            MongoDbConfig? mongoDbConfig = builder.Configuration.GetSection(nameof(MongoDbConfig)).Get<MongoDbConfig>();
            if (mongoDbConfig == null)
            {
                Log.Fatal("MongoDbConf is NULL!");
                return;
            }

            // But I want an input parameter to the constructor
            // https://cmatskas.com/net-core-dependency-injection-with-constructor-parameters-2/
            builder.Services.AddScoped<IMongoDbAccessService>(s => new MongoDbAccessService(mongoDbConfig));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            bool isConnected = CheckMongoDbServiceConnection(app);

            if (!isConnected)
            {
                Log.Error("Check to ensure MongoDB service is running!");
                Environment.Exit(-1);
            }

            app.Run();
        }

        private static bool CheckMongoDbServiceConnection(WebApplication app)
        {
            bool isConnected = false;

            using (var scope = app.Services.CreateScope())
            {
                // https://stackoverflow.com/questions/77904510/how-do-you-initialize-a-blazor-server-application-database-with-admin-user-on-ve

                IServiceProvider services = scope.ServiceProvider;
                IMongoDbAccessService _iMongoDbAccessService = services.GetRequiredService<IMongoDbAccessService>();

                ResponseStatus response = _iMongoDbAccessService.IsMongoDbConnectionActive();
                if(response is not null)
                {
                    isConnected = response.Success;
                }
            }

            return isConnected;
        }
    }
}
