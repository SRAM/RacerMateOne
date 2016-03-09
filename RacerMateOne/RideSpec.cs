using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace RacerMateOne
{
    public class RideSpec
    {
        private List<SegmentSpec> pvSegmentList = new List<SegmentSpec>();

        public List<SegmentSpec> SegmentList
        {
            get { return pvSegmentList; }
            set { pvSegmentList = value; }
        }

        public RideSpec()
        {
            pvSegmentList.Clear();
        }
        public XElement GetAsXElement()
        {
            XElement retElement = new XElement("RideSpec");
            foreach (SegmentSpec aa in this.SegmentList)
            {
                XElement bb = aa.GetAsXElement();
                retElement.Add(bb);
            }
            return retElement;
        }

    }

}
public class SegmentSpec
{

    const int maxnum = 1000;
    const int minnum = 0;
    const int mingap = 50;
    private int _startfraction = minnum;
    private int _endfraction = maxnum;

    public int StartFraction
    {
        get
        {
            return _startfraction;
        }
        set
        {
            _startfraction = Math.Max(value, minnum); //ensure never <0
            _startfraction = Math.Min(_startfraction, maxnum);
            //TODO: more bound checking here is required for end-points of the workout and enforce a minmum segment size       
        }
    }
    public int EndFraction
    {
        get
        {
            return _endfraction;
        }
        set
        {
            _endfraction = Math.Max(value, minnum); //ensure never <0
            _endfraction = Math.Min(_endfraction, maxnum);
            //TODO: more bound checking here is required for end-points of the workout and enforce a minmum segment size
        }
    }

    public SegmentSpec()
    {
    }
    public XElement GetAsXElement()
    {
        XElement retElement = new XElement("Segment", new XElement("StartFraction", this.StartFraction), new XElement("EndFraction", this.EndFraction));
        return retElement;
    }
}


