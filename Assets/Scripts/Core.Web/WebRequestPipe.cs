using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Core.Web
{
	public static class WebRequestPipe
	{
		public static event Action<WebRequest> Sended;
		public static event Action<WebRequest> RetrySend;
		public static event Action<WebRequest> Failed;

		static List<WebRequest> _processorCache = new(1);
		static Stopwatch _stopwatch = new();

		/// <summary>
		/// Try register request in pipe and send.
		/// </summary>
		/// <returns>Status of registering. If returned false, probably simillar request already </returns>
		public static async Awaitable<WebRequest> Send(this WebRequest request, WebRequestSendSettings settings)
		{
			int retryLeft = settings.RetryCount;

			while (retryLeft >= 0)
			{
				request.CurrentRequest.SetRequestHeader("User-Agent", "CKR/1.0");
				request.CurrentRequest.timeout = settings.ExpireInSeconds;
				await request.CurrentRequest.SendWebRequest();

				if (request)
				{
					settings.Sended?.Invoke();
					Sended?.Invoke(request);
					return request;
				}

				if (retryLeft <= 0)
					break;

				request = request.Recreate();
				settings.Retry?.Invoke();
				RetrySend?.Invoke(request);

				retryLeft--;
			}

			settings.Failed?.Invoke();
			Failed?.Invoke(request);
			return request;
		}
	}

	public struct WebRequestSendSettings
	{
		public int ExpireInSeconds;
		public int RetryCount;

		public Action Retry;
		public Action Failed;
		public Action Sended;

		public static WebRequestSendSettings Default => new(3);

		public WebRequestSendSettings(int retryCount)
		{
			RetryCount = retryCount;
			ExpireInSeconds = 2;

			Retry = null;
			Failed = null;
			Sended = null;
		}
	}

	public static class WebRequestSendSettingsFactory
	{
		public static WebRequestSendSettings Create()
		{
			return WebRequestSendSettings.Default;
		}

		public static WebRequestSendSettings WithRetryAction(this WebRequestSendSettings data, Action action)
		{
			data.Retry = action;
			return data;
		}

		public static WebRequestSendSettings WithFailedAction(this WebRequestSendSettings data, Action action)
		{
			data.Failed = action;
			return data;
		}

		public static WebRequestSendSettings WithSendAction(this WebRequestSendSettings data, Action action)
		{
			data.Sended = action;
			return data;
		}
	}
}