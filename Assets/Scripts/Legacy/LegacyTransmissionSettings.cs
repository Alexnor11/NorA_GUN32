using System;
using UnityEngine;

namespace Tanks.Legacy
{
	[Obsolete("Use actual TransmissionSettings")]
	public class LegacyTransmissionSettings : MonoBehaviour
	{
		private const float c_errorTorque = 100f;

		[SerializeField, Min(0f)]
		private float _torqueOffset = 30f;
		[SerializeField]
		private LegacyTransmissionData[] _data;

		

		public float GetTorque(float speed)
		{
			var index = Array.FindIndex(_data, t => t.MinSpeed >= speed && t.MaxSpeed <= speed);
			if (index == -1)
			{
				Debug.LogError($"Incorrect <b>{nameof(LegacyTransmissionSettings)}<b> configuration. \nWrong request: <b>{speed}</b>");
				return c_errorTorque;
			}

			return LegacyCurveUtility.CalcY(speed, _data[index].CoefA, _data[index].CoefB);
		}
	}
}