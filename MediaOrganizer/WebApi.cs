using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Web;
using System.Net;
using System.Web.Script;
using System.Web.Script.Serialization;

namespace MediaOrganizer
{
    class WebApi
    {
        public static string callImdb(string txtMovieName)
        {
            {
                string apikey = ConfigurationManager.AppSettings["apikey"];
                string apiurl = ConfigurationManager.AppSettings["apiurl"];
                string url = apiurl + txtMovieName.Trim() + "&" + apikey;
                
                using (WebClient wc = new WebClient())
                {
                    try
                    {
                        string html = wc.DownloadString(url);
                        var json = wc.DownloadString(url);
                        JavaScriptSerializer oJS = new JavaScriptSerializer();
                        ImdbEntity obj = new ImdbEntity();
                        obj = oJS.Deserialize<ImdbEntity>(json);
                        if (obj.Response == "True")
                        {
                            string data = obj.Genre.ToString();
                            string date = obj.Released.ToString();

                            DateTime releaseDate = Convert.ToDateTime(date);
                            TimeSpan ts = DateTime.Now - releaseDate;
                            int differenceInDays = ts.Days;
                            if (differenceInDays < 180)
                            {
                                return "New Releases";
                            }
                            else
                            {
                                string value = data.Split(',')[0];
                                return value;
                            }
                           
                        }
                        else
                        {
                            return "unknown";
                        }

                    }

                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message.ToString() + ", " + ex.InnerException.ToString());
                        return "Error, chek log";
                    }
                }
            }
        }

        public class ImdbEntity
        {
            public string Title { get; set; }
            public string Year { get; set; }
            public string Rated { get; set; }
            public string Released { get; set; }
            public string Runtime { get; set; }
            public string Genre { get; set; }
            public string Director { get; set; }
            public string Writer { get; set; }
            public string Actors { get; set; }
            public string Plot { get; set; }
            public string Language { get; set; }
            public string Country { get; set; }
            public string Awards { get; set; }
            public string Poster { get; set; }
            public string Metascore { get; set; }
            public string imdbRating { get; set; }
            public string imdbVotes { get; set; }
            public string imdbID { get; set; }
            public string Type { get; set; }
            public string Response { get; set; }
        }
    }
}
