using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MNIT_Communication.Domain;
using MNIT_Communication.Services;

namespace MNIT_Communication.Models
{
    public class BaseModel
    {
        private readonly UserProfile userProfile;

        public BaseModel(UserProfile userProfile)
        {
            this.userProfile = userProfile;
        }

        public UserProfile CurrentProfile
        {
            get { return userProfile ?? UserProfile.DefaultProfile; }
        }

        public bool HasProfile
        {
            get { return userProfile != null; }
        }

        public bool UserConfirmed
        {
            get { return HasProfile && CurrentProfile.Confirmed; }
        }
    }

    public class BaseModel<T>: BaseModel
    {
        private readonly T data;

        public BaseModel(UserProfile userProfile, T data): base(userProfile)
        {
            this.data = data;
        }

        public T Data { get { return data; } }
    }
}