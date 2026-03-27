using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJamCore
{
	/// <summary>
	/// 型ベースの Service Locator。
	/// インターフェースをキーにしてサービスを登録・取得する。
	/// </summary>
	public static class ServiceLocator
	{
		private static readonly Dictionary<Type, object> _services = new();

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetForPlayMode()
		{
			_services.Clear();
		}

		/// <summary>
		/// サービスを登録する。同じ型で再登録すると上書きされる。
		/// </summary>
		public static void Register<T>(T service)
		{
			_services[typeof(T)] = service;
		}

		/// <summary>
		/// サービスを取得する。未登録の場合は例外を投げる。
		/// </summary>
		public static T Get<T>()
		{
			if (_services.TryGetValue(typeof(T), out var service))
			{
				return (T)service;
			}

			throw new InvalidOperationException(
				$"[ServiceLocator] {typeof(T).Name} is not registered. "
				+ $"Register it in GameInitializer.Awake() etc."
			);
		}

		/// <summary>
		/// サービスの取得を試みる。未登録の場合は false を返す。
		/// </summary>
		public static bool TryGet<T>(out T service)
		{
			if (_services.TryGetValue(typeof(T), out var obj))
			{
				service = (T)obj;
				return true;
			}

			service = default;
			return false;
		}

		/// <summary>
		/// 特定のサービスを解除する。
		/// </summary>
		public static void Unregister<T>()
		{
			_services.Remove(typeof(T));
		}
	}
}
