using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tanks
{
	/// <summary>
	/// Кастомный провайдер для работы Cinemachine с новой системой ввода
	/// </summary>
	/// <remarks>Пример для студентов, если потребуется открепить камеру от танка и управлять через новую систему ввода</remarks>
	public class CinemachineInputCustomProvider : MonoBehaviour, AxisState.IInputAxisProvider
	{
		private float _x;
		private float _y;
		
		[SerializeField]
		private InputAction _horizontal;
		[SerializeField]
		private InputAction _vertical;

		public virtual float GetAxisValue(int axis)
		{
			switch (axis)
			{
				case 0:
					var x = _x;
					_x = 0f;
					return x;
				case 1:
					var y = _y;
					_y = 0f;
					return y;
				case 2:
					return _vertical.ReadValue<float>();
			}

			return 0f;
		}

		private void Update()
		{
			var vector = _horizontal.ReadValue<Vector2>();
			_x += vector.x;
			_y += vector.y;
		}
        
		private void OnEnable()
		{
			_horizontal.Enable();
			_vertical.Enable();
		}

		private void OnDisable()
		{
			_horizontal.Disable();
			_vertical.Disable();
		}
	}
}