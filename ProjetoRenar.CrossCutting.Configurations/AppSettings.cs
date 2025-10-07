using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.CrossCutting.Configurations
{
    public static class AppSettings
    {
        public static string UrlBase => "voalzira";
        //public static string UrlBase => "impettus";
        public static string ConnectionString => "data source=10.34.12.20;initial catalog=ADMINRENAR;user id=user_adminrenar;password=wq5!SUJ#L2Zb;";
        //public static string ConnectionString => "Data Source=rccpnet_admincliente.sqlserver.dbaas.com.br;Initial Catalog=rccpnet_admincliente;User ID=rccpnet_admincliente;Password=Admin@#2025;Encrypt=False";
        //public static string ConnectionString => "data source=xxxdnn2607.publiccloud.com.br;initial catalog=adminrenarhomolog;user id=user_adminrenarhomolog;password=Sidetech@10;";
    }
}

