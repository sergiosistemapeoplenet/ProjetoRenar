using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Entities
{
    public class Cliente
    {
        public string NomeCliente { get; set; }
        public string Cnpj { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string Cep { get; set; }
        public string Telefone { get; set; }
        public byte NumeroUsuarios { get; set; }
    }
}
