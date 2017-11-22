using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace UnityHelpers
{
	public static class ExtensionMethods
	{
#region DateTime
		public static string FormatISO8601(this DateTime dt)
		{
			return dt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'", System.Globalization.CultureInfo.InvariantCulture);
		}

		public static DateTime ParseAsISO8601(this string dateTime)
		{
			return dateTime.ParseAsISO8601(true);
		}

		public static DateTime ParseAsISO8601(this string dateTime, bool toUtc)
		{
			DateTime result = DateTime.Parse(dateTime);
			if (toUtc)
				result = result.ToUniversalTime();

			return result;
		}

		public static DateTime FromUnixTimeMs(this long unixTimeMs)
		{
			return Helpers.UnixTimeStart.AddMilliseconds(unixTimeMs);
		}

		public static DateTime FromUnixTime(this long unixTime)
		{
			return FromUnixTimeMs(unixTime * 1000);
		}

		public static long ToUnixTime(this DateTime date)
		{
			return Convert.ToInt64((date - Helpers.UnixTimeStart).TotalSeconds);
		}
#endregion

#region char
		public static string Repeat(this char chatToRepeat, int repeat)
		{
			return new string(chatToRepeat,repeat);
		}

		public static string Repeat(this string stringToRepeat, int repeat)
		{
			var builder = new StringBuilder(repeat*stringToRepeat.Length);
			for (int i = 0; i < repeat; i++)
				builder.Append(stringToRepeat);
			return builder.ToString();
		}
#endregion

#region string
		public static string Left(this string s, int count)
		{
			return s.Substring(0, count);
		}

		public static string Right(this string s, int count)
		{
			return s.Substring(s.Length - count, count);
		}

		public static string Mid(this string s, int index, int count)
		{
			return s.Substring(index, count);
		}

		public static string Truncate(this string value, int maxLength)
		{
			if (string.IsNullOrEmpty(value)) return value;
			return value.Length <= maxLength ? value : value.Substring(0, maxLength); 
		}

		public static string Capitalize(this string input)
		{
			if (String.IsNullOrEmpty(input))
				return null;

			return input[0].ToString().ToUpper() + input.Right(input.Length - 1);
		}

		public static bool IsNumeric(this string s)
		{
			float output;
			return float.TryParse(s, out output);
		}
#endregion

#region Type
		public static string GetGenericTypeName(this Type type)
		{
			string typeName = type.Name;

			if (!type.IsGenericType)
				return typeName;

			Type[] arguments = type.GetGenericArguments();
			var arguments2 = arguments.Select(a => GetGenericTypeName(a));
			var arguments3 = arguments2.Select(a => a.Equals("Object") ? "?" : a).ToArray();
			string argumentsString = string.Format("<{0}>", string.Join(", ", arguments3));
			return typeName = typeName.Replace("`" + arguments.Length.ToString(), argumentsString);
		}
#endregion

#region Enum
		public static String ConvertToString(this Enum eff)
		{
			return Enum.GetName(eff.GetType(), eff);
		}

		public static int ConvertToInt(this Enum eff)
		{
			return Convert.ToInt32(eff);
		}

		public static EnumType ConvertToEnum<EnumType>(this String enumValue)  
		{
			return (EnumType)Enum.Parse(typeof(EnumType), enumValue);
		}

		public static bool HasFlag(this Enum flags, Enum flag)
		{
			return (Convert.ToInt32(flags) & Convert.ToInt32(flag)) != 0;
		}
#endregion

#region byte[]
		public static string ToLogFormattedString(this byte[] ba)
		{
			const int BYTES_PER_ROW = 16;

			StringBuilder hex = new StringBuilder();
			hex.AppendFormat("Array length: {0}\r\n", ba.Length);
			hex.Append("Offset:   00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 |\r\n");
			hex.Append("--------------------------------------------------------- |\r\n");
			//          00000000  X2 X2 X2 X2 X2 X2 X2 X2 X2 X2 X2 X2 X2 X2 X2 X2 | XXXXXXXX

			int offset = 0;
			foreach(byte[] copySlice in ba.Chunked(BYTES_PER_ROW))
			{
				hex.Append(offset.ToString("00000000:"));

				for (int i = 0; i < copySlice.Length; i++)
					hex.AppendFormat(" {0:X2}", copySlice[i]);

				hex.Append(new String(' ', (BYTES_PER_ROW - copySlice.Length) * 3));
				hex.Append(" | ");
				string raw = Encoding.UTF8.GetString(copySlice);
				raw = raw.Replace("\0", "");
				hex.Append(raw);
				hex.Append("\r\n");

				offset += copySlice.Length;
			}

			return hex.ToString();
		}
#endregion

#region Vector2/Vector3
		public static string ToStringReadable(this Vector2 source)
		{
			return string.Format("({0}; {1})", source.x.ToString("F10"), source.y.ToString("F10"));
		}

		public static string ToStringReadable(this Vector3 source)
		{
			return string.Format("({0}; {1}; {2})", source.x.ToString("F10"), source.y.ToString("F10"), source.z.ToString("F10"));
		}

		public static Vector2 GetCopy(this Vector2 source)
		{
			return new Vector2(source.x, source.y);
		}

		public static Vector3 GetCopy(this Vector3 source)
		{
			return new Vector3(source.x, source.y, source.z);
		}

		public static bool EqualTo(this Vector2 a, Vector2 b)
		{
			return Vector2.SqrMagnitude(a - b) < 0.0001;
		}

		public static bool EqualTo(this Vector3 a, Vector3 b)
		{
			return Vector3.SqrMagnitude(a - b) < 0.0001;
		}
#endregion

#region Rect
		public static string ToStringReadable(this Rect source)
		{
			return string.Format("({0}; {1}; {2}; {3})", source.x, source.y, source.width, source.height);
		}
#endregion

#region Array
		public static List<T[]> Chunked<T>(this T[] arr, int chunkSize)
		{
			List<T[]> result = new List<T[]>();

			int chunksTotal = arr.Length / chunkSize + (arr.Length % chunkSize) == 0 ? 0 : 1;
			for (int si = 0; si < chunksTotal; si++)
			{
				T[] chunk = new T[chunkSize];
				Array.Copy(arr, si * chunkSize, chunk, 0, chunkSize);
			}

			return result;
		}

		public static string[] ToStringArray(this int[] integers)
		{
			string[] arr = new string[integers.Length];
			int i = 0;

			foreach (int item in integers)
				arr[i++] = item.ToString();

			return arr;
		}
#endregion

#region List
		public static IDictionary<TKey, TValue> ListToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list)
		{
			return list.ToDictionary(x => x.Key, x => x.Value);
		}
#endregion

#region Dictionary
		public static T GetTypeValue<T>(this IDictionary<System.Type, System.Object> dictionary)
		{
			return (T)dictionary[typeof(T)];
		}

		public static bool ContainsTypeKey<T>(this IDictionary<System.Type, System.Object> dictionary)
		{
			return dictionary.ContainsKey(typeof(T));
		}
#endregion

#region GameObject/Transform/Component
		public static void Activate(this UnityEngine.Component obj) { obj.gameObject.SetActive(true); }
		public static void Deactivate(this UnityEngine.Component obj) { obj.gameObject.SetActive(false); }

		public static string DumpHierarchyMap(this GameObject obj)
		{
			string res = "";

			Transform cur = obj.transform;
			while (cur != null)
			{
				res += (res.Equals("") ? "" : " <= ") + cur.gameObject.name;
				cur = cur.parent;
			}
			res += " (root)";

			return res;
		}

		public static Bounds GetObjectBounds(this GameObject target)
		{
			Bounds result = new Bounds(Vector3.zero, Vector3.zero);
			MeshFilter[] filters = target.GetComponentsInChildren<MeshFilter>();

			foreach (MeshFilter filter in filters)
				result.Encapsulate(filter.mesh.bounds);

			result.size = Vector3.Scale(result.size, target.transform.localScale);

			return result;
		}

		public static string GetPath(this Transform current)
		{
			if (current.parent == null)
				return "/" + current.name;

			return current.parent.GetPath() + "/" + current.name;
		}

		public static string GetPath(this UnityEngine.Component component)
		{
			return component.transform.GetPath() + "/" + component.GetType().ToString();
		}

		public static void SetNameAsPerPrefab(this GameObject current)
		{
			current.name = current.name.Replace("(Clone)", "").Trim();
		}
#endregion

#region Reflection
		public static object CallMethodWithReflection(this object o, string methodName, params object[] args)
		{
			var mi = o.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if (mi != null)
				return mi.Invoke (o, args);
			return null;
		}
#endregion

#region Web related
		public static string ToDataURL(this byte[] bytes, string type)
		{
			return string.Format("data:image/{0};base64,{1}", type, System.Convert.ToBase64String(bytes));
		}
#endregion

#region Coroutines
		public static System.Collections.IEnumerator ToCoroutine(this Action obj)
		{
			obj.Invoke();
			yield break;
		}

		public static void StartCoroutine(this MonoBehaviour obj, System.Collections.IEnumerator coroutine, Action callback)
		{
			obj.StartCoroutine(StartCoroutineWorker(coroutine, callback));
		}

		private static System.Collections.IEnumerator StartCoroutineWorker(System.Collections.IEnumerator coroutine, Action callback)
		{
			yield return coroutine;
			callback.Invoke();
		}
#endregion
	}
}
