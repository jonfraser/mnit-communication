using System;
using MongoDB.Bson.Serialization.Attributes;

namespace MNIT_Communication.Domain
{
    public class AlertHistory
    {
        public DateTime Timestamp { get; set; }
        public AlertStatus Status { get; set; }
        public string Detail { get; set; }
        public Guid UserId { get; set; }

        [BsonIgnore]
        public string Display
        {
            get { return Status.Name + (string.IsNullOrEmpty(Detail) ? string.Empty : " - " + Detail); }
        }
    }
}