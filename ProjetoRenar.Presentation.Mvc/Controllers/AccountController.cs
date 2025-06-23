using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using BotDetect;
using BotDetect.Web.Mvc;
using DevExpress.XtraPrinting.Native;
using DNTCaptcha.Core;
using DNTCaptcha.Core.Providers;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjetoRenar.Application.Contracts;
using ProjetoRenar.Application.Exceptions.Usuarios;
using ProjetoRenar.Application.ViewModels.Usuarios;
using ProjetoRenar.CrossCutting.Configurations;
using ProjetoRenar.Domain.Contracts.Cryptographies;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;
using ProjetoRenar.Presentation.Mvc.Configurations;
using ProjetoRenar.Presentation.Mvc.Helpers;
using ProjetoRenar.Presentation.Mvc.Models;
using Sidetech.Framework.Cryptography;
using static QRCoder.PayloadGenerator;

namespace ProjetoRenar.Presentation.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsuarioApplicationService usuarioApplicationService;
        private readonly IPerfilApplicationService perfilApplicationService;
        private readonly IDNTCaptchaValidatorService validatorService;
        private readonly IUnidadeApplicationService unidadeApplicationService;
        private readonly IUnitOfWork unitOfWork;

        public AccountController(IUsuarioApplicationService usuarioApplicationService, IPerfilApplicationService perfilApplicationService, IDNTCaptchaValidatorService validatorService, IUnidadeApplicationService unidadeApplicationService, IUnitOfWork unitOfWork)
        {
            this.usuarioApplicationService = usuarioApplicationService;
            this.perfilApplicationService = perfilApplicationService;
            this.validatorService = validatorService;
            this.unidadeApplicationService = unidadeApplicationService;
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Login()
        {
            var urlBase = AppSettings.UrlBase;

            HttpContext.Session.SetString("urlBase", urlBase);

            if (User.Identity.IsAuthenticated)
            {
                if (HttpContext.Session.GetString("urlBase").Contains("voalzira"))
                {
                    return RedirectToAction("Index", "Principal", new { area = "App" });
                }
                else if (HttpContext.Session.GetString("urlBase").Contains("impettus"))
                {
                    return RedirectToAction("Index", "ImpettusPrincipal", new { area = "App" });
                }
            }

            return View();
        }


        public IActionResult recuperarsenha()
        {
            //var urlBase = httpContextAccessor.HttpContext.Request.Host.ToString().Split('.')[0];  
            var urlBase = AppSettings.UrlBase;
            HttpContext.Session.SetString("urlBase", urlBase);

            return View();
        }

        public IActionResult cadastrarnovasenha(string token)
        {
            //var urlBase = httpContextAccessor.HttpContext.Request.Host.ToString().Split('.')[0];  
            var urlBase = AppSettings.UrlBase;
            HttpContext.Session.SetString("urlBase", urlBase);

            var model = new AtualizarSenhaViewModel();
            model.Token = token;

            try
            {
                var usuario = usuarioApplicationService.ObterTokenRecuperacaoDeSenha(token);
                if (usuario != null)
                {
                    model.IdUsuario = usuario.IDUsuario;
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Falha ao executar.";
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult recuperarsenha(RecuperarSenhaViewModel model)
        {
            //var urlBase = httpContextAccessor.HttpContext.Request.Host.ToString().Split('.')[0];  
            var urlBase = AppSettings.UrlBase;
            HttpContext.Session.SetString("urlBase", urlBase);

            if (ModelState.IsValid)
            {
                try
                {
                    var usuario = usuarioApplicationService.ObterPorEmail(model.EmailUsuario);
                    if (usuario != null)
                    {
                        var token = usuarioApplicationService.GravarTokenRecuperacaoDeSenha(usuario.IDUsuario);

                        EnviarEmailRecuperacaoDeSenha(usuario.EmailUsuario, token);
                    }

                    TempData["MensagemSucesso"] = "Se o email informado estiver correto, você receberá uma mensagem contendo o link para atualizar a sua senha de usuário.";
                    return RedirectToAction("Login", "Account");
                }
                catch (AcessoNegadoException e)
                {
                    TempData["MensagemErro"] = e.Message;
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = e.Message;
                }
            }

            return View();
        }


        private void EnviarEmailRecuperacaoDeSenha(string emailDestinatario, string token)
        {
            //var urlBase = httpContextAccessor.HttpContext.Request.Host.ToString().Split('.')[0];  
            var urlBase = AppSettings.UrlBase;
            HttpContext.Session.SetString("urlBase", urlBase);

            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("nao-responder@sistemapeoplenet.com.br", "Renar Etiquetas");
                mail.To.Add(emailDestinatario);
                mail.Subject = "Recuperação de senha solicitada no sistema Renar Etiquetas";

                string urlRecuperacao = $"http://186.202.37.229:99/account/cadastrarnovasenha?token={token}";
                string logoUrl = "http://186.202.37.229:99/images/peoplenetlogo-novo-peq.png";

                string corpoEmail = @"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='UTF-8'>
                        <title>Recuperação de Senha</title>
                        <style>
                            body {
                                font-family: Arial, sans-serif;
                                background-color: #f4f4f4;
                                margin: 0;
                                padding: 20px;
                                border: 2px solid #ccc;
                            }
                            .container {
                                max-width: 500px;
                                background-color: #ffffff;
                                padding: 25px;
                                border-radius: 10px;
                                margin: auto;
                                text-align: center;
                            }
                            h2 {
                                color: #333;
                            }
                            p {
                                font-size: 16px;
                                color: #555;
                                line-height: 1.5;
                            }
                            .button {
                                display: inline-block;
                                background-color: #007bff;
                                color: #ffffff !important;
                                padding: 12px 20px;
                                border-radius: 5px;
                                text-decoration: none;
                                font-size: 18px;
                                font-weight: bold;
                                margin-top: 20px;
                            }
                            .logo {
                                width: 150px;
                                margin-bottom: 20px;
                            }
                            .footer {
                                font-size: 12px;
                                color: #777;
                                margin-top: 20px;
                            }
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <img class='logo' src='http://186.202.37.229:99/images/peoplenetlogo-novo-peq.png' alt='Renar Etiquetas'>
                            <h2>Recuperação de Senha</h2>
                            <p>Olá,</p>
                            <p>Recebemos uma solicitação para redefinir sua senha. Clique no botão abaixo para criar uma nova senha.</p>
                            <a class='button' href='http://186.202.37.229:99/account/cadastrarnovasenha?token=" + token + @"'>Redefinir Senha</a>
                            <p><small>O link expira em 30 minutos.</small></p>
                            <p class='footer'>Este e-mail foi enviado automaticamente, por favor, não responda.</p>
                        </div>
                    </body>
                    </html>";

                mail.Body = corpoEmail;
                mail.IsBodyHtml = true;

                SmtpClient smtpClient = new SmtpClient("smtplw.com.br", 587);
                smtpClient.Credentials = new NetworkCredential("sidetech", "lJOoOdCh5452");
                smtpClient.EnableSsl = true;

                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                // Log de erro para diagnóstico
            }
        }

        [HttpPost]
        public IActionResult cadastrarnovasenha(AtualizarSenhaViewModel model)
        {
            //var urlBase = httpContextAccessor.HttpContext.Request.Host.ToString().Split('.')[0];  
            var urlBase = AppSettings.UrlBase;
            HttpContext.Session.SetString("urlBase", urlBase);

            if (ModelState.IsValid)
            {
                try
                {
                    var usuario = usuarioApplicationService.ObterTokenRecuperacaoDeSenha(model.Token);
                    usuario.SenhaUsuario = SHA256CryptoHelper.CriptografarParaSHA256(model.NovaSenha);

                    usuarioApplicationService.AtualizarSenhaUsuario(usuario.SenhaUsuario, usuario.IDUsuario);
                    usuarioApplicationService.ApagarTokenRecuperacaoDeSenha(model.IdUsuario);

                    TempData["MensagemSucesso"] = "Senha de acesso atualizada com sucesso";
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Falha ao atualizar a senha";
                }
            }

            return RedirectToAction("Login", "Account");
        }


        public IActionResult AccessDenied()
        {
            //var urlBase = httpContextAccessor.HttpContext.Request.Host.ToString().Split('.')[0];  
            var urlBase = AppSettings.UrlBase;
            HttpContext.Session.SetString("urlBase", urlBase);

            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            //var urlBase = httpContextAccessor.HttpContext.Request.Host.ToString().Split('.')[0];  
            var urlBase = AppSettings.UrlBase;
            HttpContext.Session.SetString("urlBase", urlBase);

            bool usuarioAutenticado = false;

            if (ModelState.IsValid)
            {
                try
                {
                    model.SenhaUsuario = SHA256CryptoHelper.CriptografarParaSHA256(model.SenhaUsuario);
                    var minhaConta = usuarioApplicationService.Obter(model);

                    try
                    {
                        var unidades = unitOfWork.UsuarioUnidadeRepository.GetByKey(minhaConta.IdUsuario);
                        var unidadePadrao = unidades.Where(u => u.FlagUnidadePadrao != null && u.FlagUnidadePadrao.Value).FirstOrDefault();

                        if (unidadePadrao == null)
                            unidadePadrao = unidades.FirstOrDefault();

                        minhaConta.Unidades = new List<Application.ViewModels.Unidades.ConsultaUnidadeViewModel>();
                        if(unidadePadrao != null)
                        {
                            minhaConta.Unidades.Add(unidadeApplicationService.ObterPorId(unidadePadrao.IDUnidade));
                        }
                    }
                    catch (Exception e) { }

                    if (minhaConta != null)
                    {
                        try
                        {
                            var crConnection = new DefaultCrypto(DefaultCrypto.Algorithms.Rijndael);
                            var data = crConnection.Decrypt(HttpContext.Request.Cookies["peoplenet_settings"]);

                            var configurarionSettings = JsonConvert.DeserializeObject<ConfigurationSettings>(data);
                        }
                        catch (Exception e) { Debug.WriteLine(e.Message); }

                        if (minhaConta != null)
                        {
                            var identity = new ClaimsIdentity(new[]
                                {
                              new Claim(ClaimTypes.Name, JsonConvert.SerializeObject(minhaConta)),
                              new Claim(ClaimTypes.Role, minhaConta.IDPerfil == 1 ? "Administrador" : "Operador")
                            },
                                CookieAuthenticationDefaults.AuthenticationScheme);

                            var principal = new ClaimsPrincipal(identity);
                            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                            Response.Cookies.Append("LayoutPadrao", "_LayoutRecolhido");
                            usuarioAutenticado = true;

                            if(HttpContext.Session.GetString("urlBase").Contains("voalzira"))
                            {
                                return RedirectToAction("Index", "Principal", new { area = "App" });
                            }
                            else if(HttpContext.Session.GetString("urlBase").Contains("impettus"))
                            {
                                return RedirectToAction("Index", "ImpettusPrincipal", new { area = "App" });
                            }                            
                        }
                    }
                }
                catch (AcessoNegadoException e)
                {
                    TempData["MensagemErro"] = e.Message;
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = e.Message;
                }
            }

            MvcCaptcha.ResetCaptcha("ExampleCaptcha");
            return View();
        }

        public IActionResult Logout()
        {
            //var urlBase = httpContextAccessor.HttpContext.Request.Host.ToString().Split('.')[0];  
            var urlBase = AppSettings.UrlBase;
            HttpContext.Session.SetString("urlBase", urlBase);

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Response.Cookies.Delete("peoplenet_client");
            HttpContext.Response.Cookies.Delete("peoplenet_settings");

            HttpContext.Session.Remove("filtro_etiquetas");

            return RedirectToAction("Login", "Account");
        }

        public IActionResult Error()
        {
            //var urlBase = httpContextAccessor.HttpContext.Request.Host.ToString().Split('.')[0];  
            var urlBase = AppSettings.UrlBase;
            HttpContext.Session.SetString("urlBase", urlBase);

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}