using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.Web
{
	public static class WebRequestPipe
	{
		public static event Action<WebRequest> Sended;
		public static event Action<WebRequest> RetrySend;
		public static event Action<WebRequest> Failed;

		public static Dictionary<WebRequest, WebRequestSendSettings> _piped = new();

		static List<WebRequest> _processorCache = new(1);
		static Stopwatch _stopwatch = new();

		/// <summary>
		/// Try register request in pipe and send.
		/// </summary>
		/// <returns>Status of registering. If returned false, probably simillar request already </returns>
		public static async Awaitable<WebRequest> Send(this WebRequest request, WebRequestSendSettings settings)
		{
			if (_piped.ContainsKey(request))
				return request;

			_piped.Add(request, settings);
			request.Request.timeout = (int)settings.ExpireInMilliseconds;
			int retryLeft = settings.RetryCount;

			while (retryLeft >= 0)
			{
				await request.Request.SendWebRequest();

				if (request)
				{
					_piped.Remove(request);

					Sended?.Invoke(request);
					return request;
				}

				RetrySend?.Invoke(request);
				retryLeft--;
			}

			Failed?.Invoke(request);
			return request;
		}
	}

	public struct WebRequestSendSettings
	{
		public int ExpireInMilliseconds;
		public int RetryCount;

		public static WebRequestSendSettings Default => new();

		public WebRequestSendSettings(int retryCount = 3)
		{
			RetryCount = retryCount;
			ExpireInMilliseconds = 5000;
		}
	}
}