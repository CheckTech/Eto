using System;
using Eto.Forms;
using Eto.Platform.GtkSharp.Forms.Controls;

namespace Eto.Platform.GtkSharp.Forms.Cells
{
	public interface ICellDataSource
	{
		object GetItem (Gtk.TreePath path);

		void EndCellEditing (Gtk.TreePath path, int column);

		void BeginCellEditing (Gtk.TreePath path, int column);
		
		void SetColumnMap (int dataIndex, int column);

		int RowHeight { get; set; }

		void OnCellFormatting (GridCellFormatEventArgs args);
	}
	
	public interface ICellHandler
	{
		void BindCell (ICellDataSource source, GridColumnHandler column, int columnIndex, ref int dataIndex);

		void SetEditable (Gtk.TreeViewColumn column, bool editable);

		GLib.Value GetValue (object dataItem, int column, int row);

		void HandleEvent (string eventHandler);

		void AddCells (Gtk.TreeViewColumn column);
	}
	
	public abstract class SingleCellHandler<T, W> : CellHandler<T, W>
		where T: Gtk.CellRenderer
		where W: Cell
	{
		public override void AddCells (Gtk.TreeViewColumn column)
		{
			column.PackStart (this.Control, true);
		}
		
		public override void AttachEvent (string eventHandler)
		{
			switch (eventHandler) {
			case GridView.BeginCellEditEvent:
				Control.EditingStarted += (sender, e) => {
					Source.BeginCellEditing (new Gtk.TreePath (e.Path), ColumnIndex);
				};
				break;
			default:
				base.AttachEvent (eventHandler);
				break;
			}
		}

	}

	public abstract class CellHandler<T, W> : WidgetHandler<T, W>, ICell, ICellHandler
		where W: Cell
		where T: Gtk.CellRenderer
	{
		int? dataIndex;
		int itemCol;
		int rowCol;
		
		public GridColumnHandler Column { get; private set; }

		public int ColumnIndex { get; private set; }

		public ICellDataSource Source { get; private set; }

		public bool FormattingEnabled { get; private set; }
		
		public abstract void AddCells (Gtk.TreeViewColumn column);
		
		public void BindCell (ICellDataSource source, GridColumnHandler column, int columnIndex, ref int dataIndex)
		{
			Source = source;
			Column = column;
			ColumnIndex = columnIndex;
			this.dataIndex = dataIndex;
			BindCell (ref dataIndex);
			BindBase (Control, ref dataIndex);
		}

		protected void ReBind ()
		{
			if (this.dataIndex != null) {
				var dataIndex = this.dataIndex.Value;
				BindCell (ref dataIndex);
				BindBase (Control, ref dataIndex);
			}
		}

		protected void BindBase (Gtk.CellRenderer renderer, ref int dataIndex)
		{
			if (FormattingEnabled) {
				itemCol = SetColumnMap (dataIndex);
				Column.Control.AddAttribute (Control, "item", dataIndex++);
				rowCol = SetColumnMap (dataIndex);
				Column.Control.AddAttribute (Control, "row", dataIndex++);
			}
		}

		public void Format (GridCellFormatEventArgs args)
		{
			Source.OnCellFormatting (args);
		}
		
		protected abstract void BindCell (ref int dataIndex);
		
		protected int SetColumnMap (int dataIndex)
		{
			Source.SetColumnMap (dataIndex, ColumnIndex);
			return dataIndex;
		}
		
		public abstract void SetEditable (Gtk.TreeViewColumn column, bool editable);

		protected void SetValue (string path, object value)
		{
			SetValue (new Gtk.TreePath (path), value);
		}
		
		protected void SetValue (Gtk.TreePath path, object value)
		{
			var item = Source.GetItem (path);
			SetValue (item, value);
		}
		
		public abstract void SetValue (object dataItem, object value);

		public GLib.Value GetValue (object dataItem, int dataColumn, int row)
		{
			if (FormattingEnabled) {
				if (dataColumn == itemCol)
					return new GLib.Value (dataItem);
				if (dataColumn == rowCol)
					return new GLib.Value (row);
			}
			return GetValueInternal (dataItem, dataColumn, row);
		}
		
		protected abstract GLib.Value GetValueInternal (object dataItem, int dataColumn, int row);

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Grid.CellFormattingEvent:
				FormattingEnabled = true;
				ReBind ();
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}

	}
}

