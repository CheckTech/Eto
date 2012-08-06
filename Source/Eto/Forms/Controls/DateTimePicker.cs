using System;

namespace Eto.Forms
{
	[Flags]
	public enum DateTimePickerMode
	{
		Date = 1,
		Time = 2,
		DateTime = Date | Time
	}

	public interface IDateTimePicker : ICommonControl
	{
		DateTime? Value { get; set; }

		DateTime MinDate { get; set; }

		DateTime MaxDate { get; set; }

		DateTimePickerMode Mode { get; set; }
	}
	
	public class DateTimePicker : CommonControl
	{
		IDateTimePicker inner;
		public static DateTimePickerMode DefaultMode = DateTimePickerMode.Date;
		
		public event EventHandler<EventArgs> ValueChanged;
		
		public virtual void OnValueChanged (EventArgs e)
		{
			if (ValueChanged != null)
				ValueChanged (this, e);
		}
		
		public DateTimePicker ()
			: this (Generator.Current)
		{
		}
		
		public DateTimePicker (Generator generator)
			: this (generator, typeof(IDateTimePicker))
		{
		}
		
		protected DateTimePicker (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			inner = (IDateTimePicker)Handler;
		}
		
		public DateTime MinDate {
			get { return inner.MinDate; }
			set { inner.MinDate = value; }
		}

		public DateTime MaxDate {
			get { return inner.MaxDate; }
			set { inner.MaxDate = value; }
		}
		
		public DateTime? Value {
			get { return inner.Value; }
			set { inner.Value = value; }
		}
		
		public DateTimePickerMode Mode {
			get { return inner.Mode; }
			set { inner.Mode = value; }
		}
		
		
	}
}

