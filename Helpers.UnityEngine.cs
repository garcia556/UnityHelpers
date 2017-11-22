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
		public static T CreateTemporaryObjectWithComponent<T>() where T : Component
		{
			GameObject gameObject = Helpers.CreateTemporaryObject(typeof(T).Name);
			Component component = gameObject.AddComponent<T>();
			return (T)component;
		}

		public static GameObject CreateTemporaryObject(string prefix)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = prefix + "_" + UUID();
			return gameObject;
		}

		public static Color RBGToColor(int r, int g, int b)
		{
			return HexToColor(r.ToString("X2") + g.ToString("X2") + b.ToString("X2"));
		}

		public static string ColorToHex(Color32 color)
		{
			string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
			return hex;
		}

		public static Color HexToColor(string hex, float alpha)
		{
			byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
			return new Color32(r, g, b, (byte)(alpha * 255));
		}

		public static Color HexToColor(string hex)
		{
			return Helpers.HexToColor(hex, 1.0f);
		}

		public static string GetColoredRTF(string text, Color color, float alpha)
		{
			string alphaHex = ((int)(alpha * 255)).ToString("x2");
			return string.Format("<color=#{0}{1}>{2}</color>", Helpers.ColorToHex(color).ToLowerInvariant(),  alphaHex, text);
		}

		public static string GetColoredRTF(string text, Color color)
		{
			return Helpers.GetColoredRTF(text, color, 1.0f);
		}

		public static string GetColoredRTF(string text, string colorHex)
		{
			return Helpers.GetColoredRTF(text, HexToColor(colorHex));
		}

		public static Color GetColorWithAlpha(Color color, float alpha)
		{
			color.a = alpha;
			return color;
		}

		public static void SetLayerRecursively(GameObject obj, string layerName)
		{		
			if (LayerMask.NameToLayer(layerName) < 0)
				return;

			Helpers.SetLayerRecursively(obj, LayerMask.NameToLayer(layerName));
		}

		public static void SetLayerRecursively(GameObject obj, int newLayer)
		{		
			if (null == obj)
				return;

			obj.layer = newLayer;

			foreach (Transform child in obj.transform)
			{
				if (null == child)
					continue;

				SetLayerRecursively(child.gameObject, newLayer);
			}
		}

		public static Vector2 GetMiddlePoint(Vector2 point1, Vector2 point2)
		{
			return (point1 + point2) / 2;
		}

		public static Vector3 GetMiddlePoint(Vector3 point1, Vector3 point2)
		{
			return (point1 + point2) / 2;
		}

		public static Vector3 GetMiddlePoint(Transform transform1, Transform transform2)
		{
			return GetMiddlePoint(transform1.position, transform2.position);
		}

		public static Vector3 GetMiddlePoint(GameObject gameObject1, GameObject gameObject2)
		{
			return GetMiddlePoint(gameObject1.transform.position, gameObject2.transform.position);
		}

		public static Vector3 GetMiddlePointLocal(GameObject gameObject1, GameObject gameObject2)
		{
			return GetMiddlePoint(gameObject1.transform.localPosition, gameObject2.transform.localPosition);
		}

		public static void ApplyTextureToChildrenMaterials(
			GameObject parent,
			string materialNamePart,
			string materialSlotName,
			Texture2D resource
		)
		{
			// loop through each child in object hierarchy
			foreach (MeshRenderer renderer in parent.transform.GetComponentsInChildren<MeshRenderer>())
				// changing target material properties
				foreach (Material material in renderer.materials.Where(m => m.name.Contains(materialNamePart)))
					material.SetTexture(materialSlotName, resource);
		}

		public static void ClearTextureToChildrenMaterials(
			GameObject parent,
			string materialNamePart,
			string materialSlotName
		)
		{
			Texture2D textureEmpty = null;
			ApplyTextureToChildrenMaterials(parent, materialNamePart, materialSlotName, textureEmpty);
		}
	}
}
