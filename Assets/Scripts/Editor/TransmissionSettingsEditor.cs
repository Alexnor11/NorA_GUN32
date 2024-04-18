using UnityEditor;
using UnityEngine;

namespace Tanks.Editor
{
	[CustomEditor(typeof(TransmissionSettings))]
	public class TransmissionSettingsEditor : UnityEditor.Editor
	{
		private const string c_dataName = "_data";
		private const string c_styleName = "box";

		private const string c_foldoutInfoName = "Curve Information";
		private const string c_foldoutAddName = "Add New Transmission";
		private const string c_startName = "Start:";
		private const string c_endName = "End:";
		private const string c_speedName = "Speed (x)";
		private const string c_torqueName = "Torque (y)";
		private const string c_buttonName = "Create";

		private readonly Color _additionalColorBox = new(.5f, .5f, .5f, .25f);
		private readonly Color _mainColorBox = new(1f, 0.92156863f, .0f, 0.25f);
		private readonly GUILayoutOption[] _labelOptions = new GUILayoutOption[1];
		private readonly GUILayoutOption[] _fieldOptions = new GUILayoutOption[1];
		
		private SerializedProperty _dataProp;
		private GUIContent _foldoutInfoContent;
		private GUIContent _foldoutAddContent;
		private GUIContent _startLabelContent;
		private GUIContent _endLabelContent;
		private GUIContent _speedLabelContent;
		private GUIContent _torqueLabelContent;
		private GUIContent _buttonContent;


		private bool _foldoutInfo;
		private bool _foldoutAdd;
		private Vector2 _start;
		private Vector2 _end;

		private void OnEnable()
		{
			_dataProp = serializedObject.FindProperty(c_dataName);
			_foldoutInfoContent = new GUIContent(c_foldoutInfoName);
			_foldoutAddContent = new GUIContent(c_foldoutAddName);
			_startLabelContent = new GUIContent(c_startName);
			_endLabelContent = new GUIContent(c_endName);
			_speedLabelContent = new GUIContent(c_speedName);
			_torqueLabelContent = new GUIContent(c_torqueName);
			_buttonContent = new GUIContent(c_buttonName);
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var style = new GUIStyle(c_styleName);
			var width = EditorGUIUtility.currentViewWidth;
			_labelOptions[0] = GUILayout.Width(width * .1f);
			_fieldOptions[0] = GUILayout.ExpandWidth(true);
			serializedObject.Update();
			
			EditorGUILayout.Space(10f);
			GUI.color = _mainColorBox;
			EditorGUILayout.BeginVertical(style);
			GUI.color = Color.white;
			if (_foldoutInfo = EditorGUILayout.Foldout(_foldoutInfo, _foldoutInfoContent))
				DrawCurveInfo(style);
			if (_foldoutAdd = EditorGUILayout.Foldout(_foldoutAdd, _foldoutAddContent))
				DrawFoldout(style);
			EditorGUILayout.EndVertical();
		}

		private void DrawCurveInfo(GUIStyle style)
		{
			EditorGUILayout.LabelField("Min(Speed / Torque) -> Max(Speed / Torque)");
			for (int i = 0, iMax = _dataProp.arraySize; i < iMax; i++)
			{
				EditorGUILayout.BeginHorizontal(style);
				var keys = _dataProp.GetArrayElementAtIndex(i).animationCurveValue.keys;
				var valueMax = keys[^1];
				EditorGUILayout.LabelField($"Min({keys[0].time} / {keys[0].value}) -> Max({valueMax.time} / {valueMax.value})");
				EditorGUILayout.EndHorizontal();
			}
			//GUI.color = _additionalColorBox;
			//EditorGUILayout.BeginVertical(style);
			//GUI.color = Color.white;
			
			//EditorGUILayout.EndVertical();
		}
		
		private void DrawFoldout(GUIStyle style)
		{
			//Start
			GUI.color = _additionalColorBox;
			EditorGUILayout.BeginHorizontal(style);
			GUI.color = Color.white;
			EditorGUILayout.LabelField(_startLabelContent, _labelOptions);
			EditorGUILayout.LabelField(_speedLabelContent, _labelOptions);
			_start.x = EditorGUILayout.FloatField(_start.x, _fieldOptions);
			EditorGUILayout.LabelField(_torqueLabelContent, _labelOptions);
			_start.y = EditorGUILayout.FloatField(_start.y, _fieldOptions);
			EditorGUILayout.EndHorizontal();
			//End
			GUI.color = _additionalColorBox;
			EditorGUILayout.BeginHorizontal(style);
			GUI.color = Color.white;
			EditorGUILayout.LabelField(_endLabelContent, _labelOptions);
			EditorGUILayout.LabelField(_speedLabelContent, _labelOptions);
			_end.x = EditorGUILayout.FloatField(_end.x, _fieldOptions);
			EditorGUILayout.LabelField(_torqueLabelContent, _labelOptions);
			_end.y = EditorGUILayout.FloatField(_end.y, _fieldOptions);
			EditorGUILayout.EndHorizontal();
			//Button
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUI.color = Color.green;
			if (GUILayout.Button(_buttonContent))
			{
				var size = _dataProp.arraySize;
				_dataProp.arraySize = size + 1;
				var element = _dataProp.GetArrayElementAtIndex(size);
				element.animationCurveValue = AnimationCurve.EaseInOut(_start.x, _start.y, _end.x, _end.y);
				_start = _end = Vector2.zero;
				serializedObject.ApplyModifiedProperties();
			}
			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal();
		}
	}
}