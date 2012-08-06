using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	public class TextBoxSection : Panel
	{
		public TextBoxSection ()
		{
			var layout = new DynamicLayout (this);

			layout.AddRow (new Label { Text = "Default" }, Default ());
			layout.AddRow (new Label { Text = "Different Size" }, DifferentSize ());
			layout.AddRow (new Label { Text = "Read Only" }, ReadOnly ());
			layout.AddRow (new Label { Text = "Disabled" }, Disabled ());
			layout.AddRow (new Label { Text = "Placeholder" }, Placeholder ());

			// growing space at end is blank!
			layout.Add (null);
		}
		
		Control Default ()
		{
			var control = new TextBox { Text = "Some Text" };
			LogEvents (control);
			return control;
		}
		
		Control DifferentSize ()
		{
			var control = new TextBox{ Text = "Some Text", Size = new Size (100, 50) };
			LogEvents (control);
			return control;
		}
		
		Control ReadOnly ()
		{
			var control = new TextBox{ Text = "Read only text", ReadOnly = true };
			LogEvents (control);
			return control;
		}

		Control Disabled ()
		{
			var control = Default ();
			control.Enabled = false;
			return control;
		}

		Control Placeholder ()
		{
			var control = new TextBox { PlaceholderText = "Some Placeholder" };
			LogEvents (control);
			return control;
		}

		void LogEvents (TextBox control)
		{
			control.TextChanged += delegate {
				Log.Write (control, "TextChanged, Text: {0}", control.Text);
			};
		}
	}
}

