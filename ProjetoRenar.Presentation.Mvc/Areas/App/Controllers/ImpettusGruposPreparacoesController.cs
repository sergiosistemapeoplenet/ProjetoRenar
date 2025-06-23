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
    public class ImpettusGruposPreparacoesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ImpettusGruposPreparacoesController(IUnitOfWork unitOfWork)
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

                var consulta = _unitOfWork.ImpettusGruposPreparacoesRepository.GetAll()
                    .Where(u => u.FlagSituacao == (ativo == 1) && u.NomeGrupoPreparacao != null && u.NomeGrupoPreparacao.ToLower().Contains(nome != null ? nome.ToLower() : string.Empty))
                    .OrderBy(u => u.NomeGrupoPreparacao).ToList();

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
            var model = new GruposPreparacoesCadastroViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Cadastro(GruposPreparacoesCadastroViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.FlagAtivo = true;
                    _unitOfWork.ImpettusGruposPreparacoesRepository.Insert(new ImpettusGruposPreparacoes 
                    { 
                        NomeGrupoPreparacao = model.NomeGrupoPreparacao,
                        FlagSituacao = true
                    });

                    model.NomeGrupoPreparacao = null;

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
            var dados = _unitOfWork.ImpettusGruposPreparacoesRepository.GetById(idSelecionado);

            var model = new GruposPreparacoesEdicaoViewModel
            {
                IDGrupoPreparacao = dados.IDGrupoPreparacao,
                NomeGrupoPreparacao = dados.NomeGrupoPreparacao,
                FlagAtivo = dados.FlagSituacao
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edicao(GruposPreparacoesEdicaoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dados = _unitOfWork.ImpettusGruposPreparacoesRepository.GetById(model.IDGrupoPreparacao);

                    model.FlagAtivo = dados.FlagSituacao;
                    _unitOfWork.ImpettusGruposPreparacoesRepository.Update(new ImpettusGruposPreparacoes 
                    { 
                        IDGrupoPreparacao = model.IDGrupoPreparacao,
                        NomeGrupoPreparacao = model.NomeGrupoPreparacao,
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
            _unitOfWork.ImpettusGruposPreparacoesRepository.Delete(idSelecionado);

            TempData["MensagemSucesso"] = "Grupo inativado com sucesso.";

            var dados = _unitOfWork.ImpettusGruposPreparacoesRepository.GetById(idSelecionado);

            var model = new GruposPreparacoesEdicaoViewModel
            {
                IDGrupoPreparacao = dados.IDGrupoPreparacao,
                NomeGrupoPreparacao = dados.NomeGrupoPreparacao,
                FlagAtivo = false
            };

            return View("Edicao", model);
        }

        public IActionResult Reativar(string id)
        {
            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            _unitOfWork.ImpettusGruposPreparacoesRepository.UnDelete(idSelecionado);

            TempData["MensagemSucesso"] = "Grupo reativado com sucesso.";

            var dados = _unitOfWork.ImpettusGruposPreparacoesRepository.GetById(idSelecionado);

            var model = new GruposPreparacoesEdicaoViewModel
            {
                IDGrupoPreparacao = dados.IDGrupoPreparacao,
                NomeGrupoPreparacao = dados.NomeGrupoPreparacao,
                FlagAtivo = true
            };

            return View("Edicao", model);
        }
    }
}
