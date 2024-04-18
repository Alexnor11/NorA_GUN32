using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
	/// <summary>
	/// Пул для генерации и хранения источников звука
	/// </summary>
	public class AudioPool : GameObjectPool<AudioSource>
	{
		private readonly LinkedList<AudioSource> _actives = new ();

		/// <summary>
		/// Вызов проигрывания звука
		/// </summary>
		/// <param name="position">Точка проигрыша</param>
		/// <param name="clip">Звуковой клип</param>
		public void PlaySound(Vector3 position, AudioClip clip)
		{
			var element = base.GetElement();
			_actives.AddLast(element);
			element.transform.position = position;
			element.clip = clip;
			element.Play();
		}

		private void Update()
		{
			if (_actives.Count == 0) return;
			var it = _actives.First;
			while (it != null)
			{
				var current = it;
				it = it.Next;
				
				if (current.Value.isPlaying) continue;
				current.Value.gameObject.SetActive(false);
				_actives.Remove(current);
			}
		}
	}
}