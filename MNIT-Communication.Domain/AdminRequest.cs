using System;

namespace MNIT_Communication.Domain
{
    public class AdminRequest
    {
        public Guid UserId { get; set; }
        public Guid AdministratorId { get; set; }
    }
}