using UnityEngine;

namespace Foxlair.Tools
{
	public abstract class PersistentSingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{
		public static T Instance { get; private set; }

		public virtual void Awake()
		{
			if (Instance == null)
			{
				//Debug.Log($"Instantiating new singleton of type {typeof(T)} on {gameObject} game object.");
				Instance = this as T;
				DontDestroyOnLoad(this);
			}
			else
			{
				Debug.LogWarning($"Deleting singleton of type {typeof(T)} because it already exists on {gameObject} game object.");
				Destroy(gameObject);
			}
		}

	}
}