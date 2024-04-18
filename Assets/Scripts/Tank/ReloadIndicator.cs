using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Tanks
{
	/// <summary>
	/// Визуализация системы перезарядки танка
	/// </summary>
	public class ReloadIndicator : MonoBehaviour
	{
		private Coroutine _coroutine;
		
		[SerializeField]
		private Image _indicator;
		[SerializeField]
		private Gradient _color;

		private void Awake()
		{
			if (_indicator == null)
			{
				Debug.Log($"Incorrect preferences in {nameof(ReloadIndicator)}", gameObject);
				enabled = false;
			}
			else
				(_indicator.fillAmount, _indicator.color) 
					= (1f, _color.Evaluate(1f));
		}

		/// <summary>
		/// Установить перезарядку
		/// </summary>
		/// <param name="delay">Время на перезарядку в секундах</param>
		public void SetReload(float delay)
		{
			if(_coroutine != null)
				StopCoroutine(_coroutine);
			_coroutine = StartCoroutine(Reloading(delay));
		}

		private IEnumerator Reloading(float delay)
		{
			var time = 0f;
			while (time < delay)
			{
				var percent = time / delay;
				_indicator.fillAmount = percent;
				_indicator.color = _color.Evaluate(percent);
				time += Time.deltaTime;
				yield return null;
			}

			_coroutine = null;
		}
	}
}

