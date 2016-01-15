using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Windows;

namespace RacerMateOne
{
    public class GPXTrackSegment
    {
      public double Latitude {get; set;}
      public double Longitude { get; set; }
      public double Elevation { get; set; }
      public double Course { get; set; }
      public double Speed { get; set; }
      public string Time { get; set; }

      public GPXTrackSegment(double Latitude, double Longitude, double Elevation, string Time , double Course , double Speed )
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
        public List<GPXTrackSegment> SegList;
        public string TrackName
        {
            get;
            set;
        }
        public GPXTrack(string TrackName)
        {
            this.TrackName = TrackName;
            SegList = new List<GPXTrackSegment>();
        }
    }

    public class PolygonData
    {
        public double StartValue { get; set; }
        public double EndValue { get; set; }
        public static double MaxValue { get; set; }
        public static double MinValue { get; set; }

        public PolygonData(double startValue, double endValue)
        {
            this.StartValue = startValue;
            this.EndValue = endValue;
        }
    }

    public class EditorSegmentData
    {
        public int Segment {get; set;}
        public double Length { get; set; }
        public double Wind { get; set; }
        public double StartWatt { get; set; }
        public double EndWatt { get; set; }
        public double Time { get; set; }
        public double AccumTime { get; set; }
        public double AccumDist { get; set; }
        public double Grade { get; set; }
        public double StartWattPer { get; set; }
        public double EndWattPer { get; set; }
        public CourseFilter Filter { get; set; }
        public Guid SegID { get; set; }

        public PolygonData PolygonDataValue { get; set; }

        public EditorSegmentData(int Segment)
        {
            this.Segment= Segment;
        }

        public EditorSegmentData MakeCopy(EditorSegmentData data)
        {
            EditorSegmentData CopyEditorSegmentData = new EditorSegmentData(data.Segment);
            CopyEditorSegmentData.Filter = (CourseFilter) data.Filter.Clone();
            CopyEditorSegmentData.Length = data.Length;
            CopyEditorSegmentData.Wind = data.Wind;
            CopyEditorSegmentData.StartWatt = data.StartWatt;
            CopyEditorSegmentData.EndWatt = data.EndWatt;
            CopyEditorSegmentData.Time = data.Time;
            CopyEditorSegmentData.AccumDist = data.AccumDist;
            CopyEditorSegmentData.AccumTime = data.AccumTime;
            CopyEditorSegmentData.Grade = data.Grade;
            CopyEditorSegmentData.StartWattPer = data.StartWattPer;
            CopyEditorSegmentData.EndWattPer = data.EndWattPer;
       //     CopyEditorSegmentData.IndexID = data.IndexID;


            return CopyEditorSegmentData;
        }

        public EditorSegmentData(EditorSegmentData editorSegmentData)
        {
            this.Segment = editorSegmentData.Segment;
            this.Length = editorSegmentData.Length;
       //     this.XValues = new Point(editorSegmentData.XValues.X, editorSegmentData.XValues.Y);
       //     this.YValues = new Point(editorSegmentData.YValues.X, editorSegmentData.YValues.Y);
       //     this.Change = editorSegmentData.Change;
            this.Wind = editorSegmentData.Wind;
      //      this.AccumDis = editorSegmentData.AccumDis;
      //      this.Wattage = editorSegmentData.Wattage;
      //      this.Display = editorSegmentData.Display;
     //       this.IndexID = editorSegmentData.IndexID;
     //       this.Smoothing = editorSegmentData.Smoothing;
        }
    }

    public class GPXLoader
    {
        public const string GPXNAMESPACEONEZERO = "http://www.topografix.com/GPX/1/0";
        public const string GPXNAMESPACEONEONE = "http://www.topografix.com/GPX/1/1";
        public List<GPXTrack> GPXTrackList;

        public List<GPXTrack> LoadGPXTracksList(string sFile)
        {
            XDocument gpxDoc = XDocument.Load(sFile);
            XNamespace gpx = gpxDoc.Root.Name.Namespace;
            string strNameSpace = gpx.ToString();

            if (strNameSpace == GPXNAMESPACEONEONE)
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
                        double Course = 0.0;
                        double Speed = 0.0;

                        gPXTrack.SegList.Add(new GPXTrackSegment(Latitude, Longitude, Elevation, trkSeg.Time, Course, Speed));
                    }
                    GPXTrackList.Add(gPXTrack);
                }
                return GPXTrackList;
            }
            else if (strNameSpace == GPXNAMESPACEONEZERO)
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

                        gPXTrack.SegList.Add(new GPXTrackSegment(Latitude, Longitude, Elevation, trkSeg.Time, Course, Speed));
                    }
                    GPXTrackList.Add(gPXTrack);
                }
                return GPXTrackList;
            }

            return null;
           
        }

        // http://stackoverflow.com/questions/27928/how-do-i-calculate-distance-between-two-latitude-longitude-points
        public static double FindDistance(double LatitudeA, double LongitudeA, double LatitudeB, double LongitudeB)
        {
            int R = 6371; // Radius of the earth in km
            double dLat = deg2rad(Math.Abs(LatitudeB - LatitudeA));  // deg2rad below
            double dLon = deg2rad(Math.Abs(LongitudeB - LongitudeA));
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(deg2rad(LatitudeA)) * Math.Cos(deg2rad(LatitudeB)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c; // Distance in km
            return d;
        }

        public static double deg2rad(double deg)
        {
            return deg * (Math.PI / 180);
        }

        public static double FindGrade(double ElevationA, double ElevationB, double Length) //Length in km
        {
            double grade = ((ElevationB - ElevationA)/1000)/Length;
            return grade;
        }
    }
}
