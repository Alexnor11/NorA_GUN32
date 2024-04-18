using Tanks.Interface;
using UnityEngine;
using Zenject;

namespace Tanks
{
	/// <summary>
	/// Таран, позволяющий танку регистрировать очки резни от ударов
	/// </summary>
	[RequireComponent(typeof(Rigidbody))]
	public class BattleRam : MonoBehaviour
	{
		[Inject]
		private CarnageIndicator _indicator;

		[SerializeField, Min(0.001f), Tooltip("Множитель импульса удара для конвеерсии в очки резни")]
		private float _mult = .1f;

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.gameObject.layer != UnityConstants.ObstaclesLayerInt) return;
			
			_indicator.AddPoints(Mathf.RoundToInt(Vector3.Magnitude(collision.impulse) * _mult));
		}
	}
}