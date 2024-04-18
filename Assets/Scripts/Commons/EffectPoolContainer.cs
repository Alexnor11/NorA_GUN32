using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
	/// <summary>
	/// Обработчик пулов эффектов
	/// </summary>
	public class EffectPoolContainer : MonoBehaviour
	{
		//Словарь пулов эффектов по инстансАйди
		//На самом деле, несколько спорный выбор, брать инт ключом - возможно, что выигрыша нет по сравнению с ссылкой
		//так как в нативной части Юнити ХешКод их объектов это this.m_InstanceID
		//то есть, вполне допустимо в словаре брять ключом сами Юнити-объекты
		private readonly Dictionary<int, EffectPool> _pools = new (16);

		public ParticleSystem this[ParticleSystem prefab] 
			=> GetPool(prefab).GetElement();

		public ParticleSystem this[int id]
		{
			get
			{
#if UNITY_EDITOR
				if (_pools.TryGetValue(id, out var pool))
					return pool.GetElement();
				
				throw new ApplicationException($"Use {nameof(CreatePool)} method before this[int id]!");
#else
				return _pools[id].GetElement();
#endif
			}
		}

		public EffectPool CreatePool(ParticleSystem prefab)
			=> GetPool(prefab);

		private EffectPool GetPool(ParticleSystem prefab)
		{
			if (prefab.main.stopAction != ParticleSystemStopAction.Disable)
			{
				throw new ApplicationException($"{nameof(EffectPoolContainer)}.{prefab.name} " +
					$"<i>Incorrect Effect Setting:</i> In <b>Main.StopAction</b> need <b>Disable</b> mode!");
			}
			
			var id = prefab.GetInstanceID();
			if (!_pools.TryGetValue(id, out var pool))
			{
				var obj = new GameObject(string.Concat("Pool_", prefab.name));
				var tr = obj.transform;
				tr.parent = transform;
				tr.localScale = Vector3.one;
				tr.SetPositionAndRotation(new Vector3(), Quaternion.identity);
				pool = obj.AddComponent<EffectPool>();
				pool.Init(prefab);
				_pools.Add(id, pool);
			}

			return pool;
		}
	}
}