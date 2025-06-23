using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoRenar.Presentation.Mvc.Filters
{
    public class StaticFilesAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public StaticFilesAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/candidatos"))
            {
                // Verifica se o usuário está autenticado
                if (!context.User.Identity.IsAuthenticated)
                {
                    context.Response.StatusCode = 401; // Não autorizado
                    return;
                }
            }

            await _next(context);
        }
    }
}
