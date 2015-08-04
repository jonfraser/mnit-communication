using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNIT_Communication.Domain;

namespace MNIT_Communication.Services
{
    public interface IOutageHub
    {
        Task SendNew(Alert outageDetail);
        void UpdateExisting(Alert outageDetail);
        void RemoveExisting(Alert outageDetail);
    }
}
