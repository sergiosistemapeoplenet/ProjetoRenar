using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoRenar.Application.ViewModels.Regioes;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;
using ProjetoRenar.Presentation.Mvc.Areas.App.Models;
using ProjetoRenar.Presentation.Mvc.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjetoRenar.Presentation.Mvc.Areas.App.Controllers
{
    [Area("App")]
    [Authorize(Roles = "Administrador")]
    public class ImpettusPreparacoesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ImpettusPreparacoesController(IUnitOfWork unitOfWork)
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

                var consulta = _unitOfWork.ImpettusPreparacaoRepository.GetAll()
                    .Where(u => u.FlagAtivo == (ativo == 1) && u.NomePreparacao != null && u.NomePreparacao.ToLower().Contains(nome != null ? nome.ToLower() : string.Empty))
                    .OrderBy(u => u.NomePreparacao).ToList();

                foreach (var item in consulta)
                {
                    item.NomeGrupoPreparacao = _unitOfWork.ImpettusGruposPreparacoesRepository.GetById(item.IdGrupoPreparacao.Value).NomeGrupoPreparacao;
                }

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
            var model = new PreparacoesCadastroViewModel();

            var grupos = _unitOfWork.ImpettusGruposPreparacoesRepository.GetAll().OrderBy(g => g.NomeGrupoPreparacao).ToList();
            model.Grupos = new List<SelectListItem>();
            foreach (var item in grupos)
            {
                model.Grupos.Add(new SelectListItem { Value = item.IDGrupoPreparacao.ToString(), Text = item.NomeGrupoPreparacao.ToUpper() });
            }

            model.Tipos = new List<SelectListItem>();
            model.Tipos.Add(new SelectListItem { Value = "H", Text = "Horas" });
            model.Tipos.Add(new SelectListItem { Value = "D", Text = "Dias" });

            return View(model);
        }

        [HttpPost]
        public IActionResult Cadastro(PreparacoesCadastroViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.FlagAtivo = true;
                    _unitOfWork.ImpettusPreparacaoRepository.Insert(new ImpettusPreparacao
                    {
                        FlagAtivo = true,
                        FlagCongelado = model.FlagCongelado,
                        FlagResfriado = model.FlagResfriado,
                        FlagTemperaturaAmbiente = model.FlagTemperaturaAmbiente,
                        IdGrupoPreparacao = model.IdGrupoPreparacao,
                        NomePreparacao = model.NomePreparacao,
                        Sif = model.Sif,
                        TipoValidadeResfriado = model.TipoValidadeResfriado,
                        TipoValidadeTemperaturaAmbiente = model.TipoValidadeTemperaturaAmbiente,
                        ValidadeCongelado = model.ValidadeCongelado,
                        ValidadeResfriado = model.ValidadeResfriado,
                        ValidadeTemperaturaAmbiente = model.ValidadeTemperaturaAmbiente
                    });

                    ModelState.Clear();

                    model = new PreparacoesCadastroViewModel();

                    TempData["MensagemSucesso"] = "Preparação cadastrada com sucesso.";
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Erro ao cadastrar Preparação.";
                }
            }

            var grupos = _unitOfWork.ImpettusGruposPreparacoesRepository.GetAll().OrderBy(g => g.NomeGrupoPreparacao).ToList();
            model.Grupos = new List<SelectListItem>();
            foreach (var item in grupos)
            {
                model.Grupos.Add(new SelectListItem { Value = item.IDGrupoPreparacao.ToString(), Text = item.NomeGrupoPreparacao.ToUpper() });
            }

            model.Tipos = new List<SelectListItem>();
            model.Tipos.Add(new SelectListItem { Value = "H", Text = "Horas" });
            model.Tipos.Add(new SelectListItem { Value = "D", Text = "Dias" });

            return View(model);
        }

        public IActionResult Edicao(string id)
        {
            var usuarioAutenticado = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjetoRenar.Application.ViewModels.Usuarios.MinhaContaViewModel>(User.Identity.Name);
            if (usuarioAutenticado != null && usuarioAutenticado.FlagPrimeiroAcesso != null && usuarioAutenticado.FlagPrimeiroAcesso.Value)
                return RedirectToAction("RedefinirSenha", "Principal");

            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            var dados = _unitOfWork.ImpettusPreparacaoRepository.GetById(idSelecionado);

            var model = new PreparacoesEdicaoViewModel
            {
                IdPreparacao = dados.IdPreparacao,
                FlagAtivo = dados.FlagAtivo,
                FlagCongelado = dados.FlagCongelado,
                FlagResfriado = dados.FlagResfriado,
                FlagTemperaturaAmbiente = dados.FlagTemperaturaAmbiente,
                IdGrupoPreparacao = dados.IdGrupoPreparacao,
                NomePreparacao = dados.NomePreparacao,
                Sif = dados.Sif,
                TipoValidadeResfriado = dados.TipoValidadeResfriado,
                TipoValidadeTemperaturaAmbiente = dados.TipoValidadeTemperaturaAmbiente,
                ValidadeCongelado = dados.ValidadeCongelado,
                ValidadeResfriado = dados.ValidadeResfriado,
                ValidadeTemperaturaAmbiente = dados.ValidadeTemperaturaAmbiente
            };

            var grupos = _unitOfWork.ImpettusGruposPreparacoesRepository.GetAll().OrderBy(g => g.NomeGrupoPreparacao).ToList();
            model.Grupos = new List<SelectListItem>();
            foreach (var item in grupos)
            {
                model.Grupos.Add(new SelectListItem { Value = item.IDGrupoPreparacao.ToString(), Text = item.NomeGrupoPreparacao.ToUpper() });
            }

            model.Tipos = new List<SelectListItem>();
            model.Tipos.Add(new SelectListItem { Value = "H", Text = "Horas" });
            model.Tipos.Add(new SelectListItem { Value = "D", Text = "Dias" });

            return View(model);
        }

        [HttpPost]
        public IActionResult Edicao(PreparacoesEdicaoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dados = _unitOfWork.ImpettusPreparacaoRepository.GetById(model.IdPreparacao);

                    model.FlagAtivo = dados.FlagAtivo;
                    _unitOfWork.ImpettusPreparacaoRepository.Update(new ImpettusPreparacao
                    {
                        IdPreparacao = model.IdPreparacao,
                        FlagCongelado = model.FlagCongelado,
                        FlagResfriado = model.FlagResfriado,
                        FlagTemperaturaAmbiente = model.FlagTemperaturaAmbiente,
                        IdGrupoPreparacao = model.IdGrupoPreparacao,
                        NomePreparacao = model.NomePreparacao,
                        Sif = model.Sif,
                        TipoValidadeResfriado = model.TipoValidadeResfriado,
                        TipoValidadeTemperaturaAmbiente = model.TipoValidadeTemperaturaAmbiente,
                        ValidadeCongelado = model.ValidadeCongelado,
                        ValidadeResfriado = model.ValidadeResfriado,
                        ValidadeTemperaturaAmbiente = model.ValidadeTemperaturaAmbiente,
                        FlagAtivo = true
                    });

                    TempData["MensagemSucesso"] = "Preparação atualizado com sucesso.";
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Erro ao atualizar Preparação.";
                }
            }

            var grupos = _unitOfWork.ImpettusGruposPreparacoesRepository.GetAll().OrderBy(g => g.NomeGrupoPreparacao).ToList();
            model.Grupos = new List<SelectListItem>();
            foreach (var item in grupos)
            {
                model.Grupos.Add(new SelectListItem { Value = item.IDGrupoPreparacao.ToString(), Text = item.NomeGrupoPreparacao.ToUpper() });
            }

            model.Tipos = new List<SelectListItem>();
            model.Tipos.Add(new SelectListItem { Value = "H", Text = "Horas" });
            model.Tipos.Add(new SelectListItem { Value = "D", Text = "Dias" });

            return View(model);
        }

        public IActionResult Inativar(string id)
        {
            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            _unitOfWork.ImpettusGruposPreparacoesRepository.Delete(idSelecionado);

            TempData["MensagemSucesso"] = "Preparacão inativado com sucesso.";

            var dados = _unitOfWork.ImpettusPreparacaoRepository.GetById(idSelecionado);

            var model = new PreparacoesEdicaoViewModel
            {
                IdPreparacao = dados.IdPreparacao,
                FlagCongelado = dados.FlagCongelado,
                FlagResfriado = dados.FlagResfriado,
                FlagTemperaturaAmbiente = dados.FlagTemperaturaAmbiente,
                IdGrupoPreparacao = dados.IdGrupoPreparacao,
                NomePreparacao = dados.NomePreparacao,
                Sif = dados.Sif,
                TipoValidadeResfriado = dados.TipoValidadeResfriado,
                TipoValidadeTemperaturaAmbiente = dados.TipoValidadeTemperaturaAmbiente,
                ValidadeCongelado = dados.ValidadeCongelado,
                ValidadeResfriado = dados.ValidadeResfriado,
                ValidadeTemperaturaAmbiente = dados.ValidadeTemperaturaAmbiente
            };

            var grupos = _unitOfWork.ImpettusGruposPreparacoesRepository.GetAll().OrderBy(g => g.NomeGrupoPreparacao).ToList();
            model.Grupos = new List<SelectListItem>();
            foreach (var item in grupos)
            {
                model.Grupos.Add(new SelectListItem { Value = item.IDGrupoPreparacao.ToString(), Text = item.NomeGrupoPreparacao.ToUpper() });
            }

            model.Tipos = new List<SelectListItem>();
            model.Tipos.Add(new SelectListItem { Value = "H", Text = "Horas" });
            model.Tipos.Add(new SelectListItem { Value = "D", Text = "Dias" });

            return View("Edicao", model);
        }

        public IActionResult Reativar(string id)
        {
            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            _unitOfWork.ImpettusPreparacaoRepository.UnDelete(idSelecionado);

            TempData["MensagemSucesso"] = "Preparacão reativado com sucesso.";

            var dados = _unitOfWork.ImpettusPreparacaoRepository.GetById(idSelecionado);

            var model = new PreparacoesEdicaoViewModel
            {
                IdPreparacao = dados.IdPreparacao,
                FlagCongelado = dados.FlagCongelado,
                FlagResfriado = dados.FlagResfriado,
                FlagTemperaturaAmbiente = dados.FlagTemperaturaAmbiente,
                IdGrupoPreparacao = dados.IdGrupoPreparacao,
                NomePreparacao = dados.NomePreparacao,
                Sif = dados.Sif,
                TipoValidadeResfriado = dados.TipoValidadeResfriado,
                TipoValidadeTemperaturaAmbiente = dados.TipoValidadeTemperaturaAmbiente,
                ValidadeCongelado = dados.ValidadeCongelado,
                ValidadeResfriado = dados.ValidadeResfriado,
                ValidadeTemperaturaAmbiente = dados.ValidadeTemperaturaAmbiente
            };

            var grupos = _unitOfWork.ImpettusGruposPreparacoesRepository.GetAll().OrderBy(g => g.NomeGrupoPreparacao).ToList();
            model.Grupos = new List<SelectListItem>();
            foreach (var item in grupos)
            {
                model.Grupos.Add(new SelectListItem { Value = item.IDGrupoPreparacao.ToString(), Text = item.NomeGrupoPreparacao.ToUpper() });
            }

            model.Tipos = new List<SelectListItem>();
            model.Tipos.Add(new SelectListItem { Value = "H", Text = "Horas" });
            model.Tipos.Add(new SelectListItem { Value = "D", Text = "Dias" });

            return View("Edicao", model);
        }
    }
}
