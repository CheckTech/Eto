using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IToolBarItem : IInstanceWidget
	{
	}
	
	public class ToolBarItem : InstanceWidget
	{
		public ToolBarItem (Generator g, Type type) : base(g, type)
		{
		}
	}
}
