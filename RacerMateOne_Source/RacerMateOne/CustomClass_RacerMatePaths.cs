using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Reflection;

namespace RacerMateOne
{
    /// <summary>
    /// This class will return full paths of directories used in the Racermate App in a OS-independent way
    /// </summary>
   
    public static class RacerMatePaths
    {
        private const string _racermate = @"\RacerMate";
        private const string _settings = @"\RacerMate\Settings";
        private const string _backup = @"\RacerMate\Settings\Backup";
        private const string _reporttemplates = @"\RacerMate\ReportTemplates";
        private const string _reports = @"\RacerMate\Reports";
        private const string _exports = @"\RacerMate\Exports";
        private const string _courses = @"\RacerMate\Courses";
        private const string _commoncourses = @"\Courses";
        private const string _performances = @"\RacerMate\Performances";
        private const string _debug = @"\RacerMate\Debug";
		private const string _errors = @"\RacerMate\Errors";
    

       // private const string _defaultsettings = @"\RM1_DefaultSettings.xml";
        
        public static string RacerMateFullPath
        { get { 
            string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)  + _racermate; 
            if (!Directory.Exists(zzz)) return null;
            return zzz;
              }
        }

		static string ms_BrowsePath;
		public static string BrowsePath
		{
			get
			{
				if (ms_BrowsePath == null)
					ms_BrowsePath = RacerMateFullPath;
				return ms_BrowsePath;
			}
			set
			{
				ms_BrowsePath = value;
				RM1_Settings.General.NeedSave = true;
			}
		}



        public static string SettingsFullPath
        {
            get
            {
                string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _settings;
                if (!Directory.Exists(zzz)) return null;
                return zzz;
            }
        }
        public static string DefaultSettingsFullPath //will be installed with .exe in programs folder
        {
            get
            {
                string appDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                appDir = new Uri(appDir).LocalPath;
                string zzz = appDir;
                if (!Directory.Exists(zzz)) return null;
                return zzz;
            }
        }
     
        public static string BackupFullPath
        {
            get
            {
                string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _backup;
                if (!Directory.Exists(zzz)) return null;
                return zzz;
            }
        }
        public static string ReportTemplatesFullPath
        {
            get
            {
                string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _reporttemplates;
                if (!Directory.Exists(zzz)) return null;
                return zzz;
            }
        }
        public static string ReportsFullPath
        {
            get
            {
                string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _reports;
                if (!Directory.Exists(zzz)) return null;
                return zzz;
            }
        }
        public static string CoursesFullPath
        {
            get
            {
                string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _courses;
                if (!Directory.Exists(zzz)) return null;
                return zzz;
            }
        }
		static String m_CommonCoursesFullPath = null;
        public static string CommonCoursesFullPath
        {
            get
            {
				// First time we lock in this path.
				if (m_CommonCoursesFullPath == null)
				{
					string zzz = Environment.CurrentDirectory + _commoncourses;
					if (!Directory.Exists(zzz))
						zzz = CoursesFullPath;
					m_CommonCoursesFullPath = zzz;
				}
                return m_CommonCoursesFullPath;
            }
        }
        public static string ExportsFullPath
        {
            get
            {
                string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _exports;
                if (!Directory.Exists(zzz)) return null;
                return zzz;
            }
        }
        public static string PerformancesFullPath
        {
            get
            {
                string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _performances;
                if (!Directory.Exists(zzz)) return null;
                return zzz;
            }
        }
        public static string DebugFullPath
        {
            get
            {
                string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _debug;
                if (!Directory.Exists(zzz)) return null;
                return zzz;
            }
        }

		public static string ErrorsFullPath
		{
			get
			{
				string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _errors;
				if (!Directory.Exists(zzz))
				{
					Directory.CreateDirectory(zzz);
					if (!Directory.Exists(zzz))
						return null;
				}
				return zzz;
			}
		}



        public static string RacerMateRelativePath
        {
            get
            {
                string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _racermate;
                if (!Directory.Exists(zzz)) return null;
                return _racermate ;
            }
        }
        public static string SettingsRelativePath
        {
            get
            {
                string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _settings;
                if (!Directory.Exists(zzz)) return null;
                return _settings;
            }
        }

     //   public static string DefaultSettingsRelativePath
     //   {
     //       get
     //       {
     //           string appDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
     //           appDir = new Uri(appDir).LocalPath;
     //           string zzz = appDir;
     //           if (!Directory.Exists(zzz)) return null;
     //           return zzz;
     //       }
     //   }
      
        public static string BackupRelativePath
        {
            get
            {
                string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _backup;
                if (!Directory.Exists(zzz)) return null;
                return _backup;
            }
        }
        public static string ReportTemplatesRelativePath
        {
            get
            {
                string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _reporttemplates;
                if (!Directory.Exists(zzz)) return null;
                return _reporttemplates ;
            }
        }
        public static string ReportsRelativePath
        {
            get
            {
                string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _reports;
                if (!Directory.Exists(zzz)) return null;
                return _reports;
            }
        }
        public static string CoursesRelativePath
        {
            get
            {
                string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _courses;
                if (!Directory.Exists(zzz)) return null;
                return _courses;
            }
        }
        public static string ExportsRelativePath
        {
            get
            {
                string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _exports;
                if (!Directory.Exists(zzz)) return null;
                return _exports;
            }
        }
        public static string PerformancesRelativePath
        {
            get
            {
                string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _performances;
                if (!Directory.Exists(zzz)) return null;
                return _performances;
            }
        }
        public static string DebugRelativePath
        {
            get
            {
                string zzz = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + _debug;
                if (!Directory.Exists(zzz)) return null;
                return _debug;
            }
        }
		public static string RCVFullPath
		{
			get
			{
				return RM1_Settings.General.Path_RCV;
			}
		}
		public static string EXEPath
		{
			get
			{
				Uri uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
				string path = uri.LocalPath;
				return Path.GetDirectoryName(path);
			}
		}

           
            /// <summary>
            /// Creates a relative path from one file or folder to another. from Paul Welter
            /// </summary>
            /// <param name="fromDirectory">
            /// Contains the directory that defines the start of the relative path.
            /// </param>
            /// <param name="toPath">
            /// Contains the path that defines the endpoint of the relative path.
            /// </param>
            /// <returns>
            /// The relative path from the start directory to the end path.
            /// </returns>
            /// <exception cref="ArgumentNullException"></exception>
        public static string RelativePathTo(string fromDirectory, string toPath)
        {
            if (fromDirectory == null)
                throw new ArgumentNullException("fromDirectory");
            if (toPath == null)
                throw new ArgumentNullException("toPath");
            bool isRooted = Path.IsPathRooted(fromDirectory)&& Path.IsPathRooted(toPath);
            if (isRooted)
            {
                bool isDifferentRoot = string.Compare(Path.GetPathRoot(fromDirectory), Path.GetPathRoot(toPath), true) != 0;
                if (isDifferentRoot)
                    return toPath;
            }
            StringCollection relativePath = new StringCollection();
            string[] fromDirectories = fromDirectory.Split(Path.DirectorySeparatorChar);
            string[] toDirectories = toPath.Split(Path.DirectorySeparatorChar);
            int length = Math.Min(fromDirectories.Length, toDirectories.Length);
            int lastCommonRoot = -1;
            // find common root
            for (int x = 0; x < length; x++)
            {
                if (string.Compare(fromDirectories[x], toDirectories[x], true) != 0)
                    break;
                lastCommonRoot = x;
            }
            if (lastCommonRoot == -1)
                return toPath;
            // add relative folders in from path
            for (int x = lastCommonRoot + 1; x < fromDirectories.Length; x++)
                if (fromDirectories[x].Length > 0)
                    relativePath.Add("..");
            // add to folders to path
            for (int x = lastCommonRoot + 1; x < toDirectories.Length; x++)
                relativePath.Add(toDirectories[x]);
            // create relative path
            string[] relativeParts = new string[relativePath.Count];
            relativePath.CopyTo(relativeParts, 0);
            string newPath = string.Join(Path.DirectorySeparatorChar.ToString(), relativeParts);
            return newPath;
        }



       


     
        /// <summary>
        /// constructor is called on first call to any of the properties of this class.
        /// It ensures that the directories exist before returning a path.
        /// Failure to create a path is reflected by its non-existance
        /// in which case a NULL returned by the property.
        /// </summary>
        static RacerMatePaths()
        {
            List<string> mypaths = new List<string>();
            mypaths.Clear();
            mypaths.Add(_racermate);
            mypaths.Add(_settings);
            mypaths.Add(_backup);
            mypaths.Add(_reporttemplates);
            mypaths.Add(_courses);
            mypaths.Add(_reports);
            mypaths.Add(_exports);
            mypaths.Add(_performances);
            mypaths.Add(_debug);
			mypaths.Add(_errors);

            foreach (string aaa in mypaths)
            {
                string newFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + aaa;
                if (!Directory.Exists(newFolder))
                {
                    try
                    {
                        Directory.CreateDirectory(newFolder);
                    }
                    catch { } //do nothing on failure, will be reflected in null returns on properties
                }
                try
                {
                    if (aaa == _reports)
                    {
                        string srcPath = RacerMatePaths.DefaultSettingsFullPath + @"\Art\Reports";
                        string dstFilename = newFolder + @"\aboutyou.png";
                        if(!File.Exists(dstFilename)){File.Copy(srcPath + @"\aboutyou.png", dstFilename);}
                        dstFilename = newFolder + @"\tourlogosmall.png";
                        if (!File.Exists(dstFilename)) { File.Copy(srcPath + @"\tourlogosmall.png", dstFilename); }
                        dstFilename = newFolder + @"\urllogosmall.png";
                        if (!File.Exists(dstFilename)) { File.Copy(srcPath + @"\urllogosmall.png", dstFilename); }
                    }
                    else if (aaa == _reporttemplates)
                    {
                        string srcPath = RacerMatePaths.DefaultSettingsFullPath + @"\Resources";
                        string dstFilename = newFolder + @"\RM1Logo.gif";
                        if (!File.Exists(dstFilename)) { File.Copy(srcPath + @"\RM1Logo.gif", dstFilename); }
                        dstFilename = newFolder + @"\RM1_RideOutput.xsl";
                        if (!File.Exists(dstFilename)) { File.Copy(srcPath + @"\RM1_RideOutput.xsl", dstFilename); }
                        dstFilename = newFolder + @"\RM1_RideInput.xsl";
                        if (!File.Exists(dstFilename)) { File.Copy(srcPath + @"\RM1_RideInput.xsl", dstFilename); }
                    }
                }
                catch { } //do nothing on failure, will be reflected in null returns on properties
            }
        }

    
    
    
    }
}
