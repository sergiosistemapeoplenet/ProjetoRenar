using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjetoRenar.Application.Contracts;
using ProjetoRenar.Application.Services;
using ProjetoRenar.Application.ViewModels.Etiquetas;
using ProjetoRenar.Application.ViewModels.Produtos;
using ProjetoRenar.Application.ViewModels.Usuarios;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Entities;
using ProjetoRenar.Presentation.Mvc.Areas.App.Models;
using ProjetoRenar.Presentation.Mvc.Models;
using ProjetoRenar.Presentation.Mvc.Reports;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using static ProjetoRenar.Presentation.Mvc.Reports.ImpettusEtiquetasReport;

namespace ProjetoRenar.Presentation.Mvc.Areas.App.Controllers
{
    [Area("App")]
    [Authorize(Roles = "Administrador")]
    public class ImpettusPrincipalController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ImpettusPrincipalController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var model = new ImpettusImprimirEtiquetasViewModel();

            try
            {
                var minhaContaUsuario = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjetoRenar.Application.ViewModels.Usuarios.MinhaContaViewModel>(User.Identity.Name);
               
                if (minhaContaUsuario != null && minhaContaUsuario.FlagPrimeiroAcesso != null && minhaContaUsuario.FlagPrimeiroAcesso.Value)
                    return RedirectToAction("RedefinirSenha", "Principal");
            }
            catch (Exception e)
            {
                return RedirectToAction("index", "principal");
            }

            return View(model);
        }

        public IActionResult UtilizacaoDescarte(FiltrosViewModel filtros)
        {
            var lista = _unitOfWork.ImpettusProdutoRepository.ListarControleEtiqueta();
            var etiquetas = new List<ControleEtiquetaModel>();

            foreach (var item in lista)
            {
                etiquetas.Add(new ControleEtiquetaModel
                {
                    DataImpressao = item.DataImpressao,
                    Etiqueta = JsonConvert.DeserializeObject<EtiquetaProdutoDTO>(item.ConteudoEtiqueta),
                    FlagAtivo = item.FlagAtivo,
                    Id = item.Id
                });
            }

            // Aplicar filtros no servidor
            if (!string.IsNullOrWhiteSpace(filtros.Nome))
            {
                etiquetas = etiquetas
                    .Where(e => e.Etiqueta.NomeProduto.ToLower().Contains(filtros.Nome.ToLower()))
                    .ToList();
            }

            etiquetas = etiquetas
                .Where(e => (filtros.Produtos && e.Etiqueta.FlagProduto) || (filtros.Preparacoes && e.Etiqueta.FlagPreparacao))
                .ToList();

            if (filtros.DataInicio.HasValue)
                etiquetas = etiquetas.Where(e => e.DataImpressao.Date >= filtros.DataInicio.Value.Date).ToList();

            if (filtros.DataFim.HasValue)
                etiquetas = etiquetas.Where(e => e.DataImpressao.Date <= filtros.DataFim.Value.Date).ToList();

            filtros.Etiquetas = etiquetas;

            var hoje = DateTime.Now.Date;

            filtros.Etiquetas = filtros.Etiquetas.Where(item =>
            {
                if (!DateTime.TryParse(item.Etiqueta.DataValidade, out var dataValidade))
                    return true; // mantém se não conseguir interpretar data

                switch (filtros.StatusValidade)
                {
                    case StatusValidadeFiltro.Vencido:
                        return dataValidade < hoje;
                    case StatusValidadeFiltro.AVencer:
                        var diffDias = (dataValidade - hoje).TotalDays;
                        return diffDias >= 0 && diffDias <= 1;
                    case StatusValidadeFiltro.Valido:
                        return dataValidade > hoje.AddDays(1);
                    default:
                        return true; // Todos
                }
            }).ToList();


            return View(filtros);
        }



        private static string RemoverAcentos(string texto)
        {
            var normalized = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        [HttpPost]
        public IActionResult Index(ImpettusImprimirEtiquetasViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.ListagemProdutos = new List<ImpettusProdutoModel>();

                var produtos = _unitOfWork.ImpettusProdutoRepository.GetAll();
                var preparacoes = _unitOfWork.ImpettusPreparacaoRepository.GetAll();

                if (!string.IsNullOrEmpty(model.NomeProduto))
                {
                    string nomeBusca = RemoverAcentos(model.NomeProduto).ToLower();

                    produtos = produtos
                        .Where(p => RemoverAcentos(p.NomeProduto).ToLower().Contains(nomeBusca))
                        .ToList();

                    preparacoes = preparacoes
                        .Where(p => RemoverAcentos(p.NomePreparacao).ToLower().Contains(nomeBusca))
                        .ToList();                    
                }

                if (model.CheckProdutos)
                {
                    foreach (var item in produtos)
                    {
                        model.ListagemProdutos.Add(new ImpettusProdutoModel
                        {
                            IdProduto = item.IdProduto,
                            FlagAtivo = item.FlagAtivo,
                            FlagCongelado = item.FlagCongelado,
                            FlagResfriado = item.FlagResfriado,
                            FlagTemperaturaAmbiente = item.FlagTemperaturaAmbiente,
                            IdGrupoProduto = item.IdGrupoProduto,
                            NomeGrupoProduto = _unitOfWork.ImpettusGruposProdutoRepository.GetById(item.IdGrupoProduto.Value).NomeGrupoProduto,
                            NomeProduto = item.NomeProduto,
                            Sif = item.Sif,
                            TipoValidadeResfriado = item.TipoValidadeResfriado,
                            TipoValidadeTemperaturaAmbiente = item.TipoValidadeTemperaturaAmbiente,
                            ValidadeCongelado = item.ValidadeCongelado,
                            ValidadeResfriado = item.ValidadeResfriado,
                            ValidadeTemperaturaAmbiente = item.ValidadeTemperaturaAmbiente,
                            PreparacaoProduto = "produto",
                            FlagFavorito = item.FlagFavorito
                        });
                    }
                }

                if (model.CheckPreparacoes)
                {
                    foreach (var item in preparacoes)
                    {
                        model.ListagemProdutos.Add(new ImpettusProdutoModel
                        {
                            IdProduto = item.IdPreparacao,
                            FlagAtivo = item.FlagAtivo,
                            FlagCongelado = item.FlagCongelado,
                            FlagResfriado = item.FlagResfriado,
                            FlagTemperaturaAmbiente = item.FlagTemperaturaAmbiente,
                            IdGrupoProduto = item.IdGrupoPreparacao,
                            NomeGrupoProduto = _unitOfWork.ImpettusGruposPreparacoesRepository.GetById(item.IdGrupoPreparacao.Value).NomeGrupoPreparacao,
                            NomeProduto = item.NomePreparacao,
                            Sif = item.Sif,
                            TipoValidadeResfriado = item.TipoValidadeResfriado,
                            TipoValidadeTemperaturaAmbiente = item.TipoValidadeTemperaturaAmbiente,
                            ValidadeCongelado = item.ValidadeCongelado,
                            ValidadeResfriado = item.ValidadeResfriado,
                            ValidadeTemperaturaAmbiente = item.ValidadeTemperaturaAmbiente,
                            PreparacaoProduto = "preparacao",
                            FlagFavorito = item.FlagFavorito
                        });
                    }
                }

                foreach (var item in model.ListagemProdutos)
                {
                    if(item.FlagCongelado)
                    {
                        item.DataValidadeCongelado = DateTime.Now.AddDays(item.ValidadeCongelado.Value).ToString("dd/MM/yyyy");
                    }
                    if (item.FlagResfriado && item.TipoValidadeResfriado != null && item.TipoValidadeResfriado.Equals("D"))
                    {
                        item.DataValidadeResfriado = DateTime.Now.AddDays(item.ValidadeResfriado.Value).ToString("dd/MM/yyyy");
                    }
                    if (item.FlagResfriado && item.TipoValidadeResfriado != null && item.TipoValidadeResfriado.Equals("H"))
                    {
                        item.DataValidadeResfriado = DateTime.Now.AddHours(item.ValidadeResfriado.Value).ToString("dd/MM/yyyy - HH'H'mm");
                    }
                    if (item.FlagTemperaturaAmbiente && item.TipoValidadeTemperaturaAmbiente != null && item.TipoValidadeTemperaturaAmbiente.Equals("D"))
                    {
                        item.DataValidadeTemperaturaAmbiente = DateTime.Now.AddDays(item.ValidadeTemperaturaAmbiente.Value).ToString("dd/MM/yyyy");
                    }
                    if (item.FlagTemperaturaAmbiente && item.TipoValidadeTemperaturaAmbiente != null && item.TipoValidadeTemperaturaAmbiente.Equals("H"))
                    {
                        item.DataValidadeTemperaturaAmbiente = DateTime.Now.AddHours(item.ValidadeTemperaturaAmbiente.Value).ToString("dd/MM/yyyy - HH'H'mm");
                    }
                }

                /*
                if(!string.IsNullOrEmpty(model.Tipo))
                {
                    var produtos = _unitOfWork.ImpettusProdutoRepository.GetAll();
                    var preparacoes = _unitOfWork.ImpettusPreparacaoRepository.GetAll();

                    if(!string.IsNullOrEmpty(model.NomeProduto))
                    {
                        string nomeBusca = RemoverAcentos(model.NomeProduto).ToLower();

                        produtos = produtos
                            .Where(p => RemoverAcentos(p.NomeProduto).ToLower().Contains(nomeBusca))
                            .ToList();

                        preparacoes = preparacoes
                            .Where(p => RemoverAcentos(p.NomePreparacao).ToLower().Contains(nomeBusca))
                            .ToList();
                    }

                    if(model.Tipo.Equals("1")) //Congelados
                    {
                        produtos = produtos.Where(p => p.FlagCongelado).ToList();
                        preparacoes = preparacoes.Where(p => p.FlagCongelado).ToList();

                        if (model.CheckProdutos)
                        {
                            foreach (var item in produtos)
                            {
                                model.ListagemProdutos.Add(new ImpettusProdutoModel
                                {
                                    IdProduto = item.IdProduto,
                                    DataAtual = DateTime.Now.ToString("dd/MM/yyyy"),
                                    DataValidade = DateTime.Now.AddDays(item.ValidadeCongelado.Value).ToString("dd/MM/yyyy"),
                                    FlagPreparacao = false,
                                    FlagProduto = true,
                                    NomeProduto = item.NomeProduto,
                                    TipoProduto = _unitOfWork.ImpettusGruposProdutoRepository.GetById(item.IdGrupoProduto.Value).NomeGrupoProduto,
                                    Tipo = "CONGELADO"
                                });
                            }
                        }

                        if (model.CheckPreparacoes)
                        {
                            foreach (var item in preparacoes)
                            {
                                model.ListagemProdutos.Add(new ImpettusProdutoModel
                                {
                                    IdProduto = item.IdPreparacao,
                                    DataAtual = DateTime.Now.ToString("dd/MM/yyyy"),
                                    DataValidade = DateTime.Now.AddDays(item.ValidadeCongelado.Value).ToString("dd/MM/yyyy"),
                                    FlagPreparacao = true,
                                    FlagProduto = false,
                                    NomeProduto = item.NomePreparacao,
                                    TipoProduto = _unitOfWork.ImpettusGruposPreparacoesRepository.GetById(item.IdGrupoPreparacao.Value).NomeGrupoPreparacao,
                                    Tipo = "CONGELADO"
                                });
                            }
                        }
                    }
                    else if(model.Tipo.Equals("2")) //Resfriados
                    {
                        produtos = produtos.Where(p => p.FlagResfriado).ToList();
                        preparacoes = preparacoes.Where(p => p.FlagResfriado).ToList();

                        if (model.CheckProdutos)
                        {
                            foreach (var item in produtos)
                            {
                                if(item.TipoValidadeResfriado.Equals("D"))
                                {
                                    model.ListagemProdutos.Add(new ImpettusProdutoModel
                                    {
                                        IdProduto = item.IdProduto,
                                        DataAtual = DateTime.Now.ToString("dd/MM/yyyy"),
                                        DataValidade = DateTime.Now.AddDays(item.ValidadeResfriado.Value).ToString("dd/MM/yyyy"),
                                        FlagPreparacao = false,
                                        FlagProduto = true,
                                        NomeProduto = item.NomeProduto,
                                        TipoProduto = _unitOfWork.ImpettusGruposProdutoRepository.GetById(item.IdGrupoProduto.Value).NomeGrupoProduto,
                                        Tipo = "CONGELADO"
                                    });
                                }
                                else if (item.TipoValidadeResfriado.Equals("H"))
                                {
                                    model.ListagemProdutos.Add(new ImpettusProdutoModel
                                    {
                                        IdProduto = item.IdProduto,
                                        DataAtual = DateTime.Now.ToString("dd/MM/yyyy - HH'H'mm"),
                                        DataValidade = DateTime.Now.AddHours(item.ValidadeResfriado.Value).ToString("dd/MM/yyyy - HH'H'mm"),
                                        FlagPreparacao = false,
                                        FlagProduto = true,
                                        NomeProduto = item.NomeProduto,
                                        TipoProduto = _unitOfWork.ImpettusGruposProdutoRepository.GetById(item.IdGrupoProduto.Value).NomeGrupoProduto,
                                        Tipo = "CONGELADO"
                                    });
                                }                                
                            }
                        }

                        if (model.CheckPreparacoes)
                        {
                            foreach (var item in preparacoes)
                            {
                                if (item.TipoValidadeResfriado.Equals("D"))
                                {
                                    model.ListagemProdutos.Add(new ImpettusProdutoModel
                                    {
                                        IdProduto = item.IdPreparacao,
                                        DataAtual = DateTime.Now.ToString("dd/MM/yyyy"),
                                        DataValidade = DateTime.Now.AddDays(item.ValidadeResfriado.Value).ToString("dd/MM/yyyy"),
                                        FlagPreparacao = true,
                                        FlagProduto = false,
                                        NomeProduto = item.NomePreparacao,
                                        TipoProduto = _unitOfWork.ImpettusGruposPreparacoesRepository.GetById(item.IdGrupoPreparacao.Value).NomeGrupoPreparacao,
                                        Tipo = "RESFRIADO"
                                    });
                                }
                                else if (item.TipoValidadeResfriado.Equals("H"))
                                {
                                    model.ListagemProdutos.Add(new ImpettusProdutoModel
                                    {
                                        IdProduto = item.IdPreparacao,
                                        DataAtual = DateTime.Now.ToString("dd/MM/yyyy - HH'H'mm"),
                                        DataValidade = DateTime.Now.AddHours(item.ValidadeResfriado.Value).ToString("dd/MM/yyyy - HH'H'mm"),
                                        FlagPreparacao = true,
                                        FlagProduto = false,
                                        NomeProduto = item.NomePreparacao,
                                        TipoProduto = _unitOfWork.ImpettusGruposPreparacoesRepository.GetById(item.IdGrupoPreparacao.Value).NomeGrupoPreparacao,
                                        Tipo = "RESFRIADO"
                                    });
                                }
                            }
                        }
                    }
                    else if(model.Tipo.Equals("3")) //Temperatura Ambiente
                    {
                        produtos = produtos.Where(p => p.FlagTemperaturaAmbiente).ToList();
                        preparacoes = preparacoes.Where(p => p.FlagTemperaturaAmbiente).ToList();

                        if (model.CheckProdutos)
                        {
                            foreach (var item in produtos)
                            {
                                if (item.TipoValidadeTemperaturaAmbiente.Equals("D"))
                                {
                                    model.ListagemProdutos.Add(new ImpettusProdutoModel
                                    {
                                        IdProduto = item.IdProduto,
                                        DataAtual = DateTime.Now.ToString("dd/MM/yyyy"),
                                        DataValidade = DateTime.Now.AddDays(item.ValidadeTemperaturaAmbiente.Value).ToString("dd/MM/yyyy"),
                                        FlagPreparacao = false,
                                        FlagProduto = true,
                                        NomeProduto = item.NomeProduto,
                                        TipoProduto = _unitOfWork.ImpettusGruposProdutoRepository.GetById(item.IdGrupoProduto.Value).NomeGrupoProduto,
                                        Tipo = "TEMPERATURA AMBIENTE"
                                    });
                                }
                                else if (item.TipoValidadeTemperaturaAmbiente.Equals("H"))
                                {
                                    model.ListagemProdutos.Add(new ImpettusProdutoModel
                                    {
                                        IdProduto = item.IdProduto,
                                        DataAtual = DateTime.Now.ToString("dd/MM/yyyy - HH'H'mm"),
                                        DataValidade = DateTime.Now.AddHours(item.ValidadeTemperaturaAmbiente.Value).ToString("dd/MM/yyyy - HH'H'mm"),
                                        FlagPreparacao = false,
                                        FlagProduto = true,
                                        NomeProduto = item.NomeProduto,
                                        TipoProduto = _unitOfWork.ImpettusGruposProdutoRepository.GetById(item.IdGrupoProduto.Value).NomeGrupoProduto,
                                        Tipo = "TEMPERATURA AMBIENTE"
                                    });
                                }
                            }
                        }

                        if (model.CheckPreparacoes)
                        {
                            foreach (var item in preparacoes)
                            {
                                if (item.TipoValidadeTemperaturaAmbiente.Equals("D"))
                                {
                                    model.ListagemProdutos.Add(new ImpettusProdutoModel
                                    {
                                        IdProduto = item.IdPreparacao,
                                        DataAtual = DateTime.Now.ToString("dd/MM/yyyy"),
                                        DataValidade = DateTime.Now.AddDays(item.ValidadeTemperaturaAmbiente.Value).ToString("dd/MM/yyyy"),
                                        FlagPreparacao = true,
                                        FlagProduto = false,
                                        NomeProduto = item.NomePreparacao,
                                        TipoProduto = _unitOfWork.ImpettusGruposPreparacoesRepository.GetById(item.IdGrupoPreparacao.Value).NomeGrupoPreparacao,
                                        Tipo = "TEMPERATURA AMBIENTE"
                                    });
                                }
                                else if (item.TipoValidadeTemperaturaAmbiente.Equals("H"))
                                {
                                    model.ListagemProdutos.Add(new ImpettusProdutoModel
                                    {
                                        IdProduto = item.IdPreparacao,
                                        DataAtual = DateTime.Now.ToString("dd/MM/yyyy - HH'H'mm"),
                                        DataValidade = DateTime.Now.AddHours(item.ValidadeTemperaturaAmbiente.Value).ToString("dd/MM/yyyy - HH'H'mm"),
                                        FlagPreparacao = true,
                                        FlagProduto = false,
                                        NomeProduto = item.NomePreparacao,
                                        TipoProduto = _unitOfWork.ImpettusGruposPreparacoesRepository.GetById(item.IdGrupoPreparacao.Value).NomeGrupoPreparacao,
                                        Tipo = "TEMPERATURA AMBIENTE"
                                    });
                                }
                            }
                        }
                    }       
                    
                    if(!model.ListagemProdutos.Any())
                        TempData["MensagemAlerta"] = "Nenhum resultado foi encontrado para o filtro de busca informado.";
                }
                else
                {
                    TempData["MensagemAlerta"] = "Por favor, selecione um tipo (Resfriado, Congelado ou Temperatura Ambiente).";
                }
                */
            }

            model.ListagemProdutos = model.ListagemProdutos.OrderBy(p => p.NomeProduto).ToList();

            try
            {
                var minhaContaUsuario = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjetoRenar.Application.ViewModels.Usuarios.MinhaContaViewModel>(User.Identity.Name);               
            }
            catch (Exception e)
            {
                return RedirectToAction("index", "principal");
            }

            if(model.CheckFavorito)
            {
                model.ListagemProdutos = model.ListagemProdutos.Where(p => p.FlagFavorito == 1).ToList();
            }

            return View(model);
        }

        public IActionResult DarBaixa(int id)
        {
            _unitOfWork.ImpettusProdutoRepository.BaixarControleEtiqueta(id);
            TempData["MensagemSucesso"] = "Etiqueta baixada com sucesso!";
            return RedirectToAction("UtilizacaoDescarte");
        }


        [Authorize(Roles = "Administrador")]
        public IActionResult Dashboard()
        {
            var usuarioAutenticado = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjetoRenar.Application.ViewModels.Usuarios.MinhaContaViewModel>(User.Identity.Name);
            if (usuarioAutenticado != null && usuarioAutenticado.FlagPrimeiroAcesso != null && usuarioAutenticado.FlagPrimeiroAcesso.Value)
                return RedirectToAction("RedefinirSenha", "Principal");

            var dataAtual = DateTime.Now;

            var model = new DashboardDatasViewModel
            {
                DataInicio = new DateTime(dataAtual.Year, dataAtual.Month, 1),
                DataFim = new DateTime(dataAtual.Year, dataAtual.Month, 1).AddMonths(1).AddDays(-1)
            };

            ViewBag.Tipo = "0";

            ViewBag.ConsultarQuantidadeImpressaoPeriodo_Geral = _unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_Geral(model.DataInicio.Value, model.DataFim.Value);
            ViewBag.ConsultarQuantidadeImpressaoPeriodo_Regiao = _unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_Regiao(model.DataInicio.Value, model.DataFim.Value);
            ViewBag.ConsultarQuantidadeImpressaoPeriodo_Unidade = _unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_Unidade(model.DataInicio.Value, model.DataFim.Value);
            ViewBag.ConsultarQuantidadeImpressaoPeriodo_TipoDeProduto = _unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_TipoDeProduto(model.DataInicio.Value, model.DataFim.Value);
            ViewBag.ConsultarQuantidadeImpressaoPeriodo_Produto = _unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_Produto(model.DataInicio.Value, model.DataFim.Value);

            ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Geral = _unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Geral(model.DataInicio.Value, model.DataFim.Value);
            ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Regiao = _unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Regiao(model.DataInicio.Value, model.DataFim.Value);
            ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Unidade = _unitOfWork.DashboardRepository.ConsuConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Unidade(model.DataInicio.Value, model.DataFim.Value);
            ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_TipoDeProduto = _unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_TipoDeProduto(model.DataInicio.Value, model.DataFim.Value);
            ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Produto = _unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Produto(model.DataInicio.Value, model.DataFim.Value);

            return View(model);
        }

        [HttpPost]
        public IActionResult Dashboard(DashboardDatasViewModel model, string tipo)
        {
            ViewBag.Tipo = tipo;

            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.ConsultarQuantidadeImpressaoPeriodo_Geral = _unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_Geral(model.DataInicio.Value, model.DataFim.Value);
                    ViewBag.ConsultarQuantidadeImpressaoPeriodo_Regiao = _unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_Regiao(model.DataInicio.Value, model.DataFim.Value);
                    ViewBag.ConsultarQuantidadeImpressaoPeriodo_Unidade = _unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_Unidade(model.DataInicio.Value, model.DataFim.Value);
                    ViewBag.ConsultarQuantidadeImpressaoPeriodo_TipoDeProduto = _unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_TipoDeProduto(model.DataInicio.Value, model.DataFim.Value);
                    ViewBag.ConsultarQuantidadeImpressaoPeriodo_Produto = _unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_Produto(model.DataInicio.Value, model.DataFim.Value);

                    ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Geral = _unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Geral(model.DataInicio.Value, model.DataFim.Value);
                    ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Regiao = _unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Regiao(model.DataInicio.Value, model.DataFim.Value);
                    ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Unidade = _unitOfWork.DashboardRepository.ConsuConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Unidade(model.DataInicio.Value, model.DataFim.Value);
                    ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_TipoDeProduto = _unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_TipoDeProduto(model.DataInicio.Value, model.DataFim.Value);
                    ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Produto = _unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Produto(model.DataInicio.Value, model.DataFim.Value);
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = e.Message;
                }
            }

            return View();
        }


        [HttpPost]
        public IActionResult Imprimir(string[] IdsProdutos, string[] QtdImpressoes, Dictionary<int, string> Sif, Dictionary<int, string> Lote, Dictionary<int, string> Quantidade)
        {
            try
            {
                var produtos = new List<ImpettusProdutoImpressaoModel>();
                var produtosPreparacoes = new List<ProdutoPreparacao>();

                for (int i = 0; i < IdsProdutos.Length; i++)
                {
                    var tipoProduto = IdsProdutos[i].Split(";")[1];

                    if(tipoProduto.Equals("1"))
                    {
                        var produto = _unitOfWork.ImpettusProdutoRepository.GetById(int.Parse(IdsProdutos[i].Split(";")[0]));
                        produtos.Add(new ImpettusProdutoImpressaoModel
                        {
                            FlagAtivo = produto.FlagAtivo,       
                            Id = produto.IdProduto,
                            IdGrupo = produto.IdGrupoProduto,
                            Nome = produto.NomeProduto,
                            NomeGrupo = _unitOfWork.ImpettusGruposProdutoRepository.GetById(produto.IdGrupoProduto.Value).NomeGrupoProduto,
                            QtdImpressoes = int.Parse(QtdImpressoes[i]),
                            TipoValidadeResfriado = produto.TipoValidadeResfriado,
                            TipoValidadeTemperaturaAmbiente = produto.TipoValidadeTemperaturaAmbiente,
                            ValidadeCongelado = produto.ValidadeCongelado,
                            ValidadeResfriado = produto.ValidadeResfriado,
                            ValidadeTemperaturaAmbiente = produto.ValidadeTemperaturaAmbiente,
                            ProdutoPreparacao = "produto",
                            FlagProduto = true,
                            FlagPreparacao = false,
                            Sif = Sif[produto.IdProduto],
                            Lote = Lote[produto.IdProduto],
                            Quantidade = Quantidade[produto.IdProduto],
                        });
                    }
                    else if(tipoProduto.Equals("2"))
                    {
                        var produto = _unitOfWork.ImpettusPreparacaoRepository.GetById(int.Parse(IdsProdutos[i].Split(";")[0]));
                        produtos.Add(new ImpettusProdutoImpressaoModel
                        {
                            FlagAtivo = produto.FlagAtivo,
                            Id = produto.IdPreparacao,
                            IdGrupo = produto.IdGrupoPreparacao,
                            Nome = produto.NomePreparacao,
                            NomeGrupo = _unitOfWork.ImpettusGruposPreparacoesRepository.GetById(produto.IdGrupoPreparacao.Value).NomeGrupoPreparacao,
                            QtdImpressoes = int.Parse(QtdImpressoes[i]),                           
                            TipoValidadeResfriado = produto.TipoValidadeResfriado,
                            TipoValidadeTemperaturaAmbiente = produto.TipoValidadeTemperaturaAmbiente,
                            ValidadeCongelado = produto.ValidadeCongelado,
                            ValidadeResfriado = produto.ValidadeResfriado,
                            ValidadeTemperaturaAmbiente = produto.ValidadeTemperaturaAmbiente,
                            ProdutoPreparacao = "preparacao",
                            FlagProduto = false,
                            FlagPreparacao = true,
                            Sif = Sif[produto.IdPreparacao],
                            Lote = Lote[produto.IdPreparacao],
                            Quantidade = Quantidade[produto.IdPreparacao],
                        });
                    }

                    var form = Request.Form;

                    var produtosSelecionados = form["IdsProdutos"];
                    
                    foreach (var idComSufixo in produtosSelecionados)
                    {
                        var idProdutoStr = idComSufixo.ToString().Split(';')[0];

                        if (int.TryParse(idProdutoStr, out int idProduto))
                        {
                            var nomeCampoRadio = $"tipo{idProduto}";
                            var tipoSelecionado = form[nomeCampoRadio];

                            produtosPreparacoes.Add(new ProdutoPreparacao
                            {
                                ProdutoId = idProduto,
                                TipoProdutoPreparacao = tipoSelecionado
                            });
                        }
                    }

                    try
                    {
                        var minhaContaUsuario = Newtonsoft.Json.JsonConvert.DeserializeObject<MinhaContaViewModel>(User.Identity.Name);
                    }
                    catch (Exception e)
                    {

                    }
                }

                foreach (var item in produtos)
                {
                    try
                    {
                        var itemProduto = produtosPreparacoes.FirstOrDefault(p => p.ProdutoId == item.Id && p.TipoProdutoPreparacao.Split(";")[1].Equals("produto"));
                        var itemPreparacao = produtosPreparacoes.FirstOrDefault(p => p.ProdutoId == item.Id && p.TipoProdutoPreparacao.Split(";")[1].Equals("preparacao"));

                        if (itemProduto != null)
                        {
                            if (itemProduto.TipoProdutoPreparacao.Split(";")[0].Equals("congelado")) item.FlagCongelado = true;
                            if (itemProduto.TipoProdutoPreparacao.Split(";")[0].Equals("resfriado")) item.FlagResfriado = true;
                            if (itemProduto.TipoProdutoPreparacao.Split(";")[0].Equals("temperaturaambiente")) item.FlagTemperaturaAmbiente = true;
                        }

                        if (itemPreparacao != null)
                        {
                            if (itemPreparacao.TipoProdutoPreparacao.Split(";")[0].Equals("congelado")) item.FlagCongelado = true;
                            if (itemPreparacao.TipoProdutoPreparacao.Split(";")[0].Equals("resfriado")) item.FlagResfriado = true;
                            if (itemPreparacao.TipoProdutoPreparacao.Split(";")[0].Equals("temperaturaambiente")) item.FlagTemperaturaAmbiente = true;
                        }
                    }
                    catch(Exception e)
                    {
                        return StatusCode(500, new { mensagem = "Não foi possível gerar as etiquetas para impressão. Verifique se os itens estão selecionados corretamente." });
                    }
                }

                byte[] pdfBytes = null;

                if (produtos.Any())
                {
                    foreach (var item in produtos)
                    {
                        try
                        {
                            var minhaContaUsuario = Newtonsoft.Json.JsonConvert.DeserializeObject<MinhaContaViewModel>(User.Identity.Name);

                            if (minhaContaUsuario.Unidades != null && minhaContaUsuario.Unidades.FirstOrDefault() != null)
                            {
                                if(item.ProdutoPreparacao != null && item.ProdutoPreparacao.Equals("produto"))
                                {
                                    _unitOfWork.ImpettusProdutoRepository.IncluirControleImpressao
                                        (minhaContaUsuario.Unidades.FirstOrDefault().IDUnidade, item.Id, 0, item.QtdImpressoes, minhaContaUsuario.IdUsuario);
                                }
                                if (item.ProdutoPreparacao != null && item.ProdutoPreparacao.Equals("preparacao"))
                                {
                                    _unitOfWork.ImpettusProdutoRepository.IncluirControleImpressao
                                        (minhaContaUsuario.Unidades.FirstOrDefault().IDUnidade, 0, item.Id, item.QtdImpressoes, minhaContaUsuario.IdUsuario);
                                }
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }

                    var minhaConta = JsonConvert.DeserializeObject<MinhaContaViewModel>(User.Identity.Name);
                    var usuario = _unitOfWork.UsuarioRepository.Get(minhaConta.EmailUsuario);

                    var unidade = minhaConta.Unidades.FirstOrDefault(u => u.FlagUnidadePadrao);

                    try
                    {
                        if (string.IsNullOrEmpty(usuario.Nome)) usuario.Nome = "-";

                        if (unidade != null) unidade.NomeContato = usuario.Nome;
                        minhaConta.Unidades[0].NomeContato = usuario.Nome;
                    }
                    catch (Exception e) { }
                    
                    if(unidade != null)
                    {                        
                        pdfBytes = ImpettusEtiquetasReport.ImprimirEtiqueta(produtos, unidade, _unitOfWork);
                    }
                    else if(minhaConta.Unidades != null && minhaConta.Unidades.Count > 0)
                    {
                        pdfBytes = ImpettusEtiquetasReport.ImprimirEtiqueta(produtos, minhaConta.Unidades[0], _unitOfWork);
                    }
                    else
                    {
                        return StatusCode(500, new { mensagem = "Não há uma loja configurada para este usuário." });
                    }
                }

                return File(pdfBytes, "application/pdf", $"{Guid.NewGuid()}.pdf");
            }
            catch (Exception e)
            {
                return StatusCode(500, new { mensagem = "Não foi possível realizar a impressão" });
            }
        }
    }

    public class ProdutoPreparacao
    {
        public int ProdutoId { get; set; }
        public string TipoProdutoPreparacao { get; set; }
    }

    public class ImpettusProdutoImpressaoModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public bool FlagResfriado { get; set; }
        public string TipoValidadeResfriado { get; set; }
        public int? ValidadeResfriado { get; set; }
        public bool FlagCongelado { get; set; }
        public int? ValidadeCongelado { get; set; }
        public bool FlagTemperaturaAmbiente { get; set; }
        public string TipoValidadeTemperaturaAmbiente { get; set; }
        public int? ValidadeTemperaturaAmbiente { get; set; }
        public int? IdGrupo { get; set; }
        public string NomeGrupo { get; set; }
        public bool FlagAtivo { get; set; }
        public string Sif { get; set; }
        public int QtdImpressoes { get; set; }
        public string ProdutoPreparacao { get; set; }
        public bool FlagProduto { get; set; }
        public bool FlagPreparacao{ get; set; }
        public string Lote { get; set; }
        public string Quantidade { get; set; }
    }

    public class ControleEtiquetaModel
    {
        public int Id { get; set; }
        public DateTime DataImpressao { get; set; }
        public EtiquetaProdutoDTO Etiqueta { get; set; }
        public int FlagAtivo { get; set; }
    }

    public class FiltrosViewModel
    {
        public bool Produtos { get; set; } = true;
        public bool Preparacoes { get; set; } = true;

        public string Nome { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        public List<ControleEtiquetaModel> Etiquetas { get; set; } = new List<ControleEtiquetaModel>();

        public StatusValidadeFiltro StatusValidade { get; set; } = StatusValidadeFiltro.Todos;
    }

    public enum StatusValidadeFiltro
    {
        Todos,
        Vencido,
        AVencer,
        Valido
    }
}
