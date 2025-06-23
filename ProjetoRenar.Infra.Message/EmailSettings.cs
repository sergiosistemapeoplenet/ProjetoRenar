using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Infra.Message
{
    public class EmailSettings
    {
        public virtual string Smtp { get; set; }
        public virtual string Port { get; set; }
        public virtual string Mail { get; set; }
        public virtual string User { get; set; }
        public virtual string Name { get; set; }
        public virtual string Pass { get; set; }
    }
}
