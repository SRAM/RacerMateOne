using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace RacerMateOne.Controls
{
    public partial class ColorPickerControlView : UserControl
    {
		[Flags]
		public enum Sections : int
		{
			Zero = 0,
			Theme =  1 << 0,
			Standard = 1 << 1,
			Skin = 1 << 2,
			More = 1 << 3
		}
		//============================================
		public static DependencyProperty SectionFlagsProperty = DependencyProperty.Register("SectionFlags", typeof(Sections), typeof(ColorPickerControlView),
			new FrameworkPropertyMetadata(Sections.Theme | Sections.Standard | Sections.More, new PropertyChangedCallback(_SectionFlagsChanged)));
		public Sections SectionFlags
		{
			get { return (Sections)GetValue(SectionFlagsProperty); }
			set { SetValue(SectionFlagsProperty, value); }
		}
		private static void _SectionFlagsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((ColorPickerControlView)d).OnSectionFlagsChanged();
		}
		private void OnSectionFlagsChanged()
		{
			Sections s = SectionFlags;
			S_Theme.Visibility = (s & Sections.Theme) != Sections.Zero ? Visibility.Visible : Visibility.Collapsed;
			S_Standard.Visibility = (s & Sections.Standard) != Sections.Zero ? Visibility.Visible : Visibility.Collapsed;
			S_Skin.Visibility = (s & Sections.Skin) != Sections.Zero ? Visibility.Visible : Visibility.Collapsed;
			S_More.Visibility = (s & Sections.More) != Sections.Zero ? Visibility.Visible : Visibility.Collapsed;
		}


		//============================================
		public static readonly RoutedEvent CurrentColorChangedEvent =
			EventManager.RegisterRoutedEvent(
			"CurrentColorChanged", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(ColorPickerControlView));

		public event RoutedEventHandler CurrentColorChanged
		{
			add { AddHandler(CurrentColorChangedEvent, value); }
			remove { RemoveHandler(CurrentColorChangedEvent, value); }
		}

		//============================================
		public static DependencyProperty CurrentColorProperty =
			DependencyProperty.Register("CurrentColor", typeof(SolidColorBrush), typeof(ColorPickerControlView),
			new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(_CurrentColorChanged)));
		public SolidColorBrush CurrentColor
        {
            get { return (SolidColorBrush)GetValue(CurrentColorProperty); }
            set { SetValue(CurrentColorProperty, value); }
        }
		private static void _CurrentColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((ColorPickerControlView)d).OnCurrentColorChanged();
		}
		private void OnCurrentColorChanged()
		{
			RoutedEventArgs args = new RoutedEventArgs(CurrentColorChangedEvent);
			RaiseEvent(args);
		}


        
        public static RoutedUICommand SelectColorCommand = new RoutedUICommand("SelectColorCommand","SelectColorCommand", typeof(ColorPickerControlView));
        private Window _advancedPickerWindow;

        public ColorPickerControlView()
        {
            DataContext = this;
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(SelectColorCommand, SelectColorCommandExecute));
			OnSectionFlagsChanged();
        }

        private void SelectColorCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            CurrentColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(e.Parameter.ToString()));
        }

        private static void ShowModal(Window advancedColorWindow)
        {
            //advancedColorWindow.Owner = Application.Current.MainWindow;
            advancedColorWindow.ShowDialog();
        }

        void AdvancedPickerPopUpKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                _advancedPickerWindow.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = false;
            e.Handled = false;
        }

        private void MoreColorsClicked(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = false;
            var advancedColorPickerDialog = new Dialogs.AdvancedColorPicker();
            _advancedPickerWindow = new Window
                                        {
                                            AllowsTransparency = true,
                                            Content = advancedColorPickerDialog,
                                            WindowStyle = WindowStyle.None,
                                            ShowInTaskbar = false,
                                            Background = new SolidColorBrush(Colors.Transparent),
                                            Padding = new Thickness(0),
                                            Margin = new Thickness(0),
                                            WindowState = WindowState.Normal,
                                            WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                            SizeToContent = SizeToContent.WidthAndHeight
                                        };
            _advancedPickerWindow.DragMove();
            _advancedPickerWindow.KeyDown += AdvancedPickerPopUpKeyDown;
            advancedColorPickerDialog.DialogResultEvent += AdvancedColorPickerDialogDialogResultEvent;
            advancedColorPickerDialog.Drag += AdvancedColorPickerDialogDrag;
            ShowModal(_advancedPickerWindow);
        }

        void AdvancedColorPickerDialogDrag(object sender, DragDeltaEventArgs e)
        {
            _advancedPickerWindow.DragMove();
        }

        void AdvancedColorPickerDialogDialogResultEvent(object sender, EventArgs e)
        {
            _advancedPickerWindow.Close();
            var dialogEventArgs = (Dialogs.DialogEventArgs)e;
            if (dialogEventArgs.DialogResult == Dialogs.DialogResult.Cancel)
                return;
            CurrentColor = dialogEventArgs.SelectedColor;
        }

		private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			EnableBorder.Visibility = IsEnabled ? Visibility.Collapsed:Visibility.Visible;
		}

    }
}