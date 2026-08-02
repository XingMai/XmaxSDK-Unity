using System;
using System.Diagnostics;
using UnityEngine;

namespace bytertc
{
	public enum LogLevel
	{
		Debug,
		Info,
		Warning,
		Error,
		None,
	}

	public delegate void ByteRTClogOutpoutDelegate(object msg);

	public class ByteRTCLog
	{
		public static LogLevel Level = LogLevel.Info;

		private static string prefix = "[ByteRTC]:";

		public static ByteRTClogOutpoutDelegate OutputDelegate;

		private static readonly string SDK_API_CALL = "rtc_sdk_api_call";
        private static readonly string SDK_CALLBACK = "rtc_sdk_callback";

        public static void ReportApiCall(string apiName, string logInfo)
        {
            LogInfo(prefix + ", " + SDK_API_CALL + ": " + apiName + ", " + logInfo);
        }

        public static void ReportCallback(string callbackName, string logInfo)
        {
            LogInfo(prefix + ", " + SDK_CALLBACK + ": " + callbackName + ", " + logInfo);
        }

		/// <summary>
		///   <para>Log a message to the Unity Console.</para>
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogDebug(object message)
		{
			if (Level > LogLevel.Debug)
            {
				return;
            }

			if (OutputDelegate != null)
			{
				OutputDelegate.Invoke(prefix + message);
			}

			UnityEngine.Debug.Log(prefix + message);
		}

		/// <summary>
		/// <para>Log a message to the Unity Console. If don't need to output log, don't execute func, to avoid gc</para>
		/// </summary>
		/// <param name="action">func which need to print log.</param>
		public static void LogDebug(Func<string> action)
		{
			if (Level > LogLevel.Debug)
			{
				return;
			}

			string str = action();
			
			if (OutputDelegate != null)
			{
				OutputDelegate.Invoke(prefix + str);
			}
			
			UnityEngine.Debug.Log(prefix + str);
		}

		/// <summary>
		///   <para>Logs a formatted message to the Unity Console.</para>
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogDebugFormat(string format, params object[] args)
		{
			if (Level > LogLevel.Debug)
			{
				return;
			}
			UnityEngine.Debug.LogFormat(prefix + format, args);
		}


		/// <summary>
		///   <para>Log a message to the Unity Console.</para>
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="extraTag">An extra tag used to extend prefix.</param>
		public static void LogInfo(object message, string extraTag = "")
		{
			if (Level > LogLevel.Info)
			{
				return;
			}
			
			if (OutputDelegate != null)
			{
				OutputDelegate.Invoke(prefix + GenerateExtraTag(extraTag) + message);
			}
			
			UnityEngine.Debug.Log(prefix + GenerateExtraTag(extraTag) + message);
		}

		/// <summary>
		///   <para>Logs a formatted message to the Unity Console.</para>
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogInfoFormat(string format, params object[] args)
		{
			if (Level > LogLevel.Info)
			{
				return;
			}
			UnityEngine.Debug.LogFormat(prefix + format, args);
		}


		/// <summary>
		///   <para>A variant of UnityEngine.Debug.Log that logs an error message to the console.</para>
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogError(object message)
		{
			if (Level > LogLevel.Error)
			{
				return;
			}
			
			if (OutputDelegate != null)
			{
				OutputDelegate.Invoke(prefix + message);
			}
			
			UnityEngine.Debug.LogError(prefix + message);
		}


		/// <summary>
		///   <para>Logs a formatted error message to the Unity console.</para>
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogErrorFormat(string format, params object[] args)
		{
			if (Level > LogLevel.Error)
			{
				return;
			}
			UnityEngine.Debug.LogErrorFormat(prefix + format, args);
		}


		/// <summary>
		///   <para>A variant of UnityEngine.Debug.Log that logs an error message to the console.</para>
		/// </summary>
		/// <param name="context">Object to which the message applies.</param>
		/// <param name="exception">Runtime Exception.</param>
		public static void LogException(Exception exception)
		{
			if (Level > LogLevel.Error)
			{
				return;
			}
			
			if (OutputDelegate != null)
			{
				OutputDelegate.Invoke(exception);
			}
			
			UnityEngine.Debug.LogException(exception);
		}



		/// <summary>
		///   <para>A variant of UnityEngine.Debug.Log that logs a warning message to the console.</para>
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogWarning(object message)
		{
			if (Level > LogLevel.Warning)
			{
				return;
			}
			
			if (OutputDelegate != null)
			{
				OutputDelegate.Invoke(prefix + message);
			}
			
			UnityEngine.Debug.LogWarning(prefix + message);
		}


		/// <summary>
		///   <para>Logs a formatted warning message to the Unity Console.</para>
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogWarningFormat(string format, params object[] args)
		{
			if (Level > LogLevel.Warning)
			{
				return;
			}
			UnityEngine.Debug.LogWarningFormat(prefix + format, args);
		}


		/// <summary>
		///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
		/// </summary>
		/// <param name="condition">Condition you expect to be true.</param>
		/// <param name="context">Object to which the message applies.</param>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition)
		{
			if (Level > LogLevel.Error)
			{
				return;
			}
			UnityEngine.Debug.Assert(condition);
		}


		/// <summary>
		///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
		/// </summary>
		/// <param name="condition">Condition you expect to be true.</param>
		/// <param name="context">Object to which the message applies.</param>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, object message)
		{
			if (Level > LogLevel.Error)
			{
				return;
			}
			UnityEngine.Debug.Assert(condition, prefix + message);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, string message)
		{
			if (Level > LogLevel.Error)
			{
				return;
			}
			UnityEngine.Debug.Assert(condition, prefix + message);
		}


		/// <summary>
		///   <para>Assert a condition and logs a formatted error message to the Unity console on failure.</para>
		/// </summary>
		/// <param name="condition">Condition you expect to be true.</param>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		/// <param name="context">Object to which the message applies.</param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void AssertFormat(bool condition, string format, params object[] args)
		{
			if (Level > LogLevel.Error)
			{
				return;
			}
			UnityEngine.Debug.AssertFormat(condition, prefix + format, args);
		}


		/// <summary>
		///   <para>A variant of UnityEngine.Debug.Log that logs an assertion message to the console.</para>
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void LogAssertion(object message)
		{
			if (Level > LogLevel.Error)
			{
				return;
			}
			UnityEngine.Debug.LogAssertion(prefix + message);
		}


		/// <summary>
		///   <para>Logs a formatted assertion message to the Unity console.</para>
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		/// <param name="context">Object to which the message applies.</param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void LogAssertionFormat(string format, params object[] args)
		{
			if (Level > LogLevel.Error)
			{
				return;
			}
			UnityEngine.Debug.LogAssertionFormat(prefix + format, args);
		}

		private static string GenerateExtraTag(string extraTag)
		{
			if (string.IsNullOrEmpty(extraTag))
			{
				return "";
			}
			return "{" + extraTag + "}";
		}
	}
}
