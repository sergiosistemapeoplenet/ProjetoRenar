using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ProjetoRenar.CrossCutting.Configurations;
using ProjetoRenar.Domain.Contracts.Repositories;
using ProjetoRenar.Domain.Services;
using Sidetech.Framework.Cryptography;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace ProjetoRenar.Infra.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private SqlConnection connection;
        private SqlTransaction transaction;

        public UnitOfWork(IHttpContextAccessor httpContextAccessor)
        {
            var cookie = httpContextAccessor.HttpContext.Request.Cookies["peoplenet_settings"];

            if(cookie != null)
            {
                var crConnection = new DefaultCrypto(DefaultCrypto.Algorithms.Rijndael);
                var data = crConnection.Decrypt(cookie);

                var configurarionSettings = JsonConvert.DeserializeObject<ConfigurationSettings>(data);

                connection = new SqlConnection();
                connection.ConnectionString = configurarionSettings.ConnectionString;
                connection.Open();
            }
            else
            {
                var crConnection = new DefaultCrypto(DefaultCrypto.Algorithms.Rijndael);
                //var urlBase = httpContextAccessor.HttpContext.Request.Host.ToString().Split('.')[0];  
                var urlBase = AppSettings.UrlBase;

                if (httpContextAccessor.HttpContext.Request.Cookies["peoplenet_client"] == null
                    || !crConnection.Encrypt(urlBase).Equals(httpContextAccessor.HttpContext.Request.Cookies["peoplenet_client"].ToString()))
                {
                    var configurationSettings = ObterConfiguracoes(urlBase);
                    var json = JsonConvert.SerializeObject(configurationSettings);
                    var cookieValue = crConnection.Encrypt(json);

                    httpContextAccessor.HttpContext.Response.Cookies.Append("peoplenet_client", crConnection.Encrypt(urlBase));
                    httpContextAccessor.HttpContext.Response.Cookies.Append("peoplenet_settings", cookieValue);

                    connection = new SqlConnection();
                    connection.ConnectionString = configurationSettings.ConnectionString;
                    connection.Open();
                }
            }
        }

        public void BeginTransaction()
        {
            transaction = connection.BeginTransaction();
        }

        public void Commit()
        {
            transaction.Commit();
        }

        public void Rollback()
        {
            transaction.Rollback();
        }

        public static ConfigurationSettings ObterConfiguracoes(string chaveCliente)
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
                        try { configurationSettings.Pass = crConnection.Decrypt(Convert.ToString(dr["vc_SenhaEmail"])); } catch (Exception) { }
                        configurationSettings.ChaveCliente = Convert.ToString(dr["vc_ChaveClientePack"]);
                    }

                    dr.Close();
                }
            }
            catch (Exception e) { Debug.WriteLine(e.Message); };

            return configurationSettings;
        }

        public void Dispose()
        {
            if (connection != null)
            {
                connection.Dispose();
            }
        }

        public IClienteRepository ClienteRepository => new ClienteRepository(connection);

        public IControleImpressaoRepository ControleImpressaoRepository => new ControleImpressaoRepository(connection);

        public ILayoutEtiquetaRepository LayoutEtiquetaRepository => new LayoutEtiquetaRepository(connection);

        public IOperacaoBloqueioRepository OperacaoBloqueioRepository => new OperacaoBloqueioRepository(connection);

        public IPerfilOperacaoBloqueioRepository PerfilOperacaoBloqueioRepository => new PerfilOperacaoBloqueioRepository(connection);

        public IPerfilRepository PerfilRepository => new PerfilRepository(connection);

        public IProdutoRepository ProdutoRepository => new ProdutoRepository(connection);

        public IRegiaoRepository RegiaoRepository => new RegiaoRepository(connection);

        public ITipoProdutoRepository TipoProdutoRepository => new TipoProdutoRepository(connection);

        public IUnidadeRepository UnidadeRepository => new UnidadeRepository(connection);

        public IUsuarioRepository UsuarioRepository => new UsuarioRepository(connection);

        public IUsuarioUnidadeRepository UsuarioUnidadeRepository => new UsuarioUnidadeRepository(connection);

        public IVersaoRepository VersaoRepository => new VersaoRepository(connection);

        public IDashboardRepository DashboardRepository => new DashboardRepository(connection);

        public IImpettusGruposPreparacoesRepository ImpettusGruposPreparacoesRepository => new ImpettusGruposPreparacoesRepository(connection);

        public IImpettusGruposProdutoRepository ImpettusGruposProdutoRepository => new ImpettusGruposProdutoRepository(connection);

        public IImpettusPreparacaoRepository ImpettusPreparacaoRepository => new ImpettusPreparacoesRepository(connection);

        public IImpettusProdutoRepository ImpettusProdutoRepository => new ImpettusProdutosRepository(connection);
    }
}
