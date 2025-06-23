using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoRenar.Application.Contracts;
using ProjetoRenar.Application.ViewModels.TiposProduto;
using ProjetoRenar.Presentation.Mvc.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.WebPages.Html;

namespace ProjetoRenar.Presentation.Mvc.Areas.App.Controllers
{
    [Area("App")]
    [Authorize(Roles = "Administrador")]
    public class TiposProdutoController : Controller
    {
        private readonly ITipoProdutoApplicationService _tipoProdutoApplicationService;
        private readonly ILayoutEtiquetaApplicationService _layoutEtiquetaApplicationService;

        public TiposProdutoController(ITipoProdutoApplicationService tipoProdutoApplicationService, ILayoutEtiquetaApplicationService layoutEtiquetaApplicationService)
        {
            _tipoProdutoApplicationService = tipoProdutoApplicationService;
            _layoutEtiquetaApplicationService = layoutEtiquetaApplicationService;
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

                var consulta = _tipoProdutoApplicationService.Consultar(nome, ativo);

                foreach (var item in consulta)
                {
                    item.LayoutEtiqueta = _layoutEtiquetaApplicationService.ObterLayoutEtiquetas((int)item.IDLayoutEtiqueta);
                }

                if (consulta.Count == 0)
                    TempData["MensagemAlerta"] = "Nenum registro foi encontrado para o filtro especificado.";

                ViewBag.Dados = consulta;
            }
            catch(Exception e)
            {
                TempData["MensagemErro"] = "Erro ao consultar tipos de produtos.";
            }

            return View();
        }

        public IActionResult Cadastro()
        {
            var usuarioAutenticado = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjetoRenar.Application.ViewModels.Usuarios.MinhaContaViewModel>(User.Identity.Name);
            if (usuarioAutenticado != null && usuarioAutenticado.FlagPrimeiroAcesso != null && usuarioAutenticado.FlagPrimeiroAcesso.Value)
                return RedirectToAction("RedefinirSenha", "Principal");

            var model = new CadastroTipoProdutoViewModel();
            model.LayoutsEtiqueta = ObterLayoutsEtiquetas();
            return View(model);
        }

        [HttpPost]
        public IActionResult Cadastro(CadastroTipoProdutoViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    model.FlagAtivo = true;
                    _tipoProdutoApplicationService.Cadastrar(model);

                    model.NomeTipoProduto = null;
                    model.IDLayoutEtiqueta = null;

                    ModelState.Clear();
                    TempData["MensagemSucesso"] = "Tipo de produto cadastrado com sucesso.";
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Erro ao cadastrar tipo de produtos.";
                }
            }
            else
            {
                ViewBag.FlagAltoAcucar = model.FlagAltoAcucar;
                ViewBag.FlagAltoGorduraSaturada = model.FlagAltoGorduraSaturada;
            }

            model.LayoutsEtiqueta = ObterLayoutsEtiquetas();
            return View(model);
        }

        public IActionResult Edicao(string id)
        {
            var usuarioAutenticado = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjetoRenar.Application.ViewModels.Usuarios.MinhaContaViewModel>(User.Identity.Name);
            if (usuarioAutenticado != null && usuarioAutenticado.FlagPrimeiroAcesso != null && usuarioAutenticado.FlagPrimeiroAcesso.Value)
                return RedirectToAction("RedefinirSenha", "Principal");

            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            var dados = _tipoProdutoApplicationService.Obter(idSelecionado);

            var model = new EdicaoTipoProdutoViewModel 
            { 
                IDTipoProduto = dados.IDTipoProduto,
                FlagAtivo = dados.FlagAtivo,
                IDLayoutEtiqueta = dados.IDLayoutEtiqueta,
                LayoutsEtiqueta = ObterLayoutsEtiquetas(),
                NomeTipoProduto = dados.NomeTipoProduto,
                FlagAltoAcucar = dados.FlagAltoAcucar,
                FlagAltoGorduraSaturada = dados.FlagAltoGorduraSaturada
            };

            ViewBag.FlagAltoAcucar = model.FlagAltoAcucar;
            ViewBag.FlagAltoGorduraSaturada = model.FlagAltoGorduraSaturada;

            return View(model);
        }

        [HttpPost]
        public IActionResult Edicao(EdicaoTipoProdutoViewModel model)
        {
            var dados = _tipoProdutoApplicationService.Obter(model.IDTipoProduto);
            model.FlagAtivo = dados.FlagAtivo;

            if (ModelState.IsValid)
            {
                try
                {                   
                    _tipoProdutoApplicationService.Atualizar(model);

                    TempData["MensagemSucesso"] = "Tipo de produto atualizado com sucesso.";
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Erro ao atualizar tipo de produtos.";
                }
            }

            ViewBag.FlagAltoAcucar = model.FlagAltoAcucar;
            ViewBag.FlagAltoGorduraSaturada = model.FlagAltoGorduraSaturada;

            model.LayoutsEtiqueta = ObterLayoutsEtiquetas();
            return View(model);
        }

        public IActionResult Inativar(string id)
        {
            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            _tipoProdutoApplicationService.Excluir(idSelecionado);

            TempData["MensagemSucesso"] = "Tipo de produto inativado com sucesso.";

            var dados = _tipoProdutoApplicationService.Obter(idSelecionado);

            var model = new EdicaoTipoProdutoViewModel
            {
                IDTipoProduto = dados.IDTipoProduto,
                FlagAtivo = dados.FlagAtivo,
                IDLayoutEtiqueta = dados.IDLayoutEtiqueta,
                LayoutsEtiqueta = ObterLayoutsEtiquetas(),
                NomeTipoProduto = dados.NomeTipoProduto,
                FlagAltoAcucar = dados.FlagAltoAcucar,
                FlagAltoGorduraSaturada = dados.FlagAltoGorduraSaturada
            };

            ViewBag.FlagAltoAcucar = model.FlagAltoAcucar;
            ViewBag.FlagAltoGorduraSaturada = model.FlagAltoGorduraSaturada;

            return View("Edicao", model);
        }

        public IActionResult Reativar(string id)
        {
            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            _tipoProdutoApplicationService.Reativar(idSelecionado);

            TempData["MensagemSucesso"] = "Tipo de produto reativado com sucesso.";

            var dados = _tipoProdutoApplicationService.Obter(idSelecionado);

            var model = new EdicaoTipoProdutoViewModel
            {
                IDTipoProduto = dados.IDTipoProduto,
                FlagAtivo = dados.FlagAtivo,
                IDLayoutEtiqueta = dados.IDLayoutEtiqueta,
                LayoutsEtiqueta = ObterLayoutsEtiquetas(),
                NomeTipoProduto = dados.NomeTipoProduto,
                FlagAltoAcucar = dados.FlagAltoAcucar,
                FlagAltoGorduraSaturada = dados.FlagAltoGorduraSaturada
            };

            ViewBag.FlagAltoAcucar = model.FlagAltoAcucar;
            ViewBag.FlagAltoGorduraSaturada = model.FlagAltoGorduraSaturada;

            return View("Edicao", model);
        }

        private List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ObterLayoutsEtiquetas()
        {
            var lista = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            foreach (var item in _layoutEtiquetaApplicationService.ConsultarLayoutEtiquetas(string.Empty, 1))
            {
                lista.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem 
                { Value = item.IDLayoutEtiqueta.ToString(), Text = item.NomeLayoutEtiqueta.ToUpper() });
            }

            return lista;
        }
    }
}
