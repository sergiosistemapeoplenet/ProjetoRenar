using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoRenar.Presentation.Mvc.Filters
{
    // Crie uma nova classe de middleware
    public class RemoveScriptMiddleware
    {
        private readonly RequestDelegate _next;

        public RemoveScriptMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Intercepta a requisição antes de atingir os controladores
            if (context.Request.Method == "POST")
            {
                var originalBody = context.Request.Body;
                var contentType = context.Request.ContentType;

                if(contentType != null && !contentType.Contains("multipart/form-data"))
                {
                    using (var newBody = new MemoryStream())
                    {
                        context.Request.Body = newBody;

                        using (var reader = new StreamReader(originalBody))
                        {
                            var requestBody = await reader.ReadToEndAsync();
                            var sanitizedBody = SanitizeInput(requestBody);
                            var byteArray = Encoding.UTF8.GetBytes(sanitizedBody);
                            newBody.Write(byteArray, 0, byteArray.Length);
                            newBody.Seek(0, SeekOrigin.Begin);
                        }

                        await _next(context);
                    }
                }
                else
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }

        private string SanitizeInput(string input)
        {
            if(!string.IsNullOrWhiteSpace(input))
            {
                return input.Replace("script", "").Replace("<", "").Replace(">", "");
            }

            return input;
        }
    }
}
