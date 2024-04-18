using Unity.Collections;
using UnityEngine;

namespace Tanks.Tank
{
	/// <summary>
	/// Система упрощенного отслеживания предполагаемой траектории полета снаряда
	/// </summary>
	public class ProjectileTracker : MonoBehaviour
	{
		private TurretController _controller;
		private NativeArray<Vector3> _points;

		[SerializeField]
		private LineRenderer _liner;
		[SerializeField, Range(2f, 100f)]
		private int _count = 5;
		[SerializeField, Range(0.1f, 2f)]
		private float _step = .5f;


		private void Update()
		{
			//Расчеты не берут во внимание физматериал и трения
			var transform = this.transform;
			var deltaTime = _step;
			var gravity = Physics.gravity;
			var velocity = transform.forward * _controller.Velocity;
			var position = transform.position;
			_points[0] = position;
			var i = 1;
			for (; i < _points.Length; i++)
			{
				var nextVelocity = velocity + gravity * deltaTime;
				var nextPosition = position + velocity * deltaTime;
				_points[i] = nextPosition;
				var direction = nextPosition - position;

				if (Physics.Raycast(position, direction, out var hit, Vector3.Magnitude(direction), -1,
					    QueryTriggerInteraction.Collide))
				{
					_points[i] = hit.point;
					i++;
					break;
				}

				position = nextPosition;
				velocity = nextVelocity;
			}

			_liner.positionCount = i;
			_liner.SetPositions(_points.Slice(0, i));
		}

		private void Awake()
		{
			_points = new NativeArray<Vector3>(_count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			_controller = FindObjectOfType<TurretController>();
			if (_controller == null || _liner == null)
			{
				Debug.Log("Incorrect settings!", this);
				enabled = false;
			}
		}

		private void OnDestroy()
		{
			if (_points.IsCreated) _points.Dispose();
		}
	}
}