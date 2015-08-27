namespace MNIT_Communication.Domain
{
    public class AuditType
    {
        public string EventName { get; set; }
        public AuditVerbosity AuditVerbosity { get; set; }

        public static AuditType SmsMessageSent = new AuditType
        {
            EventName = "SmsMessageSent",
            AuditVerbosity = AuditVerbosity.Outrageous
        };

        public static AuditType EmailMessageSent = new AuditType
        {
            EventName = "EmailMessageSent",
            AuditVerbosity = AuditVerbosity.Outrageous
        };

        public static AuditType EntityUpsert = new AuditType
        {
            EventName = "EntityUpdated",
            AuditVerbosity = AuditVerbosity.Medium
        };

        public static AuditType UserLogin = new AuditType
        {
            EventName = "UserLogin",
            AuditVerbosity = AuditVerbosity.Medium
        };

        public static AuditType UserLogout = new AuditType
        {
            EventName = "UserLogout",
            AuditVerbosity = AuditVerbosity.Medium
        };

        public static AuditType AlertRaised = new AuditType
        {
            EventName = "AlertRaised",
            AuditVerbosity = AuditVerbosity.Low
        };

        public static AuditType AlertCancelled = new AuditType
        {
            EventName = "AlertCancelled",
            AuditVerbosity = AuditVerbosity.Medium
        };

        public static AuditType AlertResolved = new AuditType
        {
            EventName = "AlertResolved",
            AuditVerbosity = AuditVerbosity.Medium
        };

        public static AuditType AlertUpdated = new AuditType
        {
            EventName = "AlertUpdated",
            AuditVerbosity = AuditVerbosity.High
        };

        public static AuditType UserRegistered = new AuditType
        {
            EventName = "UserRegistered",
            AuditVerbosity = AuditVerbosity.Low
        };

        public static AuditType UserConfirmed = new AuditType
        {
            EventName = "UserConfirmed",
            AuditVerbosity = AuditVerbosity.Low
        };

        public static AuditType WebJobMessageProcessing = new AuditType
        {
            EventName = "WebJobMessageProcessing",
            AuditVerbosity = AuditVerbosity.High
        };

    }
}