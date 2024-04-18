using UnityEngine;

namespace Tanks
{
	/// <summary>
	/// Пул для хранения и создания эффектов 
	/// </summary>
	public class EffectPool : GameObjectPool<ParticleSystem>
	{
		public void Init(ParticleSystem prefab, int preInitCount = 4)
		{
			_prefab = prefab;
			CallInitialize(null, preInitCount);
		}
	}
}