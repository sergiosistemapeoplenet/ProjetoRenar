using ProjetoRenar.Domain.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface IImpettusDashboardRepository
    {
        List<ConsultarQuantidadeImpressaoPeriodo_Geral> ConsultarQuantidadeImpressaoPeriodo_Geral(DateTime dataIni, DateTime dataFim);
        List<ConsultarQuantidadeImpressaoPeriodo_Produto> ConsultarQuantidadeImpressaoPeriodo_Produto(DateTime dataIni, DateTime dataFim);
        List<ConsultarQuantidadeImpressaoPeriodo_Regiao> ConsultarQuantidadeImpressaoPeriodo_Regiao(DateTime dataIni, DateTime dataFim);
        List<ConsultarQuantidadeImpressaoPeriodo_TipoDeProduto> ConsultarQuantidadeImpressaoPeriodo_TipoDeProduto(DateTime dataIni, DateTime dataFim);
        List<ConsultarQuantidadeImpressaoPeriodo_Unidade> ConsultarQuantidadeImpressaoPeriodo_Unidade(DateTime dataIni, DateTime dataFim);

        List<ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_geral> ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Geral(DateTime dataIni, DateTime dataFim);
        List<ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Produto> ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Produto(DateTime dataIni, DateTime dataFim);
        List<ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Regiao> ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Regiao(DateTime dataIni, DateTime dataFim);
        List<ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_TipoDeProduto> ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_TipoDeProduto(DateTime dataIni, DateTime dataFim);
        List<ConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Unidade> ConsuConsultarQuantidadeImpressaoPeriodoAgrupadoPorMes_Unidade(DateTime dataIni, DateTime dataFim);

        List<ConsultarQuantidadeImpressaoUltimosDias_Geral> ConsultarQuantidadeImpressaoUltimosDias_Geral(int dias);
        List<ConsultarQuantidadeImpressaoUltimosDias_Produto> ConsultarQuantidadeImpressaoUltimosDias_Produto(int dias);
        List<ConsultarQuantidadeImpressaoUltimosDias_Regiao> ConsultarQuantidadeImpressaoUltimosDias_Regiao(int dias);
        List<ConsultarQuantidadeImpressaoUltimosDias_TipoDeProduto> ConsultarQuantidadeImpressaoUltimosDias_TipoDeProduto(int dias);
        List<ConsultarQuantidadeImpressaoUltimosDias_Unidade> ConsultarQuantidadeImpressaoUltimosDias_Unidade(int dias);

        List<ConsultarQuantidadeMediaImpressaoUltimosMeses_Geral> ConsultarQuantidadeMediaImpressaoUltimosMeses_Geral(int meses);
        List<ConsultarQuantidadeMediaImpressaoUltimosMeses_Produto> ConsultarQuantidadeMediaImpressaoUltimosMeses_Produto(int meses);
        List<ConsultarQuantidadeMediaImpressaoUltimosMeses_Regiao> ConsultarQuantidadeMediaImpressaoUltimosMeses_Regiao(int meses);
        List<ConsultarQuantidadeMediaImpressaoUltimosMeses_TipoDeProduto> ConsultarQuantidadeMediaImpressaoUltimosMeses_TipoDeProduto(int meses);
        List<ConsultarQuantidadeMediaImpressaoUltimosMeses_Unidade> ConsultarQuantidadeMediaImpressaoUltimosMeses_Unidade(int meses);
    }
}
