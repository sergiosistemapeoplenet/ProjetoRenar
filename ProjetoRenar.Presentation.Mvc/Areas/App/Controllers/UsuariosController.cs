using DevExpress.Xpo.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoRenar.Application.Contracts;
using ProjetoRenar.Application.ViewModels.Usuarios;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Presentation.Mvc.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoRenar.Presentation.Mvc.Areas.App.Controllers
{
    [Area("App")]
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioApplicationService _usuarioApplicationService;
        private readonly IUnidadeApplicationService _unidadeApplicationService;
        private readonly IUnitOfWork unitOfWork;

        public UsuariosController(IUsuarioApplicationService usuarioApplicationService, IUnidadeApplicationService unidadeApplicationService, IUnitOfWork unitOfWork)
        {
            _usuarioApplicationService = usuarioApplicationService;
            _unidadeApplicationService = unidadeApplicationService;
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Consulta()
        {
            var usuarioAutenticado = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjetoRenar.Application.ViewModels.Usuarios.MinhaContaViewModel>(User.Identity.Name);
            if (usuarioAutenticado != null && usuarioAutenticado.FlagPrimeiroAcesso != null && usuarioAutenticado.FlagPrimeiroAcesso.Value)
                return RedirectToAction("RedefinirSenha", "Principal");

            var model = new FiltroConsultaUsuarioViewModel();
            model.Unidades = ObterUnidades();
            model.Perfis = ObterPerfis();

            return View(model);
        }

        [HttpPost]
        public IActionResult Consulta(FiltroConsultaUsuarioViewModel model)
        {
            try
            {
                ViewBag.Nome = model.Nome;
                ViewBag.Ativo = model.Ativo;

                var consulta = _usuarioApplicationService.Consultar()
                    .Where(u => u.FlagAtivo == (model.Ativo == 1) && u.EmailUsuario != null && u.EmailUsuario.ToLower().Contains(model.Nome != null ? model.Nome.ToLower() : string.Empty))
                    .OrderBy(u => u.EmailUsuario).ToList();

                if (consulta.Count == 0)
                    TempData["MensagemAlerta"] = "Nenum registro foi encontrado para o filtro especificado.";

                foreach (var item in consulta)
                {
                    item.Perfil = _usuarioApplicationService.ObterPerfil(item.IDPerfil);
                }

                if (model.IdPerfil != null)
                    consulta = consulta.Where(u => u.IDPerfil == model.IdPerfil).ToList();

                foreach (var item in consulta)
                {
                    var unidade = _unidadeApplicationService.Obter(item.IDUsuario);
                    var registro = unidade.Where(u => u.FlagUnidadePadrao).FirstOrDefault();
                    if (registro == null)
                        registro = unidade.FirstOrDefault();

                    if(registro != null)
                    {
                        item.IDUnidade = registro.IDUnidade;
                        item.NomeUnidade = registro.NomeUnidade;
                        item.NomeContato = registro.NomeContato;
                        item.Endereco = registro.Endereco;
                    }
                }

                if(model.IdUnidade != null)
                {
                    consulta = consulta.Where(c => c.IDUnidade == model.IdUnidade).ToList();
                }
                
                model.Consulta = consulta.OrderBy(u => u.EmailContato).ToList();
                ViewBag.Dados = consulta.OrderBy(u => u.EmailContato).ToList();

                if(consulta.Count == 0)
                {
                    TempData["MensagemAlerta"] = "Nenhum registro foi encontrado para esta consulta. Verifique os filtros informados.";
                }
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao consultar lojas.";
            }

            model.Unidades = ObterUnidades();
            model.Perfis = ObterPerfis();

            return View(model);
        }

        public IActionResult Cadastro()
        {
            var usuarioAutenticado = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjetoRenar.Application.ViewModels.Usuarios.MinhaContaViewModel>(User.Identity.Name);
            if (usuarioAutenticado != null && usuarioAutenticado.FlagPrimeiroAcesso != null && usuarioAutenticado.FlagPrimeiroAcesso.Value)
                return RedirectToAction("RedefinirSenha", "Principal");

            var model = new CadastroUsuarioViewModel();
            model.Perfis = ObterPerfis();
            model.Unidades = ObterUnidades();
            return View(model);
        }

        [HttpPost]
        public IActionResult Cadastro(CadastroUsuarioViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.FlagAtivo = true;
                    model.DataUltimoAcesso = DateTime.Now;
                    model.DataUsuarioAlteracao = DateTime.Now;
                    model.DataUsuarioInclusao = DateTime.Now;
                    model.FlagBloqueado = false;
                    model.FlagPrimeiroAcesso = true;
                    model.IDPerfil = model.IDPerfil;
                    string senhaOriginal = model.SenhaUsuario; // Captura a senha antes de criptografar
                    model.SenhaUsuario = SHA256CryptoHelper.CriptografarParaSHA256(model.SenhaUsuario);

                    _usuarioApplicationService.Cadastrar(model);

                    EnviarEmailUsuario(model.EmailUsuario, senhaOriginal);

                    model = new CadastroUsuarioViewModel();

                    ModelState.Clear();
                    TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso. Um e-mail foi enviado com as informações da conta.";
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "O email informado já está cadastrado para outro usuário.";
                }
            }

            model.Perfis = ObterPerfis();
            model.Unidades = ObterUnidades();
            return View(model);
        }

        private void EnviarEmailUsuario(string emailDestinatario, string senhaUsuario)
        {
            try
            {
                string smtpServidor = "smtplw.com.br";
                int smtpPorta = 587;
                string smtpLogin = "nao-responder@sistemapeoplenet.com.br";
                string smtpSenha = "SPpeoplenet25";

                //using (var smtpClient = new SmtpClient(smtpServidor, smtpPorta))
                //{
                //    smtpClient.Credentials = new System.Net.NetworkCredential(smtpLogin, smtpSenha);
                //    smtpClient.EnableSsl = true;

                //    var mensagem = new MailMessage
                //    {
                //        From = new MailAddress(smtpLogin, "Renar Etiquetas"),
                //        Subject = "Conta criada no sistema Renar Etiquetas",
                //        Body = $@"
                //    <p>Prezado usuário,</p>
                //    <p>Sua conta foi criada com sucesso no sistema Renar Etiquetas.</p>
                //    <p><strong>Dados de acesso:</strong></p>
                //    <ul>
                //        <li><strong>Email:</strong> {emailDestinatario}</li>
                //        <li><strong>Senha:</strong> {senhaUsuario}</li>
                //    </ul>
                //    <p>Para acessar o sistema, clique no link abaixo:</p>
                //    <p><a href='http://voalzira.renaretiquetas.com.br:99/' target='_blank'>Acessar o sistema</a></p>
                //    <p>Atenciosamente,</p>
                //    <p>Equipe Renar Etiquetas</p>",
                //        IsBodyHtml = true
                //    };

                //    mensagem.To.Add(emailDestinatario);

                //    smtpClient.Send(mensagem);
                //}

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("nao-responder@sistemapeoplenet.com.br", "Renar Etiquetas");
                mail.To.Add(emailDestinatario);
                mail.Subject = "Conta criada no sistema Renar Etiquetas";

                string corpoEmail = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Conta Criada com Sucesso</title>
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
        .dados-acesso {
            font-size: 16px;
            font-weight: bold;
            background-color: #f8f8f8;
            padding: 10px;
            border-radius: 5px;
            display: inline-block;
            text-align: left;
        }
    </style>
</head>
<body>
    <div class='container'>
        <img class='logo' src='http://voalzira.renaretiquetas.com.br/Renar/images/peoplenetlogo-novo-peq.png' alt='Renar Etiquetas'>
        <h2>Conta Criada com Sucesso</h2>
        <p>Olá,</p>
        <p>Sua conta foi criada com sucesso no sistema Renar Etiquetas.</p>
        <div class='dados-acesso'>
            <p>Email de acesso: <strong>" + emailDestinatario + @"</strong></p>
            <p>Senha de acesso: <strong>" + senhaUsuario + @"</strong></p>
        </div>
        <p>Para acessar o sistema, clique no botão abaixo:</p>
        <a class='button' href='http://voalzira.renaretiquetas.com.br/Renar'>Acessar o Sistema</a>
        <p class='footer'>Este e-mail foi enviado automaticamente, por favor, não responda.</p>
    </div>
</body>
</html>";

                mail.Body = corpoEmail;
                mail.IsBodyHtml = true;


                SmtpClient smtpClient = new SmtpClient("smtplw.com.br", 587);
                smtpClient.Credentials = new NetworkCredential("sidetech", "SPpeoplenet25");
                smtpClient.EnableSsl = true;

                smtpClient.Send(mail);

                var mailFrom = new MailAddress(smtpLogin, "Renar Etiquetas");
                var mailTo = new MailAddress(emailDestinatario);
            }
            catch (Exception ex)
            {
                //TempData["MensagemErro"] = "Houve um erro ao enviar o e-mail. Tente novamente mais tarde.";
                // Aqui você pode logar o erro para diagnóstico
            }
        }

        public IActionResult Edicao(string id)
        {
            var usuarioAutenticado = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjetoRenar.Application.ViewModels.Usuarios.MinhaContaViewModel>(User.Identity.Name);
            if (usuarioAutenticado != null && usuarioAutenticado.FlagPrimeiroAcesso != null && usuarioAutenticado.FlagPrimeiroAcesso.Value)
                return RedirectToAction("RedefinirSenha", "Principal");

            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            var dados = _usuarioApplicationService.Obter(idSelecionado);

            var model = new EdicaoUsuarioViewModel
            {
                DataUltimoAcesso = dados.DataUltimoAcesso,
                DataUsuarioAlteracao = dados.DataUsuarioAlteracao,
                DataUsuarioInclusao = dados.DataUsuarioInclusao,
                EmailUsuario = dados.EmailUsuario,
                FlagAtivo = dados.FlagAtivo,
                FlagBloqueado = dados.FlagBloqueado,
                FlagPrimeiroAcesso = dados.FlagPrimeiroAcesso,
                IDPerfil = dados.IDPerfil,
                IdUsuario = dados.IDUsuario,
                IDUsuarioAlteracao = dados.IDUsuarioAlteracao,
                IDUsuarioInclusao = dados.IDUsuarioInclusao,
                Perfis = ObterPerfis(),
            };

            var unidade = _unidadeApplicationService.Obter(model.IdUsuario.Value);
            var registro = unidade.Where(u => u.FlagUnidadePadrao).FirstOrDefault();
            if (registro == null)
                registro = unidade.FirstOrDefault();

            if (registro != null)
            {
                model.IDUnidade = registro.IDUnidade;
            }

            model.Unidades = ObterUnidades();
            return View(model);
        }

        [HttpPost]
        public IActionResult Edicao(EdicaoUsuarioViewModel model)
        {
            var dados = _usuarioApplicationService.Obter(model.IdUsuario.Value);
            model.FlagAtivo = dados.FlagAtivo;

            if (ModelState.IsValid)
            {
                try
                {                   
                    _usuarioApplicationService.Atualizar(model);

                    if(!string.IsNullOrEmpty(model.NovaSenha))
                    {
                        _usuarioApplicationService.AtualizarSenhaUsuario
                            (SHA256CryptoHelper.CriptografarParaSHA256(model.NovaSenha), model.IdUsuario.Value);
                        
                        unitOfWork.UsuarioRepository.DefinirPrimeiroAcesso((short)model.IdUsuario);
                    }

                    unitOfWork.UsuarioUnidadeRepository.Delete((short)model.IdUsuario);
                    unitOfWork.UsuarioUnidadeRepository.Insert(new Domain.Entities.UsuarioUnidade
                    {
                        FlagUnidadePadrao = true,
                        IDUnidade = (short)(model.IDUnidade.Value),
                        IDUsuario = (short)(model.IdUsuario.Value)
                    });

                    TempData["MensagemSucesso"] = "Usuário atualizado com sucesso.";
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "O email informado já está cadastrado para outro usuário.";
                }
            }

            var unidade = _unidadeApplicationService.Obter(model.IdUsuario.Value);
            var registro = unidade.Where(u => u.FlagUnidadePadrao).FirstOrDefault();
            if (registro == null)
                registro = unidade.FirstOrDefault();

            if (registro != null)
            {
                model.IDUnidade = registro.IDUnidade;
            }

            model.Perfis = ObterPerfis();
            model.Unidades = ObterUnidades();
            return View(model);
        }

        public IActionResult Inativar(string id)
        {
            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            _usuarioApplicationService.Excluir(idSelecionado);

            TempData["MensagemSucesso"] = "Usuário inativado com sucesso.";

            var dados = _usuarioApplicationService.Obter(idSelecionado);

            var model = new EdicaoUsuarioViewModel
            {
                DataUltimoAcesso = dados.DataUltimoAcesso,
                DataUsuarioAlteracao = dados.DataUsuarioAlteracao,
                DataUsuarioInclusao = dados.DataUsuarioInclusao,
                EmailUsuario = dados.EmailUsuario,
                FlagAtivo = dados.FlagAtivo,
                FlagBloqueado = dados.FlagBloqueado,
                FlagPrimeiroAcesso = dados.FlagPrimeiroAcesso,
                IDPerfil = dados.IDPerfil,
                IdUsuario = dados.IDUsuario,
                IDUsuarioAlteracao = dados.IDUsuarioAlteracao,
                IDUsuarioInclusao = dados.IDUsuarioInclusao,
                Perfis = ObterPerfis(),
            };

            var unidade = _unidadeApplicationService.Obter(model.IdUsuario.Value);
            var registro = unidade.Where(u => u.FlagUnidadePadrao).FirstOrDefault();
            if (registro == null)
                registro = unidade.FirstOrDefault();

            if (registro != null)
            {
                model.IDUnidade = registro.IDUnidade;
            }

            model.Unidades = ObterUnidades();
            model.Perfis = ObterPerfis();

            return View("Edicao", model);
        }

        public IActionResult Reativar(string id)
        {
            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            _usuarioApplicationService.Reativar(idSelecionado);

            TempData["MensagemSucesso"] = "Usuário reativado com sucesso.";

            var dados = _usuarioApplicationService.Obter(idSelecionado);

            var model = new EdicaoUsuarioViewModel
            {
                DataUltimoAcesso = dados.DataUltimoAcesso,
                DataUsuarioAlteracao = dados.DataUsuarioAlteracao,
                DataUsuarioInclusao = dados.DataUsuarioInclusao,
                EmailUsuario = dados.EmailUsuario,
                FlagAtivo = dados.FlagAtivo,
                FlagBloqueado = dados.FlagBloqueado,
                FlagPrimeiroAcesso = dados.FlagPrimeiroAcesso,
                IDPerfil = dados.IDPerfil,
                IdUsuario = dados.IDUsuario,
                IDUsuarioAlteracao = dados.IDUsuarioAlteracao,
                IDUsuarioInclusao = dados.IDUsuarioInclusao,
                Perfis = ObterPerfis(),
            };

            var unidade = _unidadeApplicationService.Obter(model.IdUsuario.Value);
            var registro = unidade.Where(u => u.FlagUnidadePadrao).FirstOrDefault();
            if (registro == null)
                registro = unidade.FirstOrDefault();

            if (registro != null)
            {
                model.IDUnidade = registro.IDUnidade;
            }

            model.Unidades = ObterUnidades();
            model.Perfis = ObterPerfis();

            return View("Edicao", model);
        }

        private List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ObterPerfis()
        {
            var lista = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            foreach (var item in _usuarioApplicationService.ConsultarPerfis())
            {
                lista.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                { Value = item.IDPerfil.ToString(), Text = item.NomePerfil.ToUpper() });
            }

            return lista;
        }

        private List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ObterUnidades()
        {
            var lista = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            foreach (var item in _unidadeApplicationService.Consultar())
            {
                lista.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                { Value = item.IDUnidade.ToString(), Text = item.NomeUnidade.ToUpper() });
            }

            return lista.OrderBy(l => l.Text).ToList();
        }
    }
}
