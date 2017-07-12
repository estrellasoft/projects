using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Web;


namespace MediaOrganizer
{
    class Program
    {
        static string movieFolder = ConfigurationManager.AppSettings["moviesFolder"];
        static string logfolder = ConfigurationManager.AppSettings["logfolder"];
        static string logfilename = ConfigurationManager.AppSettings["logfilename"];
 
        private static List<string> excludedDirectories = new List<string>() {"genre","Genre"};
        static void Main(string[] args)
        {

            //Get a list of all the movie folders excluse the genres folder which is also off the root
            var filteredDirs = Directory.GetDirectories(movieFolder).Where(d => !isExcluded(excludedDirectories, d));

            //rename the movies in the folder to the folder name
              foreach (string folderName in filteredDirs)
            {
                string movieName = new DirectoryInfo(folderName).Name;
                DirectoryInfo d = new DirectoryInfo(folderName);//
                FileInfo[] files = d.GetFiles("*.mp4"); //Getting Media files

                //move movies from their folder to the root
                foreach (FileInfo file in files)
                {
                    System.IO.File.Move(folderName + "\\" + file.Name, movieFolder + "\\" + movieName + ".mp4");
                    Console.WriteLine("Renaming " + folderName + "\\" + file.Name + " to " + movieFolder + "\\" + movieName + ".mp4");
                   
                    //Log anything that affects the fiole system with a timestamp
                    using (StreamWriter w = File.AppendText(logfolder + logfilename))
                    {
                        Log("Renaming " + folderName + "\\" + file.Name + " to " + movieFolder + "\\" + movieName + ".mp4", w);
                    }
                }

                // if directory is empty 
                if (IsDirectoryEmpty(d))
                {
                    d.Delete();
                    Console.WriteLine("Folder " + d + " deleted");
                }

                else
                {
                    Console.WriteLine("Folder " + d + " not empty. See log for details.");
                    using (StreamWriter w = File.AppendText(logfolder + logfilename))
                    {
                        Log("Folder " + d + " not empty", w);
                    }
                }
              
            }

              classifyTitles();
              Console.WriteLine("Process Completed.Press any key to close.");
            Console.ReadLine();
        }

        public static void classifyTitles()
        {
            //get genre for all movies in root and move to existing folder or create new folder.
            
            DirectoryInfo d = new DirectoryInfo(movieFolder);//
                FileInfo[] files = d.GetFiles("*.mp4"); //Getting Media files

                //move movies from root to genre
                foreach (FileInfo file in files)
                {
                    //replace underscores with plus "+"
                    string baseFilename = file.Name.Replace(file.Extension, "");
                    baseFilename = baseFilename.Replace("_", "+");
                    string genre = getMovieDetails(baseFilename);

                    // if folder exists copy to folder
                    //remember to check CASE

                    try
                    {
                        //create new folder then copy
                        if (!Directory.Exists(movieFolder + "\\" + "Genre" + "\\" + genre))
                        {
                            // Try to create the directory.
                            DirectoryInfo di = Directory.CreateDirectory(movieFolder + "\\" + "Genre" + "\\" + genre);
                           
                        }
                        
                            
                        else
                        {

                        }
                        //copy to existing/new  folder
                        System.IO.File.Move(movieFolder + "\\" + file, movieFolder + "\\" + "Genre" + "\\" + genre + "\\" + file);
                    }
                    catch (IOException ioex)
                    {
                        Console.WriteLine(ioex.Message);
                    }
                    //if folder does not exist, created foler, then copy file
                }
        }

        public static string getMovieDetails(string movieTitle)
        {
            //handle possibility that multiple titles can be returned
            //string movieTitle = "resurrection+of+gavin+stone";
            string genre = WebApi.callImdb(movieTitle);
            //Console.WriteLine(genre);
            return genre;
            
        }

        static bool isExcluded(List<string> exludedDirList, string target)
        {
            return exludedDirList.Any(d => new DirectoryInfo(target).Name.Equals(d));
        }

        public static bool IsDirectoryEmpty(DirectoryInfo directory)
        {
            FileInfo[] files = directory.GetFiles();
            DirectoryInfo[] subdirs = directory.GetDirectories();

            return (files.Length == 0 && subdirs.Length == 0);
        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
            w.WriteLine("{0}", logMessage);

        }
      
    }
}
