using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoRenar.Application.Contracts;
using ProjetoRenar.Application.ViewModels.Produtos;
using ProjetoRenar.Application.ViewModels.TiposProduto;
using ProjetoRenar.Presentation.Mvc.Helpers;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace ProjetoRenar.Presentation.Mvc.Areas.App.Controllers
{
    [Area("App")]
    [Authorize(Roles = "Administrador")]
    public class ProdutosController : Controller
    {
        private readonly IProdutoApplicationService _produtoApplicationService;
        private readonly ITipoProdutoApplicationService _tipoProdutoApplicationService;
        private readonly ILayoutEtiquetaApplicationService _layoutEtiquetaApplicationService;

        public ProdutosController(IProdutoApplicationService produtoApplicationService, ITipoProdutoApplicationService tipoProdutoApplicationService, ILayoutEtiquetaApplicationService layoutEtiquetaApplicationService)
        {
            _produtoApplicationService = produtoApplicationService;
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

                var consulta = _produtoApplicationService.Consultar(nome, ativo);

                foreach (var item in consulta)
                {
                    item.TipoProduto = _tipoProdutoApplicationService.Obter((int)item.IDTipoProduto);
                }

                if (consulta.Count == 0)
                    TempData["MensagemAlerta"] = "Nenum registro foi encontrado para o filtro especificado.";

                ViewBag.Dados = consulta;
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = "Erro ao consultar produtos.";
            }

            return View();
        }

        public IActionResult Cadastro()
        {
            var usuarioAutenticado = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjetoRenar.Application.ViewModels.Usuarios.MinhaContaViewModel>(User.Identity.Name);
            if (usuarioAutenticado != null && usuarioAutenticado.FlagPrimeiroAcesso != null && usuarioAutenticado.FlagPrimeiroAcesso.Value)
                return RedirectToAction("RedefinirSenha", "Principal");

            var model = new CadastroProdutoViewModel();
            model.TiposProduto = ObterTiposProduto();
            return View(model);
        }

        [HttpPost]
        public IActionResult Cadastro(CadastroProdutoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.FlagAtivo = true;
                    _produtoApplicationService.Cadastrar(model);

                    model = new CadastroProdutoViewModel();

                    ModelState.Clear();
                    TempData["MensagemSucesso"] = "Produto cadastrado com sucesso.";
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 8152 || sqlEx.Message.Contains("String or binary data would be truncated"))
                    {
                        string mensagemErro = "Erro ao cadastrar produto. Um ou mais campos excederam o tamanho permitido.";

                        // Tenta identificar o nome do campo na mensagem (apenas para SQL Server 2019+ com Trace Flag 460 ativo)
                        if (sqlEx.Message.Contains("column"))
                        {
                            var campo = ExtrairCampoDaMensagem(sqlEx.Message);
                            if (!string.IsNullOrEmpty(campo))
                            {
                                mensagemErro += $" Campo: {campo}.";
                            }
                        }

                        TempData["MensagemErro"] = mensagemErro;
                    }
                    else
                    {
                        TempData["MensagemErro"] = "Erro ao cadastrar produto.";
                    }
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Erro ao cadastrar produto.";
                }
            }

            model.TiposProduto = ObterTiposProduto();
            return View(model);
        }

        private string ExtrairCampoDaMensagem(string mensagemErro)
        {
            // Exemplo de mensagem: "String or binary data would be truncated in table 'dbo.Produtos', column 'Nome'."
            var match = Regex.Match(mensagemErro, @"column '(\w+)'");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        public IActionResult Edicao(string id)
        {
            var usuarioAutenticado = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjetoRenar.Application.ViewModels.Usuarios.MinhaContaViewModel>(User.Identity.Name);
            if (usuarioAutenticado != null && usuarioAutenticado.FlagPrimeiroAcesso != null && usuarioAutenticado.FlagPrimeiroAcesso.Value)
                return RedirectToAction("RedefinirSenha", "Principal");

            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            var dados = _produtoApplicationService.Obter(idSelecionado);

            var model = new EdicaoProdutoViewModel
            {
                AcucarAdicionado = dados.AcucarAdicionado?.ToString(),
                AcucarAdicionadoValorDiario = dados.AcucarAdicionadoValorDiario?.ToString(),
                AcucarTotal = dados.AcucarTotal?.ToString(),
                AcucarTotalValorDiario = dados.AcucarTotalValorDiario?.ToString(),
                Carboidratos = dados.Carboidratos?.ToString(),
                CarboidratosValorDiario = dados.CarboidratosValorDiario?.ToString(),
                CodigoBarra = dados.CodigoBarra?.ToString(),
                DiasValidade = dados.DiasValidade?.ToString(),
                FibraAlimentar = dados.FibraAlimentar?.ToString(),
                FibraAlimentarValorDiario = dados.FibraAlimentarValorDiario?.ToString(),
                FlagFatiacopo = dados.FlagFatiacopo,
                GorduraSaturada = dados.GorduraSaturada?.ToString(),
                GorduraSaturadaValorDiario = dados.GorduraSaturadaValorDiario?.ToString(),
                GorduraTotal = dados.GorduraTotal?.ToString(),
                GorduraTrans = dados.GorduraTrans?.ToString(),
                GorduraTransValorDiario = dados.GorduraTransValorDiario?.ToString(),
                Gtotvd = dados.Gtotvd?.ToString(),
                IDProduto = dados.IDProduto,
                IDTipoProduto = dados.IDTipoProduto,
                Info1 = dados.Info1,
                Info2 = dados.Info2,
                NomeProduto = dados.NomeProduto,
                Peso = dados.Peso?.ToString(),
                PorcaoAcucarAdicionado = dados.PorcaoAcucarAdicionado?.ToString(),
                PorcaoAcucarTotal = dados.PorcaoAcucarTotal?.ToString(),
                PorcaoCarboidratos = dados.PorcaoCarboidratos?.ToString(),
                PorcaoEmbalagem = dados.PorcaoEmbalagem,
                PorcaoFatia = dados.PorcaoFatia,
                PorcaoFibraAlimentar = dados.PorcaoFibraAlimentar?.ToString(),
                PorcaoGorduraSaturada = dados.PorcaoGorduraSaturada?.ToString(),
                PorcaoGorduraTotal = dados.PorcaoGorduraTotal?.ToString(),
                PorcaoGorduraTrans = dados.PorcaoGorduraTrans?.ToString(),
                PorcaoProteina = dados.PorcaoProteina?.ToString(),
                PorcaoSodio = dados.PorcaoSodio?.ToString(),
                PorcaoValorEnergetico = dados.PorcaoValorEnergetico?.ToString(),
                Proteina = dados.Proteina?.ToString(),
                ProteinaValorDiario = dados.ProteinaValorDiario?.ToString(),
                QtdImpressoes = dados.QtdImpressoes,
                Receita = dados.Receita,
                Sodio = dados.Sodio?.ToString(),
                SodioValorDiario = dados.SodioValorDiario?.ToString(),
                TipoProduto = dados.TipoProduto,
                ValorEnergetico = dados.ValorEnergetico?.ToString(),
                ValorEnergeticoValorDiario = dados.ValorEnergeticoValorDiario?.ToString(),
                FlagAtivo = dados.FlagAtivo,
                TiposProduto = ObterTiposProduto(),
                Info3 = dados.Info3,
                Link = dados.Link
            };


            return View(model);
        }

        [HttpPost]
        public IActionResult Edicao(EdicaoProdutoViewModel model)
        {
            var dados = _produtoApplicationService.Obter(model.IDProduto);

            model.FlagAtivo = dados.FlagAtivo;

            if (ModelState.IsValid)
            {
                try
                {                    
                    _produtoApplicationService.Atualizar(model);

                    TempData["MensagemSucesso"] = "Produto atualizado com sucesso.";
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 8152 || sqlEx.Message.Contains("String or binary data would be truncated"))
                    {
                        string mensagemErro = "Erro ao cadastrar produto. Um ou mais campos excederam o tamanho permitido.";

                        // Tenta identificar o nome do campo na mensagem (apenas para SQL Server 2019+ com Trace Flag 460 ativo)
                        if (sqlEx.Message.Contains("column"))
                        {
                            var campo = ExtrairCampoDaMensagem(sqlEx.Message);
                            if (!string.IsNullOrEmpty(campo))
                            {
                                mensagemErro += $" Campo: {campo}.";
                            }
                        }

                        TempData["MensagemErro"] = mensagemErro;
                    }
                    else
                    {
                        TempData["MensagemErro"] = "Erro ao cadastrar produto.";
                    }
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Erro ao atualizar produto.";
                }
            }

            model.TiposProduto = ObterTiposProduto();
            return View(model);
        }

        public IActionResult Inativar(string id)
        {
            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            _produtoApplicationService.Excluir(idSelecionado);

            TempData["MensagemSucesso"] = "Tipo de produto inativado com sucesso.";

            var dados = _produtoApplicationService.Obter(idSelecionado);

            var model = new EdicaoProdutoViewModel
            {
                AcucarAdicionado = dados.AcucarAdicionado?.ToString(),
                AcucarAdicionadoValorDiario = dados.AcucarAdicionadoValorDiario?.ToString(),
                AcucarTotal = dados.AcucarTotal?.ToString(),
                AcucarTotalValorDiario = dados.AcucarTotalValorDiario?.ToString(),
                Carboidratos = dados.Carboidratos?.ToString(),
                CarboidratosValorDiario = dados.CarboidratosValorDiario?.ToString(),
                CodigoBarra = dados.CodigoBarra?.ToString(),
                DiasValidade = dados.DiasValidade?.ToString(),
                FibraAlimentar = dados.FibraAlimentar?.ToString(),
                FibraAlimentarValorDiario = dados.FibraAlimentarValorDiario?.ToString(),
                FlagFatiacopo = dados.FlagFatiacopo,
                GorduraSaturada = dados.GorduraSaturada?.ToString(),
                GorduraSaturadaValorDiario = dados.GorduraSaturadaValorDiario?.ToString(),
                GorduraTotal = dados.GorduraTotal?.ToString(),
                GorduraTrans = dados.GorduraTrans?.ToString(),
                GorduraTransValorDiario = dados.GorduraTransValorDiario?.ToString(),
                Gtotvd = dados.Gtotvd?.ToString(),
                IDProduto = dados.IDProduto,
                IDTipoProduto = dados.IDTipoProduto,
                Info1 = dados.Info1,
                Info2 = dados.Info2,
                NomeProduto = dados.NomeProduto,
                Peso = dados.Peso?.ToString(),
                PorcaoAcucarAdicionado = dados.PorcaoAcucarAdicionado?.ToString(),
                PorcaoAcucarTotal = dados.PorcaoAcucarTotal?.ToString(),
                PorcaoCarboidratos = dados.PorcaoCarboidratos?.ToString(),
                PorcaoEmbalagem = dados.PorcaoEmbalagem,
                PorcaoFatia = dados.PorcaoFatia,
                PorcaoFibraAlimentar = dados.PorcaoFibraAlimentar?.ToString(),
                PorcaoGorduraSaturada = dados.PorcaoGorduraSaturada?.ToString(),
                PorcaoGorduraTotal = dados.PorcaoGorduraTotal?.ToString(),
                PorcaoGorduraTrans = dados.PorcaoGorduraTrans?.ToString(),
                PorcaoProteina = dados.PorcaoProteina?.ToString(),
                PorcaoSodio = dados.PorcaoSodio?.ToString(),
                PorcaoValorEnergetico = dados.PorcaoValorEnergetico?.ToString(),
                Proteina = dados.Proteina?.ToString(),
                ProteinaValorDiario = dados.ProteinaValorDiario?.ToString(),
                QtdImpressoes = dados.QtdImpressoes,
                Receita = dados.Receita,
                Sodio = dados.Sodio?.ToString(),
                SodioValorDiario = dados.SodioValorDiario?.ToString(),
                TipoProduto = dados.TipoProduto,
                ValorEnergetico = dados.ValorEnergetico?.ToString(),
                ValorEnergeticoValorDiario = dados.ValorEnergeticoValorDiario?.ToString(),
                FlagAtivo = dados.FlagAtivo,
                TiposProduto = ObterTiposProduto(),
                Info3 = dados.Info3,
                Link = dados.Link
            };


            return View("Edicao", model);
        }

        public IActionResult Reativar(string id)
        {
            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            _produtoApplicationService.Reativar(idSelecionado);

            TempData["MensagemSucesso"] = "Produto reativado com sucesso.";

            var dados = _produtoApplicationService.Obter(idSelecionado);

            var model = new EdicaoProdutoViewModel
            {
                AcucarAdicionado = dados.AcucarAdicionado?.ToString(),
                AcucarAdicionadoValorDiario = dados.AcucarAdicionadoValorDiario?.ToString(),
                AcucarTotal = dados.AcucarTotal?.ToString(),
                AcucarTotalValorDiario = dados.AcucarTotalValorDiario?.ToString(),
                Carboidratos = dados.Carboidratos?.ToString(),
                CarboidratosValorDiario = dados.CarboidratosValorDiario?.ToString(),
                CodigoBarra = dados.CodigoBarra?.ToString(),
                DiasValidade = dados.DiasValidade?.ToString(),
                FibraAlimentar = dados.FibraAlimentar?.ToString(),
                FibraAlimentarValorDiario = dados.FibraAlimentarValorDiario?.ToString(),
                FlagFatiacopo = dados.FlagFatiacopo,
                GorduraSaturada = dados.GorduraSaturada?.ToString(),
                GorduraSaturadaValorDiario = dados.GorduraSaturadaValorDiario?.ToString(),
                GorduraTotal = dados.GorduraTotal?.ToString(),
                GorduraTrans = dados.GorduraTrans?.ToString(),
                GorduraTransValorDiario = dados.GorduraTransValorDiario?.ToString(),
                Gtotvd = dados.Gtotvd?.ToString(),
                IDProduto = dados.IDProduto,
                IDTipoProduto = dados.IDTipoProduto,
                Info1 = dados.Info1,
                Info2 = dados.Info2,
                NomeProduto = dados.NomeProduto,
                Peso = dados.Peso?.ToString(),
                PorcaoAcucarAdicionado = dados.PorcaoAcucarAdicionado?.ToString(),
                PorcaoAcucarTotal = dados.PorcaoAcucarTotal?.ToString(),
                PorcaoCarboidratos = dados.PorcaoCarboidratos?.ToString(),
                PorcaoEmbalagem = dados.PorcaoEmbalagem,
                PorcaoFatia = dados.PorcaoFatia,
                PorcaoFibraAlimentar = dados.PorcaoFibraAlimentar?.ToString(),
                PorcaoGorduraSaturada = dados.PorcaoGorduraSaturada?.ToString(),
                PorcaoGorduraTotal = dados.PorcaoGorduraTotal?.ToString(),
                PorcaoGorduraTrans = dados.PorcaoGorduraTrans?.ToString(),
                PorcaoProteina = dados.PorcaoProteina?.ToString(),
                PorcaoSodio = dados.PorcaoSodio?.ToString(),
                PorcaoValorEnergetico = dados.PorcaoValorEnergetico?.ToString(),
                Proteina = dados.Proteina?.ToString(),
                ProteinaValorDiario = dados.ProteinaValorDiario?.ToString(),
                QtdImpressoes = dados.QtdImpressoes,
                Receita = dados.Receita,
                Sodio = dados.Sodio?.ToString(),
                SodioValorDiario = dados.SodioValorDiario?.ToString(),
                TipoProduto = dados.TipoProduto,
                ValorEnergetico = dados.ValorEnergetico?.ToString(),
                ValorEnergeticoValorDiario = dados.ValorEnergeticoValorDiario?.ToString(),
                FlagAtivo = dados.FlagAtivo,
                TiposProduto = ObterTiposProduto(),
                Info3 = dados.Info3,
                Link = dados.Link
            };


            return View("Edicao", model);
        }

        public IActionResult ObterQrCode(string link)
        {
            try
            {
                QRCoder.QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCoder.QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                return new FileContentResult(BitmapToBytes(qrCodeImage), "image/jpeg");
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }

            return View();
        }

        private static byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        private List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ObterTiposProduto()
        {
            var lista = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            foreach (var item in _tipoProdutoApplicationService.Consultar(string.Empty, 1))
            {
                lista.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                { Value = item.IDTipoProduto.ToString(), Text = item.NomeTipoProduto.ToUpper() });
            }

            return lista;
        }
    }
}
