using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProjetoRenar.Application.Contracts;
using ProjetoRenar.Application.Services;
using ProjetoRenar.CrossCutting.Configurations;
using ProjetoRenar.Domain.Contracts.Cryptographies;
using ProjetoRenar.Domain.Contracts.Messages;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Contracts.Services;
using ProjetoRenar.Domain.Entities;
using ProjetoRenar.Domain.Services;
using ProjetoRenar.Infra.Cryptography;
using ProjetoRenar.Infra.Message;
using ProjetoRenar.Infra.Repository;
using ProjetoRenar.Presentation.Mvc.Configurations;
using ProjetoRenar.Presentation.Mvc.Helpers;
using Sidetech.Framework.Cryptography;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoRenar.Presentation.Mvc
{
    public static class DependencyInjection
    {
        public static void Register(IServiceCollection services)
        {
            services.AddTransient<IRegiaoApplicationService, RegiaoApplicationService>();
            services.AddTransient<IUsuarioApplicationService, UsuarioApplicationService>();
            services.AddTransient<ILayoutEtiquetaApplicationService, LayoutEtiquetaApplicationService>();
            services.AddTransient<IProdutoApplicationService, ProdutoApplicationService>();
            services.AddTransient<ITipoProdutoApplicationService, TipoProdutoApplicationService>();
            services.AddTransient<IPerfilApplicationService, PerfilApplicationService>();
            services.AddTransient<IEtiquetasApplicationService, EtiquetasApplicationService>();
            services.AddTransient<IUnidadeApplicationService, UnidadeApplicationService>();
            services.AddTransient<IUsuarioDomainService, UsuarioDomainService>();
            services.AddTransient<IPerfilDomainService, PerfilDomainService>();
            services.AddTransient<ITipoProdutoDomainService, TipoProdutoDomainService>();
            services.AddTransient<IProdutoDomainService, ProdutoDomainService>();
            services.AddTransient<IUnidadeDomainService, UnidadeDomainService>();
            services.AddTransient<ILayoutEtiquetaDomainService, LayoutEtiquetaDomainService>();
            services.AddTransient<IUsuarioUnidadeDomainService, UsuarioUnidadeDomainService>();
            services.AddTransient<IRegiaoDomainService, RegiaoDomainService>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IUsuarioRepository, UsuarioRepository>();
            services.AddTransient<IPerfilRepository, PerfilRepository>();
            services.AddTransient<ITipoProdutoRepository, TipoProdutoRepository>();
            services.AddTransient<IProdutoRepository, ProdutoRepository>();
            services.AddTransient<IUnidadeRepository, UnidadeRepository>();
            services.AddTransient<IUsuarioUnidadeRepository, UsuarioUnidadeRepository>();
            services.AddTransient<IRegiaoRepository, RegiaoRepository>();
            services.AddTransient<IMD5Cryptography, MD5Cryptography>();
            services.AddTransient<IEmailMessage, EmailMessage>();
            services.AddTransient<IDashboardRepository, DashboardRepository>();
        }
    }

    public static class ConfiguracaoGlobalCliente
    {
        public static ConfigurationSettings Get(string chaveCliente)
        {
            var configurationSettings = new ConfigurationSettings();
            var crConnection = new DefaultCrypto(DefaultCrypto.Algorithms.Rijndael);

            try
            {
                using (var connection = new SqlConnection(AppSettings.ConnectionString))
                {
                    connection.Open();

                    var cmd = new SqlCommand("select * from View_ClienteSistemas where vc_ChaveCliente = @vc_ChaveCliente", connection);
                    cmd.Parameters.AddWithValue("@vc_ChaveCliente", chaveCliente);
                    var dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        configurationSettings.ConnectionString = crConnection.Decrypt(Convert.ToString(dr["vc_ConnectionString"]));
                        configurationSettings.Smtp = Convert.ToString(dr["vc_EnderecoSmtp"]);
                        configurationSettings.Port = Convert.ToString(dr["in_PortaSmtp"]);
                        configurationSettings.Mail = Convert.ToString(dr["vc_ContaEmail"]);
                        configurationSettings.Name = Convert.ToString(dr["vc_NomeEmail"]);
                        configurationSettings.User = Convert.ToString(dr["vc_UsuarioSMTP"]);
                        configurationSettings.SenhaExpressaoRegular = Convert.ToString(dr["SenhaExpressaoRegular"]);
                        configurationSettings.SenhaMensagemOrientacao = Convert.ToString(dr["SenhaMensagemOrientacao"]);
                        configurationSettings.Pass = crConnection.Decrypt(Convert.ToString(dr["vc_SenhaEmail"]));
                        configurationSettings.ChaveCliente = Convert.ToString(dr["vc_ChaveClientePack"]);
                    }

                    dr.Close();
                }
            }
            catch (Exception e) { Debug.WriteLine(e.Message); };

            return configurationSettings;
        }
    }
}
