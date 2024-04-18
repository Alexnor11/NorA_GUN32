using UnityEngine;

namespace Tanks
{
	/// <summary>
	/// Создает вращение неба для живости картинки
	/// </summary>
	public class SkyRotation : MonoBehaviour
	{
		private float _value;
		private Material _skyBox;

		private static readonly int RotationProperty = Shader.PropertyToID("_Rotation");
		
		[SerializeField, Min(0f)]
		[Tooltip("Скорость вращения неба, может быть отрицательной")]
		private float _rotateSpeed = 1f;

		private void Start()
		{
			_skyBox = new Material(RenderSettings.skybox);
			RenderSettings.skybox = _skyBox;
		}

		private void LateUpdate()
		{
			_value = (_value + _rotateSpeed * Time.deltaTime) % 360f;
			_skyBox.SetFloat(RotationProperty, _value);
		}
	}
}