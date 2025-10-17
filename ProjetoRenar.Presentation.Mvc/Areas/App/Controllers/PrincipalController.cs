using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjetoRenar.Application.ViewModels.Usuarios;
using ProjetoRenar.Application.Exceptions.Usuarios;
using ProjetoRenar.Application.Contracts;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Text;
using ProjetoRenar.Presentation.Mvc.Utils;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Sidetech.Framework.Cryptography;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProjetoRenar.CrossCutting.Configurations;
using ProjetoRenar.Application.ViewModels.Etiquetas;
using ProjetoRenar.Application.ViewModels.Produtos;
using ProjetoRenar.Presentation.Mvc.Reports;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using ProjetoRenar.Presentation.Mvc.Models;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Presentation.Mvc.Helpers;

namespace ProjetoRenar.Presentation.Mvc.Areas.App.Controllers
{
    [Area("App")]
    [Authorize]
    public class PrincipalController : Controller
    {
        private readonly IHostingEnvironment environment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUsuarioApplicationService usuarioApplicationService;
        private readonly IPerfilApplicationService perfilApplicationService;
        private readonly IEtiquetasApplicationService etiquetasApplicationService;
        private readonly ITipoProdutoApplicationService tipoProdutoApplicationService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IUnidadeApplicationService unidadeApplicationService;
        private readonly IProdutoApplicationService produtoApplicationService;

        public PrincipalController(IHostingEnvironment environment, IHttpContextAccessor httpContextAccessor, IUsuarioApplicationService usuarioApplicationService, IPerfilApplicationService perfilApplicationService, IEtiquetasApplicationService etiquetasApplicationService, ITipoProdutoApplicationService tipoProdutoApplicationService, IUnitOfWork unitOfWork, IUnidadeApplicationService unidadeApplicationService, IProdutoApplicationService produtoApplicationService)
        {
            this.environment = environment;
            this.httpContextAccessor = httpContextAccessor;
            this.usuarioApplicationService = usuarioApplicationService;
            this.perfilApplicationService = perfilApplicationService;
            this.etiquetasApplicationService = etiquetasApplicationService;
            this.tipoProdutoApplicationService = tipoProdutoApplicationService;
            this.unitOfWork = unitOfWork;
            this.unidadeApplicationService = unidadeApplicationService;
            this.produtoApplicationService = produtoApplicationService;
        }

        public IActionResult NaoAutorizado()
        {
            return View();
        }

        public IActionResult RedefinirSenha()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RedefinirSenha(RedefinirSenhaViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var minhaContaUsuario = Newtonsoft.Json.JsonConvert.DeserializeObject
                        <ProjetoRenar.Application.ViewModels.Usuarios.MinhaContaViewModel>(User.Identity.Name);

                    model.NovaSenhaUsuario = SHA256CryptoHelper.CriptografarParaSHA256(model.NovaSenhaUsuario);
                    model.IDUsuarioInclusao = minhaContaUsuario.IdUsuario;
                    model.EmailInclusao = minhaContaUsuario.EmailUsuario;

                    usuarioApplicationService.RedefinirSenha(model);

                    model = new RedefinirSenhaViewModel();

                    ModelState.Clear();
                    TempData["MensagemSucesso"] = "Senha atualizada com sucesso. Acesse o sistema novamente para usar a nova senha.";

                    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    HttpContext.Response.Cookies.Delete("peoplenet_client");
                    HttpContext.Response.Cookies.Delete("peoplenet_settings");

                    HttpContext.Session.Remove("filtro_etiquetas");

                    return RedirectToAction("Login", "Account", new { area = "" });
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = e.Message;
                }
            }

            return View();
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

            ViewBag.ConsultarQuantidadeImpressaoPeriodo_Geral = unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_Geral(model.DataInicio.Value, model.DataFim.Value);
            ViewBag.ConsultarQuantidadeImpressaoPeriodo_Regiao = unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_Regiao(model.DataInicio.Value, model.DataFim.Value);
            ViewBag.ConsultarQuantidadeImpressaoPeriodo_Unidade = unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_Unidade(model.DataInicio.Value, model.DataFim.Value).OrderBy(u => u.NomeUnidade).ToList();
            ViewBag.ConsultarQuantidadeImpressaoPeriodo_TipoDeProduto = unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_TipoDeProduto(model.DataInicio.Value, model.DataFim.Value);
            ViewBag.ConsultarQuantidadeImpressaoPeriodo_Produto = unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_Produto(model.DataInicio.Value, model.DataFim.Value);

            ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Geral = unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Geral(model.DataInicio.Value, model.DataFim.Value);
            ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Regiao = unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Regiao(model.DataInicio.Value, model.DataFim.Value);
            ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Unidade = unitOfWork.DashboardRepository.ConsuConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Unidade(model.DataInicio.Value, model.DataFim.Value).OrderBy(u => u.NomeUnidade).ToList();
            ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_TipoDeProduto = unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_TipoDeProduto(model.DataInicio.Value, model.DataFim.Value);
            ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Produto = unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Produto(model.DataInicio.Value, model.DataFim.Value);

            return View(model);
        }

        [HttpPost]
        public IActionResult Dashboard(DashboardDatasViewModel model, string tipo)
        {
            ViewBag.Tipo = tipo;

            if(ModelState.IsValid)
            {
                try
                {
                    ViewBag.ConsultarQuantidadeImpressaoPeriodo_Geral = unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_Geral(model.DataInicio.Value, model.DataFim.Value);
                    ViewBag.ConsultarQuantidadeImpressaoPeriodo_Regiao = unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_Regiao(model.DataInicio.Value, model.DataFim.Value);
                    ViewBag.ConsultarQuantidadeImpressaoPeriodo_Unidade = unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_Unidade(model.DataInicio.Value, model.DataFim.Value);
                    ViewBag.ConsultarQuantidadeImpressaoPeriodo_TipoDeProduto = unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_TipoDeProduto(model.DataInicio.Value, model.DataFim.Value);
                    ViewBag.ConsultarQuantidadeImpressaoPeriodo_Produto = unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodo_Produto(model.DataInicio.Value, model.DataFim.Value);

                    ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Geral = unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Geral(model.DataInicio.Value, model.DataFim.Value);
                    ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Regiao = unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Regiao(model.DataInicio.Value, model.DataFim.Value);
                    ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Unidade = unitOfWork.DashboardRepository.ConsuConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Unidade(model.DataInicio.Value, model.DataFim.Value);
                    ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_TipoDeProduto = unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_TipoDeProduto(model.DataInicio.Value, model.DataFim.Value);
                    ViewBag.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Produto = unitOfWork.DashboardRepository.ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Produto(model.DataInicio.Value, model.DataFim.Value);
                }
                catch(Exception e)
                {
                    TempData["MensagemErro"] = e.Message;
                }
            }

            return View();
        }

        public IActionResult Index()
        {
            var model = new ImprimirEtiquetasViewModel();

            try
            {
                var minhaContaUsuario = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjetoRenar.Application.ViewModels.Usuarios.MinhaContaViewModel>(User.Identity.Name);
                model.ListagemTiposDeProduto = ObterTiposDeProduto();

                if (minhaContaUsuario != null && minhaContaUsuario.FlagPrimeiroAcesso != null && minhaContaUsuario.FlagPrimeiroAcesso.Value)
                    return RedirectToAction("RedefinirSenha", "Principal");

                /*
                if(HttpContext.Session.GetString("filtro_etiquetas") != null)
                {
                    model.IdTipoProduto = int.Parse(HttpContext.Session.GetString("filtro_etiquetas"));
                    model.ListagemProdutos = etiquetasApplicationService.ObterProdutos(model.IdTipoProduto.Value);

                    var idLayoutEtiqueta = etiquetasApplicationService.ObterTiposDeProduto()
                        .FirstOrDefault(e => e.IDTipoProduto == model.IdTipoProduto.Value)
                        .IDLayoutEtiqueta;

                    model.LayoutEtiqueta = etiquetasApplicationService.ObterLayoutEtiqueta(idLayoutEtiqueta.Value);
                }
                */
            }
            catch (Exception e)
            {
                return RedirectToAction("index", "principal");
            }

            model.ListagemTiposDeProduto = ObterTiposDeProduto();
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(ImprimirEtiquetasViewModel model)
        {
            if(ModelState.IsValid)
            {
                model.ListagemProdutos = produtoApplicationService.Consultar(model.NomeProduto, 1).OrderBy(p => p.NomeProduto).ToList();

                if(model.ListagemProdutos == null || model.ListagemProdutos.Count == 0)
                {
                    TempData["MensagemAlerta"] = "Nenhum produto foi encontrado para o tipo selecionado.";
                }
                else
                {
                    foreach (var item in model.ListagemProdutos)
                    {
                        var tipo = tipoProdutoApplicationService.Obter(item.IDTipoProduto.Value);
                        item.TipoProduto = tipo;
                    }

                    /*
                    var idLayoutEtiqueta = etiquetasApplicationService.ObterTiposDeProduto()
                        .FirstOrDefault(e => e.IDTipoProduto == model.IdTipoProduto.Value)
                        .IDLayoutEtiqueta;

                    model.LayoutEtiqueta = etiquetasApplicationService.ObterLayoutEtiqueta(idLayoutEtiqueta.Value);
                    */

                    //HttpContext.Session.SetString("filtro_etiquetas", model.NomeProduto);
                }
            }

            try
            {
                var minhaContaUsuario = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjetoRenar.Application.ViewModels.Usuarios.MinhaContaViewModel>(User.Identity.Name);
                model.ListagemTiposDeProduto = ObterTiposDeProduto();
            }
            catch (Exception e)
            {
                return RedirectToAction("index", "principal");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Imprimir(string[] IdsProdutos, string[] QtdImpressoes)
        {
            try
            {
                var produtos = new List<ConsultaProdutoViewModel>();
                for (int i = 0; i < IdsProdutos.Length; i++)
                {
                    var produto = etiquetasApplicationService.ObterProduto(int.Parse(IdsProdutos[i]));
                    produto.QtdImpressoes = int.Parse(QtdImpressoes[i]);

                    produtos.Add(produto);

                    try
                    {
                        var minhaContaUsuario = Newtonsoft.Json.JsonConvert.DeserializeObject<MinhaContaViewModel>(User.Identity.Name);

                        if (minhaContaUsuario.Unidades != null && minhaContaUsuario.Unidades.FirstOrDefault() != null)
                        {
                            unitOfWork.ProdutoRepository.IncluirControleImpressao
                                (minhaContaUsuario.Unidades.FirstOrDefault().IDUnidade, produto.IDProduto, produto.QtdImpressoes, minhaContaUsuario.IdUsuario);
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }

                byte[] pdfBytes = null;
                
                if(produtos.Any())
                {
                    var minhaConta = JsonConvert.DeserializeObject<MinhaContaViewModel>(User.Identity.Name);
                    var unidade = minhaConta.Unidades.FirstOrDefault(u => u.FlagUnidadePadrao);
                    if (unidade == null && minhaConta.Unidades != null && minhaConta.Unidades.Count == 1)
                    {
                        unidade = minhaConta.Unidades.FirstOrDefault();
                    }
                    if(unidade == null)
                    {
                        unidade = unidadeApplicationService.Consultar().FirstOrDefault();
                    }

                    foreach (var item in produtos)
                    {
                        item.TipoProduto = tipoProdutoApplicationService.Obter(Convert.ToInt32(item.IDTipoProduto));
                    }

                    pdfBytes = EtiquetasReport.ImprimirEtiqueta(produtos, unidade, environment);

                    /*
                    var tipoProduto = tipoProdutoApplicationService.Obter((int) produtos.FirstOrDefault().IDTipoProduto);

                    foreach (var item in produtos)
                    {
                        item.TipoProduto = tipoProduto;
                    }

                    switch (tipoProduto.IDLayoutEtiqueta)
                    {
                        case 1:
                            pdfBytes = EtiquetasReport.ImprimirEtiqueta(TipoLayoutEtiqueta.Bolo_1Coluna, produtos, unidade);
                            break;

                        case 2:
                            pdfBytes = EtiquetasReport.ImprimirEtiqueta(TipoLayoutEtiqueta.BoloPote_2Colunas, produtos, unidade);
                            break;

                        case 3:
                            pdfBytes = EtiquetasReport.ImprimirEtiqueta(TipoLayoutEtiqueta.Fatia_2Colunas, produtos, unidade);
                            break;
                    }
                    */
                }                

                return File(pdfBytes, "application/pdf", $"{Guid.NewGuid()}.pdf");
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = e.Message;                
            }

            return RedirectToAction("Index");
        }

        public IActionResult Layout(string url)
        {
            var minhaConta = JsonConvert.DeserializeObject<MinhaContaViewModel>(User.Identity.Name);
            var layout = "_LayoutRecolhido";
            Response.Cookies.Append("LayoutPadrao", layout);

            if (url.ToLower().Contains("buscapadrao"))
            {
                url = url.ToLower().Replace("buscapadrao", "consulta");
            }
            else if (url.ToLower().Contains("buscaavancada"))
            {
                url = url.ToLower().Replace("buscaavancada", "consulta");
            }

            return Redirect(url);
        }

        private List<SelectListItem> ObterTiposDeProduto()
        {
            var lista = new List<SelectListItem>();
            foreach (var item in etiquetasApplicationService.ObterTiposDeProduto().Where(t => t.FlagAtivo.Value))
            {
                lista.Add(new SelectListItem 
                { 
                    Value = item.IDTipoProduto.ToString(), 
                    Text = item.NomeTipoProduto.ToUpper() 
                });
            }

            return lista.OrderBy(p => p.Text).ToList();
        }
    }
}