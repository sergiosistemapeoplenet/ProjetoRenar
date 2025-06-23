using Bogus;
using Microsoft.AspNetCore.Authorization;
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
using ProjetoRenar.Presentation.Mvc.Reports;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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

            model.ListagemTipos = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            model.ListagemTipos.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = "1",
                Text = "Congelados"
            });
            model.ListagemTipos.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = "2",
                Text = "Resfriados"
            });
            model.ListagemTipos.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = "3",
                Text = "Temperatura ambiente"
            });

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
                                    DataAtual = DateTime.Now.ToString("dd/MM/yy"),
                                    DataValidade = DateTime.Now.AddDays(item.ValidadeCongelado.Value).ToString("dd/MM/yy"),
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
                                    DataAtual = DateTime.Now.ToString("dd/MM/yy"),
                                    DataValidade = DateTime.Now.AddDays(item.ValidadeCongelado.Value).ToString("dd/MM/yy"),
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
                                        DataAtual = DateTime.Now.ToString("dd/MM/yy"),
                                        DataValidade = DateTime.Now.AddDays(item.ValidadeResfriado.Value).ToString("dd/MM/yy"),
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
                                        DataAtual = DateTime.Now.ToString("dd/MM/yy - HH'H'mm"),
                                        DataValidade = DateTime.Now.AddHours(item.ValidadeResfriado.Value).ToString("dd/MM/yy - HH'H'mm"),
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
                                        DataAtual = DateTime.Now.ToString("dd/MM/yy"),
                                        DataValidade = DateTime.Now.AddDays(item.ValidadeResfriado.Value).ToString("dd/MM/yy"),
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
                                        DataAtual = DateTime.Now.ToString("dd/MM/yy - HH'H'mm"),
                                        DataValidade = DateTime.Now.AddHours(item.ValidadeResfriado.Value).ToString("dd/MM/yy - HH'H'mm"),
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
                                        DataAtual = DateTime.Now.ToString("dd/MM/yy"),
                                        DataValidade = DateTime.Now.AddDays(item.ValidadeTemperaturaAmbiente.Value).ToString("dd/MM/yy"),
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
                                        DataAtual = DateTime.Now.ToString("dd/MM/yy - HH'H'mm"),
                                        DataValidade = DateTime.Now.AddHours(item.ValidadeTemperaturaAmbiente.Value).ToString("dd/MM/yy - HH'H'mm"),
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
                                        DataAtual = DateTime.Now.ToString("dd/MM/yy"),
                                        DataValidade = DateTime.Now.AddDays(item.ValidadeTemperaturaAmbiente.Value).ToString("dd/MM/yy"),
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
                                        DataAtual = DateTime.Now.ToString("dd/MM/yy - HH'H'mm"),
                                        DataValidade = DateTime.Now.AddHours(item.ValidadeTemperaturaAmbiente.Value).ToString("dd/MM/yy - HH'H'mm"),
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

            model.ListagemTipos = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            model.ListagemTipos.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = "1",
                Text = "Congelados"
            });
            model.ListagemTipos.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = "2",
                Text = "Resfriados"
            });
            model.ListagemTipos.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = "3",
                Text = "Temperatura ambiente"
            });

            return View(model);
        }


        [HttpPost]
        public IActionResult Imprimir(string[] IdsProdutos, string[] QtdImpressoes, string tipo)
        {
            try
            {
                var produtos = new List<ImpettusProdutoImpressaoModel>();
                for (int i = 0; i < IdsProdutos.Length; i++)
                {
                    var tipoProduto = IdsProdutos[i].Split(";")[1];

                    if(tipoProduto.Equals("1"))
                    {
                        var produto = _unitOfWork.ImpettusProdutoRepository.GetById(int.Parse(IdsProdutos[i].Split(";")[0]));
                        produtos.Add(new ImpettusProdutoImpressaoModel
                        {
                            FlagAtivo = produto.FlagAtivo,
                            FlagCongelado = tipo.Equals("1"),
                            FlagResfriado = tipo.Equals("2"),
                            FlagTemperaturaAmbiente = tipo.Equals("3"),
                            Id = produto.IdProduto,
                            IdGrupo = produto.IdGrupoProduto,
                            Nome = produto.NomeProduto,
                            NomeGrupo = _unitOfWork.ImpettusGruposProdutoRepository.GetById(produto.IdGrupoProduto.Value).NomeGrupoProduto,
                            QtdImpressoes = int.Parse(QtdImpressoes[i]),
                            Sif = produto.Sif,
                            TipoValidadeResfriado = produto.TipoValidadeResfriado,
                            TipoValidadeTemperaturaAmbiente = produto.TipoValidadeTemperaturaAmbiente,
                            ValidadeCongelado = produto.ValidadeCongelado,
                            ValidadeResfriado = produto.ValidadeResfriado,
                            ValidadeTemperaturaAmbiente = produto.ValidadeTemperaturaAmbiente
                        });
                    }
                    else if(tipoProduto.Equals("2"))
                    {
                        var produto = _unitOfWork.ImpettusPreparacaoRepository.GetById(int.Parse(IdsProdutos[i].Split(";")[0]));
                        produtos.Add(new ImpettusProdutoImpressaoModel
                        {
                            FlagAtivo = produto.FlagAtivo,
                            FlagCongelado = tipo.Equals("1"),
                            FlagResfriado = tipo.Equals("2"),
                            FlagTemperaturaAmbiente = tipo.Equals("3"),
                            Id = produto.IdPreparacao,
                            IdGrupo = produto.IdGrupoPreparacao,
                            Nome = produto.NomePreparacao,
                            NomeGrupo = _unitOfWork.ImpettusGruposPreparacoesRepository.GetById(produto.IdGrupoPreparacao.Value).NomeGrupoPreparacao,
                            QtdImpressoes = int.Parse(QtdImpressoes[i]),
                            Sif = produto.Sif,
                            TipoValidadeResfriado = produto.TipoValidadeResfriado,
                            TipoValidadeTemperaturaAmbiente = produto.TipoValidadeTemperaturaAmbiente,
                            ValidadeCongelado = produto.ValidadeCongelado,
                            ValidadeResfriado = produto.ValidadeResfriado,
                            ValidadeTemperaturaAmbiente = produto.ValidadeTemperaturaAmbiente
                        });
                    }

                    try
                    {
                        var minhaContaUsuario = Newtonsoft.Json.JsonConvert.DeserializeObject<MinhaContaViewModel>(User.Identity.Name);
                    }
                    catch (Exception e)
                    {

                    }
                }

                byte[] pdfBytes = null;

                if (produtos.Any())
                {
                    var minhaConta = JsonConvert.DeserializeObject<MinhaContaViewModel>(User.Identity.Name);
                    var unidade = minhaConta.Unidades.FirstOrDefault(u => u.FlagUnidadePadrao);                    
                    pdfBytes = ImpettusEtiquetasReport.ImprimirEtiqueta(produtos);
                }

                return File(pdfBytes, "application/pdf", $"{Guid.NewGuid()}.pdf");
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = e.Message;
            }

            return RedirectToAction("Index");
        }
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
    }
}
