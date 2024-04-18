using System;
using System.Collections;
using UnityEngine;

namespace Tanks.Tests
{
	//todo сюда нужно еще логику поворота пушки и выстрел сделать
	public class TestInputController : BaseInputController
	{
		private float _delay;
		
		[Tooltip("Сила нажатия педали газа при стремлении к цели")]
		[SerializeField, Range(0f, 1f)]
		private float _accelerationPower = 1f;
		[SerializeField, Range(0f, 30f)]
		private float _fireDelaySec = 1f;

		private void Start()
		{
			_delay = _fireDelaySec;
		}

		private void Update()
		{
			Acceleration = _accelerationPower;
			_delay -= Time.deltaTime;
			if (_delay <= 0f)
			{
				CallFire();
				_delay = _fireDelaySec;
			}
		}

		public override void ManualUpdate() { }
	}
}