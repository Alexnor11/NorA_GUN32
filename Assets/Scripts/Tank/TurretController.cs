using Tanks.Interface;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Tanks
{
	/// <summary>
	/// Контроллер башни
	/// </summary>
	public class TurretController : MonoBehaviour
	{
		private float _fireTime = -5f;
		//Ссылка на наш собственный трансформ
		private Transform _transform;
		//Ссылка на 3D визуализацию перезарядки
		private ReloadIndicator _reloader;
		//Минимальный угол наклона ствола
		private Quaternion _min;
		//Максимальный угол наклона ствола
		private Quaternion _max;
		//Текущее смещение наклона угла ствола от минимума к максимуму
		private float _gunAngleDelta;
		//Флаг - нашли-ли мы физтело танка (для отдачи)
		private bool _findBody;
		//Флаг - нашли-ли мы 3D индикатор перезарядки
		private bool _findReloader;

		//Пул эффектов взрыва
		private EffectPool _hitEffects;
		//Ссылка на физтело танка
		private Rigidbody _body;

		[Inject]
		private CarnageIndicator _carnageIndicator;
		[Inject]
		private ProjectilePool _pool;
		[Inject]
		private AudioPool _audioPool;

		
		[Header("---Turret Params---")]
		[SerializeField, Tooltip("Ось ствола башни")]
		private Transform _gunAxis;
		[SerializeField, ReadOnly(ReadOnlyAttribute.ReadOnlyMode.Runtime)]
		[Tooltip("Граничный интервал наклона ствола башни")]
		private Interval _gunAngleLimit = new() { Min = -90f, Max = 90f };

		[Tooltip("Скорость вращения башни")]
		[SerializeField, Space]
		private float _horRotateSpeed = 0.1f;
		[Tooltip("Скорость наклона ствола")]
		[SerializeField]
		private float _vertRotateSpeed = 0.1f;
		
		[field: Header("---Gun Params---")]
		[field: SerializeField, Space]
		[field: Tooltip("Точка выстрела")]
		public Transform FirePoint { get; private set; }
		[SerializeField]
		private BaseInputController _controller;
		[field: SerializeField, Min(0.1f)]
		[field: Tooltip("Скорость, вылетаемого снаряда")]
		public float Velocity { get; private set; } = 100f;
		[SerializeField, Range(0.05f, 5f)]
		[Tooltip("Время перезарядки орудия в секундах")]
		private float _reloadDelay = 1.5f;
		[SerializeField, Range(1f, 50f)]
		[Tooltip("Кол-во выпускаемых частиц дыма при выстреле")]
		private int _emitSmokeCount = 3;
		[SerializeField, Min(0f), Tooltip("Сила, прикладываемая к танку в противоположную сторону выстрела")]
		private float _firePower = 120_000f;

		[Header("---Effect Refs---")]
		[SerializeField, Space, Tooltip("Эффект выстрела у дула")]
		private ParticleSystem _muzzleEffect;
		[SerializeField, Tooltip("Эффект дыма от выстрела")]
		private ParticleSystem _smokeEffect;
		[SerializeField, Tooltip("Префаб эффекта взрыва снаряда")]
		private ParticleSystem _hitEffect;
		[SerializeField, Tooltip("Звук выстрела")]
		private AudioClip _fireSound;
		[SerializeField, Tooltip("Звук взрыва снаряда")]
		private AudioClip _hitSound;
		
		[Inject]
		private void Construct(EffectPoolContainer container)
		{
			_hitEffects = container.CreatePool(_hitEffect);
		}
		
		private void Awake()
		{
			_body = GetComponentInParent<Rigidbody>();
			_findBody = _body != null;
			
			_reloader = GetComponentInChildren<ReloadIndicator>();
			_findReloader = _reloader != null;

			if (_controller == null)
			{
				Debug.LogError("Turret without controller", this);
				enabled = false;
			}
			
			_controller.OnFireEvent += FireOnPerformed;
			_transform = transform;
			_min = Quaternion.Euler(_gunAngleLimit.Min, 0f, 0f);
			_max = Quaternion.Euler(_gunAngleLimit.Max, 0f, 0f);
			if (_pool.IsBroken)
			{
				Debug.LogError("Pool is broken! Need reference", _pool);
				return;
			}
			_pool.CallInitialize(t => t.OnExplosionHandler += ProjectileExplosion, 128);
		}

		private void Update()
		{
			var deltaTime = Time.deltaTime;
			var delta = _controller.TurretRotate;
			_transform.RotateAround(_transform.position, _transform.up, (delta.x * _horRotateSpeed * deltaTime));

			_gunAngleDelta = Mathf.Clamp01(_gunAngleDelta - delta.y * _vertRotateSpeed * deltaTime);
			_gunAxis.localRotation = Quaternion.Lerp(_min, _max, _gunAngleDelta);
		}
		
		private void FireOnPerformed()
		{
			var time = Time.time;
			if (time - _fireTime < _reloadDelay) return;
			if(_findReloader) _reloader.SetReload(_reloadDelay);
			_fireTime = time;

			//Set direction and position
			var bullet = _pool.GetElement();
			var forward = FirePoint.forward;
			var position = FirePoint.position;
			bullet.Velocity = forward * Velocity;
			var transform = bullet.transform;
			transform.position = position;
			transform.forward = forward;
			//Create fire effect
			_muzzleEffect.Play();
			_smokeEffect.Emit(_emitSmokeCount);
			//Shot audio
			if(!_audioPool.IsBroken)
				_audioPool.PlaySound(position, _fireSound);
			//Add force
			if(_findBody)
				_body.AddForce(this.transform.forward * -_firePower);
		}
		
		private void ProjectileExplosion(Vector3 point, int damage)
		{
			_carnageIndicator.AddPoints(damage);
			var fire = _hitEffects.GetElement();
			fire.transform.position = point;
			fire.Play();
			if(!_audioPool.IsBroken)
				_audioPool.PlaySound(point, _hitSound);
		}
	}

}
