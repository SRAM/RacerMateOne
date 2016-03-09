using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RacerMateOne.CourseEditorDev
{
    public enum CourseType
    {
        DISTANCEGRADE,
        TIMEGRADE,
        TIMEWATT,
        DISTANCEWATT,
        TIMEPERAT,
        DISTANCEPERAT
    }

    public enum Measurement
    {
        METRIC,
        ENGLISH
    }

    public enum ShowIcon
    {
        NONE,
        STOP,
        OK
    }
    
}
