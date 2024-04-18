using UnityEngine;

namespace Tanks.Interface
{
	/// <summary>
	/// Сеттинги изменения счетчика бойни при наборе очков
	/// </summary>
	[CreateAssetMenu(menuName = "Settings/Indicator", fileName = "Indicator", order = 1)]
	public class CarnageSettings : ScriptableObject
	{
		[field: SerializeField]
		[Tooltip("Кол-во должно подгоняться под кол-во делений на индикаторе")]
		public CarnagePreset[] Settings { get; private set; } = new CarnagePreset[6];
		
		[System.Serializable]
		public struct CarnagePreset
		{
			[Range(0.1f, 10f)]
			public float Mult;
			public string Text;
			public AnimationClip Clip;
		}
	}
}