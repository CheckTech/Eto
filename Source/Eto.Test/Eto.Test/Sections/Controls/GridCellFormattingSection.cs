using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	public class GridCellFormattingSection : GridViewSection
	{

		protected override void LogEvents (GridView control)
		{
			control.RowHeight = 36;
			var font = new Font (FontFamily.Serif, 18, FontStyle.Italic);
			control.CellFormatting += (sender, e) => {
				// Log.Write (control, "Formatting Row: {1}, Column: {2}, Item: {0}", e.Item, e.Row, control.Columns.IndexOf (e.Column));
				e.Font = font;
				e.BackgroundColor = Color.Blue;
				e.ForegroundColor = Color.Green;
			};
		}

	}
}

