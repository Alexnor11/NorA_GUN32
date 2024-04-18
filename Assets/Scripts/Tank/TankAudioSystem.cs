using UnityEngine;
using Zenject;

namespace Tanks
{
	public class TankAudioSystem : MonoBehaviour
	{
		[Inject]
		private TankController _tank;
		
		[Header("---References---")]
		[SerializeField, Tooltip("Источник звука двигателя танка")]
		private AudioSource _motorAudioSource;
		[SerializeField, Tooltip("Звук работающего под нагрузкой двигателя")]
		private AudioClip _workingAudioClip;

		[Header("---Parameters---")]
		[Tooltip("Громкость источника звука двигателя")]
		[SerializeField]
		private Vector2 _engineVolumeInterval = new (.1f, .7f);
		[Tooltip("Скорость проигрывания звука двигателя")]
		[SerializeField]
		private Vector2 _enginePitchInterval = new(.6f, 1.5f);
		[Tooltip("Скорость, при которой двигатель максимально шумит")]
		[SerializeField, Range(0f, 200f)]
		private float _maxTankSpeed = 110f;
		
		private void Start()
		{
			if (_motorAudioSource == null
			 || _workingAudioClip == null)
			{
				enabled = false;
				Debug.LogError($"<b>{nameof(TankAudioSystem)}</b>: Incorrect configuration!", this);
			}

			if (!_motorAudioSource.loop)
			{
				_motorAudioSource.loop = true;
				Debug.LogError("Motor audio source not loop!", _motorAudioSource);
			}

			_motorAudioSource.clip = _workingAudioClip;
			_motorAudioSource.Play();
		}

		private void Update()
		{
			var delta = Mathf.InverseLerp(0f, _maxTankSpeed, _tank.CurrentSpeed);
			_motorAudioSource.volume = Mathf.Lerp(_engineVolumeInterval.x, _engineVolumeInterval.y, delta);
			_motorAudioSource.pitch = Mathf.Lerp(_enginePitchInterval.x, _enginePitchInterval.y, delta);
		}
	}
}