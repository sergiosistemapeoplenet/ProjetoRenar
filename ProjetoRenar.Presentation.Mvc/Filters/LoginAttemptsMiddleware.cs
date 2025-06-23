using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoRenar.Presentation.Mvc.Filters
{
    public class LoginAttemptsMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, int> LoginAttempts = new ConcurrentDictionary<string, int>();
        private const int MaxLoginAttempts = 3; // Número máximo de tentativas permitidas

        public LoginAttemptsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            // Verifique se o IP está bloqueado
            if (IsBlocked(ipAddress))
            {
                context.Response.StatusCode = 403; // Forbidden
                return;
            }

            // Chame o próximo middleware na pipeline
            await _next(context);

            // Se a tentativa de login falhar, aumente a contagem de tentativas
            if (context.Response.StatusCode == 401) // Unauthorized
            {
                var currentAttempts = LoginAttempts.AddOrUpdate(ipAddress, 1, (key, value) => value + 1);

                // Se atingir o limite máximo de tentativas, bloqueie o IP
                if (currentAttempts >= MaxLoginAttempts)
                {
                    BlockIp(ipAddress);
                    context.Response.StatusCode = 403; // Forbidden
                }
            }
        }

        private bool IsBlocked(string ipAddress)
        {
            // Verifique se o IP está na lista de IPs bloqueados
            // Pode ser implementado com um cache, banco de dados, etc.
            // Neste exemplo, estamos usando uma lista simples em memória.
            return BlockedIps.ContainsKey(ipAddress);
        }

        private void BlockIp(string ipAddress)
        {
            // Adicione o IP à lista de IPs bloqueados
            // Pode ser implementado com um cache, banco de dados, etc.
            // Neste exemplo, estamos usando uma lista simples em memória.
            BlockedIps.TryAdd(ipAddress, 0);

            // Configurar um temporizador para desbloquear o IP após um período
            var timer = new System.Timers.Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
            timer.Elapsed += (sender, args) => UnblockIp(ipAddress, timer);
            timer.AutoReset = false;
            timer.Start();
        }

        private void UnblockIp(string ipAddress, System.Timers.Timer timer)
        {
            // Remover o IP da lista de IPs bloqueados
            // Pode ser implementado com um cache, banco de dados, etc.
            // Neste exemplo, estamos usando uma lista simples em memória.
            BlockedIps.TryRemove(ipAddress, out _);

            // Parar o temporizador
            timer.Stop();
            timer.Dispose();
        }

        private static readonly ConcurrentDictionary<string, int> BlockedIps = new ConcurrentDictionary<string, int>();
    }
}
