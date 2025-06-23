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
    public class ImpettusProdutosController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ImpettusProdutosController(IUnitOfWork unitOfWork)
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

                var consulta = _unitOfWork.ImpettusProdutoRepository.GetAll()
                    .Where(u => u.FlagAtivo == (ativo == 1) && u.NomeProduto != null && u.NomeProduto.ToLower().Contains(nome != null ? nome.ToLower() : string.Empty))
                    .OrderBy(u => u.NomeProduto).ToList();

                foreach (var item in consulta)
                {
                    item.NomeGrupoProduto = _unitOfWork.ImpettusGruposProdutoRepository.GetById(item.IdGrupoProduto.Value).NomeGrupoProduto;
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
            var model = new ProdutoCadastroViewModel();

            var grupos = _unitOfWork.ImpettusGruposProdutoRepository.GetAll().OrderBy(g => g.NomeGrupoProduto).ToList();
            model.Grupos = new List<SelectListItem>();
            foreach (var item in grupos)
            {
                model.Grupos.Add(new SelectListItem { Value = item.IDGrupoProduto.ToString(), Text = item.NomeGrupoProduto.ToUpper() });
            }

            model.Tipos = new List<SelectListItem>();
            model.Tipos.Add(new SelectListItem { Value = "H", Text = "Horas" });
            model.Tipos.Add(new SelectListItem { Value = "D", Text = "Dias" });

            return View(model);
        }

        [HttpPost]
        public IActionResult Cadastro(ProdutoCadastroViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.FlagAtivo = true;
                    _unitOfWork.ImpettusProdutoRepository.Insert(new ImpettusProduto
                    {
                        FlagAtivo = true,
                        FlagCongelado = model.FlagCongelado,
                        FlagResfriado = model.FlagResfriado,
                        FlagTemperaturaAmbiente = model.FlagTemperaturaAmbiente,
                        IdGrupoProduto = model.IdGrupoProduto,
                        NomeProduto = model.NomeProduto,
                        Sif = model.Sif,
                        TipoValidadeResfriado = model.TipoValidadeResfriado,
                        TipoValidadeTemperaturaAmbiente = model.TipoValidadeTemperaturaAmbiente,
                        ValidadeCongelado = model.ValidadeCongelado,
                        ValidadeResfriado = model.ValidadeResfriado,
                        ValidadeTemperaturaAmbiente = model.ValidadeTemperaturaAmbiente
                    });

                    ModelState.Clear();

                    model = new ProdutoCadastroViewModel();

                    TempData["MensagemSucesso"] = "Produto cadastrado com sucesso.";
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Erro ao cadastrar produto.";
                }
            }

            var grupos = _unitOfWork.ImpettusGruposProdutoRepository.GetAll().OrderBy(g => g.NomeGrupoProduto).ToList();
            model.Grupos = new List<SelectListItem>();
            foreach (var item in grupos)
            {
                model.Grupos.Add(new SelectListItem { Value = item.IDGrupoProduto.ToString(), Text = item.NomeGrupoProduto.ToUpper() });
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
            var dados = _unitOfWork.ImpettusProdutoRepository.GetById(idSelecionado);

            var model = new ProdutoEdicaoViewModel
            {
                IdProduto = dados.IdProduto,
                FlagAtivo = dados.FlagAtivo,
                FlagCongelado = dados.FlagCongelado,
                FlagResfriado = dados.FlagResfriado,
                FlagTemperaturaAmbiente = dados.FlagTemperaturaAmbiente,
                IdGrupoProduto = dados.IdGrupoProduto,
                NomeProduto = dados.NomeProduto,
                Sif = dados.Sif,
                TipoValidadeResfriado = dados.TipoValidadeResfriado,
                TipoValidadeTemperaturaAmbiente = dados.TipoValidadeTemperaturaAmbiente,
                ValidadeCongelado = dados.ValidadeCongelado,
                ValidadeResfriado = dados.ValidadeResfriado,
                ValidadeTemperaturaAmbiente = dados.ValidadeTemperaturaAmbiente
            };

            var grupos = _unitOfWork.ImpettusGruposProdutoRepository.GetAll().OrderBy(g => g.NomeGrupoProduto).ToList();
            model.Grupos = new List<SelectListItem>();
            foreach (var item in grupos)
            {
                model.Grupos.Add(new SelectListItem { Value = item.IDGrupoProduto.ToString(), Text = item.NomeGrupoProduto.ToUpper() });
            }

            model.Tipos = new List<SelectListItem>();
            model.Tipos.Add(new SelectListItem { Value = "H", Text = "Horas" });
            model.Tipos.Add(new SelectListItem { Value = "D", Text = "Dias" });

            return View(model);
        }

        [HttpPost]
        public IActionResult Edicao(ProdutoEdicaoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dados = _unitOfWork.ImpettusProdutoRepository.GetById(model.IdProduto);

                    model.FlagAtivo = dados.FlagAtivo;
                    _unitOfWork.ImpettusProdutoRepository.Update(new ImpettusProduto
                    {
                        IdProduto = model.IdProduto,
                        FlagCongelado = model.FlagCongelado,
                        FlagResfriado = model.FlagResfriado,
                        FlagTemperaturaAmbiente = model.FlagTemperaturaAmbiente,
                        IdGrupoProduto = model.IdGrupoProduto,
                        NomeProduto = model.NomeProduto,
                        Sif = model.Sif,
                        TipoValidadeResfriado = model.TipoValidadeResfriado,
                        TipoValidadeTemperaturaAmbiente = model.TipoValidadeTemperaturaAmbiente,
                        ValidadeCongelado = model.ValidadeCongelado,
                        ValidadeResfriado = model.ValidadeResfriado,
                        ValidadeTemperaturaAmbiente = model.ValidadeTemperaturaAmbiente,
                        FlagAtivo = true
                    });

                    TempData["MensagemSucesso"] = "Produto atualizado com sucesso.";
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Erro ao atualizar produto.";
                }
            }

            var grupos = _unitOfWork.ImpettusGruposProdutoRepository.GetAll().OrderBy(g => g.NomeGrupoProduto).ToList();
            model.Grupos = new List<SelectListItem>();
            foreach (var item in grupos)
            {
                model.Grupos.Add(new SelectListItem { Value = item.IDGrupoProduto.ToString(), Text = item.NomeGrupoProduto.ToUpper() });
            }

            model.Tipos = new List<SelectListItem>();
            model.Tipos.Add(new SelectListItem { Value = "H", Text = "Horas" });
            model.Tipos.Add(new SelectListItem { Value = "D", Text = "Dias" });

            return View(model);
        }

        public IActionResult Inativar(string id)
        {
            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            _unitOfWork.ImpettusGruposProdutoRepository.Delete(idSelecionado);

            TempData["MensagemSucesso"] = "Produto inativado com sucesso.";

            var dados = _unitOfWork.ImpettusProdutoRepository.GetById(idSelecionado);

            var model = new ProdutoEdicaoViewModel
            {
                IdProduto = dados.IdProduto,
                FlagCongelado = dados.FlagCongelado,
                FlagResfriado = dados.FlagResfriado,
                FlagTemperaturaAmbiente = dados.FlagTemperaturaAmbiente,
                IdGrupoProduto = dados.IdGrupoProduto,
                NomeProduto = dados.NomeProduto,
                Sif = dados.Sif,
                TipoValidadeResfriado = dados.TipoValidadeResfriado,
                TipoValidadeTemperaturaAmbiente = dados.TipoValidadeTemperaturaAmbiente,
                ValidadeCongelado = dados.ValidadeCongelado,
                ValidadeResfriado = dados.ValidadeResfriado,
                ValidadeTemperaturaAmbiente = dados.ValidadeTemperaturaAmbiente
            };

            var grupos = _unitOfWork.ImpettusGruposProdutoRepository.GetAll().OrderBy(g => g.NomeGrupoProduto).ToList();
            model.Grupos = new List<SelectListItem>();
            foreach (var item in grupos)
            {
                model.Grupos.Add(new SelectListItem { Value = item.IDGrupoProduto.ToString(), Text = item.NomeGrupoProduto.ToUpper() });
            }

            model.Tipos = new List<SelectListItem>();
            model.Tipos.Add(new SelectListItem { Value = "H", Text = "Horas" });
            model.Tipos.Add(new SelectListItem { Value = "D", Text = "Dias" });

            return View("Edicao", model);
        }

        public IActionResult Reativar(string id)
        {
            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            _unitOfWork.ImpettusProdutoRepository.UnDelete(idSelecionado);

            TempData["MensagemSucesso"] = "Produto reativado com sucesso.";

            var dados = _unitOfWork.ImpettusProdutoRepository.GetById(idSelecionado);

            var model = new ProdutoEdicaoViewModel
            {
                IdProduto = dados.IdProduto,
                FlagCongelado = dados.FlagCongelado,
                FlagResfriado = dados.FlagResfriado,
                FlagTemperaturaAmbiente = dados.FlagTemperaturaAmbiente,
                IdGrupoProduto = dados.IdGrupoProduto,
                NomeProduto = dados.NomeProduto,
                Sif = dados.Sif,
                TipoValidadeResfriado = dados.TipoValidadeResfriado,
                TipoValidadeTemperaturaAmbiente = dados.TipoValidadeTemperaturaAmbiente,
                ValidadeCongelado = dados.ValidadeCongelado,
                ValidadeResfriado = dados.ValidadeResfriado,
                ValidadeTemperaturaAmbiente = dados.ValidadeTemperaturaAmbiente
            };

            var grupos = _unitOfWork.ImpettusGruposProdutoRepository.GetAll().OrderBy(g => g.NomeGrupoProduto).ToList();
            model.Grupos = new List<SelectListItem>();
            foreach (var item in grupos)
            {
                model.Grupos.Add(new SelectListItem { Value = item.IDGrupoProduto.ToString(), Text = item.NomeGrupoProduto.ToUpper() });
            }

            model.Tipos = new List<SelectListItem>();
            model.Tipos.Add(new SelectListItem { Value = "H", Text = "Horas" });
            model.Tipos.Add(new SelectListItem { Value = "D", Text = "Dias" });

            return View("Edicao", model);
        }
    }
}
