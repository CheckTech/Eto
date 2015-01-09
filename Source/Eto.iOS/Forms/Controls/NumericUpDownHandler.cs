using System;
using System.Reflection;
using SD = System.Drawing;
using Eto.Forms;
using UIKit;
using Eto.Drawing;

namespace Eto.iOS.Forms.Controls
{
	public class NumericUpDownHandler : IosControl<UITextField, NumericUpDown, NumericUpDown.ICallback>, NumericUpDown.IHandler
	{
		public NumericUpDownHandler()
		{
			Control = new UITextField();
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			return SizeF.Max(base.GetNaturalSize(availableSize), new SizeF(60, 0));
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.KeyboardType = UIKeyboardType.NumberPad;
			Control.ReturnKeyType = UIReturnKeyType.Done;
			Control.BorderStyle = UITextBorderStyle.RoundedRect;
			Control.ShouldReturn = (textField) =>
			{
				textField.ResignFirstResponder();
				return true;
			};
			Control.EditingChanged += (sender, e) => Callback.OnValueChanged(Widget, EventArgs.Empty);
		}

		public bool ReadOnly
		{
			get;
			set;
		}

		public double Value
		{
			get
			{
				double value;
				if (double.TryParse(Control.Text, out value))
					return value;
				return 0;
			}
			set
			{
				Control.Text = value.ToString();
			}
		}

		public double MinValue
		{
			get;
			set;
		}

		public double MaxValue
		{
			get;
			set;
		}

		public override Font Font
		{
			get { return base.Font; }
			set
			{
				base.Font = value;
				Control.Font = value.ToUI();
			}
		}

		public double Increment
		{
			get;
			set;
		}

		public int DecimalPlaces
		{
			get;
			set;
		}

		public Color TextColor
		{
			get { return Control.TextColor.ToEto(); }
			set { Control.TextColor = value.ToNSUI(); }
		}
	}
}
