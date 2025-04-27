using System;
using System.ComponentModel;
using System.Reflection;

namespace FurnitureMovement.Data
{
	public static class StatusPriorityMethods
	{
		/// <summary>
		/// Получает описание из атрибута Description для значения enum.
		/// </summary>
		public static string GetDescription(this Enum value)
		{
			Type type = value.GetType();
			string name = Enum.GetName(type, value);
			if (name == null)
				return value.ToString();

			FieldInfo field = type.GetField(name);
			if (field == null)
				return value.ToString();

			DescriptionAttribute attr =
				Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
				as DescriptionAttribute;

			return attr?.Description ?? value.ToString();
		}

		/// <summary>
		/// Получает все значения enum с их описаниями.
		/// </summary>
		public static Dictionary<T, string> GetAllDescriptions<T>() where T : Enum
		{
			var result = new Dictionary<T, string>();
			foreach (T value in Enum.GetValues(typeof(T)))
			{
				result.Add(value, value.GetDescription());
			}
			return result;
		}
	}
}