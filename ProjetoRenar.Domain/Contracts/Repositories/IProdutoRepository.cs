using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Repositories
{
    public interface IProdutoRepository
    {
        List<Produto> GetAll();
        List<Produto> GetAll(string NomeProduto, int FlagAtivo);
        List<Produto> GetByTipo(int idTipo);
        Produto GetById(int id);
        Produto GetByCodigoBarra(string codigoBarra);
        void Insert(Produto produto);
        void Update(Produto produto);
        void Delete(int id);
        void UnDelete(int id);
        void IncluirControleImpressao(int idUnidade, int idProduto, int quantidadeEtiqueta, int idUsuario);
    }
}