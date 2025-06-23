using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DevExpress.Data.Filtering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using PeopleNetRH.Application.Contracts;
using PeopleNetRH.Application.ViewModels.AnotacoesCandidato;
using PeopleNetRH.Application.ViewModels.ArquivosCandidatoProcessoSeletivoEtapa;
using PeopleNetRH.Application.ViewModels.BeneficiosVaga;
using PeopleNetRH.Application.ViewModels.Candidatos;
using PeopleNetRH.Application.ViewModels.Dashboards;
using PeopleNetRH.Application.ViewModels.Empresas;
using PeopleNetRH.Application.ViewModels.Etapas;
using PeopleNetRH.Application.ViewModels.Funcionarios;
using PeopleNetRH.Application.ViewModels.HistoricoCandidatoProcessoSeletivo;
using PeopleNetRH.Application.ViewModels.HistoricoCandidatoProcessoSeletivoEtapa;
using PeopleNetRH.Application.ViewModels.LinhaDoTempo;
using PeopleNetRH.Application.ViewModels.LinhaDoTempoCandidato;
using PeopleNetRH.Application.ViewModels.LogIntegracaoPackUp;
using PeopleNetRH.Application.ViewModels.ProcessosSeletivos;
using PeopleNetRH.Application.ViewModels.Requisitos;
using PeopleNetRH.Application.ViewModels.Usuarios;
using PeopleNetRH.CrossCutting.Configurations;
using PeopleNetRH.Domain.Contracts.Repositories;
using PeopleNetRH.Domain.Entities;
using PeopleNetRH.Presentation.Mvc.Configurations;
using PeopleNetRH.Presentation.Mvc.Utils;
using Sidetech.Framework.Cryptography;

namespace PeopleNetRH.Presentation.Mvc.Areas.App.Controllers
{
    [Area("App")]
    [Authorize(Roles = "Administrador")]
    public class ConsultaTesteController : Controller
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICandidatoApplicationService candidatoApplicationService;
        private readonly IHabilidadeApplicationService habilidadeApplicationService;
        private readonly IEmpresaApplicationService empresaApplicationService;
        private readonly IProcessoSeletivoApplicationService processoSeletivoApplicationService;
        private readonly IFuncaoApplicationService funcaoApplicationService;
        private readonly ISetorApplicationService setorApplicationService;
        private readonly IFuncionarioApplicationService funcionarioApplicationService;
        private readonly IEtapasApplicationService etapasApplicationService;
        private readonly IUsuarioApplicationService responsaveisApplicationService;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IAnotacaoCandidatoApplicationService anotacaoCandidatoApplicationService;
        private readonly IRequisitoApplicationService requisitoApplicationService;
        private readonly ICandidatoLinhaDoTempoApplicationService candidatoLinhaDoTempoApplicationService;
        private readonly IHistoricoCandidatoProcessoSeletivoApplicationService historicoCandidatoProcessoSeletivoApplicationService;
        private readonly IHistoricoCandidatoProcessoSeletivoEtapaApplicationService historicoCandidatoProcessoSeletivoEtapaApplicationService;
        private readonly IUsuarioApplicationService usuarioApplicationService;
        private readonly IVagaPretendidaApplicationService vagaPretendidaApplicationService;
        private readonly IAvaliacaoApplicationService avaliacaoApplicationService;
        private readonly ILogIntegracaoPackUpApplicationService logIntegracaoPackUpApplicationService;
        private readonly IBeneficiosVagaApplicationService beneficiosVagaApplicationService;
        private readonly IArquivoCandidatoProcessoSeletivoEtapaApplicationService arquivoCandidatoProcessoSeletivoEtapaApplicationService;

        public ConsultaTesteController(IHttpContextAccessor httpContextAccessor, ICandidatoApplicationService candidatoApplicationService, IHabilidadeApplicationService habilidadeApplicationService, IEmpresaApplicationService empresaApplicationService, IProcessoSeletivoApplicationService processoSeletivoApplicationService, IFuncaoApplicationService funcaoApplicationService, ISetorApplicationService setorApplicationService, IFuncionarioApplicationService funcionarioApplicationService, IEtapasApplicationService etapasApplicationService, IUsuarioApplicationService responsaveisApplicationService, IHostingEnvironment hostingEnvironment, IAnotacaoCandidatoApplicationService anotacaoCandidatoApplicationService, IRequisitoApplicationService requisitoApplicationService, ICandidatoLinhaDoTempoApplicationService candidatoLinhaDoTempoApplicationService, IHistoricoCandidatoProcessoSeletivoApplicationService historicoCandidatoProcessoSeletivoApplicationService, IHistoricoCandidatoProcessoSeletivoEtapaApplicationService historicoCandidatoProcessoSeletivoEtapaApplicationService, IUsuarioApplicationService usuarioApplicationService, IVagaPretendidaApplicationService vagaPretendidaApplicationService, IAvaliacaoApplicationService avaliacaoApplicationService, ILogIntegracaoPackUpApplicationService logIntegracaoPackUpApplicationService, IBeneficiosVagaApplicationService beneficiosVagaApplicationService, IArquivoCandidatoProcessoSeletivoEtapaApplicationService arquivoCandidatoProcessoSeletivoEtapaApplicationService)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.candidatoApplicationService = candidatoApplicationService;
            this.habilidadeApplicationService = habilidadeApplicationService;
            this.empresaApplicationService = empresaApplicationService;
            this.processoSeletivoApplicationService = processoSeletivoApplicationService;
            this.funcaoApplicationService = funcaoApplicationService;
            this.setorApplicationService = setorApplicationService;
            this.funcionarioApplicationService = funcionarioApplicationService;
            this.etapasApplicationService = etapasApplicationService;
            this.responsaveisApplicationService = responsaveisApplicationService;
            this.hostingEnvironment = hostingEnvironment;
            this.anotacaoCandidatoApplicationService = anotacaoCandidatoApplicationService;
            this.requisitoApplicationService = requisitoApplicationService;
            this.candidatoLinhaDoTempoApplicationService = candidatoLinhaDoTempoApplicationService;
            this.historicoCandidatoProcessoSeletivoApplicationService = historicoCandidatoProcessoSeletivoApplicationService;
            this.historicoCandidatoProcessoSeletivoEtapaApplicationService = historicoCandidatoProcessoSeletivoEtapaApplicationService;
            this.usuarioApplicationService = usuarioApplicationService;
            this.vagaPretendidaApplicationService = vagaPretendidaApplicationService;
            this.avaliacaoApplicationService = avaliacaoApplicationService;
            this.logIntegracaoPackUpApplicationService = logIntegracaoPackUpApplicationService;
            this.beneficiosVagaApplicationService = beneficiosVagaApplicationService;
            this.arquivoCandidatoProcessoSeletivoEtapaApplicationService = arquivoCandidatoProcessoSeletivoEtapaApplicationService;
        }
        
        public IActionResult Consulta()
        {
            var filtro = new FiltroCandidatoViewModel();
            filtro.Consulta = candidatoApplicationService.ObterTodos();


            return View("Consulta", filtro);

        }
    }
}
