using System;
using UnityEngine;

namespace Tanks
{
	/// <summary>
	/// Базовый класс для обработки инпута танка
	/// </summary>
	public abstract class BaseInputController : MonoBehaviour
	{
		/// <summary>
		/// Ускорение, определяющее подачу топлива в ДВС
		/// </summary>
		/// <remarks>Изменяется в диапазоне от -1 (задний ход) до 1 (полный ход)</remarks>
		public float Acceleration { get; protected set; }
		/// <summary>
		/// Степень поворота руля, не зависит от максимального угла поворота колес
		/// </summary>
		/// <remarks>Изменяется в диапазоне от -1 (лево) до 1 (право)</remarks>
		public float TankRotate { get; protected set; }
		/// <summary>
		/// Ручной тормоз, он либо выжат, либо нет
		/// </summary>
		public bool HandBrake { get; protected set; }
		/// <summary>
		/// Смещение пушки
		/// </summary>
		public Vector2 TurretRotate { get; protected set; }

		/// <summary>
		/// Событие выстрела из орудия
		/// </summary>
		public event Action OnFireEvent;

		/// <summary>
		/// Обновление данных инпута
		/// </summary>
		/// <remarks>Упрощает работу, в случае смены типа цикла обновления</remarks>
		public abstract void ManualUpdate();

		protected void CallFire() => OnFireEvent?.Invoke();
	}
}
