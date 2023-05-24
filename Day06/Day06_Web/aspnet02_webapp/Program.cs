using aspnet02_boardapp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;

namespace aspnet02_boardapp
{
    public class Program
    {
        // ASP.NET 실행을 위한 구성 초기화
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            // Data에서 만든 ApplicationDbContext를 사용하겠다는 설정 추가
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(
                // appsettings.json ConnectionStrings 내 연결 문자열 할당
                builder.Configuration.GetConnectionString("DefaultConnection"),
                // 연결 문자열로 DB의 서버 버전 자동으로 가져옴
                ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
            ));
            // 2. ASP.NET Identity - ASP.NET 계정을 위한 서비스 설정
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // 비밀번호 정책 변경 설정
            builder.Services.Configure<IdentityOptions>(Options =>
            {
                // Custom Password policy
                Options.Password.RequireDigit = false; // 영문자 필요여부
                Options.Password.RequireLowercase = false; // 소문자 필요여부
                Options.Password.RequireUppercase = false; // 대문자 필요여부
                Options.Password.RequireNonAlphanumeric = false; // 특수문자 필요여부
                Options.Password.RequiredLength = 4; // 최소 패스워드 길이수
                Options.Password.RequiredUniqueChars = 0; // 암호 고유문자 갯수
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();    // 3. ASP.NET Identity - 계정추가
            app.UseAuthorization();     // 권한

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}