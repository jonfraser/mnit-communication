using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNIT.ErrorLogging;

namespace MNIT_Communication.Domain
{
    public class Error : BaseEntity
    {
        public Error(IError<Guid> baseError, Guid userId)
        {
            this.Id = baseError.ID;
            this.AdditionalInformation = baseError.AdditionalInformation;
            this.ClientAddress = baseError.ClientAddress;
            this.ErrorHash = baseError.ErrorHash;
            this.ErrorTime = baseError.ErrorTime;
            this.FullErrorString = baseError.FullErrorString;
            this.RootErrorMessage = baseError.RootErrorMessage;
            this.UserId = userId;
        }

        public string AdditionalInformation { get; private set; }
        public string ClientAddress { get; private set; }
        public string ErrorHash { get; private set; }
        public DateTime ErrorTime { get; private set; }
        public string FullErrorString { get; private set; }
        public string RootErrorMessage { get; private set; }
        public Guid UserId { get; private set; }
    }
}
