using System;
using Tanks.Tests;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Tanks.Legacy.Editor
{
	[CustomPropertyDrawer(typeof(LegacyTransmissionData))]
	[Obsolete]
	public class LegacyTransmissionDataPropertyDrawer : PropertyDrawer
	{
		private const float c_step = .1f;
		
		private const string c_coefAField = "CoefAField";
		private const string c_coefALowButton = "CoefALowButton";
		private const string c_coefAUpButton = "CoefAUpButton";
		private const string c_coefBField = "CoefBField";
		private const string c_coefBLowButton = "CoefBLowButton";
		private const string c_coefBUpButton = "CoefBUpButton";
		
		private const string c_minSpeedField = "MinSpeedField";
		private const string c_maxSpeedField = "MaxSpeedField";
		
		private const string c_minTorqueField = "MinTorqueField";
		private const string c_maxTorqueField = "MaxTorqueField";
		
		private static VisualTreeAsset _view;

		private SerializedProperty _coefAProperty;
		private SerializedProperty _coefBProperty;
		private SerializedProperty _minSpeedProperty;
		private SerializedProperty _maxSpeedProperty;
		private FloatField _minTorque;
		private FloatField _maxTorque;

		public event Action OnUpdateParameterHandler;
		
		internal static void Configuration(VisualTreeAsset view)
			=> _view = view;
		
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var view = _view.Instantiate();
			//Bind CoefA
			_coefAProperty = property.FindPropertyRelative(nameof(LegacyTransmissionData.CoefA));
			var coefAField = view.Q<FloatField>(c_coefAField);
			coefAField.BindProperty(_coefAProperty);
			coefAField.RegisterCallback<ChangeEvent<float>>(OnChange);
			var button = view.Q<Button>(c_coefALowButton);
			button.clickable = new Clickable(_ => LowClick(coefAField));
			button = view.Q<Button>(c_coefAUpButton);
			button.clickable = new Clickable(_ => UpClick(coefAField));
			//Bind CoefB
			_coefBProperty = property.FindPropertyRelative(nameof(LegacyTransmissionData.CoefB));
			var coefBField = view.Q<FloatField>(c_coefBField);
			coefBField.BindProperty(_coefBProperty);
			coefBField.RegisterCallback<ChangeEvent<float>>(OnChange);
			button = view.Q<Button>(c_coefBLowButton);
			button.clickable = new Clickable(_ => LowClick(coefBField));
			button = view.Q<Button>(c_coefBUpButton);
			button.clickable = new Clickable(_ => UpClick(coefBField));
			//Bind Speed
			_minSpeedProperty = property.FindPropertyRelative(nameof(LegacyTransmissionData.MinSpeed));
			var field = view.Q<FloatField>(c_minSpeedField);
			field.BindProperty(_minSpeedProperty);
			field.RegisterCallback<ChangeEvent<float>>(OnChange);
			_maxSpeedProperty = property.FindPropertyRelative(nameof(LegacyTransmissionData.MaxSpeed));
			field = view.Q<FloatField>(c_maxSpeedField);
			field.BindProperty(_maxSpeedProperty);
			field.RegisterCallback<ChangeEvent<float>>(OnChange);
			//Bind torque
			_minTorque = view.Q<FloatField>(c_minTorqueField);
			_minTorque.SetEnabled(false);
			_maxTorque = view.Q<FloatField>(c_maxTorqueField);
			_maxTorque.SetEnabled(false);
			return view;
		}

		private void LowClick(FloatField field)
		{
			var value = field.value;
			value -= c_step;
			if (value < 0f) value = 0f;
			field.value = value;
		}
		
		private void UpClick(FloatField field)
		{
			field.value += c_step;
		}
		
		private void OnChange(ChangeEvent<float> arg)
		{
			_minTorque.value = LegacyCurveUtility.CalcY(_minSpeedProperty.floatValue, _coefAProperty.floatValue,
				_coefBProperty.floatValue);
			_maxTorque.value = LegacyCurveUtility.CalcY(_maxSpeedProperty.floatValue, _coefAProperty.floatValue,
				_coefBProperty.floatValue);
			
			OnUpdateParameterHandler?.Invoke();
		}
	}
}

