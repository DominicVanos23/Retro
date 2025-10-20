namespace MajokoRentals
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ------------------------
            // Add services to the container
            // ------------------------
            builder.Services.AddControllersWithViews();

            // Enable in-memory caching for session
            builder.Services.AddDistributedMemoryCache();

            // Enable session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // ✅ Register IHttpContextAccessor so it can be injected in views/controllers
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // ------------------------
            // Configure the HTTP request pipeline
            // ------------------------
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Enable session before authentication/authorization
            app.UseSession();

            // Add authentication middleware if you implement it later
            app.UseAuthorization();

            // Default route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
