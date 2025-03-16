using UnityEngine;

namespace Source.Scripts
{
    public class GameNote : MonoBehaviour
    {
        [SerializeField] private Vector3 _movementDirection;

        private Rigidbody _rigidbody;
        
        private float _movementSpeed;

        public int TrackId { get; private set; }

        public void Construct(int trackId, float movementSpeed)
        {
            _rigidbody = GetComponent<Rigidbody>();
            TrackId = trackId;
            _movementSpeed = movementSpeed;
        }

        private void FixedUpdate()
        {
            _rigidbody.MovePosition(transform.position + _movementDirection * (_movementSpeed * Time.fixedDeltaTime));
        }
    }
}