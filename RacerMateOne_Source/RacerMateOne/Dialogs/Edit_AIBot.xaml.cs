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
using System.Windows.Shapes;

namespace RacerMateOne.Dialogs
{
	/// <summary>
	/// Interaction logic for Edit_DelayBot.xaml
	/// </summary>
	public partial class Edit_AIBot : Window
	{
		public Edit_AIBot()
		{
			InitializeComponent();
		}
		bool m_bClosing;


		Dictionary<String,RadioButton> m_RB = new Dictionary<string,RadioButton>();

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			m_bClosing = true;
			Fade.Stop();
			FadeAnim.To = 0.0f;
			Fade.Begin();
		}

		private void FadeAnim_Completed(object sender, EventArgs e)
		{
			if (m_bClosing)
				Close();
		}

		private void Edit_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private void RadioButton_Checked(object sender, RoutedEventArgs e)
		{
			RadioButton rb = sender as RadioButton;
			String[] ss = ((String)rb.Tag).Split(',');
			if (m_Bot != null)
			{
				m_Bot.SetAttack(ss[0], ss[1]);
			}
		}


		AIBot m_Bot;
		public AIBot Bot
		{
			get { return m_Bot; }
			set
			{
				m_Bot = null;
				String t = value.Frequency + "," + value.Strength;
				foreach (UIElement e in Types.Children)
				{
					((RadioButton)e).IsChecked = String.Compare((String)((RadioButton)e).Tag, t, true) == 0;
				}
				m_Bot = value;
			}
		}
	


	}
}
