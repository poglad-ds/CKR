using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.Web
{
	public class WebRequest : IDisposable
	{
		public UnityWebRequest CurrentRequest => _request;

		enum Mode
		{
			Get,
			Post,
			Put,
			Delete
		}

		UnityWebRequest _request;
		string _uri;
		string _data;
		Mode _mode;

		public static WebRequest CreateGet(string uri)
		{
			var obj = new WebRequest
			{
				_request = UnityWebRequest.Get(uri),
				_mode = Mode.Get,
				_uri = uri
			};

			return obj;
		}

		public static WebRequest CreatePost(string uri, string data)
		{
			var obj = new WebRequest
			{
				_request = UnityWebRequest.PostWwwForm(uri, data),
				_mode = Mode.Post,
				_uri = uri,
				_data = data
			};

			return obj;
		}

		public WebRequest Recreate()
		{
			var request = _mode switch
			{
				Mode.Get => CreateGet(_uri),
				Mode.Post => CreatePost(_uri, _data),
				_ => null
			};

			Dispose();
			return request;
		}

		public (bool result, T data) Parse<T>(T reuseableObject = null) where T : class, IJsonObject
		{
			if (!this)
				return (false, null);

			if (reuseableObject is null)
				return (true, JsonUtility.FromJson<T>(CurrentRequest.downloadHandler.text));

			JsonUtility.FromJsonOverwrite(CurrentRequest.downloadHandler.text, reuseableObject);
			return (true, reuseableObject);
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
			return (request.CurrentRequest.result == UnityWebRequest.Result.Success) &&
				request.CurrentRequest.downloadHandler.text != null &&
				request.CurrentRequest.downloadHandler.text != string.Empty;
		}
	}

	public interface IJsonObject { }
}