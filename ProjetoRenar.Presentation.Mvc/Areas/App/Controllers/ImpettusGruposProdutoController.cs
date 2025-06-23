using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoRenar.Application.ViewModels.Regioes;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;
using ProjetoRenar.Presentation.Mvc.Areas.App.Models;
using ProjetoRenar.Presentation.Mvc.Helpers;
using System;
using System.Linq;

namespace ProjetoRenar.Presentation.Mvc.Areas.App.Controllers
{
    [Area("App")]
    [Authorize(Roles = "Administrador")]
    public class ImpettusGruposProdutoController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ImpettusGruposProdutoController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

                var consulta = _unitOfWork.ImpettusGruposProdutoRepository.GetAll()
                    .Where(u => u.FlagSituacao == (ativo == 1) && u.NomeGrupoProduto != null && u.NomeGrupoProduto.ToLower().Contains(nome != null ? nome.ToLower() : string.Empty))
                    .OrderBy(u => u.NomeGrupoProduto).ToList();

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
            var model = new GruposProdutoCadastroViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Cadastro(GruposProdutoCadastroViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.FlagAtivo = true;
                    _unitOfWork.ImpettusGruposProdutoRepository.Insert(new ImpettusGruposProduto
                    {
                        NomeGrupoProduto = model.NomeGrupoProduto,
                        FlagSituacao = true
                    });

                    model.NomeGrupoProduto = null;

                    ModelState.Clear();
                    TempData["MensagemSucesso"] = "Grupo cadastrado com sucesso.";
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Erro ao cadastrar grupo.";
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
            var dados = _unitOfWork.ImpettusGruposProdutoRepository.GetById(idSelecionado);

            var model = new GruposProdutoEdicaoViewModel
            {
                IDGrupoProduto = dados.IDGrupoProduto,
                NomeGrupoProduto = dados.NomeGrupoProduto,
                FlagAtivo = dados.FlagSituacao
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edicao(GruposProdutoEdicaoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dados = _unitOfWork.ImpettusGruposProdutoRepository.GetById(model.IDGrupoProduto);

                    model.FlagAtivo = dados.FlagSituacao;
                    _unitOfWork.ImpettusGruposProdutoRepository.Update(new ImpettusGruposProduto
                    {
                        IDGrupoProduto = model.IDGrupoProduto,
                        NomeGrupoProduto = model.NomeGrupoProduto,
                        FlagSituacao = true
                    });

                    TempData["MensagemSucesso"] = "Grupo atualizado com sucesso.";
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Erro ao atualizar grupo.";
                }
            }

            return View(model);
        }

        public IActionResult Inativar(string id)
        {
            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            _unitOfWork.ImpettusGruposProdutoRepository.Delete(idSelecionado);

            TempData["MensagemSucesso"] = "Grupo inativado com sucesso.";

            var dados = _unitOfWork.ImpettusGruposProdutoRepository.GetById(idSelecionado);

            var model = new GruposProdutoEdicaoViewModel
            {
                IDGrupoProduto = dados.IDGrupoProduto,
                NomeGrupoProduto = dados.NomeGrupoProduto,
                FlagAtivo = false
            };

            return View("Edicao", model);
        }

        public IActionResult Reativar(string id)
        {
            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            _unitOfWork.ImpettusGruposProdutoRepository.UnDelete(idSelecionado);

            TempData["MensagemSucesso"] = "Grupo reativado com sucesso.";

            var dados = _unitOfWork.ImpettusGruposProdutoRepository.GetById(idSelecionado);

            var model = new GruposProdutoEdicaoViewModel
            {
                IDGrupoProduto = dados.IDGrupoProduto,
                NomeGrupoProduto = dados.NomeGrupoProduto,
                FlagAtivo = true
            };

            return View("Edicao", model);
        }
    }
}
