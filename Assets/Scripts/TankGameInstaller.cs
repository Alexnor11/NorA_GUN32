using Cinemachine;
using Tanks.Interface;
using UnityEngine;
using Zenject;

namespace Tanks
{
	/// <summary>
	/// Установщик для предподготовки зависимостей
	/// </summary>
	public class TankGameInstaller : MonoInstaller
	{
		private TankControls _controls;
		
		public override void InstallBindings()
		{
			_controls = new TankControls();
			Container.BindInstance(_controls.Tank);
			Container.BindInstance(_controls.Turret);

#region Check Audio Pool
			var audioPool = GetComponentInChildren<AudioPool>();
			if (audioPool == null)
			{
				Debug.LogError($"Can't find {nameof(AudioPool)!}", this);
				EmergencyStop();
			}
			if(audioPool.IsBroken)
				Debug.LogError("Audio pool is broken!", audioPool);
			else
				audioPool.CallInitialize(poolCapacity: 8);
			
			Container.BindInstance(audioPool);
#endregion
			
			
#region Find UI elements
			var indicator = FindObjectOfType<CarnageIndicator>();
			var speedometer = FindObjectOfType<Speedometer>();
			if (indicator == null || speedometer == null)
			{
				Debug.LogError("Can't find UI Components!", this);
				EmergencyStop();
				indicator = new CarnageIndicator();//create broken component
				speedometer = new Speedometer();//create broken component
			}
			Container.BindInstance(indicator);
			Container.BindInstance(speedometer);
#endregion

#region Find tank
			var tank = FindObjectOfType<TankController>();
			var turret = FindObjectOfType<TurretController>();
			if (tank == null || turret == null)
			{
				Debug.LogError("Can't find Tank Components!", this);
				EmergencyStop();
				tank = new TankController();//create broken component
				turret = new TurretController();//create broken component
			}
			Container.BindInstance(tank);
			Container.BindInstance(turret);
#endregion
			
#region Find camera
			var camera = FindObjectOfType<CinemachineVirtualCamera>();
			if (camera == null)
			{
				Debug.LogError("Can't find Cinemachine camera!", this);
				EmergencyStop();
				camera = new CinemachineVirtualCamera();//create broken component
			}
			Container.BindInstance(camera);
#endregion
		}

		private void OnEnable()
		{
			_controls.Enable();
		}

		private void OnDisable()
		{
			_controls.Disable();
		}

		private void OnDestroy()
		{
			_controls.Dispose();
		}

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		private void EmergencyStop()
		{
#if UNITY_EDITOR && false
			UnityEditor.EditorApplication.isPlaying = false;
#endif
		}
	}

}
