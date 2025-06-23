using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjetoRenar.Application.Profiles;
using ProjetoRenar.Domain.Contracts.Cryptographies;
using ProjetoRenar.Domain.Contracts.Messages;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Contracts.Services;
using ProjetoRenar.Domain.Services;
using ProjetoRenar.Infra.Cryptography;
using ProjetoRenar.Infra.Message;
using ProjetoRenar.Infra.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetoRenar.Presentation.Mvc.Filters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjetoRenar.CrossCutting.Configurations;
using Sidetech.Framework.Cryptography;
using ProjetoRenar.Application.ViewModels.Usuarios;
using Microsoft.AspNetCore.Routing;
using System.Globalization;
using DNTCaptcha.Core;

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
                //options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication
                (CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            services.AddMvc(options =>
            {
                options.Filters.Add(new ConfigurationActionFilter());
                options.Filters.Add(new PerfilActionFilter());
            }).AddSessionStateTempDataProvider();

            //services.AddRouting(options => options.LowercaseUrls = true);

            // Configurações de sessão
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(7); // Mantém a sessão ativa por 1 dia
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Configurações de autenticação
            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(7); // Tempo de expiração de 1 dia
                options.SlidingExpiration = true; // Renova o tempo de expiração a cada requisição
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
            });

            services.AddHsts(options =>
            {
                options.IncludeSubDomains = true; // Inclua subdomínios no cabeçalho HSTS (opcional)
                options.MaxAge = TimeSpan.FromDays(365); // Especifique a duração do HSTS em dias
            });

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN"; // Nome do cabeçalho personalizado para o token anti-CSRF
                options.Cookie.Name = "XSRF-TOKEN";   // Nome do cookie para o token anti-CSRF
                options.Cookie.HttpOnly = false;       // O cookie pode ser lido por JavaScript
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

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
            //app.UseMiddleware<StaticFilesAuthenticationMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error/500");
            }

            app.UseHsts();
            app.UseHttpsRedirection(); // Redireciona todas as solicitações HTTP para HTTPS

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

            //app.UseMiddleware<NoCache>();
            
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
           
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "areas", template: "{area:exists}/{controller=principal}/{action=index}/{id?}");
                routes.MapRoute(name: "default", template: "{controller=account}/{action=login}/{id?}");
            });
        }
    }

    public class ConfigurationActionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => 0;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var crConnection = new DefaultCrypto(DefaultCrypto.Algorithms.Rijndael);
            //var urlBase = context.HttpContext.Request.Host.ToString().Split('.')[0];    
            var urlBase = AppSettings.UrlBase;

            if (context.HttpContext.Request.Cookies["peoplenet_client"] == null
                || !crConnection.Encrypt(urlBase).Equals(context.HttpContext.Request.Cookies["peoplenet_client"].ToString()))
            {
                var configurationSettings = ConfiguracaoGlobalCliente.Get(urlBase);
                var json = JsonConvert.SerializeObject(configurationSettings);
                var cookieValue = crConnection.Encrypt(json);

                context.HttpContext.Response.Cookies.Append("peoplenet_client", crConnection.Encrypt(urlBase));
                context.HttpContext.Response.Cookies.Append("peoplenet_settings", cookieValue);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }

    public class PerfilActionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => 0;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            /*
            if(context.HttpContext.User.Identity.IsAuthenticated && context.HttpContext.User.Identity.Name != null)
            {
                var minhaConta = JsonConvert.DeserializeObject<MinhaContaViewModel>
                (context.HttpContext.User.Identity.Name);

                if(minhaConta != null && minhaConta.RecursosBloqueados != null)
                {
                    var bloqueado = false;
                    foreach (var item in minhaConta.RecursosBloqueados)
                    {
                        if (context.HttpContext.Request.Path.Value.Contains(item))
                        {
                            bloqueado = true;
                            break;
                        }
                    }

                    if (bloqueado)
                    {
                        context.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                    { "controller", "Principal" },
                    { "action", "NaoAutorizado" },
                    { "areas", "App" }
                        });
                    }
                }
            }
            */
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
