using UnityEngine;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Reflection;

namespace UnityHelpers
{
	public static partial class Helpers
	{
		public static Action EmptyCallback { get { return () => {}; } }
		public static DateTime UnixTimeStart { get { return new DateTime(1970, 1, 1, 0, 0, 0); } }

		public static string UUID()
		{
			return Guid.NewGuid().ToString("N").ToUpper();
		}

		public static string MD5(string str)
		{
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(Encoding.UTF8.GetBytes(str)); ;
			return BitConverter.ToString(retVal).Replace("-", "");
		}

		public static object GetPropValue(object src, string propName)
		{
			return src.GetType().GetProperty(propName).GetValue(src, null);
		}

		public static void SetPropertyValue(object src, string propName, object propValue)
		{
			src.GetType().GetProperty(propName).SetValue(src, propValue, null);
		}

		public static void SetPrivateFieldValue(object instance, string propertyName, object newValue)
		{
			Type type = instance.GetType();
			FieldInfo field = type.GetField(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
			field.SetValue(instance, newValue);
		}

		public static UInt16 ReverseBytes(UInt16 value)
		{
			return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
		}

		public static UInt32 ReverseBytes(UInt32 value)
		{
			return
				(value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
				(value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
		}

		public static UInt64 ReverseBytes(UInt64 value)
		{
			return
				(value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
				(value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) <<  8 |
				(value & 0x000000FF00000000UL) >>  8 | (value & 0x0000FF0000000000UL) >> 24 |
				(value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
		}

		public static IEnumerable<T> GetEnumValues<T>()
		{
			return Enum.GetValues(typeof(T)).Cast<T>();
		}

		public static int[] GetRandomNumbers(int start, int count)
		{
			return Enumerable.Range(start, count).OrderBy(x => Guid.NewGuid()).ToArray();
		}

		public static int GetKilobytes(int bytes)
		{
			return GetKilobytes((ulong)Mathf.Clamp(bytes, 0, bytes));
		}

		public static int GetMegabytes(int bytes)
		{
			return GetMegabytes((ulong)Mathf.Clamp(bytes, 0, bytes));
		}

		public static int GetKilobytes(ulong bytes)
		{
			return Mathf.RoundToInt(bytes / 1024.0f);
		}

		public static int GetMegabytes(ulong bytes)
		{
			return Mathf.RoundToInt(Helpers.GetKilobytes(bytes) / 1024.0f);
		}
	}
}
