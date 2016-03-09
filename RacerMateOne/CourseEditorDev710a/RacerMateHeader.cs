using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RacerMateOne.CourseEditorDev
{
    public class RacerMateHeader
    {
        public string CreatorExe { get; set; }
        public string Date { get; set; }
        public string Version { get; set; }
        public string Copyright { get; set; }
        public string Comment { get; set; }
        public string CompressType { get; set; }
        public RacerMateHeader(string CreatorExe, string Date, string Version, string Copyright, string Comment, string CompressType)
        {
            this.CreatorExe = CreatorExe;
            this.Date = Date;
            this.Version = Version;
            this.Copyright = Copyright;
            this.Comment = Comment;
            this.CompressType = CompressType;
        }
    }
}
