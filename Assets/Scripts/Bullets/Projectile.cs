using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
	/// <summary>
	/// Компонент снаряда, отвечающий за логику взрыва
	/// </summary>
	[RequireComponent(typeof(Rigidbody))]
	public class Projectile : MonoBehaviour
	{
		//Дистанция каста взрыва
		private const float c_castDistance = .1f;
		
		//Оптимизационный массив, для экономии памяти
		private static readonly RaycastHit[] _hits = new RaycastHit[16];
		//Подстраховка, чтобы избежать повторного наложения взрыва на композитных объектах
		private static readonly HashSet<Rigidbody> _set = new(16);
		//Список слоев, которые участвуют в регистрации взрывов
		private static readonly string[] _layerNames = { UnityConstants.ObstaclesLayerName };
		
		//Физическая маска регистрации взрыва
		private int _mask;
		//Задержка перед автодетонацией
		private float _delay;
		//Физтело снаряда
		private Rigidbody _body;

		[Tooltip("Радиус поражающей зоны")]
		[SerializeField, Min(0.1f)]
		private float _explosionRadius = 1.3f;
		[Tooltip("Сила взрыва")]
		[SerializeField, Min(0.1f)]
		private float _explosionForce = 5f;
		[Tooltip("Макимальное время жизни снаряда при выстреле")]
		[SerializeField, Min(1f)]
		private float _lifeTime = 7f;

		/// <summary>
		/// Оповещение о поражении окружения
		/// </summary>
		public event Action<Vector3, int> OnExplosionHandler;

		/// <summary>
		/// Вектор стремления снаряда
		/// </summary>
		public Vector3 Velocity
		{
			get => _body.velocity;
			set => _body.velocity = value;
		}
		
		private void Awake()
		{
			_body = GetComponent<Rigidbody>();
			_mask = LayerMask.GetMask(_layerNames);
		}

		private void OnEnable()
			//Активация таймера при пересоздании снаряда
			=> _delay = _lifeTime;

		private void Update()
		{
			//Уменьшение времени жизни снаряда
			_delay -= Time.deltaTime;
			//Проверка на то, что время жизни закончилось
			if(_delay <= 0f)
				OnExplosion(transform.position);
		}
		
		private void OnCollisionEnter(Collision collision)
		{
			OnExplosion(collision.GetContact(0).point);
		}

		/// <summary>
		/// Обработка взрыва снаряда
		/// </summary>
		/// <param name="explosionPoint">Точка детонации</param>
		private void OnExplosion(in Vector3 explosionPoint)
		{
			var transform = this.transform;
			//Каст зоны поражения с целью поиска всех препятствий в ней
			var count = Physics.SphereCastNonAlloc(transform.position, _explosionRadius, 
				transform.forward, _hits, c_castDistance, _mask);
			//Накладывание ударной волны на пораженные объекты
			for (int i = 0; i < count; i++)
			{
				var body = _hits[i].rigidbody;
				if(body == null) continue;
				//Если в сете нет еще этого твердого тела - добавляем и обрабатываем урон
				if(_set.Add(body))
					body.AddExplosionForce(_explosionForce, explosionPoint, _explosionRadius);
			}
			
			_set.Clear();
			gameObject.SetActive(false);
			//Оповещаем о силе, нанесенной взрывом
			OnExplosionHandler?.Invoke(explosionPoint, Mathf.RoundToInt(_explosionForce * count));
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, _explosionRadius);
		}
	}
}