using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Tanks.Interface
{
	/// <summary>
	/// Менеджер по управлению очками
	/// </summary>
	public class CarnageIndicator : MonoBehaviour
	{
		//Имеется-ли правильно настроенный источник звука
		private bool _hasAudio;
		//Минимальный размер шрифта текста суммы очков
		//для анимирования добавления очков
		private float _minSumFontSize;
		//Предыдущий индекс настройки из пресета анимаций индикатора
		private int _prevIndex;
		//Задержка между перетеканиями очков из индикатора в сумму
		private float _overflowDelay;
		//Текущее кол-во очков в индикаторе
		private float _currentPoints;
		//Текущее кол-во очков в сумме
		private float _sumPoints;

		//Анимационный проигрыватель у сообщения индикатора
		private Animation _messageAnim;

		[Header("---References---")]
		[SerializeField, Tooltip("Поле суммы заработанных очков")]
		private TextMeshProUGUI _sum;
		[SerializeField, Tooltip("Поле текущего пула очков для перетекания в сумму")]
		private TextMeshProUGUI _currentPointsText;
		[SerializeField, Tooltip("Поле с текстовым сообщением по заполнению индикатора")]
		private TextMeshProUGUI _message;
		[SerializeField, Tooltip("Шкала заполнения индикатора")]
		private Image _indicatorFill;
		[SerializeField, Tooltip("Звуковой проигрыватель счетчика суммы")]
		private AudioSource _tickerSum;
		[SerializeField, Tooltip("Звуки обновления статуса индикатора")]
		private AudioClip[] _indicatorUpdateClips;

		[SerializeField, Tooltip("Цветовой градиент шкалы индикатора"), Space]
		private Gradient _indicatorGradient;
		[SerializeField, Tooltip("Кол-во очков для заполнения всей шкалы индикатора")]
		private int _indicatorMaxPoints = 1_000;
		[SerializeField, Tooltip("Громкость звука переключения индикатора"), Range(0f, 100f)]
		private float _switchIndicatorVolume = 20f;
		
		[SerializeField, Tooltip("Задержка между перетеканиями очков из индикатора в сумму"), Space, Range(.1f, 100f)]
		private float _overflowPointsPerSecond = .2f;
		[SerializeField, Tooltip("Максимальный размер шрифта поля суммы"), Range(5f, 100f)]
		private float _maxSumFontSize = 45f;
		[SerializeField, Tooltip("Настройки заполнения индикатора")]
		private CarnageSettings _settings;

		/// <summary>
		/// Текущее кол-во очков в индикаторе
		/// </summary>
		public int CurrentPoints => Mathf.RoundToInt(_currentPoints);

		/// <summary>
		/// Текущее кол-во очков в сумме
		/// </summary>
		public float Sum => Mathf.RoundToInt(_sumPoints);

		/// <summary>
		/// Текущий множитель очков при перетекании
		/// </summary>
		public float CurrentMult { get; private set; } = 1f;

		/// <summary>
		/// Добавить очков в индикатор
		/// </summary>
		/// <param name="value">Кол-во добавляемых очков</param>
		public void AddPoints(int value)
		{
			_currentPoints += value;
			UpdateIndicator();
		}

		private void Start()
		{
			if (_sum == null 
			|| _currentPointsText == null
			|| _message == null
			|| _indicatorFill == null
			|| _settings == null)
			{
				Debug.LogError("Incorrect settings in Indicator!", gameObject);
				enabled = false;
				return;
			}
			//prepare UI
			_message.text = _sum.text = _currentPointsText.text = string.Empty;
			_indicatorFill.fillAmount = 0f;
			_indicatorFill.color = _indicatorGradient.Evaluate(0f);

			_hasAudio = _tickerSum != null;
			if(_hasAudio && _tickerSum.clip != null && !_tickerSum.playOnAwake && !_tickerSum.loop) { }
			else
			{
				Debug.Log("Incorrect AudioSource!", gameObject);
				_hasAudio = false;
			}
			
			_messageAnim = _message.GetComponent<Animation>();
			if (_messageAnim == null)
			{
				_messageAnim = _message.gameObject.AddComponent<Animation>();
				_messageAnim.playAutomatically = false;
				Debug.Log($"Auto add {nameof(Animation)} component for Message object!");
			}
			
			var mults = _indicatorFill.GetComponentsInChildren<TextMeshProUGUI>();
			if (mults.Length != _settings.Settings.Length)
			{
				Debug.LogError("Incorrect settings in Indicator!", _settings);
				enabled = false;
				return;
			}

			_minSumFontSize = _sum.fontSize;
			if (_minSumFontSize > _maxSumFontSize)
			{
				Debug.LogError("Start with incorrect fontSize settings!", _sum.gameObject);
			}
			for (int i = 0, iMax = mults.Length; i < iMax; i++)
				mults[i].text = Math.Round(_settings.Settings[i].Mult, 1).ToString();
		}

		private void Update()
		{
			if (_overflowDelay > 0f)
			{
				_overflowDelay -= Time.deltaTime;
				//Animation
				_sum.fontSize = Mathf.Lerp(_minSumFontSize, _maxSumFontSize, _overflowDelay / _overflowPointsPerSecond);
				return;
			}
			
			if (_currentPoints == .0f) return;

			_overflowDelay = _overflowPointsPerSecond;
			_currentPoints--;
			//Множитель влияет на конверсию очков из индикатора в сумму
			_sumPoints += CurrentMult;
			if(_hasAudio) _tickerSum.Play();
			UpdateIndicator();
		}
		
		private void UpdateIndicator()
		{
			_currentPointsText.text = _currentPoints.ToString();
			_sum.text = Sum.ToString();
			var percent = _currentPoints / _indicatorMaxPoints;
			_indicatorFill.fillAmount = percent;
			_indicatorFill.color = _indicatorGradient.Evaluate(percent);
			var length = _settings.Settings.Length;
			var index = Mathf.Clamp((int)MathF.Floor(percent * length) - 1, 0, length - 1);
			//Оптимизация, чтобы каждый кадр не переназначать и не перезапускать анимации
			if (_prevIndex == index) return;

			if (_hasAudio)
			{
				var audio = _indicatorUpdateClips[Random.Range(0, _indicatorUpdateClips.Length)];
				_tickerSum.PlayOneShot(audio, _switchIndicatorVolume);
			}
			
			var preset = _settings.Settings[index];
			_message.text = string.IsNullOrEmpty(preset.Text)
				? string.Empty : preset.Text;

			_messageAnim.clip = preset.Clip;
			if (preset.Clip == null)
				_messageAnim.Stop();
			else 
				_messageAnim.Play();

			CurrentMult = preset.Mult;
			_prevIndex = index;
		}
	}
}