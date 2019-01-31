using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.BusinessLogic.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendActivationMessage(string receiver, string activationKey);
        Task<bool> SendRecoveryMessage(string receiver, string recoveryKey);
    }
}
