using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MNIT_Communication.Domain;
using MNIT_Communication.Models;
using MNIT_Communication.Services;

namespace MNIT_Communication.Controllers
{
    public class BaseController : Controller
    {
        private readonly IRuntimeContext runtimeContext;

        public BaseController(IRuntimeContext runtimeContext)
        {
            this.runtimeContext = runtimeContext;
        }

        protected async Task<BaseModel> GetBaseModel()
        {
            var userProfile = await runtimeContext.CurrentProfile();
            return new BaseModel(userProfile);
        }

        protected async Task<BaseModel<T>> GetBaseModel<T>(T data)
        {
            var userProfile = await runtimeContext.CurrentProfile();
            return new BaseModel<T>(userProfile, data);
        }

        public async Task<ViewResult> BaseView()
        {
            return View(await GetBaseModel());
        }

        public async Task<ViewResult> BaseView<T>(T data)
        {
            return View(await GetBaseModel(data));
        }
    }
}