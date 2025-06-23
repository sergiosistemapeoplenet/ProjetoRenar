using Dapper;
using ProjetoRenar.Domain.Contracts.Dtos;
using ProjetoRenar.Domain.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ProjetoRenar.Infra.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly SqlConnection _connection;

        public DashboardRepository(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public List<ConsultarQuantidadeImpressaoPeriodo_Geral> ConsultarQuantidadeImpressaoPeriodo_Geral(DateTime dataIni, DateTime dataFim)
        {
            return _connection.Query<ConsultarQuantidadeImpressaoPeriodo_Geral>
                ("Renar.SP_ConsultarQuantidadeImpressaoPeriodo", new
                {
                    DataInicio = dataIni,
                    DataFim = dataFim,
                    TipoConsulta = 0
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeImpressaoPeriodo_Produto> ConsultarQuantidadeImpressaoPeriodo_Produto(DateTime dataIni, DateTime dataFim)
        {
            return _connection.Query<ConsultarQuantidadeImpressaoPeriodo_Produto>
                ("Renar.SP_ConsultarQuantidadeImpressaoPeriodo", new
                {
                    DataInicio = dataIni,
                    DataFim = dataFim,
                    TipoConsulta = 4
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeImpressaoPeriodo_Regiao> ConsultarQuantidadeImpressaoPeriodo_Regiao(DateTime dataIni, DateTime dataFim)
        {
            return _connection.Query<ConsultarQuantidadeImpressaoPeriodo_Regiao>
                ("Renar.SP_ConsultarQuantidadeImpressaoPeriodo", new
                {
                    DataInicio = dataIni,
                    DataFim = dataFim,
                    TipoConsulta = 1
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeImpressaoPeriodo_TipoDeProduto> ConsultarQuantidadeImpressaoPeriodo_TipoDeProduto(DateTime dataIni, DateTime dataFim)
        {
            return _connection.Query<ConsultarQuantidadeImpressaoPeriodo_TipoDeProduto>
                ("Renar.SP_ConsultarQuantidadeImpressaoPeriodo", new
                {
                    DataInicio = dataIni,
                    DataFim = dataFim,
                    TipoConsulta = 3
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeImpressaoPeriodo_Unidade> ConsultarQuantidadeImpressaoPeriodo_Unidade(DateTime dataIni, DateTime dataFim)
        {
            return _connection.Query<ConsultarQuantidadeImpressaoPeriodo_Unidade>
                ("Renar.SP_ConsultarQuantidadeImpressaoPeriodo", new
                {
                    DataInicio = dataIni,
                    DataFim = dataFim,
                    TipoConsulta = 2
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_geral> ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Geral(DateTime dataIni, DateTime dataFim)
        {
            return _connection.Query<ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_geral>
                ("Renar.SP_ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes", new
                {
                    DataInicio = dataIni,
                    DataFim = dataFim,
                    TipoConsulta = 0
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Produto> ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Produto(DateTime dataIni, DateTime dataFim)
        {
            return _connection.Query<ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Produto>
                ("Renar.SP_ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes", new
                {
                    DataInicio = dataIni,
                    DataFim = dataFim,
                    TipoConsulta = 4
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Regiao> ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Regiao(DateTime dataIni, DateTime dataFim)
        {
            return _connection.Query<ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Regiao>
                ("Renar.SP_ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes", new
                {
                    DataInicio = dataIni,
                    DataFim = dataFim,
                    TipoConsulta = 1
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_TipoDeProduto> ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_TipoDeProduto(DateTime dataIni, DateTime dataFim)
        {
            return _connection.Query<ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_TipoDeProduto>
                ("Renar.SP_ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes", new
                {
                    DataInicio = dataIni,
                    DataFim = dataFim,
                    TipoConsulta = 3
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Unidade> ConsuConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Unidade(DateTime dataIni, DateTime dataFim)
        {
            return _connection.Query<ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Unidade>
                ("Renar.SP_ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes", new
                {
                    DataInicio = dataIni,
                    DataFim = dataFim,
                    TipoConsulta = 2
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeImpressaoUltimosDias_Geral> ConsultarQuantidadeImpressaoUltimosDias_Geral(int dias)
        {
            return _connection.Query<ConsultarQuantidadeImpressaoUltimosDias_Geral>
                ("Renar.SP_ConsultarQuantidadeImpressaoUltimosDias", new
                {
                    QuantidadeDias = dias,
                    TipoConsulta = 2
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeImpressaoUltimosDias_Produto> ConsultarQuantidadeImpressaoUltimosDias_Produto(int dias)
        {
            return _connection.Query<ConsultarQuantidadeImpressaoUltimosDias_Produto>
                ("Renar.SP_ConsultarQuantidadeImpressaoUltimosDias", new
                {
                    QuantidadeDias = dias,
                    TipoConsulta = 4
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeImpressaoUltimosDias_Regiao> ConsultarQuantidadeImpressaoUltimosDias_Regiao(int dias)
        {
            return _connection.Query<ConsultarQuantidadeImpressaoUltimosDias_Regiao>
                ("Renar.SP_ConsultarQuantidadeImpressaoUltimosDias", new
                {
                    QuantidadeDias = dias,
                    TipoConsulta = 1
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeImpressaoUltimosDias_TipoDeProduto> ConsultarQuantidadeImpressaoUltimosDias_TipoDeProduto(int dias)
        {
            return _connection.Query<ConsultarQuantidadeImpressaoUltimosDias_TipoDeProduto>
                ("Renar.SP_ConsultarQuantidadeImpressaoUltimosDias", new
                {
                    QuantidadeDias = dias,
                    TipoConsulta = 3
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeImpressaoUltimosDias_Unidade> ConsultarQuantidadeImpressaoUltimosDias_Unidade(int dias)
        {
            return _connection.Query<ConsultarQuantidadeImpressaoUltimosDias_Unidade>
                ("Renar.SP_ConsultarQuantidadeImpressaoUltimosDias", new
                {
                    QuantidadeDias = dias,
                    TipoConsulta = 2
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeMediaImpressaoUltimosMeses_Geral> ConsultarQuantidadeMediaImpressaoUltimosMeses_Geral(int meses)
        {
            return _connection.Query<ConsultarQuantidadeMediaImpressaoUltimosMeses_Geral>
                ("Renar.SP_ConsultarQuantidadeMediaImpressaoUltimosMeses", new
                {
                    QuantidadeMeses = meses,
                    TipoConsulta = 0
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeMediaImpressaoUltimosMeses_Produto> ConsultarQuantidadeMediaImpressaoUltimosMeses_Produto(int meses)
        {
            return _connection.Query<ConsultarQuantidadeMediaImpressaoUltimosMeses_Produto>
                ("Renar.SP_ConsultarQuantidadeMediaImpressaoUltimosMeses", new
                {
                    QuantidadeMeses = meses,
                    TipoConsulta = 4
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeMediaImpressaoUltimosMeses_Regiao> ConsultarQuantidadeMediaImpressaoUltimosMeses_Regiao(int meses)
        {
            return _connection.Query<ConsultarQuantidadeMediaImpressaoUltimosMeses_Regiao>
                ("Renar.SP_ConsultarQuantidadeMediaImpressaoUltimosMeses", new
                {
                    QuantidadeMeses = meses,
                    TipoConsulta = 1
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeMediaImpressaoUltimosMeses_TipoDeProduto> ConsultarQuantidadeMediaImpressaoUltimosMeses_TipoDeProduto(int meses)
        {
            return _connection.Query<ConsultarQuantidadeMediaImpressaoUltimosMeses_TipoDeProduto>
                ("Renar.SP_ConsultarQuantidadeMediaImpressaoUltimosMeses", new
                {
                    QuantidadeMeses = meses,
                    TipoConsulta = 3
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }

        public List<ConsultarQuantidadeMediaImpressaoUltimosMeses_Unidade> ConsultarQuantidadeMediaImpressaoUltimosMeses_Unidade(int meses)
        {
            return _connection.Query<ConsultarQuantidadeMediaImpressaoUltimosMeses_Unidade>
                ("Renar.SP_ConsultarQuantidadeMediaImpressaoUltimosMeses", new
                {
                    QuantidadeMeses = meses,
                    TipoConsulta = 2
                }, commandType: System.Data.CommandType.StoredProcedure).ToList();
        }
    }
}
