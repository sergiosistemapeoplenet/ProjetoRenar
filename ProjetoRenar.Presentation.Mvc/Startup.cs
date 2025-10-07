using System;
using System.Globalization;
using DNTCaptcha.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ProjetoRenar.Application.Profiles;
using ProjetoRenar.Application.ViewModels.Usuarios;
using ProjetoRenar.CrossCutting.Configurations;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Presentation.Mvc.Filters;
using Sidetech.Framework.Cryptography;

namespace ProjetoRenar.Presentation.Mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDNTCaptcha(options =>
                options.UseCookieStorageProvider().ShowThousandsSeparators(false)
            );

            DependencyInjection.Register(services);
            AutoMapperConfig.Register();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Autenticação com cookie persistente
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            // Configuração do cookie de autenticação
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(365); // 1 ano
                options.SlidingExpiration = true; // renova a cada requisição
                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/logout";
            });

            // Sessão HTTP
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(365); // 1 ano sem expirar
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(new ConfigurationActionFilter());
                options.Filters.Add(new PerfilActionFilter());
            }).AddSessionStateTempDataProvider();

            services.AddHsts(options =>
            {
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
                options.Cookie.Name = "XSRF-TOKEN";
                options.Cookie.HttpOnly = false;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var cultureInfo = new CultureInfo("pt-BR");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                await next.Invoke();
            });

            app.UseMiddleware<LoginAttemptsMiddleware>();
            app.UseMiddleware<RemoveScriptMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error/500");
            }

            app.UseHsts();
            app.UseHttpsRedirection();

            app.Use(async (ctx, next) =>
            {
                await next();

                if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
                {
                    string originalPath = ctx.Request.Path.Value;
                    ctx.Items["originalPath"] = originalPath;
                    ctx.Request.Path = "/error/404";
                    await next();
                }
                else if (ctx.Response.StatusCode == 500 && !ctx.Response.HasStarted)
                {
                    string originalPath = ctx.Request.Path.Value;
                    ctx.Items["originalPath"] = originalPath;
                    ctx.Request.Path = "/error/500";
                    await next();
                }
            });

            app.UseStaticFiles();
            app.UseCookiePolicy();

            // ORDEM CORRETA
            app.UseSession();           // Sessão antes da autenticação
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=principal}/{action=index}/{id?}"
                );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=account}/{action=login}/{id?}"
                );
            });
        }
    }

    public class ConfigurationActionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => 0;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var crConnection = new DefaultCrypto(DefaultCrypto.Algorithms.Rijndael);
            var urlBase = AppSettings.UrlBase;

            if (context.HttpContext.Request.Cookies["peoplenet_client"] == null
                || !crConnection.Encrypt(urlBase).Equals(context.HttpContext.Request.Cookies["peoplenet_client"]))
            {
                var configurationSettings = ConfiguracaoGlobalCliente.Get(urlBase);
                var json = JsonConvert.SerializeObject(configurationSettings);
                var cookieValue = crConnection.Encrypt(json);

                context.HttpContext.Response.Cookies.Append("peoplenet_client", crConnection.Encrypt(urlBase));
                context.HttpContext.Response.Cookies.Append("peoplenet_settings", cookieValue);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }

    public class PerfilActionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => 0;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // lógica comentada no original
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
