using ProjetoRenar.Application.ViewModels.Regioes;
using ProjetoRenar.Application.ViewModels.Unidades;
using ProjetoRenar.Application.ViewModels.Usuarios;
using ProjetoRenar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Application.Contracts
{
    public interface IUnidadeApplicationService
    {
        void Cadastrar(CadastroUnidadeViewModel model);
        void Atualizar(EdicaoUnidadeViewModel model);
        void Excluir(int id);
        void Reativar(int id);
        List<ConsultaUnidadeViewModel> Obter(int idUsuario);
        List<ConsultaUnidadeViewModel> Consultar();
        List<ConsultaRegiaoViewModel> ConsultarRegioes();
        ConsultaUnidadeViewModel ObterPorId(int id);
    }
}
