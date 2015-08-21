using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace MNIT_Communication.Domain
{
	public class Alert: BaseEntity
	{
	    
	    public Alertable Service { get; set; }
        public string Summary { get; set; }
		public DateTime Start { get; set; }
		
        public Guid RaisedBy { get; set; }

	    private IList<AlertHistory> history = new List<AlertHistory>();
	    public IList<AlertHistory> History
	    {
	        get { return history; }
	        set { history = value; }
	    }

        [BsonIgnore] //Don't persist 
        public bool UserSubscribed { get; set; }

        [BsonIgnore] //Don't persist 
        public bool IsCurrent => !LastUpdate.Status.Equals(AlertStatus.Resolved) && !LastUpdate.Status.Equals(AlertStatus.Cancelled);

	    [BsonIgnore] //Don't persist 
        public AlertHistory LastUpdate
        {
            get
            {
                var lastHistory = history.OrderByDescending(h => h.Timestamp).FirstOrDefault();
                
                if (lastHistory != null)
                {
                    return lastHistory;
                }

                return new AlertHistory
                {
                    Timestamp = Start,
                    Detail = Summary,
                    Status = AlertStatus.Raised,
                    UserId = RaisedBy
                };
            }
        }

        [BsonIgnore] //Don't persist 
        public DateTime UpdateDate {
            get
            {
                if (LastUpdate != null)
                {
                    return LastUpdate.Timestamp;
                }

                return Start;
            } 
        }

    }
}
