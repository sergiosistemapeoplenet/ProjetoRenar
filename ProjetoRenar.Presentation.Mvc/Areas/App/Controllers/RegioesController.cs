using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoRenar.Application.Contracts;
using ProjetoRenar.Application.ViewModels.Regioes;
using ProjetoRenar.Presentation.Mvc.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoRenar.Presentation.Mvc.Areas.App.Controllers
{
    [Area("App")]
    [Authorize(Roles = "Administrador")]
    public class RegioesController : Controller
    {
        private readonly IRegiaoApplicationService _regiaoApplicationService;

        public RegioesController(IRegiaoApplicationService regiaoApplicationService)
        {
            _regiaoApplicationService = regiaoApplicationService;
        }

        public IActionResult Consulta()
        {
            var usuarioAutenticado = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjetoRenar.Application.ViewModels.Usuarios.MinhaContaViewModel>(User.Identity.Name);
            if (usuarioAutenticado != null && usuarioAutenticado.FlagPrimeiroAcesso != null && usuarioAutenticado.FlagPrimeiroAcesso.Value)
                return RedirectToAction("RedefinirSenha", "Principal");

            return View();
        }

        [HttpPost]
        public IActionResult Consulta(string nome, int ativo)
        {
            try
            {
                ViewBag.Nome = nome;
                ViewBag.Ativo = ativo;

                var consulta = _regiaoApplicationService.Consultar()
                    .Where(u => u.FlagAtivo == (ativo == 1) && u.NomeRegiao != null && u.NomeRegiao.ToLower().Contains(nome != null ? nome.ToLower() : string.Empty))
                    .OrderBy(u => u.NomeRegiao).ToList();

                if (consulta.Count == 0)
                    TempData["MensagemAlerta"] = "Nenum registro foi encontrado para o filtro especificado.";

                ViewBag.Dados = consulta;
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao consultar lojas.";
            }

            return View();
        }

        public IActionResult Cadastro()
        {
            var model = new CadastroRegiaoViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Cadastro(CadastroRegiaoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.FlagAtivo = true;
                    _regiaoApplicationService.Cadastrar(model);

                    model.NomeRegiao = null;

                    ModelState.Clear();
                    TempData["MensagemSucesso"] = "Região cadastrada com sucesso.";
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Erro ao cadastrar região.";
                }
            }

            return View(model);
        }

        public IActionResult Edicao(string id)
        {
            var usuarioAutenticado = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjetoRenar.Application.ViewModels.Usuarios.MinhaContaViewModel>(User.Identity.Name);
            if (usuarioAutenticado != null && usuarioAutenticado.FlagPrimeiroAcesso != null && usuarioAutenticado.FlagPrimeiroAcesso.Value)
                return RedirectToAction("RedefinirSenha", "Principal");

            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            var dados = _regiaoApplicationService.Obter(idSelecionado);

            var model = new EdicaoRegiaoViewModel
            {
                IDRegiao = dados.IDRegiao,
                NomeRegiao = dados.NomeRegiao,
                FlagAtivo = dados.FlagAtivo
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edicao(EdicaoRegiaoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dados = _regiaoApplicationService.Obter(model.IDRegiao);

                    model.FlagAtivo = dados.FlagAtivo;
                    _regiaoApplicationService.Atualizar(model);

                    TempData["MensagemSucesso"] = "Região atualizada com sucesso.";
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Erro ao atualizar tipo de produtos.";
                }
            }

            return View(model);
        }

        public IActionResult Inativar(string id)
        {
            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            _regiaoApplicationService.Excluir(idSelecionado);

            TempData["MensagemSucesso"] = "Região inativada com sucesso.";

            var dados = _regiaoApplicationService.Obter(idSelecionado);

            var model = new EdicaoRegiaoViewModel
            {
                FlagAtivo = dados.FlagAtivo,
                IDRegiao = dados.IDRegiao,
                NomeRegiao = dados.NomeRegiao    
            };

            return View("Edicao", model);
        }

        public IActionResult Reativar(string id)
        {
            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            _regiaoApplicationService.Reativar(idSelecionado);

            TempData["MensagemSucesso"] = "Região reativada com sucesso.";

            var dados = _regiaoApplicationService.Obter(idSelecionado);

            var model = new EdicaoRegiaoViewModel
            {
                FlagAtivo = dados.FlagAtivo,
                IDRegiao = dados.IDRegiao,
                NomeRegiao = dados.NomeRegiao
            };

            return View("Edicao", model);
        }
    }
}
