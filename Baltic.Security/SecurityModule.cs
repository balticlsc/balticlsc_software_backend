using System.Text;
using System.Threading.Tasks;
using Baltic.Security.Auth;
using Baltic.Security.Utils;
using Baltic.Web.Interfaces;
using Baltic.Web.Module;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Baltic.Security
{
    public static class Parser 
    {
        public static bool ParseBoolean(string s, bool defaultValue)
        {
            if (bool.TryParse(s, out var result))
            {
                return result;
            }
            return defaultValue;
        }
    }
    
    public class SecurityModule : IModule
    {
        private IWebHostEnvironment _environment;
        private IConfiguration _configuration;
        
        public SecurityModule()
        {
        }
        
        public void AddModule(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _environment = environment;
            _configuration = configuration;
            
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    if (environment != null)
                    {
                        options.IncludeErrorDetails = environment.IsDevelopment();
                    }

                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = configuration["Bearer:Issuer"],
                        ValidateIssuer = true,

                        ValidAudience = configuration["Bearer:Issuer"],
                        ValidateAudience = true,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Bearer:Key"])),
                        ValidateIssuerSigningKey = true,

                        ValidateLifetime = true
                    };
                   
                    options.SecurityTokenValidators.Clear(); 
                    
                    var checkSessionId = !(environment.IsDevelopment() && !(Parser.ParseBoolean(configuration["Bearer:CheckSessionId"], true)));
                    
                    options.SecurityTokenValidators.Add(new CustomJwtSecurityTokenHandler(checkSessionId));
                })              
                .AddCookie(options =>
                {
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = (context) =>
                        {
                            context.HttpContext.Response.Redirect(configuration.GetValue<string>("SingleSignOnConfiguration:BaseUrl"));
                            return Task.CompletedTask;
                        }
                    };
                    
                    options.AccessDeniedPath = new PathString("/error/denied");
                    options.LogoutPath = new PathString("/login/signOut");
                });
            
            services.AddAuthorization(options =>
            {
                options.AddPolicies();
            });

            services.AddScoped<IUserSessionRoutine, UserRegistryRepository>();
        }

        public void UseModule(IApplicationBuilder app)
        {
            UserRegistryRepository.InitializeRepository((_environment != null && _environment.IsDevelopment()));
            
            app.UseAuthentication();
            app.UseAuthorization(); 
        }
    }
}