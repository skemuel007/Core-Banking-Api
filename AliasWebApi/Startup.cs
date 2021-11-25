using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using AliasWebApi.Models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.WebSockets;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading;
using AliasWebApiCore.Filters;
using AliasWebApiCore.Middleware;
using AliasWebApiCore.Models;
using Microsoft.AspNetCore.Http;
using AliasWebApiCore.Models.Identity;
using AliasWebApiCore.Services;
using AliasWebApiCore.Signal_R;
using AutoMapper;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Swashbuckle.AspNetCore.Swagger;

namespace AliasWebApiCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            //Log.Logger = new LoggerConfiguration()
            //    //.MinimumLevel.Information()
            //    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            //    .MinimumLevel.Override("System", LogEventLevel.Warning)
            //    //.WriteTo.Seq("http://localhost:5341/")
            //    //.WriteTo.RollingFile(new JsonFormatter(), Path.Combine("Logs", "log-{Date}.json"))
            //    .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Identity Db Context
            services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("AppDbContext")));
            ////Alias Db Context
            //services.AddDbContext<AslBankingDbContext>(options =>
            //options.UseSqlServer(Configuration.GetConnectionString("AslBankingDbContext")));

            //Add Identity and Jwt
            services.AddIdentity<ApplicationUser,IdentityRole>(option =>
            {
                option.Password.RequireDigit = false;
                option.Password.RequiredLength = 3;
                option.Password.RequiredUniqueChars = 0;
                option.Password.RequireLowercase = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireUppercase = false;

                // Lockout settings
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                option.Lockout.MaxFailedAccessAttempts = 3;
                option.Lockout.AllowedForNewUsers = true;

                // User settings
                //option.User.RequireUniqueEmail = true;
                //option.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>()
              .AddDefaultTokenProviders();
            services.AddAuthentication( o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                } /*Uncomment this if you don't want to use JWT for all of your api*/)
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = "http://localhost:65300",
                        ValidAudience = "http://localhost:65300",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("qwertyuiopasdfghjklzxcvbnm123456")),
                    };
                });
            services.AddSignalR();
            services.AddResponseCompression();
            services.AddDataProtection();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAny", a =>
                {
                    a.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin()
                        .AllowCredentials()
                        .SetPreflightMaxAge(TimeSpan.FromSeconds(5000));
                });
            });
            //services.AddMvc()
            //    .AddJsonOptions(options =>
            //    {
            //        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            //        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //    });
            services.AddMvc(o =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                o.Filters.Add(new AuthorizeFilter(policy));
            })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
            services.AddAuthorization(options => options.AddPolicy("Admin", policy => policy.RequireClaim("CanAssignRoles", "true")));
            services.AddOptions();
            //services.Configure<TokenOptions>(Configuration.GetSection("TokenOptions"));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Alias Web Api", Version = "v1" });
                // Swagger 2.+ support
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Bearer Authentication",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(security);
            });
            //services.AddTransient<IEmailSender, EmailSender>();
            //services.Configure<AuthMessageSenderOptions>(Configuration);
            //call this in case you need aspnet-user-authtype/aspnet-user-identity
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("AslBankingDbContext")));
            services.AddTransient<ITransactionService, TransactionService>();
            //services.AddTransient<IScheduler, Scheduler>();
            //services.AddTransient<IUserInfoInMemory, UserInfoInMemory>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, /*IScheduler scheduler,*/ILoggerFactory loggerFactory)
        {
            app
                .Use(async (context, next) =>
                {
                    if (string.IsNullOrWhiteSpace(context.Request.Headers["Authorization"]))
                    {
                        try
                        {
                            if (context.Request.QueryString.HasValue)
                            {
                                var token = context.Request.QueryString.Value
                                    .Split('&')
                                    .SingleOrDefault(x =>
                                    {
                                        var i = x.IndexOf("t=", StringComparison.CurrentCultureIgnoreCase);
                                        return i > -1 && i < 2;
                                    })?
                                    .Split('=')
                                    .LastOrDefault();

                                if (!string.IsNullOrWhiteSpace(token) &&
                                    string.IsNullOrWhiteSpace(context.Request.Headers["Authorization"]))
                                {

                                    context.Request.Headers.Add("Authorization", new[] { $"Bearer {token}" });
                                    context.Request.Headers.Remove("Origin");
                                    context.Request.Headers.Add("Origin","*");
                                }
                            }
                        }
                        catch
                        {
                            // if multiple headers it may throw an error.  Ignore both.
                        }
                    }
                    await next();
                });
            //loggerFactory.AddSerilog();
            app.UseCors("AllowAny");
            app.UseResponseCompression();   
            app.UseDeveloperExceptionPage();
            app.UseBrowserLink();
            app.UseDatabaseErrorPage();
            app.UseStaticFiles(); // For the wwwroot folder
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
                RequestPath = "/Files",
                EnableDirectoryBrowsing = false
            });
            app.UseStatusCodePages();
            app.UseAuthentication();
            app.UseSignalR(routes =>
            {
                routes.MapHub<Message>("chat");
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Alias Web Api");
            });
            //app.UseHangfireDashboard();
            //app.UseHangfireDashboard("/hangfire", new DashboardOptions
            //{
            //    Authorization = new[] { new HangfireAuthorizationFilter() }
            //});
            //app.UseHangfireServer();
            //BackgroundJob.Enqueue(() => scheduler.DiscoverServices(null));
            app.UseMvcWithDefaultRoute();
        }
    }
}
