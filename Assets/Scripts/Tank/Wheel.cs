using UnityEngine;

namespace Tanks
{
    /// <summary>
    /// Управляет логикой колес и эффектами
    /// </summary>
    public class Wheel : MonoBehaviour
    {
        [SerializeField, Tooltip("Ссылка на коллайдер колеса")]
        private WheelCollider _collider;
        [SerializeField, Tooltip("Ссылка на меш колеса")]
        private Transform _mesh;

        /// <summary>
        /// Угол поворота колеса вдоль своей оси Y
        /// </summary>
        public float SteerAngle
        {
            get => _collider.steerAngle;
            set => _collider.steerAngle = value;
        }

        /// <summary>
        /// Флаг, на земле-ли стоит колесо
        /// </summary>
        public bool IsGrounded => _collider.isGrounded;

        /// <summary>
        /// Крутящий момент колеса
        /// </summary>
        public float Torque
        {
            get => _collider.motorTorque;
            set => _collider.motorTorque = value;
        }

        /// <summary>
        /// Тормозной момент колеса
        /// </summary>
        public float Brake
        {
            get => _collider.brakeTorque;
            set => _collider.brakeTorque = value;
        }

        /// <summary>
        /// Информация о коллизии колеса
        /// </summary>
        public WheelHit GetGroundHit
        {
            get
            {
                _collider.GetGroundHit(out var hit);
                return hit;
            }
        }

        private void Start()
        {
            if (_collider == null || _mesh == null)
            {
                Debug.LogError("Incorrect configuration", this);
            }
        }
        
        /// <summary>
        /// Обновление визуальной составляющей колеса по физической
        /// </summary>
        public void UpdateVisual()
        {
            _collider.GetWorldPose(out var position, out var rotation);
            _mesh.SetPositionAndRotation(position, rotation);
        }
    }
}