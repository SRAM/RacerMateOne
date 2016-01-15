using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace RacerMateOne
{
	public class Cmd_ToggleLog : ICommand
	{
		public event EventHandler CanExecuteChanged;
		public void Execute(object parameter)
		{
			if (Window_Log.Instance.IsVisible)
				Window_Log.Instance.Hide();
			else
				Window_Log.Instance.Show();
		}

		public bool CanExecute(object parameter)
		{
			if (CanExecuteChanged == null)
				return true;
			return true;
		}
	}
	public class Cmd_OnKey : ICommand
	{
		public event EventHandler CanExecuteChanged;
		public void Execute(object parameter)
		{
			AppWin.Instance.DirectKey(parameter.ToString()[0]);
		}

		public bool CanExecute(object parameter)
		{
			if (CanExecuteChanged == null)
				return true;
			return true;
		}
	}
}
