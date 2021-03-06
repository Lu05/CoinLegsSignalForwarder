namespace CoinLegsSignalForwarder
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var path = FileSystemHelper.GetBaseDirectory();
            var builder = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<CustomExceptionHandlerMiddleware>();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors();

            app.UseEndpoints(endpoints => { endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}"); });
        }
    }
}
