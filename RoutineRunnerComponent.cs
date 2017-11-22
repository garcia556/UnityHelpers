using UnityEngine;
using System.Collections;

namespace UnityHelpers
{
	public static class RoutineRunner
	{
		public const string GAMEOBJECT_NAME = "RoutineRunner";

		private static RoutineRunnerComponent runnerComponent; // cached instance
		private static RoutineRunnerComponent Component // singleton
		{
			get
			{
				if (runnerComponent == null)
				{
					GameObject gameObject = new GameObject(GAMEOBJECT_NAME);
					runnerComponent = gameObject.AddComponent<RoutineRunnerComponent>();
					GameObject.DontDestroyOnLoad(gameObject);
				}

				return runnerComponent;
			}
		}

		public static void			StartAsync	(IEnumerator coroutine)							{ Component.Async(coroutine, null); }
		public static void			StartAsync	(IEnumerator coroutine, System.Action callback)	{ Component.Async(coroutine, callback); }
		public static IEnumerator	Sync		(IEnumerator coroutine)							{ return Component.Sync(coroutine); }
	}

	public class RoutineRunnerComponent : MonoBehaviour
	{
		public void			Async	(IEnumerator coroutine)							{				this.StartCoroutine(this.AsyncWorker(coroutine, null	));	}
		public void			Async	(IEnumerator coroutine, System.Action onDone)	{				this.StartCoroutine(this.AsyncWorker(coroutine, onDone	));	}
		public IEnumerator	Sync	(IEnumerator coroutine)							{ yield return	this.StartCoroutine(coroutine);								}

		private IEnumerator AsyncWorker(IEnumerator coroutine, System.Action onDone)
		{
			yield return coroutine;
			if (onDone != null)
				onDone.Invoke();
		}
	}
}
