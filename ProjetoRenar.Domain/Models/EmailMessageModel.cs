using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Models
{
    public class EmailMessageModel
    {
        public virtual string To { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Body { get; set; }
        public virtual bool IsBodyHtml { get; set; }
    }
}
