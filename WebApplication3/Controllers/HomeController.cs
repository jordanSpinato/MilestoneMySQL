using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Threading;
using System.Web.Mvc;
using System.Web.Services.Discovery;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {

        //public SqlConnection  Connection = new SqlConnection("server=localhost" +
        //                               "Trusted_Connection=yes;" +
        //                               "database=ent-v-jordan.timerModel.dbo;" +
        //                               "connection timeout=30");

        // connection to the database
       public SqlConnection MyConnection = new SqlConnection("Data Source=ENT-V-JORDAN;Initial Catalog=timerModel;Integrated Security=True");

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Timer()
        {

            ViewBag.Message = "Milestones";
            
            TimerModel model = new TimerModel(); // create model object
            DateTime todayDate = DateTime.Now;
                       
            // assign current date to today dates
            // assign end date to end date
            model.DayOfYear = todayDate.DayOfYear;

            model.EndDate = todayDate;
            model.TimeStamp = todayDate;
            model.Success = false;

            //"ddd-mmm-dd-yyyy"
            return View(model);
        }

        // used to post to the DB
        [HttpPost]
        public ActionResult Timer(TimerModel model)
        {
            // open the database when the submit button is pressed
            MyConnection.Open();
            
            DateTime todayDate = DateTime.Now;
	        TimeSpan time = new TimeSpan();

			model.TimeStamp = todayDate;
	        model.TimeStampString = todayDate.ToString("D");
	        model.DeadlineDateString = model.EndDate.ToString("D");

	        time = model.EndDate - todayDate;

	        int totalDays = time.Days + 1;
	        model.TotalDays = totalDays;

            // get today and get end date. Find the difference between them and store in days remaining
	     
            /* Database Connection */
            model.Success = true;


            timerModelEntities database = new timerModelEntities(); 
			
            string sqlString = "INSERT INTO Milestones(TimeStamp,Description,EndDate,TotalProjectDays,TimeStampString,EndDateString) " +
                               "VALUES(@TimeStamp, @Description, @EndDate, @TotalProjectDays, @TimeStampString, @EndDateString)";
            SqlCommand myCommand = new SqlCommand(sqlString, MyConnection);

            myCommand.Parameters.AddWithValue("@TimeStamp", model.TimeStamp);
            myCommand.Parameters.AddWithValue("@Description", model.Description);
            myCommand.Parameters.AddWithValue("@EndDate", model.EndDate);
            myCommand.Parameters.AddWithValue("@TotalProjectDays", totalDays);
			myCommand.Parameters.AddWithValue("@TimeStampString", model.TimeStampString);
			myCommand.Parameters.AddWithValue("@EndDateString", model.DeadlineDateString);
            
			/* Execution of query */
			myCommand.ExecuteNonQuery();
            model.Title = "Success";

            MyConnection.Close();

            return View(model);

        }


        // pull from the SQL database
        public ActionResult ViewTimes()
        {
            TimerModel model = new TimerModel(); // create model object
            DateTime todayDate = DateTime.Now;
	        DateTime endDate = new DateTime();

			// lists that will be assigned to the model
			List<string> endDateListString = new List<string>();
			List<string> startDateListString = new List<string>();


			List<string> descriptionList = new List<string>();
			List<DateTime> timeStampList = new List<DateTime>();
			List<int> daysRemaining = new List<int>();
			List<double> percentList = new List<double>();


			// open connection
			MyConnection.Open();
            
			const string sqlStatement = "SELECT * from Milestones WHERE EndDate > @today";
	        const string sqlCountStatement = "SELECT COUNT(*) FROM Milestones WHERE EndDate > @today";

			SqlCommand command = new SqlCommand(sqlStatement, MyConnection);
			SqlCommand secondCommand = new SqlCommand(sqlCountStatement, MyConnection);

			command.Parameters.AddWithValue("@today", todayDate.Date);
			secondCommand.Parameters.AddWithValue("@today", todayDate.Date);



			model.TotalEntries = (int)secondCommand.ExecuteScalar();
	

	        using (SqlDataReader reader = command.ExecuteReader())
			{
				while (reader.Read())
				{
					timeStampList.Add(reader.GetDateTime(0));
					descriptionList.Add(reader.GetString(1));
					endDate = reader.GetDateTime(2);
					startDateListString.Add(reader.GetString(4));
					endDateListString.Add(reader.GetString(5));

					/* calculate percent of completed task */
					var top = todayDate - reader.GetDateTime(0);
					var topDays = top.Days;
					var bottom = reader.GetInt32(3);
					var ans = (double)topDays / bottom * 100;
				    var percent = Math.Round(ans, 2);
					//var percent = (int)ans;
				    percentList.Add(percent);

					/* calculate days remaining end date - today = days remaining */
					var daysLeft = endDate - todayDate;
					var day = daysLeft.Days + 1;
					daysRemaining.Add(day);
				}
			}


	        model.EndDates = endDateListString;
	        model.StartDates = startDateListString;
	        model.DescriptionList = descriptionList;
	        model.ListOfDaysRemaining = daysRemaining;
	        model.TimeStamps = timeStampList;
	        model.PercentList = percentList;

		
            MyConnection.Close();
            return View(model);
        }


        // pull from the SQL database
        public ActionResult ViewAllTimes()
        {
            TimerModel model = new TimerModel(); // create model object
            DateTime todayDate = DateTime.Now;
            DateTime endDate = new DateTime();

            // lists that will be assigned to the model
            List<string> endDateListString = new List<string>();
            List<string> startDateListString = new List<string>();


            List<string> descriptionList = new List<string>();
            List<DateTime> timeStampList = new List<DateTime>();
            List<int> daysRemaining = new List<int>();
            List<double> percentList = new List<double>();


            // open connection
            MyConnection.Open();

            const string sqlStatement = "SELECT * from Milestones";
            const string sqlCountStatement = "SELECT COUNT(*) FROM Milestones";

            SqlCommand command = new SqlCommand(sqlStatement, MyConnection);
            SqlCommand secondCommand = new SqlCommand(sqlCountStatement, MyConnection);

            

            model.TotalEntries = (int)secondCommand.ExecuteScalar();


            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    timeStampList.Add(reader.GetDateTime(0));
                    descriptionList.Add(reader.GetString(1));
                    endDate = reader.GetDateTime(2);
                    startDateListString.Add(reader.GetString(4));
                    endDateListString.Add(reader.GetString(5));

                    /* calculate percent of completed task */
                    var top = todayDate - reader.GetDateTime(0);
                    var topDays = top.Days;
                    var bottom = reader.GetInt32(3);
                    var ans = (double)topDays / bottom * 100;
                    var percent = Math.Round(ans, 2);
                    //var percent = (int)ans;
                    if (todayDate > endDate)
                        percent = 100;
                    percentList.Add(percent);

                    /* calculate days remaining end date - today = days remaining */
                    var daysLeft = endDate - todayDate;
                    var day = daysLeft.Days + 1;
                    daysRemaining.Add(day);
                }
            }


            model.EndDates = endDateListString;
            model.StartDates = startDateListString;
            model.DescriptionList = descriptionList;
            model.ListOfDaysRemaining = daysRemaining;
            model.TimeStamps = timeStampList;
            model.PercentList = percentList;


            MyConnection.Close();
            return View(model);
        }



    }
}