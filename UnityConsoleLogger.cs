using UnityEngine;
using System;
using System.Collections;

namespace UnityHelpers
{
	public class UnityConsoleLogger
	{
		private string name;

		public UnityConsoleLogger(Type type) : this(type.FullName) { }

		public UnityConsoleLogger(string name)
		{
			this.name = name;
		}

		public void Info(string message)
		{
			this.Debug(message);
		}

		public void Info(string format, params object[] args)
		{
			this.Debug(format, args);
		}

		public void Debug(string format, params object[] args)
		{
			this.Debug(string.Format(format, args));
		}

		private string PrepareMessage(string message)
		{
			return string.Format("{0} [{1}]: {2}", DateTime.UtcNow.ToString("o"), this.name, message);
		}

		public void Debug(string message)
		{
			UnityEngine.Debug.Log(this.PrepareMessage(message));
		}

		public void Error(string message)
		{
			UnityEngine.Debug.LogError(this.PrepareMessage(message));
		}

		public void Error(string message, params object[] args)
		{
			UnityEngine.Debug.LogError(this.PrepareMessage(string.Format(message, args)));
		}

		public void Warn(string message)
		{
			UnityEngine.Debug.LogWarning(this.PrepareMessage(message));
		}

		public void Warn(string message, params object[] args)
		{
			UnityEngine.Debug.LogWarning(this.PrepareMessage(string.Format(message, args)));
		}
	}
}
