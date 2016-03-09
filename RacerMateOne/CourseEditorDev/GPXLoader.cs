using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

// http://weblogs.asp.net/jimjackson/archive/2009/03/10/using-linq-to-xml-with-c-to-read-gpx-files.aspx
// http://www.topografix.com/gpx_manual.asp

namespace RacerMateOne.CourseEditorDev
{
    public class TrackSegment
    {
      public double Latitude {get; set;}
      public double Longitude { get; set; }
      public double Elevation { get; set; }
      public double Course { get; set; }
      public double Speed { get; set; }

      public string Time { get; set; }

      //public TrackSegment(double Latitude, double Longitude, double Elevation, DateTime Time = default(DateTime))
      public TrackSegment(double Latitude, double Longitude, double Elevation, string Time = null, double Course = 0.0, double Speed = 0.0)
        {
            this.Elevation = Elevation;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Time = Time;
            this.Course = Course;
            this.Speed = Speed;
        }
    }

    public class GPXTrack
    {
        public List<TrackSegment> SegList;
        public string TrackName
        {
            get;
            set;
        }
        public GPXTrack(string TrackName)
        {
            this.TrackName = TrackName;
            SegList = new List<TrackSegment>();
        }
    }

    //public class RacerMateTrack
    //{
    //    public int Divisions;

    //    public double Length { get; set; }
    //    public double Grade { get; set; }
    //    public double Wind { get; set; }
    //    public double Rotation { get; set; }
    //    public RacerMateTrack(int Divisions, double Length, double Grade, double Wind, double Rotation)
    //    {
    //        this.Divisions = Divisions;
    //        this.Length = Length;
    //        this.Grade = Grade;
    //        this.Wind = Wind;
    //        this.Rotation = Rotation;

    //    }
    //}

    public class GPXLoader
    {

        public List<GPXTrack> GPXTrackList;

        //public GPXLoader()
        //{
        //    GPXTrackList = new List<GPXTrack>();
        //}
        /// <summary> 
        /// Load the Xml document for parsing 
        /// </summary> 
        /// <param name="sFile">Fully qualified file name (local)</param> 
        /// <returns>XDocument</returns> 
        private XDocument GetGpxDoc(string sFile)
        {
            XDocument gpxDoc = XDocument.Load(sFile);
            return gpxDoc;
        }

        /// <summary> 
        /// Load the namespace for a standard GPX document 
        /// </summary> 
        /// <returns></returns> 
        private XNamespace GetGpxNameSpace()
        {
            XNamespace gpx = XNamespace.Get("http://www.topografix.com/GPX/1/1");
            return gpx;
        }

        /// <summary> 
        /// When passed a file, open it and parse all waypoints from it. 
        /// </summary> 
        /// <param name="sFile">Fully qualified file name (local)</param> 
        /// <returns>string containing line delimited waypoints from 
        /// the file (for test)</returns> 
        /// <remarks>Normally, this would be used to populate the 
        /// appropriate object model</remarks> 
        public string LoadGPXWaypoints(string sFile)
        {
            XDocument gpxDoc = GetGpxDoc(sFile);
            XNamespace gpx = GetGpxNameSpace();

            XNamespace df = gpxDoc.Root.Name.Namespace;


            var waypoints = from waypoint in gpxDoc.Descendants(gpx + "wpt")
                            select new
                            {
                                Latitude = waypoint.Attribute("lat").Value,
                                Longitude = waypoint.Attribute("lon").Value,
                                Elevation = waypoint.Element(gpx + "ele") != null ?
                                    waypoint.Element(gpx + "ele").Value : null,
                                Name = waypoint.Element(gpx + "name") != null ?
                                    waypoint.Element(gpx + "name").Value : null,
                                Dt = waypoint.Element(gpx + "cmt") != null ?
                                    waypoint.Element(gpx + "cmt").Value : null
                            };

            StringBuilder sb = new StringBuilder();
            foreach (var wpt in waypoints)
            {
                // This is where we'd instantiate data 
                // containers for the information retrieved. 
                sb.Append(
                  string.Format("Name:{0} Latitude:{1} Longitude:{2} Elevation:{3} Date:{4}\n",
                  wpt.Name, wpt.Latitude, wpt.Longitude,
                  wpt.Elevation, wpt.Dt));
            }

            return sb.ToString();
        }

        /// <summary> 
        /// When passed a file, open it and parse all tracks 
        /// and track segments from it. 
        /// </summary> 
        /// <param name="sFile">Fully qualified file name (local)</param> 
        /// <returns>string containing line delimited waypoints from the 
        /// file (for test)</returns> 
        public string LoadGPXTracks(string sFile)
        {
            XDocument gpxDoc = GetGpxDoc(sFile);
            XNamespace gpx = GetGpxNameSpace();

            XNamespace df = gpxDoc.Root.Name.Namespace;

            var tracks = from track in gpxDoc.Descendants(gpx + "trk")
                         select new
                         {
                             Name = track.Element(gpx + "name") != null ?
                              track.Element(gpx + "name").Value : null,
                             Segs = (
                                  from trackpoint in track.Descendants(gpx + "trkpt")
                                  select new
                                  {
                                      Latitude = trackpoint.Attribute("lat").Value,
                                      Longitude = trackpoint.Attribute("lon").Value,
                                      Elevation = trackpoint.Element(gpx + "ele") != null ?
                                        trackpoint.Element(gpx + "ele").Value : null,
                                      Time = trackpoint.Element(gpx + "time") != null ?
                                        trackpoint.Element(gpx + "time").Value : null
                                  }
                                )
                         };
            

            StringBuilder sb = new StringBuilder();
            foreach (var trk in tracks)
            {
                // Populate track data objects. 
                foreach (var trkSeg in trk.Segs)
                {
                    // Populate detailed track segments 
                    // in the object model here. 
                    sb.Append(
                      string.Format("Track:{0} - Latitude:{1} Longitude:{2} " +
                                   "Elevation:{3} Date:{4}\n",
                      trk.Name, trkSeg.Latitude,
                      trkSeg.Longitude, trkSeg.Elevation,
                      trkSeg.Time));
                }
            }
            return sb.ToString();
        }

        // http://stackoverflow.com/questions/27928/how-do-i-calculate-distance-between-two-latitude-longitude-points
        public static double FindDistance(double LatitudeA, double LongitudeA, double LatitudeB, double LongitudeB)
        {
            int R = 6371; // Radius of the earth in km
            double dLat = deg2rad(Math.Abs(LatitudeB - LatitudeA));  // deg2rad below
            double dLon = deg2rad(Math.Abs(LongitudeB - LongitudeA));
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(deg2rad(LatitudeA)) * Math.Cos(deg2rad(LatitudeB)) *  Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c; // Distance in km
            return d;
        }

        public static double deg2rad(double deg)
        {
            return deg * (Math.PI / 180);
        }

        
        /// <summary> 
        /// When passed a file, open it and parse all tracks 
        /// and track segments from it. 
        /// </summary> 
        /// <param name="sFile">Fully qualified file name (local)</param> 
        /// <returns>string containing line delimited waypoints from the 
        /// file (for test)</returns> 
        public List<GPXTrack> LoadGPXTracksList(string sFile)
        {
            XDocument gpxDoc = GetGpxDoc(sFile);
            XNamespace gpx = gpxDoc.Root.Name.Namespace;
            string strNameSpace = gpx.ToString();
           
            if (strNameSpace == "http://www.topografix.com/GPX/1/1")
            {
                var tracks = from track in gpxDoc.Descendants(gpx + "trk")
                             select new
                             {
                                 Name = track.Element(gpx + "name") != null ?
                                  track.Element(gpx + "name").Value : null,
                                 Segs = (
                                      from trackpoint in track.Descendants(gpx + "trkpt")
                                      select new
                                      {
                                          Latitude = trackpoint.Attribute("lat").Value,
                                          Longitude = trackpoint.Attribute("lon").Value,
                                          Elevation = trackpoint.Element(gpx + "ele") != null ?
                                            trackpoint.Element(gpx + "ele").Value : null,
                                          Time = trackpoint.Element(gpx + "time") != null ?
                                            trackpoint.Element(gpx + "time").Value : null
                                      }
                                    )
                             };

                // StringBuilder sb = new StringBuilder();
                int count = tracks.Count();
                GPXTrackList = new List<GPXTrack>();
                foreach (var trk in tracks)
                {
                    GPXTrack gPXTrack = new GPXTrack(trk.Name);
                    // Populate track data objects. 
                    foreach (var trkSeg in trk.Segs)
                    {
                        double Latitude = string.IsNullOrEmpty(trkSeg.Latitude) ? 0 : double.Parse(trkSeg.Latitude);
                        double Longitude = string.IsNullOrEmpty(trkSeg.Longitude) ? 0 : double.Parse(trkSeg.Longitude);
                        double Elevation = string.IsNullOrEmpty(trkSeg.Elevation) ? 0 : double.Parse(trkSeg.Elevation);

                        gPXTrack.SegList.Add(new TrackSegment(Latitude, Longitude, Elevation, trkSeg.Time));
                    }
                    GPXTrackList.Add(gPXTrack);
                }
                return GPXTrackList;
            }
            else if (strNameSpace == "http://www.topografix.com/GPX/1/0")
            {
                var tracks = from track in gpxDoc.Descendants(gpx + "trk")
                             select new
                             {
                                 Name = track.Element(gpx + "name") != null ?
                                  track.Element(gpx + "name").Value : null,
                                 Segs = (
                                      from trackpoint in track.Descendants(gpx + "trkpt")
                                      select new
                                      {
                                          Latitude = trackpoint.Attribute("lat").Value,
                                          Longitude = trackpoint.Attribute("lon").Value,

                                          Elevation = trackpoint.Element(gpx + "ele") != null ?
                                             trackpoint.Element(gpx + "ele").Value : null,

                                          Time = trackpoint.Element(gpx + "time") != null ?
                                            trackpoint.Element(gpx + "time").Value : null,

                                          Course = trackpoint.Element(gpx + "course") != null ?
                                            trackpoint.Element(gpx + "course").Value : null,

                                          Speed = trackpoint.Element(gpx + "speed") != null ?
                                             trackpoint.Element(gpx + "speed").Value : null
                                      }
                                    )
                             };

                // StringBuilder sb = new StringBuilder();
                int count = tracks.Count();
                GPXTrackList = new List<GPXTrack>();
                foreach (var trk in tracks)
                {
                    GPXTrack gPXTrack = new GPXTrack(trk.Name);
                    int c = trk.Segs.Count();
                    // Populate track data objects. 
                    foreach (var trkSeg in trk.Segs)
                    {
                        double Latitude = string.IsNullOrEmpty(trkSeg.Latitude) ? 0 : double.Parse(trkSeg.Latitude);
                        double Longitude = string.IsNullOrEmpty(trkSeg.Longitude) ? 0 : double.Parse(trkSeg.Longitude);
                        double Elevation = string.IsNullOrEmpty(trkSeg.Elevation) ? 0 : double.Parse(trkSeg.Elevation);
                        double Course = string.IsNullOrEmpty(trkSeg.Course) ? 0 : double.Parse(trkSeg.Course);
                        double Speed = string.IsNullOrEmpty(trkSeg.Speed) ? 0 : double.Parse(trkSeg.Speed);

                        gPXTrack.SegList.Add(new TrackSegment(Latitude, Longitude, Elevation, trkSeg.Time, Course, Speed));
                    }
                    GPXTrackList.Add(gPXTrack);
                }
                return GPXTrackList;
            }

            return null;
           
        }
    } 

}
