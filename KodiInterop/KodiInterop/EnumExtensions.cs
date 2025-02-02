using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Smx.KodiInterop
{
	public static class EnumExtensions
	{
		public static string GetString<T>(this T value) where T : struct, IConvertible {
			string memberName = Enum.GetName(typeof(T), value);
			FieldInfo field = typeof(T).GetField(memberName);
			StringValueAttribute strAttr = (StringValueAttribute)field.GetCustomAttribute(typeof(StringValueAttribute), false);
			return strAttr?.Value ?? memberName; // Try to get the StringValueAttribute value, fallback on memberName
		}
	}
}
