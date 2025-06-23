using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction();
        void Commit();
        void Rollback();

        IClienteRepository ClienteRepository { get; }       
        IControleImpressaoRepository ControleImpressaoRepository { get; }
        ILayoutEtiquetaRepository LayoutEtiquetaRepository { get; }
        IOperacaoBloqueioRepository OperacaoBloqueioRepository { get; }
        IPerfilOperacaoBloqueioRepository PerfilOperacaoBloqueioRepository { get; }
        IPerfilRepository PerfilRepository { get; }
        IProdutoRepository ProdutoRepository { get; }
        IRegiaoRepository RegiaoRepository { get; }
        ITipoProdutoRepository TipoProdutoRepository { get; }
        IUnidadeRepository UnidadeRepository { get; }
        IUsuarioRepository UsuarioRepository { get; }
        IUsuarioUnidadeRepository UsuarioUnidadeRepository { get; }
        IVersaoRepository VersaoRepository { get; }
        IDashboardRepository DashboardRepository { get; }

        IImpettusGruposPreparacoesRepository ImpettusGruposPreparacoesRepository { get; }
        IImpettusGruposProdutoRepository ImpettusGruposProdutoRepository { get; }
        IImpettusPreparacaoRepository ImpettusPreparacaoRepository { get; }
        IImpettusProdutoRepository ImpettusProdutoRepository { get; }
    }
}
