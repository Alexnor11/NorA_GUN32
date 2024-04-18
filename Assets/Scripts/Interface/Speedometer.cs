using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace Tanks.Interface
{
    /// <summary>
    /// Индикатор скоростного состояния
    /// </summary>
    public class Speedometer : MonoBehaviour
    {
        private float _time;
        
        [Inject]
        private TankController _player;

        [SerializeField, Tooltip("Максимальная скорость на спидометре")]
        private float _maxSpeed = 180f;
        [SerializeField, Tooltip("Интервал углов поворота стрелки относительно UI разметки")]
        private Vector2 _range = new (90f, -90f);
        [Tooltip("Задержка обновления цифрового циферблата для избегания глюков при округлении")]
        [SerializeField, Min(0f)]
        private float _textSpeedUpdateDelay = 0.25f;
        
        [Header("---References---")]
        [SerializeField, Tooltip("Стрелка циферблата")]
        private RectTransform _arrow;
        [SerializeField, Tooltip("Цифровой циферблат")]
        private TextMeshProUGUI _text;

        private void Start()
        {
            if (_arrow == null || _text == null)
            {
                Debug.LogError("Incorrect init params!", gameObject);
                enabled = false;
            }
        }

        private void Update()
        {
            //Угол поворота стрелки это значение между интвералом наклона
            //считается по дельте равной соотношению текущей к максимальной скорости
            var angle = Mathf.Lerp(_range.x, _range.y, _player.CurrentSpeed / _maxSpeed);
            _arrow.eulerAngles = new Vector3(0f, 0f, angle);

            var time = Time.time;
            //Обновление спидометра
            if (time - _time > _textSpeedUpdateDelay)
            {
                //Округление, чтобы не отображать дробную часть
                _text.text = Math.Round(_player.CurrentSpeed).ToString();
                _time = time;
            }
        }
    }
}