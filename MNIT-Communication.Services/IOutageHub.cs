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
        Task NotifyChange(Alert outageDetail);
    }
}
