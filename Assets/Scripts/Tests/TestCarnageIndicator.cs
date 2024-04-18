using Tanks.Interface;
using UnityEngine;

namespace Tanks.Tests
{
	/// <summary>
	/// Тестовый компонент для имитации заполнения шкалы бойни
	/// </summary>
	[RequireComponent(typeof(CarnageIndicator))]
	public class TestCarnageIndicator : MonoBehaviour
	{
#if UNITY_EDITOR
		
		private float _time;
		
		private CarnageIndicator _indicator;

		[SerializeField, Min(1f)]
		private int _points = 1;
		[SerializeField, Range(0.05f, 1f)]
		private float _delay = .2f;
		
		public bool Stopped { get; private set; }
		
		private void Start()
		{
			_indicator = GetComponent<CarnageIndicator>();
		}

		private void Update()
		{
			if (Stopped) return;
			_time -= Time.deltaTime;
			if (_time > 0f) return;

			_time = _delay;
			_indicator.AddPoints(_points);
		}
		
#endif
		
#if UNITY_EDITOR

		[UnityEditor.CustomEditor(typeof(TestCarnageIndicator))]
		private class TestCarnageIndicatorEditor : UnityEditor.Editor
		{
			private TestCarnageIndicator _indicator;
			private GUIContent _startButton;
			private GUIContent _stopButton;

			private void OnEnable()
			{
				_indicator = target as TestCarnageIndicator;
				_startButton = new GUIContent("Start");
				_stopButton = new GUIContent("Stop");
			}

			public override void OnInspectorGUI()
			{
				(Color color, GUIContent content) = _indicator.Stopped
					? (Color.green, _startButton) : (Color.red, _stopButton);
				
				base.OnInspectorGUI();
				if (!Application.isPlaying) return;
				
				UnityEditor.EditorGUILayout.Space(15f);
				UnityEditor.EditorGUILayout.BeginHorizontal("box");
				GUILayout.FlexibleSpace();
				GUI.color = color;
				if (GUILayout.Button(content))
				{
					_indicator.Stopped = !_indicator.Stopped;
				}
				GUI.color = Color.white;
				UnityEditor.EditorGUILayout.EndHorizontal();
			}
		}
#endif
	}
}