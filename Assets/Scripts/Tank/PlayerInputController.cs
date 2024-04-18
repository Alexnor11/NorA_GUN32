using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Tanks
{
	/// <summary>
	/// Обработчик пользовательского ввода
	/// </summary>
	public class PlayerInputController : BaseInputController
	{
		[Inject]
		private TankControls.TankActions _tank;
		[Inject]
		private TankControls.TurretActions _turret;

		[SerializeField, Min(0.1f), Tooltip("Скорость вращения башни")]
		private float _rotateSpeed = 1f;
		
		private void Awake()
		{
			_tank.Handbreak.performed += HandbreakChangeEventHandler;
			_tank.Handbreak.canceled += HandbreakChangeEventHandler;
			_turret.Fire.performed += FireChangeEventHandler;
		}

		/// <summary>
		/// Метод обновления считывания ввода
		/// </summary>
		/// <remarks>Используется вручную в необходимом цикле обновления</remarks>
		public override void ManualUpdate()
		{
			var direction = _tank.Movement.ReadValue<Vector2>();
			Acceleration = direction.y;
			if (direction.x.Equals(0f))
				TankRotate = TankRotate + (TankRotate > 0f ? -_rotateSpeed : _rotateSpeed) * Time.deltaTime;
			else
				TankRotate = TankRotate + direction.x * _rotateSpeed * Time.deltaTime;
			TankRotate = Mathf.Clamp(TankRotate, -1f, 1f);
			TurretRotate = _turret.Focus.ReadValue<Vector2>();
		}
		
		private void HandbreakChangeEventHandler(InputAction.CallbackContext obj)
			=> HandBrake = obj.performed;
		private void FireChangeEventHandler(InputAction.CallbackContext obj)
			=> CallFire();
		
		private void OnDestroy()
		{
			_tank.Handbreak.performed -= HandbreakChangeEventHandler;
			_tank.Handbreak.canceled -= HandbreakChangeEventHandler;
			_turret.Fire.performed -= FireChangeEventHandler;
		}
	}
}
