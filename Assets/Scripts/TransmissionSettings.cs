using System;
using UnityEngine;

namespace Tanks
{
	/// <summary>
	/// Сеттинги трансмиссии двигателя танка
	/// </summary>
	[CreateAssetMenu(menuName = "Settings/Transmission", fileName = "Transmission", order = 0)]
	public class TransmissionSettings : ScriptableObject
	{
		private const float c_errorEngine = .5f;
		private const float c_errorTorque = 100f;
		
		[SerializeField, Tooltip("Графики зависимости мощности от скорости по передачам")]
		private AnimationCurve[] _data;

		/// <summary>
		/// Расчет скорости вращения двигателя на определенной физскорости
		/// </summary>
		/// <param name="speed">Скорость перемещения</param>
		/// <returns>[0, 1] - раскрутка ротора на текущей передачи</returns>
		public float EngineSpeed(float speed)//todo не использую, заменил на аудио систему
		{
			if (FindData(speed, out var curve))
			{
				return Mathf.InverseLerp(curve.keys[0].time, curve.keys[^1].time, speed);
			}
			return c_errorEngine;
		}
		
		/// <summary>
		/// Получить крутящий момент
		/// </summary>
		/// <param name="speed">Скорость танка</param>
		/// <returns>КРутящий момент, выдаваемый двигателем</returns>
		public float GetTorque(float speed)
			=> FindData(speed, out var curve)
				? curve.Evaluate(speed)
				: c_errorTorque;

		private bool FindData(float speed, out AnimationCurve curve)
		{
			bool compare(AnimationCurve curve, float value)
			{
				var keys = curve.keys;
				return value >= keys[0].time && value <= keys[^1].time;
			}
			
			var index = Array.FindIndex(_data, t => compare(t, speed));
			if (index == -1)
			{
				Debug.LogError($"Incorrect <b>{nameof(TransmissionSettings)}<b> configuration. \nWrong request: <b>{speed}</b>");
				curve = null;
				return false;
			}

			curve = _data[index];
			return true;
		}
	}
}