using System;
using Cinemachine;
using UnityEngine;
using Zenject;

namespace Tanks
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BaseInputController))]
    public class TankController : MonoBehaviour
    {
        private const float c_convertMeterInSecFromKmInH = 3.6f;

        private Rigidbody _body;
        private BaseInputController _controller;
        
        private Vector3 _prevPosition;
        private float _prevRotation;
        private float _currentSteerAngle;
        
        [Inject]
        private CinemachineVirtualCamera _camera;

        [Header("---References---"), SerializeField]
        [Tooltip("Ссылки на четыре колеса танка")]
        private Wheel[] _wheels = new Wheel[4];
        [SerializeField, Tooltip("Источник звука скольжения шин по поверхности")]
        private AudioSource _skidAudioSource;
        [SerializeField, Tooltip("Графики мощности двигателя на разных передачах\ntime - Speed | value - Torque")]
        private TransmissionSettings _transmission;
        
        [SerializeField, Space, Range(5f, 50f)] 
        private float _maxSteerAngle = 25f;

        [SerializeField, Min(0f)]
        private float _maxHandbrakeTorque = float.MaxValue;
        [SerializeField] 
        private Vector3 _centreOfMass;

        [SerializeField, Tooltip("Дополнительная сила придавливания танка к земле. Улучшает сцепление с трассой")] 
        private float _downforce = 100f;
        [SerializeField, Tooltip("Пороговое значение, при котором скольжение колеса создает эффекты и звуки")] 
        private float _slipLimit = .3f;
        [SerializeField, Tooltip("Множитель мощности двигателя при заднем ходе")]
        private float _reverseMult = .4f;

        [SerializeField, Range(10f, 300f)]
        private float _maxSpeedFOV = 200f;
        [SerializeField]
        private Vector2 _fov = new (40f, 40f);

#if UNITY_EDITOR
        [SerializeField]
        private bool _debugTorque;
#endif
        
        /// <summary>
        /// Текущая скорость танка в горизонтальной плоскости
        /// </summary>
        public float CurrentSpeed { get; private set; }
        /// <summary>
        /// Скорость двигателя, относительно активной передачи
        /// </summary>
        /// <remarks>0 - минимальная скорость передачи | 1 - максимальная скорость передачи</remarks>
        public float EngineSpeed => _transmission.EngineSpeed(CurrentSpeed);

        private void Start()
        {
            _body = GetComponent<Rigidbody>();
            _body.centerOfMass = _centreOfMass;
            _controller = GetComponent<BaseInputController>();
            _prevPosition = transform.position;

            if (_skidAudioSource == null)
            {
                _skidAudioSource = gameObject.AddComponent<AudioSource>();
                _skidAudioSource.playOnAwake = false;
                _skidAudioSource.loop = true;
                Debug.LogError("AudioSource is null!", this);
            }
#if UNITY_EDITOR
            //Проверка ссылочных типов на предмет некорректной сборки
            var error = false;
            if (_transmission == null)
            {
                Debug.LogError("Need set Transmission!", gameObject);
                error = true;
            }
            if (_wheels == null || _wheels.Length == 0
             || Array.FindIndex(_wheels, t => t == null) != -1)
            {
                Debug.LogError("Need configure Wheels!", gameObject);
                error = true;
            }
            if (error) UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private void FixedUpdate()
        {
            _controller.ManualUpdate();
            var angle = _controller.TankRotate * _maxSteerAngle;
            _wheels[0].SteerAngle = angle;
            _wheels[1].SteerAngle = angle;

            CalculateSpeed();
            ApplyDrive();

            AddDownForce();
            CheckForWheelSpin();
        }

        private void CalculateSpeed()
        {
            var transform = this.transform;
            var position = transform.position;
            position.y = 0f;
            var distance = Vector3.Distance(_prevPosition, position);
            _prevPosition = position;

            CurrentSpeed = (float)Math.Round((double)distance / Time.deltaTime * c_convertMeterInSecFromKmInH, 1);
            _camera.m_Lens.FieldOfView = Mathf.Lerp(_fov.x, _fov.y, Mathf.InverseLerp(0f, _maxSpeedFOV, CurrentSpeed));
        }

        private void ApplyDrive()
        {
            var torque = _controller.Acceleration * _transmission.GetTorque(CurrentSpeed);
            if (_controller.Acceleration < 0f)
                torque *= _reverseMult;
#if UNITY_EDITOR
            if(_debugTorque)
                Debug.Log($"Torque: {torque}");
#endif
            var handbreak = _controller.HandBrake ? _maxHandbrakeTorque : 0f;
            for (int i = 0, iMax = _wheels.Length; i < iMax; i++)
            {
                (_wheels[i].Torque, _wheels[i].Brake) = (torque, handbreak);
            }
        }

        //Дополнительное прижимание танка к земле, для лучшего сцепления с трассой
        //чем выше скорость, тем сильнее прижатие
        private void AddDownForce()
        {
            var value = -transform.up * (_downforce * _body.velocity.magnitude);
            _body.AddForce(value);
        }

        //Проверка, нужно-ли включать трейл и звук скольжения
        private void CheckForWheelSpin()
        {
            for (int i = 0; i < 4; i++)
            {
                var wheelHit = _wheels[i].GetGroundHit;
                //Скользит-ли колесо по трассе
                if (Mathf.Abs(Mathf.Max(wheelHit.forwardSlip, wheelHit.sidewaysSlip)) >= _slipLimit)
                {
                    if (_skidAudioSource.isPlaying) continue;
                    _skidAudioSource.Play();
                }
                //Если скольжения нет - выключаем звук и трейл
                if (!_skidAudioSource.isPlaying) continue;
                _skidAudioSource.Stop();
            }
        }

        private void Update()
        {
            for (int i = 0, iMax = _wheels.Length; i < iMax; i++)
                _wheels[i].UpdateVisual();
        }
        
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.TransformPoint(_centreOfMass), .2f);
            }
        }
    }
}