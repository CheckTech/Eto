using System;
using Eto.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;

namespace Eto.Serialization.Json
{
	public class TableLayoutConverter : JsonConverter
	{
		public override bool CanWrite
		{
			get { return false; }
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(TableRow).IsAssignableFrom(objectType) || typeof(TableCell).IsAssignableFrom(objectType);
		}

		public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			object instance;
			JContainer container;
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			if (reader.TokenType == JsonToken.StartArray)
			{
				container = JArray.Load(reader);
				if (objectType == typeof(TableRow))
				{
					var row = new TableRow();
					instance = row;
					serializer.Populate(container.CreateReader(), row.Cells);
				}
				else if (objectType == typeof(TableCell))
				{
					var table = new TableLayout();
					serializer.Populate(container.CreateReader(), table.Rows);
					instance = new TableCell(table);
				}
				else
					throw new EtoException("Invalid object graph");
			}
			else
			{
				container = JObject.Load(reader);
				if (container["$type"] == null)
				{
					if (container["Rows"] != null)
						instance = new TableLayout();
					else if (container["Cells"] != null)
						instance = new TableRow();
					else if (container["Control"] != null)
						instance = new TableCell();
					else if (objectType == typeof(TableRow))
						instance = new TableRow();
					else if (objectType == typeof(TableCell))
						instance = new TableCell();
					else
						throw new EtoException("Could not infer the type of object to create");

					serializer.Populate(container.CreateReader(), instance);
				}
				else
				{
					var type = Type.GetType((string)container["$type"]);
					if (!typeof(TableCell).IsAssignableFrom(type))
					{
						var cell = new TableCell();
						cell.Control = serializer.Deserialize(container.CreateReader()) as Control;
						instance = cell;
					}
					else
					{
						instance = serializer.Deserialize(container.CreateReader());
					}
				}
			}
			if (objectType == typeof(TableRow) && !(instance is TableRow))
			{
				var row = new TableRow();
				var cell = instance as TableCell;
				if (cell == null)
					cell = new TableCell(instance as Control);
				row.Cells.Add(cell);
				return row;
			}

			return instance;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
