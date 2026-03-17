using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.Web
{
	public class WebRequest : IDisposable
	{
		public static Dictionary<string, WebRequest> _createdGet = new();
		public static Dictionary<string, WebRequest> _createdPost = new();

		public UnityWebRequest Request => _request;
		UnityWebRequest _request;

		public static WebRequest CreateGet(string uri)
		{
			if (_createdGet.TryGetValue(uri, out var request))
				return request;

			var obj = new WebRequest
			{
				_request = UnityWebRequest.Get(uri)
			};

			_createdGet.Add(uri, obj);
			return obj;
		}

		public static WebRequest CreatePost(string uri, string data)
		{
			if (_createdPost.TryGetValue(uri, out var request))
				return request;

			var obj = new WebRequest
			{
				_request = UnityWebRequest.PostWwwForm(uri, data)
			};

			_createdPost.Add(uri, obj);
			return obj;
		}

		public T Parse<T>(T reuseableObject = null) where T : class, IJsonObject
		{
			if (!this)
				return null;

			if (reuseableObject is null)
				return JsonUtility.FromJson<T>(Request.downloadHandler.text);

			JsonUtility.FromJsonOverwrite(Request.downloadHandler.text, reuseableObject);
			return reuseableObject;
		}

		public void Dispose()
		{
			_request.Dispose();
		}

		public static bool operator true(WebRequest request)
		{
			return Check(request);
		}

		public static bool operator false(WebRequest request)
		{
			return !Check(request);
		}

		public static bool operator !(WebRequest request)
		{
			return !Check(request);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool Check(WebRequest request)
		{
			return (request.Request.result == UnityWebRequest.Result.Success) &&
				request.Request.downloadHandler.text != null &&
				request.Request.downloadHandler.text != string.Empty;
		}
	}

	public interface IJsonObject { }
}