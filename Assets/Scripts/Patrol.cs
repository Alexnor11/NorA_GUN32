using System;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tanks
{
	/// <summary>
	/// Система патрулирования, основанная на расширении Cinemachine
	/// </summary>
	[RequireComponent(typeof(CinemachinePath))]
	public class Patrol : MonoBehaviour
	{
		//Рандомно выставленная скорость патрулирования
		private float _speed;
		//Текущее значение пройденного пути
		private float _value;
		
		[SerializeField, Tooltip("Настроенный путь движения")]
		private CinemachinePath _path;
		[SerializeField, Tooltip("Цель, двигаемая по пути")]
		private Transform _target;

		[Tooltip("Минимальная скорость движения по пути")]
		[SerializeField, Range(0.1f, 1f)]
		private float _minSpeed = 0.2f;
		[Tooltip("Максимальная скорость движения по пути")]
		[SerializeField, Range(0.1f, 1f)]
		private float _maxSpeed = 8f;

		private void Start()
		{
			_speed = Random.Range(_minSpeed, _maxSpeed);
		}

		private void Update()
		{
			_value += _speed * Time.deltaTime;
			_target.position = _path.EvaluatePosition(_value);
			_target.rotation = _path.EvaluateOrientation(_value);
		}
	}
}