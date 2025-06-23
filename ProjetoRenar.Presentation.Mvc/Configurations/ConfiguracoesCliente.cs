using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoRenar.Presentation.Mvc.Configurations
{
    public class ConfiguracoesCliente
    {
        public string ConnectionString { get; set; }
        public string Smtp { get; set; }
        public string Port { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }
        public string Pass { get; set; }
        public string ChaveCliente { get; set; }
        public string Logotipo { get; set; }
    }
}
