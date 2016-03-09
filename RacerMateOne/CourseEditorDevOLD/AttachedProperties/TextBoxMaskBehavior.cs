using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace RacerMateOne.CourseEditorDev.AttachedProperties
{
    public class TextBoxMaskBehavior
    {
        public static readonly DependencyProperty MaskProperty = DependencyProperty.RegisterAttached("Mask", typeof(MaskType), typeof(TextBoxMaskBehavior), new FrameworkPropertyMetadata(MaskChangedCallback));

        public static MaskType GetMask(DependencyObject obj)
        {
            return (MaskType)obj.GetValue(MaskProperty);
        }

        public static void SetMask(DependencyObject obj, MaskType value)
        {
            obj.SetValue(MaskProperty, value);
        }


        private static void MaskChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is TextBox)
                (e.OldValue as TextBox).PreviewTextInput -= TextBox_PreviewTextInput;

            TextBox _this = (d as TextBox);
            if (_this == null)
                return;

            if (GetMask(_this) != MaskType.Any)
            {
                _this.PreviewTextInput += TextBox_PreviewTextInput;
            }

        }

        private static void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox _this = (sender as TextBox);
            if (_this.Text == string.Empty || _this.Text[0] == '0')
            {
                e.Handled = false;
                return;
            }

            string cur = _this.Text + e.Text;

            // ^\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$  // for curency 
            // @"^-?[0-9]*(?:\.[0-9]*)?$" // decimal with -ve numbers
            //Regex regex = new Regex("^[0-9]+$"); Numbers only

            MaskType mask = GetMask(_this);

            Regex regex = mask == MaskType.Decimal ? new Regex(@"^-?[0-9]*(?:\.[0-9]*)?$") : new Regex("^[0-9]+$");
            if (!regex.IsMatch(cur))
            {
                e.Handled = true;
            }
        }
    }

    public enum MaskType
    {
        Any,
        Integer,
        Decimal
    }
}