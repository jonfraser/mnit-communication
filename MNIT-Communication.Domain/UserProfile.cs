using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MongoDB.Bson.Serialization.Attributes;

namespace MNIT_Communication.Domain
{
    public class UserProfile: BaseEntity
    {
        private const string HealthEmailSuffix = "@health.qld.gov.au";
        
        public string EmailAdressInternal { get; set; }
		public string ExternalProvider { get; set; }
        public string ExternalId { get; set; }
		public string EmailAddressExternalProvider { get; set; }
		public string MobilePhoneNumber { get; set; }
        public Guid ConfirmationSecret { get; set; }
        public bool Confirmed { get; set; }
        public bool IsAdmin { get; set; }
        public Guid AdminGrantedBy { get; set; }
        
        private string displayName;
        
        [BsonIgnore]
        public string DisplayName
        {
            get
            {
                return displayName ?? string.Format("{0} (via {1})", EmailAddressExternalProvider, (ExternalProvider ?? "").ToUpper());
            }
            private set { displayName = value; }
        }

        [BsonIgnore]
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(EmailAdressInternal))
                    return string.Empty;

                var name = EmailAdressInternal.Replace(HealthEmailSuffix, "");
                name = name.Replace(".", " ");

                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;
                name = textInfo.ToTitleCase(name);

                return name;
            }

        }

        private IList<Guid> alertSubscriptions = new List<Guid>();
        
        public IList<Guid> AlertSubscriptions
        {
            get { return alertSubscriptions; }
            set { alertSubscriptions = value; }
        }

        public static UserProfile DefaultProfile
        {
            get
            {
                var defaultProfile = new UserProfile
                {
                    DisplayName = "UN-CONFIRMED ACCOUNT"
                };

                return defaultProfile;
            }
        }

        [BsonIgnore]
        public dynamic Summary
        {
            get
            {
                var summary = new
                {
                    this.Id,
                    this.EmailAdressInternal,
                    this.Name
                };

                return summary;
            }
        }
    }
}
