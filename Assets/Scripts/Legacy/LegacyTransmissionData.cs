using System;
using UnityEngine;

namespace Tanks.Legacy
{
	[Serializable, Obsolete]
	public struct LegacyTransmissionData
	{
		[Min(0f)]
		public float CoefA;
		[Min(0f)]
		public float CoefB;
		[Min(0f)]
		public float MinSpeed;
		[Min(0f)]
		public float MaxSpeed;
	}
}