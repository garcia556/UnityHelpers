using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityHelpers
{
	public static class Interpolate
	{
		public const string GAMEOBJECT_NAME = "_UnityHelpers_Interpolate_";

		public enum EaseType
		{
			Linear,
			EaseInQuad,
			EaseOutQuad,
			EaseInOutQuad,
			EaseInCubic,
			EaseOutCubic,
			EaseInOutCubic,
			EaseInQuart,
			EaseOutQuart,
			EaseInOutQuart,
			EaseInQuint,
			EaseOutQuint,
			EaseInOutQuint,
			EaseInSine,
			EaseOutSine,
			EaseInOutSine,
			EaseInExpo,
			EaseOutExpo,
			EaseInOutExpo,
			EaseInCirc,
			EaseOutCirc,
			EaseInOutCirc
		}

		public delegate float Function(float a, float b, float c, float d);

		public class ChangeInTimeComponent : MonoBehaviour
		{
			private EaseType easeType;
			private Type dataType;
			private float durationTime;

			private DataFloat dataFloat;
			private DataVector3 dataVector3;

			private string debug = "";

			public class DataFloat
			{
				public float valueFrom;
				public Action<float> onValueChanged;
				public Action onChangeDone;
				public float distance;
			}

			public class DataVector3
			{
				public Vector3 valueFrom;
				public Action<Vector3> onValueChanged;
				public Action onChangeDone;
				public Vector3 distance;
			}

			private float elapsedTime;

			public void InitFloat(EaseType easeType, float valueFrom, float valueTo, float durationTime, Action onChangeDone, Action<float> onValueChanged)
			{
				this.dataType = typeof(float);
				this.easeType = easeType;
				this.durationTime = durationTime;

				this.dataFloat = new DataFloat()
				{
					valueFrom = valueFrom
						, onValueChanged = onValueChanged
						, onChangeDone = onChangeDone
						, distance = valueTo - valueFrom
				};

				this.enabled = true;
			}

			public void InitVector3To(EaseType easeType, Vector3 valueFrom, Vector3 valueTo, float durationTime, Action onChangeDone, Action<Vector3> onValueChanged)
			{
				this.dataType = typeof(Vector3);
				this.easeType = easeType;
				this.durationTime = durationTime;

				this.dataVector3 = new DataVector3()
				{
					valueFrom = valueFrom
						, onValueChanged = onValueChanged
						, onChangeDone = onChangeDone
						, distance = valueTo - valueFrom
				};

				this.enabled = true;
			}

			public void InitVector3(EaseType easeType, Vector3 valueFrom, Vector3 distance, float durationTime, Action onChangeDone, Action<Vector3> onValueChanged)
			{
				this.dataType = typeof(Vector3);
				this.easeType = easeType;
				this.durationTime = durationTime;

				this.dataVector3 = new DataVector3()
				{
					valueFrom = valueFrom
						, onValueChanged = onValueChanged
						, onChangeDone = onChangeDone
						, distance = distance
				};

				this.enabled = true;
			}

			void Awake()
			{
				this.enabled = false;
			}

			void FixedUpdate()
			{
				if (this.elapsedTime >= this.durationTime)
				{
					if (this.dataType == typeof(float))
					{
						//Debug.Log(this.debug);
						if (this.dataFloat.onChangeDone != null)
							this.dataFloat.onChangeDone.Invoke();
					}
					else
					if (this.dataType == typeof(Vector3))
					{
						//	Debug.Log(this.debug);
						if (this.dataVector3.onChangeDone != null)
							this.dataVector3.onChangeDone.Invoke();
					}

					GameObject.Destroy(this.gameObject);
				}

				if (this.dataType == typeof(float))
				{
					if (this.dataFloat.onValueChanged != null)
					{
						float value = Interpolate.Ease(easeType).Invoke(this.dataFloat.valueFrom, this.dataFloat.distance, this.elapsedTime, this.durationTime);
						this.dataFloat.onValueChanged.Invoke(value);
					}
				}
				else
				if (this.dataType == typeof(Vector3))
				{
					if (this.dataVector3.onValueChanged != null)
					{
						float x = Interpolate.Ease(easeType).Invoke(this.dataVector3.valueFrom.x, this.dataVector3.distance.x, this.elapsedTime, this.durationTime);
						float y = Interpolate.Ease(easeType).Invoke(this.dataVector3.valueFrom.y, this.dataVector3.distance.y, this.elapsedTime, this.durationTime);
						float z = Interpolate.Ease(easeType).Invoke(this.dataVector3.valueFrom.z, this.dataVector3.distance.z, this.elapsedTime, this.durationTime);

						Vector3 change = new Vector3(x, y, z);

						this.debug += change.ToStringReadable() + "\n";
						this.dataVector3.onValueChanged.Invoke(change);
					}
				}

				this.elapsedTime += Time.fixedDeltaTime;
			}
		}

		// 1
		public static void ChangeFloatInTime<T>(EaseType easeType, float valueFrom, float valueTo, float durationTime)
		{
			Interpolate.ChangeFloatInTime(easeType, valueFrom, valueTo, durationTime, null, null);
		}

		public static void ChangeFloatInTime(EaseType easeType, float valueFrom, float valueTo, float durationTime, Action onChangeDone)
		{
			Interpolate.ChangeFloatInTime(easeType, valueFrom, valueTo, durationTime, onChangeDone, null);
		}

		public static void ChangeFloatInTime(EaseType easeType, float valueFrom, float valueTo, float durationTime, Action onChangeDone, Action<float> onValueChanged)
		{
			GameObject changeActionGO = new GameObject();
			changeActionGO.name = GAMEOBJECT_NAME + Guid.NewGuid().ToString();
			changeActionGO.AddComponent<ChangeInTimeComponent>().InitFloat(easeType, valueFrom, valueTo, durationTime, onChangeDone, onValueChanged);
		}

		// 2
		public static void ChangePositionInTime(EaseType easeType, Transform transform, Vector3 valueTo, float durationTime)
		{
			Interpolate.ChangePositionInTime(easeType, transform, valueTo, durationTime, null);
		}

		public static void ChangePositionInTime(EaseType easeType, Transform transform, Vector3 valueTo, float durationTime, Action onChangeDone)
		{
			GameObject changeActionGO = new GameObject();
			changeActionGO.name = GAMEOBJECT_NAME + Guid.NewGuid().ToString();
			changeActionGO.AddComponent<ChangeInTimeComponent>().InitVector3To(
				easeType
				, transform.position
				, valueTo
				, durationTime
				, onChangeDone
				, (Vector3 valueChanged) =>
				{
					transform.position = valueChanged;
				}
			);
		}

		// 3
		public static void ChangeRotationInTime(EaseType easeType, Transform transform, Vector3 valueTo, float durationTime)
		{
			Interpolate.ChangeRotationInTime(easeType, transform, valueTo, durationTime, null);
		}

		public static void ChangeRotationInTime(EaseType easeType, Transform transform, Vector3 valueTo, float durationTime, Action onChangeDone)
		{
			Vector3 distance = valueTo - transform.eulerAngles;

			distance.x = (distance.x + 180) % 360 - 180;
			distance.y = (distance.y + 180) % 360 - 180;
			distance.z = (distance.z + 180) % 360 - 180;

			GameObject changeActionGO = new GameObject();
			changeActionGO.name = GAMEOBJECT_NAME + Guid.NewGuid().ToString();
			changeActionGO.AddComponent<ChangeInTimeComponent>().InitVector3(
				easeType
				, transform.eulerAngles
				, distance
				, durationTime
				, onChangeDone
				, (Vector3 valueChanged) =>
				{
					transform.eulerAngles = valueChanged;
				}
			);
		}

		public static Function Ease(EaseType type)
		{
			// Source Flash easing functions:
			// http://gizma.com/easing/
			// http://www.robertpenner.com/easing/easing_demo.html

			Function f = null;
			switch (type)
			{
				case EaseType.Linear			: f = Interpolate.Linear; break;
				case EaseType.EaseInQuad		: f = Interpolate.EaseInQuad; break;
				case EaseType.EaseOutQuad		: f = Interpolate.EaseOutQuad; break;
				case EaseType.EaseInOutQuad		: f = Interpolate.EaseInOutQuad; break;
				case EaseType.EaseInCubic		: f = Interpolate.EaseInCubic; break;
				case EaseType.EaseOutCubic		: f = Interpolate.EaseOutCubic; break;
				case EaseType.EaseInOutCubic	: f = Interpolate.EaseInOutCubic; break;
				case EaseType.EaseInQuart		: f = Interpolate.EaseInQuart; break;
				case EaseType.EaseOutQuart		: f = Interpolate.EaseOutQuart; break;
				case EaseType.EaseInOutQuart	: f = Interpolate.EaseInOutQuart; break;
				case EaseType.EaseInQuint		: f = Interpolate.EaseInQuint; break;
				case EaseType.EaseOutQuint		: f = Interpolate.EaseOutQuint; break;
				case EaseType.EaseInOutQuint	: f = Interpolate.EaseInOutQuint; break;
				case EaseType.EaseInSine		: f = Interpolate.EaseInSine; break;
				case EaseType.EaseOutSine		: f = Interpolate.EaseOutSine; break;
				case EaseType.EaseInOutSine		: f = Interpolate.EaseInOutSine; break;
				case EaseType.EaseInExpo		: f = Interpolate.EaseInExpo; break;
				case EaseType.EaseOutExpo		: f = Interpolate.EaseOutExpo; break;
				case EaseType.EaseInOutExpo		: f = Interpolate.EaseInOutExpo; break;
				case EaseType.EaseInCirc		: f = Interpolate.EaseInCirc; break;
				case EaseType.EaseOutCirc		: f = Interpolate.EaseOutCirc; break;
				case EaseType.EaseInOutCirc		: f = Interpolate.EaseInOutCirc; break;
			}

			return f;
		}

		/**
	     * Linear interpolation (same as Mathf.Lerp)
	     */
		static float Linear(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime to be <= duration
			if (elapsedTime > duration)
			{
				elapsedTime = duration;
			}

			return distance * (elapsedTime / duration) + start;
		}

		/**
	     * quadratic easing in - accelerating from zero velocity
	     */
		static float EaseInQuad(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime so that it cannot be greater than duration
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			return distance * elapsedTime * elapsedTime + start;
		}

		/**
	     * quadratic easing out - decelerating to zero velocity
	     */
		static float EaseOutQuad(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime so that it cannot be greater than duration
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			return -distance * elapsedTime * (elapsedTime - 2) + start;
		}

		/**
	     * quadratic easing in/out - acceleration until halfway, then deceleration
	     */
		static float EaseInOutQuad(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime so that it cannot be greater than duration
			elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
			if (elapsedTime < 1)
				return distance / 2 * elapsedTime * elapsedTime + start;

			elapsedTime--;
			return -distance / 2 * (elapsedTime * (elapsedTime - 2) - 1) + start;
		}

		/**
	     * cubic easing in - accelerating from zero velocity
	     */
		static float EaseInCubic(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime so that it cannot be greater than duration
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			return distance * elapsedTime * elapsedTime * elapsedTime + start;
		}

		/**
	     * cubic easing out - decelerating to zero velocity
	     */
		static float EaseOutCubic(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime so that it cannot be greater than duration
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			elapsedTime--;
			return distance * (elapsedTime * elapsedTime * elapsedTime + 1) + start;
		}

		/**
	     * cubic easing in/out - acceleration until halfway, then deceleration
	     */
		static float EaseInOutCubic(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime so that it cannot be greater than duration
			elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
			if (elapsedTime < 1)
				return distance / 2 * elapsedTime * elapsedTime * elapsedTime + start;

			elapsedTime -= 2;
			return distance / 2 * (elapsedTime * elapsedTime * elapsedTime + 2) + start;
		}

		/**
	     * quartic easing in - accelerating from zero velocity
	     */
		static float EaseInQuart(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime so that it cannot be greater than duration
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
		}

		/**
	     * quartic easing out - decelerating to zero velocity
	     */
		static float EaseOutQuart(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime so that it cannot be greater than duration
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			elapsedTime--;
			return -distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 1) + start;
		}

		/**
	     * quartic easing in/out - acceleration until halfway, then deceleration
	     */
		static float EaseInOutQuart(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime so that it cannot be greater than duration
			elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
			if (elapsedTime < 1)
				return distance / 2 * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;

			elapsedTime -= 2;
			return -distance / 2 * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 2) + start;
		}


		/**
	     * quintic easing in - accelerating from zero velocity
	     */
		static float EaseInQuint(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime so that it cannot be greater than duration
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
		}

		/**
	     * quintic easing out - decelerating to zero velocity
	     */
		static float EaseOutQuint(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime so that it cannot be greater than duration
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			elapsedTime--;
			return distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 1) + start;
		}

		/**
	     * quintic easing in/out - acceleration until halfway, then deceleration
	     */
		static float EaseInOutQuint(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime so that it cannot be greater than duration
			elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2f);
			if (elapsedTime < 1)
				return distance / 2 * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;

			elapsedTime -= 2;
			return distance / 2 * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 2) + start;
		}

		/**
	     * sinusoidal easing in - accelerating from zero velocity
	     */
		static float EaseInSine(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime to be <= duration
			if (elapsedTime > duration)
			{
				elapsedTime = duration;
			}

			return -distance * Mathf.Cos(elapsedTime / duration * (Mathf.PI / 2)) + distance + start;
		}

		/**
	     * sinusoidal easing out - decelerating to zero velocity
	     */
		static float EaseOutSine(float start, float distance, float elapsedTime, float duration)
		{
			if (elapsedTime > duration)
			{
				elapsedTime = duration;
			}

			return distance * Mathf.Sin(elapsedTime / duration * (Mathf.PI / 2)) + start;
		}

		/**
	     * sinusoidal easing in/out - accelerating until halfway, then decelerating
	     */
		static float EaseInOutSine(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime to be <= duration
			if (elapsedTime > duration)
			{
				elapsedTime = duration;
			}

			return -distance / 2 * (Mathf.Cos(Mathf.PI * elapsedTime / duration) - 1) + start;
		}

		/**
	     * exponential easing in - accelerating from zero velocity
	     */
		static float EaseInExpo(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime to be <= duration
			if (elapsedTime > duration)
			{
				elapsedTime = duration;
			}

			return distance * Mathf.Pow(2, 10 * (elapsedTime / duration - 1)) + start;
		}

		/**
	     * exponential easing out - decelerating to zero velocity
	     */
		static float EaseOutExpo(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime to be <= duration
			if (elapsedTime > duration)
			{
				elapsedTime = duration;
			}

			return distance * (-Mathf.Pow(2, -10 * elapsedTime / duration) + 1) + start;
		}

		/**
	     * exponential easing in/out - accelerating until halfway, then decelerating
	     */
		static float EaseInOutExpo(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime so that it cannot be greater than duration
			elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
			if (elapsedTime < 1)
				return distance / 2 *  Mathf.Pow(2, 10 * (elapsedTime - 1)) + start;

			elapsedTime--;
			return distance / 2 * (-Mathf.Pow(2, -10 * elapsedTime) + 2) + start;
		}

		/**
	     * circular easing in - accelerating from zero velocity
	     */
		static float EaseInCirc(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime so that it cannot be greater than duration
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			return -distance * (Mathf.Sqrt(1 - elapsedTime * elapsedTime) - 1) + start;
		}

		/**
	     * circular easing out - decelerating to zero velocity
	     */
		static float EaseOutCirc(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime so that it cannot be greater than duration
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			elapsedTime--;
			return distance * Mathf.Sqrt(1 - elapsedTime * elapsedTime) + start;
		}

		/**
	     * circular easing in/out - acceleration until halfway, then deceleration
	     */
		static float EaseInOutCirc(float start, float distance, float elapsedTime, float duration)
		{
			// clamp elapsedTime so that it cannot be greater than duration
			elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
			if (elapsedTime < 1)
				return -distance / 2 * (Mathf.Sqrt(1 - elapsedTime * elapsedTime) - 1) + start;

			elapsedTime -= 2;
			return distance / 2 * (Mathf.Sqrt(1 - elapsedTime * elapsedTime) + 1) + start;
		}
	}
}
