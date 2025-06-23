using ProjetoRenar.Application.ViewModels.Etiquetas;
using ProjetoRenar.Application.ViewModels.LayoutEtiquetas;
using ProjetoRenar.Application.ViewModels.Produtos;
using ProjetoRenar.Application.ViewModels.TiposProduto;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Contracts
{
    public interface ILayoutEtiquetaApplicationService
    {
        void Cadastrar(CadastroLayoutEtiquetaViewModel model);
        void Atualizar(EdicaoLayoutEtiquetaViewModel model);

        List<ConsultaLayoutEtiquetaViewModel> ConsultarLayoutEtiquetas(string nomeLayoutEtiqueta, int flagAtivo);
        ConsultaLayoutEtiquetaViewModel ObterLayoutEtiquetas(int id);
    }
}
