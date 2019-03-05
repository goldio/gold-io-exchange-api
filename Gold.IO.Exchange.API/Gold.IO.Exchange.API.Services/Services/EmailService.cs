using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Utils.Helpers;
using Gold.IO.Exchange.API.Utils.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.BusinessLogic.Services
{
    public class EmailService : IEmailService
    {
        public async Task<bool> SendActivationMessage(string receiver, string activationKey)
        {
            var message = new SendMailModel
            {
                MailTo = receiver,
                Subject = "[GOLD.IO Exchange] Activation",
                Message = $"http://188.42.174.122/#/activation/?key={activationKey}"
            };

            return await SMTPHelper.SendMail(message);
        }

        public async Task<bool> SendRecoveryMessage(string receiver, string recoveryKey)
        {
            var message = new SendMailModel
            {
                MailTo = receiver,
                Subject = "[GOLD.IO Exchange] Recovery",
                Message = $"http://188.42.174.122/#/recovery/?key={recoveryKey}"
            };

            return await SMTPHelper.SendMail(message);
        }
    }
}
