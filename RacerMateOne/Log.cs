// Uncomment the line below to enable Logging
//#define LOG_ENABLED

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using System.Windows.Markup;
using System.Xml;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;
using System.Threading;
using System.Windows.Threading;

namespace RacerMateOne
{
    public static class Log
    {
        [Flags]
        public enum Flags
        {
            Zero = 0,
            FlowChart = (1 << 0),
            Debug = (1 << 1),

            Mask = ((1 << 2) - 1)
        }
        public class Entry
        {
            public int Num;
            public String Message;
            public DateTime TimeStamp;
            public Flags Flags;
            public String ThreadName;
            public Entry(String str, Flags flags)
            {
                Num = ms_Num++;
                Message = str;
                TimeStamp = DateTime.Now;
                Flags = flags;
                ThreadName = Thread.CurrentThread.Name;
            }
        };
        class ThreadObject : DispatcherObject
        {
            delegate void _Notify();
            DispatcherOperation m_DO;
            public void Notify()
            {
                lock (this)
                {
                    if (m_DO == null)
                        m_DO = Dispatcher.BeginInvoke(DispatcherPriority.Render, new _Notify(NotifyS));
                }
            }
            void NotifyS()
            {
                lock (this)
                {
                    m_DO = null;
                    updateMain();
                }
            }
        }
        static LinkedListNode<Entry> ms_LastSent;
        static ThreadObject ms_Lock = new ThreadObject();

        static int ms_MaxListSize = 100;
        static LinkedList<Entry> ms_List = new LinkedList<Entry>();
        static int ms_Num = 0;
        static Log()  {
#if LOG_ENABLED
#if DEBUG
            //System.Diagnostics.Debug.WriteLine("Log.cs constructor, Created Log");
            Log.WriteLine("Log.cs constructor, Created Log");
#endif

            WriteLine("Created");
#endif //LOG_ENABLED
        }

        static List<Entry> ms_TempList = new List<Entry>();
        static void updateMain()
        {
#if LOG_ENABLED
            lock (ms_Lock)
            {
                if (LogEvent != null)
                {
                    if (ms_LastSent == null)
                    {
                        ms_LastSent = ms_List.First;
                        ms_TempList.Add( ms_LastSent.Value );
                    }
                    if (ms_LastSent != null && ms_LastSent.Next != null)
                    {
                        do
                        {
                            ms_LastSent = ms_LastSent.Next;
                            ms_TempList.Add( ms_LastSent.Value );
                        } while( ms_LastSent.Next != null );
                    }
                }
                while (ms_List.Count > ms_MaxListSize)
                {
                    if (ms_List.First == ms_LastSent)
                        ms_LastSent = null;
                    ms_List.RemoveFirst();
                }
            }
            if (ms_TempList.Count > 0 && LogEvent != null)
            {
                foreach (Entry e in ms_TempList)
                {
                    LogEvent(e);
                }
                ms_TempList.Clear();
            }
#endif //LOG_ENABLED
        }

        public delegate void LogHandler(Entry e);
        public static event LogHandler LogEvent;

        public static void WriteLine(String str)
        {
#if LOG_ENABLED
            WriteLine(str, Flags.Zero);
#endif //LOG_ENABLED
        }
        public static void WriteLine(String str, bool flowchart)
        {
#if LOG_ENABLED
            WriteLine(str, flowchart ? Flags.FlowChart : Flags.Zero);
#endif //LOG_ENABLED
        }
        public static void WriteLine(String str, Flags flags)
        {
#if LOG_ENABLED
            Entry entry = new Entry(str, flags);
            lock (ms_Lock)
            {
                ms_List.AddLast(entry);
            }
            ms_Lock.Notify();
#endif //LOG_ENABLED
        }
        public static void Debug(String str)
        {
#if LOG_ENABLED
            WriteLine(str, Flags.Debug);
#endif //LOG_ENABLED
        }

        // Return any entry that has been sent..
        public static List<Entry> GetList(bool all)
        {
            List<Entry> list = new List<Entry>();
#if LOG_ENABLED
            lock (ms_Lock)
            {
                LinkedListNode<Entry> node;
                LinkedListNode<Entry> last = all ? null:ms_LastSent;
                if (all || ms_LastSent != null)
                {
                    for (node = ms_List.First; node != last; node = node.Next)
                    {
                        list.Add(node.Value);
                    }
                }
            }
#endif //LOG_ENABLED
            return list;
        }

    }

}
