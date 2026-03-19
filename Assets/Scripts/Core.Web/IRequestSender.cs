using System.Threading;
using UnityEngine;

namespace Core.Web
{
	public interface IRequestSender<T>
	{
		public Awaitable<(bool success, T result)> Request(CancellationTokenSource cancellationToken);
	}
}