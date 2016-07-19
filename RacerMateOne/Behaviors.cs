using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Input;
using System.ComponentModel;

namespace RacerMateOne
{

    #region Documentation Tags
    /// <summary>
    ///     WPF Maskable TextBox class. Just specify the TextBoxMaskBehavior.Mask attached property to a TextBox. 
    ///     It protect your TextBox from unwanted non numeric symbols and make it easy to modify your numbers.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Class Information:
    ///	    <list type="bullet">
    ///         <item name="authors">Authors: Ruben Hakopian</item>
    ///         <item name="date">February 2009</item>
    ///         <item name="originalURL">http://www.rubenhak.com/?p=8</item>
    ///     </list>
    /// </para>
    /// </remarks>
    #endregion
    public class TextBoxMaskBehavior
    {
        #region MinimumValue Property

        public static double GetMinimumValue(DependencyObject obj)
        {
            return (double)obj.GetValue(MinimumValueProperty);
        }

        public static void SetMinimumValue(DependencyObject obj, double value)
        {
            obj.SetValue(MinimumValueProperty, value);
        }

        public static readonly DependencyProperty MinimumValueProperty =
            DependencyProperty.RegisterAttached(
                "MinimumValue",
                typeof(double),
                typeof(TextBoxMaskBehavior),
                new FrameworkPropertyMetadata(double.NaN, MinimumValueChangedCallback)
                );

        private static void MinimumValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox _this = (d as TextBox);
            ValidateTextBox(_this);
        }
        #endregion

        #region MaximumValue Property

        public static double GetMaximumValue(DependencyObject obj)
        {
            return (double)obj.GetValue(MaximumValueProperty);
        }

        public static void SetMaximumValue(DependencyObject obj, double value)
        {
            obj.SetValue(MaximumValueProperty, value);
        }

        public static readonly DependencyProperty MaximumValueProperty =
            DependencyProperty.RegisterAttached(
                "MaximumValue",
                typeof(double),
                typeof(TextBoxMaskBehavior),
                new FrameworkPropertyMetadata(double.NaN, MaximumValueChangedCallback)
                );

        private static void MaximumValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox _this = (d as TextBox);
            ValidateTextBox(_this);
        }
        #endregion

        #region Mask Property

        public static MaskType GetMask(DependencyObject obj)
        {
            return (MaskType)obj.GetValue(MaskProperty);
        }

        public static void SetMask(DependencyObject obj, MaskType value)
        {
            obj.SetValue(MaskProperty, value);
        }

        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.RegisterAttached(
                "Mask",
                typeof(MaskType),
                typeof(TextBoxMaskBehavior),
                new FrameworkPropertyMetadata(MaskChangedCallback)
                );

        private static void MaskChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is TextBox)
            {
                (e.OldValue as TextBox).PreviewTextInput -= TextBox_PreviewTextInput;
                DataObject.RemovePastingHandler((e.OldValue as TextBox), (DataObjectPastingEventHandler)TextBoxPastingEventHandler);
            }

            TextBox _this = (d as TextBox);
            if (_this == null)
                return;

            if ((MaskType)e.NewValue != MaskType.Any)
            {
                _this.PreviewTextInput += TextBox_PreviewTextInput;
                DataObject.AddPastingHandler(_this, (DataObjectPastingEventHandler)TextBoxPastingEventHandler);
            }

            ValidateTextBox(_this);
        }

        #endregion

        #region Private Static Methods

        private static void ValidateTextBox(TextBox _this)
        {
            if (GetMask(_this) != MaskType.Any)
            {
                _this.Text = ValidateValue(GetMask(_this), _this.Text, GetMinimumValue(_this), GetMaximumValue(_this));
            }
        }

        private static void TextBoxPastingEventHandler(object sender, DataObjectPastingEventArgs e)
        {
            TextBox _this = (sender as TextBox);
            string clipboard = e.DataObject.GetData(typeof(string)) as string;
            clipboard = ValidateValue(GetMask(_this), clipboard, GetMinimumValue(_this), GetMaximumValue(_this));
            if (!string.IsNullOrEmpty(clipboard))
            {
                _this.Text = clipboard;
            }
            e.CancelCommand();
            e.Handled = true;
        }

        private static void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox _this = (sender as TextBox);
            bool isValid = IsSymbolValid(GetMask(_this), e.Text);
            e.Handled = !isValid;
            if (isValid)
            {
                int caret = _this.CaretIndex;
                string text = _this.Text;
                bool textInserted = false;
                int selectionLength = 0;

                if (_this.SelectionLength > 0)
                {
                    text = text.Substring(0, _this.SelectionStart) +
                            text.Substring(_this.SelectionStart + _this.SelectionLength);
                    caret = _this.SelectionStart;
                }

                if (e.Text == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                {
                    while (true)
                    {
                        int ind = text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
                        if (ind == -1)
                            break;

                        text = text.Substring(0, ind) + text.Substring(ind + 1);
                        if (caret > ind)
                            caret--;
                    }

                    if (caret == 0)
                    {
                        text = "0" + text;
                        caret++;
                    }
                    else
                    {
                        if (caret == 1 && string.Empty + text[0] == NumberFormatInfo.CurrentInfo.NegativeSign)
                        {
                            text =  NumberFormatInfo.CurrentInfo.NegativeSign + "0" + text.Substring(1);
                            caret++;
                        }
                    }

                    if (caret == text.Length)
                    {
                        selectionLength = 1;
                        textInserted = true;
                        text = text + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0";
                        caret++;
                    }
                }
                else if (e.Text == NumberFormatInfo.CurrentInfo.NegativeSign)
                {
                    textInserted = true;
                    if (_this.Text.Contains(NumberFormatInfo.CurrentInfo.NegativeSign))
                    {
                        text = text.Replace(NumberFormatInfo.CurrentInfo.NegativeSign, string.Empty);
                        if (caret != 0)
                            caret--;
                    }
                    else
                    {
                        text = NumberFormatInfo.CurrentInfo.NegativeSign + _this.Text;
                        caret++;
                    }
                }

                if (!textInserted)
                {
                    text = text.Substring(0, caret) + e.Text +
                        ((caret < _this.Text.Length) ? text.Substring(caret) : string.Empty);

                    caret++;
                }

				if (text == "-0")
					text = "-";

                try
                {
                    double val = Convert.ToDouble(text);
                    double newVal = ValidateLimits(GetMinimumValue(_this), GetMaximumValue(_this), val);
                    if (val != newVal)
                    {
                        text = newVal.ToString();
                    }
                    else if (val == 0)
                    {
                        if (!text.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                            text = "0";
                    }
                }
                catch
                {
					if (text != "-") // Just let it through for now.
				        text = "0";
                }

                while (text.Length > 1 && text[0] == '0' && string.Empty + text[1] != NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                {
                    text = text.Substring(1);
                    if (caret > 0)
                        caret--;
                }

                while (text.Length > 2 && string.Empty + text[0] == NumberFormatInfo.CurrentInfo.NegativeSign && text[1] == '0' && string.Empty + text[2] != NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                {
                    text = NumberFormatInfo.CurrentInfo.NegativeSign + text.Substring(2);
                    if (caret > 1)
                        caret--;
                }

                if (caret > text.Length)
                    caret = text.Length;

                _this.Text = text;
                _this.CaretIndex = caret;
                _this.SelectionStart = caret;
                _this.SelectionLength = selectionLength;
                e.Handled = true;
            }
        }

        private static string ValidateValue(MaskType mask, string value, double min, double max)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            value = value.Trim();
            switch (mask)
            {
                case MaskType.Integer:
                    try
                    {
                        Convert.ToInt64(value);
                        return value;
                    }
                    catch
                    {
                    }
                    return string.Empty;

                case MaskType.Decimal:
                    try
                    {
                        Convert.ToDouble(value);

                        return value;
                    }
                    catch
                    {
                    }
                    return string.Empty;

                
            }

            return value;
        }

        private static double ValidateLimits(double min, double max, double value)
        {
            if (!min.Equals(double.NaN))
            {
                if (value < min)
                    return min;
            }

            if (!max.Equals(double.NaN))
            {
                if (value > max)
                    return max;
            }

            return value;
        }

        private static bool IsSymbolValid(MaskType mask, string str)
        {
            switch (mask)
            {
                case MaskType.Any:
                    return true;

                case MaskType.Integer:
                    if (str == NumberFormatInfo.CurrentInfo.NegativeSign)
                        return true;
                    break;

                case MaskType.Decimal:
                    if (str == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator ||
                        str == NumberFormatInfo.CurrentInfo.NegativeSign)
                        return true;
                    break;

                
            }

            if (mask.Equals(MaskType.Integer) || mask.Equals(MaskType.Decimal))
            {
                foreach (char ch in str)
                {
                    if (!Char.IsDigit(ch))
                        return false;
                }

                return true;
            }

            return false;
        }

        #endregion
    }

    public enum MaskType
    {
        Any,
        Integer,
        Decimal
    }

	public static class SafeDate
	{
		public static readonly CultureInfo Std = CultureInfo.InvariantCulture;
		public static readonly List<CultureInfo> TryCultures = new List<CultureInfo>();

		static SafeDate()
		{
			CultureInfo c;
			TryCultures.Add( CultureInfo.InvariantCulture );
			c = CultureInfo.CurrentCulture; if (!TryCultures.Contains( c )) TryCultures.Add( c );
			try { c = CultureInfo.CreateSpecificCulture("en-US"); if (!TryCultures.Contains(c)) TryCultures.Add(c); }
			catch { }
			try { c = CultureInfo.CreateSpecificCulture("fr-FR"); if (!TryCultures.Contains(c)) TryCultures.Add(c); }
			catch { }
			try { c = CultureInfo.CreateSpecificCulture("ja-JP"); if (!TryCultures.Contains(c)) TryCultures.Add(c); }
			catch { }
		}
		public static String StdString(DateTime dt)
		{
			return dt.ToString("G", Std);
		}
		public static bool TryParse(String str, out DateTime datetime )
		{
			foreach (CultureInfo c in TryCultures)
			{
				if (DateTime.TryParse(str, c,DateTimeStyles.None, out datetime))
					return true;
			}
			datetime = new DateTime();
			return false;
		}
		public static DateTime Parse(String str)
		{
			DateTime dt;
			if (!TryParse(str, out dt))
				throw new Exception(String.Format("Cannot parse this string to a date: \"{0}\"", str));
			return dt;
		}
	}



	public class FaderFrame : Frame
	{
		#region FadeDuration

		public static readonly DependencyProperty FadeDurationProperty =
			DependencyProperty.Register("FadeDuration", typeof(Duration), typeof(FaderFrame),
				new FrameworkPropertyMetadata(new Duration(TimeSpan.FromMilliseconds(100))));

		/// <summary>
		/// FadeDuration will be used as the duration for Fade Out and Fade In animations
		/// </summary>
		public Duration FadeDuration
		{
			get { return (Duration)GetValue(FadeDurationProperty); }
			set { SetValue(FadeDurationProperty, value); }
		}

		#endregion

		public FaderFrame()
			: base()
		{
			// watch for navigations
			Navigating+= OnNavigating;
			LoadCompleted += OnLoadCompleted;
            
		}

		public override void OnApplyTemplate()
		{
			// get a reference to the frame's content presenter
			// this is the element we will fade in and out
            _contentPresenter = GetTemplateChild("PART_FrameCP") as ContentPresenter;

			DependencyPropertyDescriptor desc = DependencyPropertyDescriptor.FromProperty(FrameworkElement.OpacityProperty, 
													typeof(FrameworkElement));
			desc.AddValueChanged(_contentPresenter,new EventHandler(OpacityChanged) );

			base.OnApplyTemplate();
		}


		void OpacityChanged(object sender, EventArgs args)
		{
          //Debug.WriteLine("opacity changed");
          if (AppWin.Instance.MainRender3D.IsVisible)
          {
          //    Debug.WriteLine(" Opacity on MainRender3d.isvisible " + _contentPresenter.Opacity);
              AppWin.Instance.MainRender3D.Opacity = _contentPresenter.Opacity;
          }
		}



		protected void OnLoadCompleted(object sender, NavigationEventArgs e)
		{
			if (_first && Content != null && Content is Page)
			{
				Page cpage = (Page)Content;
				if (cpage.Background is SolidColorBrush)
				{
					Background = new SolidColorBrush(((SolidColorBrush)(cpage.Background)).Color);
				}
				_first = false;
			}
		}
        private bool _first = true;
        private bool _allowDirectNavigation = false;
        private ContentPresenter _contentPresenter = null;
        private NavigatingCancelEventArgs _navArgs = null;

        
		protected void OnNavigating(object sender, NavigatingCancelEventArgs e)
		{
			// if we did not internally initiate the navigation:
			//   1. cancel the navigation,
			//   2. cache the target,
			//   3. disable hittesting during the fade, and
			//   4. fade out the current content
          //  Debug.WriteLine("onNavigation triggered to " + e.Content);
			if (Content != null && !_allowDirectNavigation && _contentPresenter != null)
			{
                e.Cancel = true;
                _navArgs = e;
				_contentPresenter.IsHitTestVisible = false;
                AnimationTimeline da = new DoubleAnimation(0.0d, FadeDuration);
                da.Completed += FadeOutCompleted;
                //Debug.WriteLine("added the fade out completed event" + _navArgs.Content);
                //da.BeginTime = TimeSpan.FromSeconds(1.0);
                HandoffBehavior asd = HandoffBehavior.SnapshotAndReplace;
              //  Debug.WriteLine("Arming the fade-out animation");
                _contentPresenter.BeginAnimation(OpacityProperty, da, asd);
                _contentPresenter.InvalidateVisual();
              
			}
			_allowDirectNavigation = false;
            //FaderFrame  aaa = (FaderFrame)sender;
           // Debug.WriteLine("Fader OnNavigating done" );
           
            
		}


		private void FadeOutCompleted(object sender, EventArgs e)
		{
			// after the fade out
			//   1. re-enable hittesting
			//   2. initiate the delayed navigation
			//   3. invoke the FadeIn animation at Loaded priority
           // Debug.WriteLine("fade-out completed has fired");
            (sender as AnimationClock).Completed -= FadeOutCompleted;
			if (_contentPresenter != null)
			{
				_contentPresenter.IsHitTestVisible = true;

				_allowDirectNavigation = true;
            //    Debug.WriteLine("Going to Navigate with _allow=" + _allowDirectNavigation);
				switch (_navArgs.NavigationMode)
				{
					case NavigationMode.New:
						if (_navArgs.Uri == null)
						{
							NavigationService.Navigate(_navArgs.Content);
						}
						else
						{
							NavigationService.Navigate(_navArgs.Uri);
						}
						break;

					case NavigationMode.Back:
						if (NavigationService.CanGoBack)
						{
							NavigationService.GoBack();
						}
						break;

					case NavigationMode.Forward:
						if (NavigationService.CanGoForward)
						{
							NavigationService.GoForward();
						}
						break;

					case NavigationMode.Refresh:
						NavigationService.Refresh();
						break;
				}
                //Debug.WriteLine("Done the Navigation switch " + _allowDirectNavigation + " Launching thread for animations" );
                //Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                //    (ThreadStart)delegate()
                //    {
                       // Debug.WriteLine("Started arming for color/fadein");

						Page cpage = (Page)_navArgs.Content;
						if (cpage.Background is SolidColorBrush)
						{
							SolidColorBrush brush = (SolidColorBrush)cpage.Background;
							Color color = brush.Color;
                          //  Debug.WriteLine("Background is solidcolo and colr is : " + color.A + " " + color.R + " " + color.G + " " + color.B + " ");
                            Color curcolor = ((SolidColorBrush)Background).Color;
                            if (color != ((SolidColorBrush)Background).Color)
							{
                               
                               Debug.WriteLine("that's a background change from " + curcolor.A + " " + curcolor.R + " " + curcolor.G + " " + curcolor.B + " ");
                                SolidColorBrush bkbrush = new SolidColorBrush();
								bkbrush.Color = ((SolidColorBrush)Background).Color;

								Duration d = new Duration(TimeSpan.FromSeconds(FadeDuration.TimeSpan.TotalSeconds / 2));
                                Debug.WriteLine("Arming the color change");
                                ColorAnimation coloranim = new ColorAnimation(color, d);
								coloranim.AccelerationRatio = 1.0d;

								coloranim.Completed += ColorChangeCompleted;
								bkbrush.BeginAnimation(SolidColorBrush.ColorProperty, coloranim);
                                _contentPresenter.InvalidateVisual(); //be sure there is a rescan
								Background = bkbrush;
								return;
							}
						}
                        Debug.WriteLine("arming the Fade-in ");
						DoubleAnimation da = new DoubleAnimation(1.0d, FadeDuration);
						da.AccelerationRatio = 1.0d;
						da.Completed += AnimationDone;
						_contentPresenter.BeginAnimation(OpacityProperty, da);
                        _contentPresenter.InvalidateVisual();
                    //});
			}
		}
       
        private void AnimationDone(object sender, EventArgs e)
		{
            (sender as AnimationClock).Completed -= AnimationDone;
            //Debug.WriteLine("Fader fade-in Animation done");
		}


		private void ColorChangeCompleted(object sender, EventArgs e)
		{
            Debug.WriteLine("Behaviors.cs, Colorchange triggered");
            (sender as AnimationClock).Completed -= ColorChangeCompleted;
            DoubleAnimation da = new DoubleAnimation(1.0d, FadeDuration);
			da.AccelerationRatio = 1.0d;
            da.Completed += AnimationDone;
				Debug.WriteLine("Arming the fade - in after the color changer");
			_contentPresenter.BeginAnimation(OpacityProperty, da);
            _contentPresenter.InvalidateVisual();
		}
       
    }


	public class BoolToOppositeBoolConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			if (targetType != typeof(bool))
				throw new InvalidOperationException("The target must be a boolean");

			return !(bool)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		#endregion
	}

	/// <summary>
	/// Drag drop helper class.
	/// </summary>
	public class DragDropHandler
	{
		private FrameworkElement dragElement = null;
		private bool dragging = false;
		private bool inDragDrop = false;
		private Point dragStart;
		private DataObject dataObject = null;

		public DragDropHandler(FrameworkElement dragElement, DataObject dataObject)
		{
			this.dragElement = dragElement;
			this.dataObject = dataObject;

			dragElement.MouseLeftButtonDown += new MouseButtonEventHandler(dragElement_MouseLeftButtonDown);
			dragElement.MouseMove += new MouseEventHandler(dragElement_MouseMove);
		}

		void dragElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!dragElement.IsMouseCaptured)
			{
				dragging = true;
				dragStart = e.GetPosition(dragElement);
			}
		}

		void dragElement_MouseMove(object sender, MouseEventArgs e)
		{
			if (dragging && !inDragDrop)
			{
				Point currentPos = e.GetPosition(dragElement);

				if ((Math.Abs(currentPos.X - dragStart.X) > 5) || (Math.Abs(currentPos.Y - dragStart.Y) > 5))
				{
					dragElement.CaptureMouse();

					inDragDrop = true;
					DragDropEffects de = DragDrop.DoDragDrop(dragElement, dataObject, DragDropEffects.Move);
					inDragDrop = false;
					dragging = false;
					dragElement.ReleaseMouseCapture();
				}
			}
		}
	}

	public static class UIChildFinder
	{
		public static DependencyObject FindChild(this DependencyObject reference, string childName, Type childType)
		{
			DependencyObject foundChild = null;
			if (reference != null)
			{
				int childrenCount = VisualTreeHelper.GetChildrenCount(reference);
				for (int i = 0; i < childrenCount; i++)
				{
					var child = VisualTreeHelper.GetChild(reference, i);
					// If the child is not of the request child type child
					if (child.GetType() != childType)
					{
						// recursively drill down the tree
						foundChild = FindChild(child, childName, childType);
					}
					else if (!string.IsNullOrEmpty(childName))
					{
						var frameworkElement = child as FrameworkElement;
						// If the child's name is set for search
						if (frameworkElement != null && frameworkElement.Name == childName)
						{
							// if the child's name is of the request name
							foundChild = child;
							break;
						}
					}
					else
					{
						// child element found.
						foundChild = child;
						break;
					}
				}
			}
			return foundChild;
		}
	}

	public class RadioBoolToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (value.ToString() == parameter.ToString());
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return parameter;
		}
	}

	[ValueConversion(typeof(bool), typeof(double))]
	public class EnableOpacityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToBoolean(value) ? 1.0 : 0.5;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new Exception("The method or operation of metric label to setting is not implemented.");
		}
	}


	public class MetricWeightLabelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			//targetType= typeof(string);
			return RM1_Settings.General.Metric ? "kg":"lbs";
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new Exception("The method or operation of metric label to setting is not implemented.");
		}
	}

	[ValueConversion(typeof(float), typeof(string))]
	public class MetricWeightValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			//targetType= typeof(string);
			float aa = 0;
			if (!RM1_Settings.General.Metric)
			{
				aa = (float)((System.Convert.ToDouble(value)));
				return aa.ToString("####.0");
			}
			else
			{
				aa = (float)((System.Convert.ToDouble(value)) * 0.45359237);
				return (aa.ToString("####.0"));
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			float aa = 0;
			if (!RM1_Settings.General.Metric)
			{
				try { aa = (float)((System.Convert.ToDouble(value))); } catch {}
				return aa;
			}
			else
			{
				try { aa = (float)((System.Convert.ToDouble(value)) / 0.45359237); } catch { }
				return aa;
			}
		}
	}

	[ValueConversion(typeof(double), typeof(string))]
	public class TenthsConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value == null ? null : String.Format("{0:F1}", value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(double), typeof(string))]
	public class DpConverter : IValueConverter
	{
		String m_format;
		public DpConverter(int dp)
		{
			m_format = "{0:F" + dp + "}";
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value == null ? null : String.Format(m_format, value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}


	public static class ConvertConst
	{
		public const double LBStoKGS = 0.45359237;
		public const double KGStoLBS = 2.204622621849;

		public const double MetersToMiles = 0.000621371192;
		public const double MilesToMeters = 1609.344000000000;

		public const double MillimetersToInches = 0.039370078740;
		public const double InchesToMilimeters = 25.400000000000;

		public const double InchesToMeters = 0.025400000000;

		public const double KilometersToMeters = 1000.000000000000;

		public const double KilometersToMiles = 0.621371192237;
		public const double MilesToKilometers = 1.609344000000;

		public const double MPHToMetersPerSecond = 0.447040000000;
		public const double KPHToMetersPerSecond = 0.277777777778;

		public const double MetersPerSecondToMPH = 2.236936292054;
		public const double MetersPerSecondToKPH = 3.600000000000;

		public const double MetersToKilometers = 0.001000000000;

		public const double MetersToFeet = 3.280839895013;
		public const double FeetToMeters = 0.304800000000;

		public const double HundredNanosecondToSecond = 0.000000100000;
		public const double SecondToHundredNanosecond = 10000000.0;

		public const double HundredNanosecondToMilliSecond = 0.000100000;
		public const double MilliSecondToHundredNanosecond = 10000.0;

		public static double MilesOrKilometersToMeters { get { return RM1_Settings.General.Metric ? KilometersToMeters:MilesToMeters; } }
        public static double MilesOrKilometersToKilometers { get { return RM1_Settings.General.Metric ? 1.0 : MilesToKilometers; } }
        
        public static double KilometersToMilesOrKilometers { get { return RM1_Settings.General.Metric ? 1.0 : KilometersToMiles; } }

		public static double MetersToMilesOrKilometers { get { return RM1_Settings.General.Metric ? MetersToKilometers:MetersToMiles; } }
		public static double MetersPerSecondToMPHOrKPH { get { return RM1_Settings.General.Metric ? MetersPerSecondToKPH : MetersPerSecondToMPH; } }
		public static double MPHOrKPHToMetersPerSecond { get { return RM1_Settings.General.Metric ? KPHToMetersPerSecond : MPHToMetersPerSecond; } }
		public static double MetersToMetersOrFeet { get { return RM1_Settings.General.Metric ? 1 : MetersToFeet; } }

		public static double FeetOrMetersToMeters { get { return RM1_Settings.General.Metric ? 1 : FeetToMeters; } }

		/// <summary>
		/// Standart .1 Miles or meters
		/// "3.1 Miles" for example.
		/// </summary>
		/// <param name="meters"></param>
		/// <returns></returns>
		public static String TextDistance(double meters)
		{
			return RM1_Settings.General.Metric ?
				String.Format("{0:F1} km", meters * MetersToKilometers) :
				String.Format("{0:F1} miles", meters * MetersToMiles);
		}

		public static String DisplayDistancekkk(double meters)
		{
			//return String.Format("{0:F1}", meters * (RM1_Settings.General.Metric ? MetersToKilometers:MetersToMiles));
            return String.Format("{0:F1}", meters * MetersToKilometers);
        }


		/// <summary>
		/// Translates meters per second to either mph or kph
		/// "3.1 mph" for example.
		/// </summary>
		/// <param name="meters_per_second"></param>
		/// <returns></returns>
		public static String TextSpeed( double meters_per_second )
		{
			return RM1_Settings.General.Metric ?
				String.Format("{0:F1} kph", meters_per_second * MetersPerSecondToKPH):
				String.Format("{0:F1} mph", meters_per_second * MetersPerSecondToMPH, "miles");
		}
		public static String TextMPHorKPH { get { return RM1_Settings.General.Metric ? "kph" : "mph"; } }
		public static String TextMPHorKPH_C { get { return RM1_Settings.General.Metric ? "KPH" : "MPH"; } }
		public static String TextHeight(double meters)
		{
			return RM1_Settings.General.Metric ?
				String.Format("{0:F1} m", meters) :
				String.Format("{0:F1} ft", meters * MetersToFeet);
		}

		public static double RoundToKPHorMPH(double kph_or_mph)
		{
			double v;
			v = kph_or_mph * (RM1_Settings.General.Metric ? ConvertConst.MetersPerSecondToKPH : ConvertConst.MetersPerSecondToMPH);
			v = Math.Round(v);
			v = v * (RM1_Settings.General.Metric ? ConvertConst.KPHToMetersPerSecond : ConvertConst.MPHToMetersPerSecond);
			return v;
		}
		public static String TextDistanceLabel
		{
			get { return RM1_Settings.General.Metric ? "Km" : "Miles"; }
		}

		const double UseAsFeet = ConvertConst.FeetToMeters * 1056;
		public static String MetersToDistanceString(double dist)
		{
			return RM1_Settings.General.Metric ?
				dist < 1000.0 ? String.Format("{0:F0}m", dist) : String.Format("{0:F2} km", dist * ConvertConst.MetersToKilometers) :
					dist < UseAsFeet ? String.Format("{0:F0}'", dist * ConvertConst.MetersToFeet) :
										  String.Format("{0:F2} miles", dist * ConvertConst.MetersToMiles);
		}
        public static String MetersToDistanceStringWithoutUnits(double dist)
        {
            return RM1_Settings.General.Metric ?
                dist < 1000.0 ? String.Format("{0:F0}m", dist) : String.Format("{0:F1}", dist * ConvertConst.MetersToKilometers) :
                    dist < UseAsFeet ? String.Format("{0:F0}'", dist * ConvertConst.MetersToFeet) :
                                          String.Format("{0:F1}", dist * ConvertConst.MetersToMiles);
        }
		public const double mps_mph_2 = ConvertConst.MPHToMetersPerSecond * 2;
		public const double mps_mph_5 = ConvertConst.MPHToMetersPerSecond * 5;
		public const double mps_mph_10 = ConvertConst.MPHToMetersPerSecond * 10;
		public const double mps_mph_15 = ConvertConst.MPHToMetersPerSecond * 15;
		public const double mps_mph_20 = ConvertConst.MPHToMetersPerSecond * 20;

		public const double RadiansToDegrees = 57.295779513082;
		public const double DegreesToRadians = 0.017453292520;
	}

}
