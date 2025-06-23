using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoRenar.Application.Contracts;
using ProjetoRenar.Application.ViewModels.Unidades;
using ProjetoRenar.Presentation.Mvc.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoRenar.Presentation.Mvc.Areas.App.Controllers
{
    [Area("App")]
    [Authorize(Roles = "Administrador")]
    public class UnidadesController : Controller
    {
        private readonly IUnidadeApplicationService _unidadeApplicationService;

        public UnidadesController(IUnidadeApplicationService unidadeApplicationService)
        {
            _unidadeApplicationService = unidadeApplicationService;
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

                var consulta = _unidadeApplicationService.Consultar()
                    .Where(u => u.FlagAtivo == (ativo == 1) && u.NomeUnidade != null && u.NomeUnidade.ToLower().Contains(nome != null ? nome.ToLower() : string.Empty))
                    .OrderBy(u => u.NomeUnidade).ToList();

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
            var usuarioAutenticado = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjetoRenar.Application.ViewModels.Usuarios.MinhaContaViewModel>(User.Identity.Name);
            if (usuarioAutenticado != null && usuarioAutenticado.FlagPrimeiroAcesso != null && usuarioAutenticado.FlagPrimeiroAcesso.Value)
                return RedirectToAction("RedefinirSenha", "Principal");

            CadastroUnidadeViewModel model = new CadastroUnidadeViewModel();
            model.Regioes = ObterRegioes();

            return View(model);
        }

        [HttpPost]
        public IActionResult Cadastro(CadastroUnidadeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.FlagAtivo = true;
                    _unidadeApplicationService.Cadastrar(model);

                    model = new CadastroUnidadeViewModel();

                    ModelState.Clear();
                    TempData["MensagemSucesso"] = "Unidade cadastrada com sucesso.";
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Erro ao cadastrar unidade.";
                }
            }

            model.Regioes = ObterRegioes();
            return View(model);
        }

        public IActionResult Edicao(string id)
        {
            var usuarioAutenticado = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjetoRenar.Application.ViewModels.Usuarios.MinhaContaViewModel>(User.Identity.Name);
            if (usuarioAutenticado != null && usuarioAutenticado.FlagPrimeiroAcesso != null && usuarioAutenticado.FlagPrimeiroAcesso.Value)
                return RedirectToAction("RedefinirSenha", "Principal");

            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            var dados = _unidadeApplicationService.ObterPorId(idSelecionado);

            var model = new EdicaoUnidadeViewModel
            {
                Cep = dados.Cep,       
                RazaoSocial = dados.RazaoSocial,
                CNPJ = dados.CNPJ,
                EmailContato = dados.EmailContato,
                Endereco = dados.Endereco,
                FlagAtivo = dados.FlagAtivo,
                FlagImprimeCodigoBarra = dados.FlagImprimeCodigoBarra,
                HorarioFuncionamento = dados.HorarioFuncionamento,
                IDRegiao = dados.IDRegiao,
                IdUnidade = dados.IDUnidade,
                NomeContato = dados.NomeContato,
                NomeUnidade = dados.NomeUnidade,
                NumeroContato = dados.NumeroContato,
                Regioes = ObterRegioes(),
                SerialImpressora = dados.SerialImpressora
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edicao(EdicaoUnidadeViewModel model)
        {
            var dados = _unidadeApplicationService.ObterPorId(model.IdUnidade);
            model.FlagAtivo = dados.FlagAtivo;

            if (ModelState.IsValid)
            {
                try
                {                    
                    _unidadeApplicationService.Atualizar(model);

                    TempData["MensagemSucesso"] = "Unidade atualizada com sucesso.";
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = "Erro ao atualizar unidade.";
                }
            }

            model.Regioes = ObterRegioes();
            return View(model);
        }

        public IActionResult Inativar(string id)
        {
            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            _unidadeApplicationService.Excluir(idSelecionado);

            TempData["MensagemSucesso"] = "Unidade inativada com sucesso.";

            var dados = _unidadeApplicationService.ObterPorId(idSelecionado);

            var model = new EdicaoUnidadeViewModel
            {
                Cep = dados.Cep,
                CNPJ = dados.CNPJ,
                EmailContato = dados.EmailContato,
                Endereco = dados.Endereco,
                FlagAtivo = dados.FlagAtivo,
                FlagImprimeCodigoBarra = dados.FlagImprimeCodigoBarra,
                HorarioFuncionamento = dados.HorarioFuncionamento,
                IDRegiao = dados.IDRegiao,
                IdUnidade = dados.IDUnidade,
                NomeContato = dados.NomeContato,
                NomeUnidade = dados.NomeUnidade,
                NumeroContato = dados.NumeroContato,
                Regioes = ObterRegioes(),
                SerialImpressora = dados.SerialImpressora
            };

            return View("Edicao", model);
        }

        public IActionResult Reativar(string id)
        {
            var idSelecionado = int.Parse(EncryptionHelper.Decrypt(id));
            _unidadeApplicationService.Reativar(idSelecionado);

            TempData["MensagemSucesso"] = "Unidade reativada com sucesso.";

            var dados = _unidadeApplicationService.ObterPorId(idSelecionado);

            var model = new EdicaoUnidadeViewModel
            {
                Cep = dados.Cep,
                CNPJ = dados.CNPJ,
                EmailContato = dados.EmailContato,
                Endereco = dados.Endereco,
                FlagAtivo = dados.FlagAtivo,
                FlagImprimeCodigoBarra = dados.FlagImprimeCodigoBarra,
                HorarioFuncionamento = dados.HorarioFuncionamento,
                IDRegiao = dados.IDRegiao,
                IdUnidade = dados.IDUnidade,
                NomeContato = dados.NomeContato,
                NomeUnidade = dados.NomeUnidade,
                NumeroContato = dados.NumeroContato,
                Regioes = ObterRegioes(),
                SerialImpressora = dados.SerialImpressora
            };

            return View("Edicao", model);
        }

        private List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ObterRegioes()
        {
            var lista = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            foreach (var item in _unidadeApplicationService.ConsultarRegioes())
            {
                lista.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                { Value = item.IDRegiao.ToString(), Text = item.NomeRegiao.ToUpper() });
            }

            return lista.OrderBy(s => s.Text).ToList();
        }
    }
}
