using UnityEngine;

// class controls player's moves and movement visual effects

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Transform _innerContainer;
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _maxMoveSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 3f;
        [SerializeField] private ParticleSystem _boosterParticles;

        private Rigidbody _rigidbody;
        private Touch _touch;
        

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                _boosterParticles.Play();
                _touch = Input.touches[0];
                var force = Vector3.zero;

                if (_touch.phase == TouchPhase.Moved)
                {
                    var newRotation = Quaternion.Euler(_innerContainer.eulerAngles.x, _innerContainer.eulerAngles.x, -_touch.deltaPosition.x * _rotationSpeed * 0.5f);
                    _innerContainer.rotation = Quaternion.Lerp(_innerContainer.rotation, newRotation, Time.deltaTime * _rotationSpeed);

                    force += new Vector3(_touch.deltaPosition.x, 0, 0);
                }

                _rigidbody.AddForce(force += Vector3.up * _moveSpeed, ForceMode.Impulse);
            }
            else if (_boosterParticles.isPlaying)
            {
                _boosterParticles.Stop();
            }
            else
            {
                var newRotation = Quaternion.Euler(_innerContainer.eulerAngles.x, _innerContainer.eulerAngles.x, 0);
                _innerContainer.rotation = Quaternion.Lerp(_innerContainer.rotation, newRotation, Time.deltaTime * _rotationSpeed);

                _rigidbody.AddForce(Vector3.down * _moveSpeed, ForceMode.Impulse);
            }

            if (_rigidbody.velocity.magnitude > _maxMoveSpeed)
            {
                _rigidbody.velocity = _rigidbody.velocity.normalized * _maxMoveSpeed;
            }
        }
    }
}
