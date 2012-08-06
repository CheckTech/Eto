﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
#if DESKTOP
using System.Windows.Markup;
#endif

namespace Eto.Forms
{
#if DESKTOP
	[ContentProperty("Items")]
#endif
	public class DynamicRow
	{
		List<DynamicItem> items = new List<DynamicItem> ();

		public List<DynamicItem> Items
		{
			get { return items; }
		}

		public DynamicRow ()
		{ }

		public DynamicRow (IEnumerable<DynamicItem> items)
		{
			this.items.AddRange (items);
		}

	}

#if DESKTOP
	[ContentProperty("Rows")]
#endif
	public class DynamicTable : DynamicItem
	{
		List<DynamicRow> rows = new List<DynamicRow> ();

		public List<DynamicRow> Rows
		{
			get { return rows; }
		}

		public TableLayout Layout { get; private set; }

		public DynamicTable Parent { get; set; }

		public Padding? Padding { get; set; }

		public Size? Spacing { get; set; }

		internal DynamicRow CurrentRow { get; set; }

		public Container Container { get; internal set; }

		public void Add (DynamicItem item)
		{
			if (CurrentRow != null)
				CurrentRow.Items.Add (item);
			else
				AddRow (item);
		}

		public void AddRow (DynamicItem item)
		{
			var row = new DynamicRow ();
			row.Items.Add (item);
			rows.Add (row);
		}

		public void AddRow (DynamicRow row)
		{
			rows.Add (row);
		}

		public override Control Generate (DynamicLayout layout)
		{
			if (rows.Count == 0)
				return null;
			int cols = rows.Max (r => r.Items.Count);

			if (Container == null) {
				Container = new Panel ();
				this.Layout = new TableLayout (Container, cols, rows.Count);
			}
			else {
				this.Layout = new TableLayout (null, cols, rows.Count);
				Container.SetInnerLayout ();
			}
			var tableLayout = this.Layout;
			var padding = this.Padding ?? layout.DefaultPadding;
			if (padding != null)
				tableLayout.Padding = padding.Value;

			var spacing = this.Spacing ?? layout.DefaultSpacing;
			if (spacing != null)
				tableLayout.Spacing = spacing.Value;

			for (int cy = 0; cy < rows.Count; cy++) {
				var row = rows[cy];
				for (int cx = 0; cx < row.Items.Count; cx++) {
					var item = row.Items[cx];
					item.Generate (layout, tableLayout, cx, cy);
				}
			}
			return Container;
		}
	}
}
