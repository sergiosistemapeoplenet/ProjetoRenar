using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.CrossCutting.Configurations
{
    public static class AppSettings
    {
        public static string UrlBase => "grupoimpettus";
        public static string ConnectionString => "Data Source=xxxdnn2607.publiccloud.com.br;Initial Catalog=adminrenarhomolog;User ID=user_adminrenarhomolog;Password=Sidetech@10;Encrypt=False";

        //public static string UrlBase => "voalzirahomolog";
        //public static string ConnectionString => "data source=xxxdnn2607.publiccloud.com.br;initial catalog=adminrenarhomolog;user id=user_adminrenarhomolog;password=Sidetech@10;";
    }
}
