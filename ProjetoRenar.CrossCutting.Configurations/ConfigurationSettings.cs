using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.CrossCutting.Configurations
{
    public class ConfigurationSettings
    {
        public string ConnectionString { get; set; }
        public string Smtp { get; set; }
        public string Port { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public string ChaveCliente { get; set; }
        public string Logotipo { get; set; }
        public string SenhaExpressaoRegular { get; set; }
        public string SenhaMensagemOrientacao { get; set; }
    }
}
