using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
	/// <summary>
	/// Родитель, для определения набора сущностей, стандартный пример паттерна "Object Pool" 
	/// </summary>
	/// <typeparam name="T">Тип компонентов, которые содержаться в пуле</typeparam>
	public abstract class GameObjectPool<T> : MonoBehaviour, IEnumerable<T>, IReadOnlyCollection<T> 
		where T : Component  
	{
		//Список всех элементов
		private List<T> _elements;
		
		[SerializeField]
		protected T _prefab;
		
		/// <summary>
		/// Кол-во элементов в пуле
		/// </summary>
		/// <remarks>Включает и активные и неактивные</remarks>
		public int Count => _elements.Count;

		/// <summary>
		/// Метод постинициализации при создании элемента пула
		/// </summary>
		public Action<T> CreateCallback { get; set; }

		/// <summary>
		/// Проверяет работоспособность компонента
		/// </summary>
		public bool IsBroken => _prefab == null;

		/// <summary>
		/// Возвращает первый найденный выключенный элемент из пула и включает его
		/// </summary>
		/// <returns>Свободный элементт для переиспользования</returns>
		/// <remarks>Если в пуле нет свободных элементов - создает новый</remarks>
		public virtual T GetElement()
		{
			var element = _elements.Find(t => !t.gameObject.activeSelf);

			if (element == null)
			{
				element = GameObject.Instantiate(_prefab, transform);
				CreateCallback?.Invoke(element);
				_elements.Add(element);
			}
			else
			{
				element.gameObject.SetActive(true);
			}

			return element;
		}

		/// <summary>
		/// Возвращает элемент обратно в пул (и выключает его игровой объект)
		/// </summary>
		/// <param name="element">Ссылка на освободившийся элемент</param>
		public void ReturnElement(T element)
		{
			element.gameObject.SetActive(false);
		}
		
		/// <summary>
		/// Освобождает весь пул элементов (выключает их игровые объекты)
		/// </summary>
		public void DisableAllElements()
		{
			for(int i = 0, iMax = _elements.Count; i < iMax; i++)
				_elements[i].gameObject.SetActive(false);
		}

		/// <summary>
		/// Вызов преинициализации пула
		/// </summary>
		/// <param name="prefab">Установка префаба для создания элементов</param>
		/// <param name="callback">Метод постинициализации каждого элемента</param>
		/// <param name="poolCapacity">Изначальная ёмкость пула</param>
		public void CallInitialize(T prefab, Action<T> callback = null, int poolCapacity = 4)
		{
			_prefab = prefab;
			CallInitialize(callback, poolCapacity);
		}
		
		/// <summary>
		/// Вызов преинициализации пула
		/// </summary>
		/// <param name="callback">Метод постинициализации каждого элемента</param>
		/// <param name="poolCapacity">Изначальная ёмкость пула</param>
		public void CallInitialize(Action<T> callback = null, int poolCapacity = 4)
		{
			if (_elements is not null) return;
			CreateCallback = callback;
			if (poolCapacity <= 0)
			{
				Debug.LogError($"Invalid value: capacity = {poolCapacity}", gameObject);
				poolCapacity = 4;
			}
			_elements = new List<T>(poolCapacity);
			var transform = this.transform;
			for (int i = 0; i < poolCapacity; i++)
			{
				var element = GameObject.Instantiate(_prefab, transform);
				CreateCallback?.Invoke(element);
				element.gameObject.SetActive(false);
				_elements.Add(element);
			}
		}

		public IEnumerator<T> GetEnumerator() => _elements.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => _elements.GetEnumerator();
	}
}