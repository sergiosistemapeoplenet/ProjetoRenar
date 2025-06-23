using ProjetoRenar.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoRenar.Domain.Contracts.Messages
{
    public interface IEmailMessage
    {
        void Send(EmailMessageModel model);
        void SendWithAttachment(EmailMessageModel model, byte[] pdfBytes, string attachmentFileName);
    }
}
