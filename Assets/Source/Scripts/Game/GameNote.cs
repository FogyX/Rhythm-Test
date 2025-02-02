using System;
using UnityEngine;

namespace Source.Scripts.Game
{
    public class GameNote : MonoBehaviour
    {
        [field: SerializeField] public float MovementSpeed { get; private set; }

        [SerializeField] private Vector3 _movementDirection;

        private Rigidbody2D _rigidbody;
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            _rigidbody.MovePosition(transform.position + _movementDirection * (MovementSpeed * Time.fixedDeltaTime));
        }
    }
}