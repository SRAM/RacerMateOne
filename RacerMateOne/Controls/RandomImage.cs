using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.IO;

namespace RacerMateOne.Controls
{
	public class RandomImage: Image
	{
		//==============================================
		class Info
		{
			int m_Current = 0;
			List<String> m_Files = new List<string>();
			public Info(String template)
			{
				try
				{
					String basepath = (new Uri(System.IO.Path.GetDirectoryName(Assembly.GetAssembly(typeof(AppWin)).CodeBase))).LocalPath;
					String[] files = Directory.GetFiles(basepath + @"\Art\UI", template);
					List<String> t = new List<String>(files);
					// Randomize the list
					Random r = new Random();
					int index = 0;
					while (t.Count > 0)
					{
						index = r.Next(0, t.Count); //Choose a random object in the list
						m_Files.Add(t[index]); //add it to the new, random list
						t.RemoveAt(index); //remove to avoid duplicates
					}
				}
				catch { }
			}
			public String File 
			{ 
				get 
				{ 
					if (m_Files.Count == 0)
						return null;
					String ans = m_Files[m_Current++];
					if (m_Current >= m_Files.Count)
						m_Current = 0;
					return ans;
				}
			}
		}
		//==============================================
		public static DependencyProperty PatternProperty = DependencyProperty.Register("Pattern", typeof(String), typeof(RandomImage),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(_PatternChanged)));
		public String Pattern
		{
			get { return (String)this.GetValue(PatternProperty); }
			set { this.SetValue(PatternProperty, value); }
		}
		private static void _PatternChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RandomImage)d).OnPatternChanged();			
		}
		private static Dictionary<String, Info> ms_InfoMap = new Dictionary<string,Info>();
		private void OnPatternChanged()
		{
			String s = Pattern;
			if (s != null)
			{
				Info info;
				if (!ms_InfoMap.TryGetValue(s, out info))
				{
					info = new Info(s);
					ms_InfoMap[s] = info;
				}
				if (m_Info == info)
					return;
				m_Info = info;
				m_File = info.File;
			}
		}
		private Info m_Info;


		//==============================================

		public RandomImage()
		{
			Loaded += new RoutedEventHandler(RandomImage_Loaded);
		}

		bool m_bInit;
		String m_t_File;
		String m_File
		{
			get { return m_t_File; }
			set
			{
				if (m_t_File == value)
					return;
				m_t_File = value;
				UpdateImage();
			}
		}

		void UpdateImage()
		{
			if (!m_bInit)
				return;
			if (m_t_File != null && File.Exists(m_t_File))
			{
				try
				{
					Uri uri = new Uri(m_t_File);
					BitmapImage bmi = new BitmapImage(uri);
					Source = bmi;
				}
				catch
				{
					Source = null;
				}
			}
			else
				Source = null;
		}

		private void RandomImage_Loaded(object sender, RoutedEventArgs e)
		{
			m_bInit = true;
			UpdateImage();
		}
	}
}
