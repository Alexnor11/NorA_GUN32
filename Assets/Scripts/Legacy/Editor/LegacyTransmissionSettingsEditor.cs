using System;
using Tanks.Tests;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tanks.Legacy.Editor
{
	/// <summary>
	/// Тестовый редактор для отброшенного класса
	/// </summary>
	[CustomEditor(typeof(LegacyTransmissionSettings))]
	[Obsolete("For presentation")]
	public class LegacyTransmissionSettingsEditor : UnityEditor.Editor
	{
		private const string c_dataName = "_data";

		private SerializedProperty _dataProp;

		private float _minTorque;
		private float _maxTorque;
		
		[SerializeField]
		private VisualTreeAsset _view;

		private void OnEnable()
		{
			LegacyTransmissionDataPropertyDrawer.Configuration(_view);
			_dataProp = serializedObject.FindProperty(c_dataName);
		}

		public override VisualElement CreateInspectorGUI()
		{
			var root = new VisualElement();
			var it = serializedObject.GetIterator();
			it.Next(true);
			while (it.NextVisible(false))
			{
				var prop = new PropertyField(it);
				root.Add(prop);
			}

			return root;
		}

		private void OnSceneGUI()
		{
			Handles.color = Color.green;
			Vector3 start = default, end = default;
			var x = default(float);
			for (int i = 0, iMax = _dataProp.arraySize; i < iMax; i++)
			{
				var property = _dataProp.GetArrayElementAtIndex(i);
				var coefA = property.FindPropertyRelative(nameof(LegacyTransmissionData.CoefA)).floatValue;
				var coefB = property.FindPropertyRelative(nameof(LegacyTransmissionData.CoefB)).floatValue;
				var min = property.FindPropertyRelative(nameof(LegacyTransmissionData.MinSpeed)).floatValue;
				var max = property.FindPropertyRelative(nameof(LegacyTransmissionData.MaxSpeed)).floatValue;

				for (; min <= max; min += 0.5f)
				{
					Handles.DrawLine(start, end);
					start = end;
					end = new Vector3(min, LegacyCurveUtility.CalcY(min, coefA, coefB), 0f);
				}
			}
		}
	}
}