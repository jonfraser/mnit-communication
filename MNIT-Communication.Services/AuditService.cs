using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using MNIT_Communication.Domain;

namespace MNIT_Communication.Services
{
    public interface IAuditService
    {
        void LogAuditEvent(AuditEvent auditEvent);
        Task LogAuditEventAsync(AuditEvent auditEvent);
    }

    public class AuditService: IAuditService
    {
        private readonly IRepository repository;
        private readonly AuditVerbosity verbosity = AuditVerbosity.Low;

        public AuditService(IRepository repository)
        {
            this.repository = repository;
            Enum.TryParse(CloudConfigurationManager.GetSetting("AuditVerbosity"), out verbosity);
        }

        public void LogAuditEvent(AuditEvent auditEvent)
        {
            Task.Run(() => LogAuditEventAsync(auditEvent));
        }

        public async Task LogAuditEventAsync(AuditEvent auditEvent)
        {
            var runtimeContext = ServiceLocator.Resolve<IRuntimeContext>();

            if (verbosity >= auditEvent.AuditType.AuditVerbosity)
            {
                auditEvent.Id = Guid.NewGuid();
                auditEvent.DateTimeStamp = DateTime.Now;
                auditEvent.IpAddress = runtimeContext.UserHostAddress;
                if (await runtimeContext.HasProfile())
                {
                    auditEvent.ChangedById = (await runtimeContext.CurrentProfile()).Id;
                }

                await repository.Upsert(auditEvent);
            }
        }
    }
}
