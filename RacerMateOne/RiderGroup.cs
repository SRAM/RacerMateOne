using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Data;
using System.Xml.Linq;
using System.Windows;
using Microsoft.Win32;



namespace RacerMateOne
{
	public class RiderGroup: INotifyPropertyChanged, ICloneable
	{
		string m_Name = "";
		public string Name
		{
			get { return m_Name; }
			set
			{
				if (m_Name != value)
				{
					m_Name = value;
					OnPropertyChanged("Name");
				}
			}
		}
		public struct Node
		{
			public Rider Rider;
			public Bot Bot;
			public int Num;
		}
		public Node[] Nodes = new Node[8];

		string m_Key = "";
		public string Key
		{
			get { return m_Key; }
		}
		//===========================================
		public RiderGroup()
		{
			m_Key = Guid.NewGuid().ToString();
			ms_DB[m_Key] = this;
			Clear();
			GroupList.Add(this);
		}
		public RiderGroup(string key)
		{
			m_Key = key;
			ms_DB[key] = this;
			Clear();
			GroupList.Add(this);
		}

		//===========================================

		public void SaveFromUnits()
		{
			int cnt = 0;
			foreach (Unit unit in Unit.Units)
			{
				Nodes[cnt].Rider = unit.Rider;
				Nodes[cnt].Bot = unit.Bot;
				cnt++;
			}
		}
		public void LoadToUnits()
		{
			int cnt = 0;
			foreach (Unit unit in Unit.Units)
			{
				unit.Rider = Nodes[cnt].Rider;
				unit.Bot = Nodes[cnt].Bot;
				cnt++;
			}
		}

		public void Remove()
		{
			ms_DB.Remove(Key);
		}

		//================================================
		public object Clone()
		{
			RiderGroup ng = new RiderGroup();
			int i;
			for (i = 0; i < 8; i++)
				ng.Nodes[i] = Nodes[i];
			ng.Name = Name;
			return ng;
		}
		//================================================

		public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
		//================================================

		public override string ToString()
		{
			return m_Name;
		}

		public void Clear()
		{
			for (int i = 0; i < 8; i++)
			{
				Nodes[i].Bot = null;
				Nodes[i].Rider = null;
				Nodes[i].Num = i;
			}
		}

		//=================================================================
		public static Dictionary<String, RiderGroup> ms_DB = new Dictionary<string, RiderGroup>();
		public static ObservableCollection<RiderGroup> GroupList = new ObservableCollection<RiderGroup>();
		static RiderGroup m_Selected;
		public static RiderGroup Selected
		{
			get { return m_Selected; }
			set
			{
				if (value != null)
					m_Selected = value;
			}
		}

		public static void LoadDB(XDocument indoc)
		{
			XElement rootNode = indoc.Root;
			XElement dbnode = rootNode.Element("RiderGroups");
			if (dbnode != null)
				LoadDB(dbnode);
		}
		public static void LoadDB(XElement dbnode)
		{
			XAttribute att;
			IEnumerable<XElement> nodelist = dbnode.Elements("Group");
			string skey;
			skey = (att = dbnode.Attribute("Selected")) != null ? att.Value.ToString() : "";
			foreach (XElement ele in nodelist)
			{
				String key = ele.Attribute("GUID").Value.ToString();
	
				RiderGroup r;
				
				if (ms_DB.ContainsKey(key))
					r = ms_DB[key];
				else
					r = new RiderGroup(key);
				r.Clear();

				if ((att = ele.Attribute("Name")) != null) r.m_Name = att.Value;
				IEnumerable<XElement> rlist = dbnode.Elements("Rider");
				foreach (XElement relem in rlist)
				{
					XAttribute a;
					if ((a = relem.Attribute("Number")) != null)
					{
						int num = Convert.ToInt32(a.Value);
						if (num > 1 && num <= 8)
						{
							num--;
							if ((a = relem.Attribute("RiderKey")) != null)
								r.Nodes[num].Rider = Riders.FindRiderByKey(a.Value.ToString());
						}
					}
				}
				if (key == skey)
					Selected = r;
			}
			if (Selected != null)
				Selected.LoadToUnits();
		}

		public XElement ToXElement()
		{
			XElement node = new XElement("Group",
				new XAttribute("Name", Name),
				new XAttribute("GUID", Key));
			foreach( Node n in Nodes )
			{
				if (n.Rider != null) //  || n.Bot)
				{
					if (n.Rider != null)
					{
						node.Add( new XElement("Rider",
							new XAttribute("Number", n.Num+1 ),
							new XAttribute("RiderName", n.Rider.IDName ),
							new XAttribute("RiderKey", n.Rider.DatabaseKey)));
					}
				}
			}
			return node;
		}
						
		// Dump out the DB to an xlement.
		public static XElement SaveDB()
		{
			XElement db = new XElement("RiderGroups",
				new XAttribute("Current",m_Selected.Key));
			foreach (KeyValuePair<String,RiderGroup> k in ms_DB)
			{
				RiderGroup r = k.Value;
				db.Add(r.ToXElement());
			}
			return db;
		}

		static RiderGroup()
		{
			m_Selected = new RiderGroup("Default");
			m_Selected.Name = "Default";
		}
	}
}
