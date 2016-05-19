using System;
using System.Collections.Generic;

namespace WebApplication3.Models
{
    public class TimerModel
    {
        public int DayOfYear { get; set; }
        public string Title { get; set; }
        public int Output { get; set; }


		public string Description { get; set; }
	    public DateTime TimeStamp { get; set; }
		public string TimeStampString { get; set; }
		public DateTime EndDate { get; set;  }
		public string DeadlineDateString { get; set;  }
		public bool Success { get; set; }
		public int TotalDays { get; set; }
		public int TotalEntries { get; set; }

		public List<string> DescriptionList { get; set; }
	    public List<DateTime> TimeStamps { get; set; }
	    public List<string> EndDates { get; set; }
		public List<string> StartDates { get; set; }
		public List<int> ListOfDaysRemaining { get; set; }
		public List<double> PercentList { get; set; }





	}

}

