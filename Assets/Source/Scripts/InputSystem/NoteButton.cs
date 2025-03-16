using UnityEngine;
using System;

namespace Assets.Source.Scripts.InputSystem
{
    public class NoteButton : MonoBehaviour
    {
        private int _noteId;
        private Collider _collider;
        private Animator _animator;

        public event Action<int> Pressed;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    Physics.Raycast(ray, out RaycastHit hit);
            //    if (hit.collider == _collider)
            //    {
            //        Pressed?.Invoke(_noteId);
            //        _animator.Play("PressTint", -1, 0.0f);
            //    }
            //}

            if (Input.touchCount <= 0)
                return;
            
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    Physics.Raycast(ray, out RaycastHit hit);
                    if (hit.collider == _collider)
                    {
                        Debug.Log($"Touch: {touch.fingerId} began");
                        Pressed?.Invoke(_noteId);
                        _animator.Play("PressTint", -1, 0.0f);
                    }
                }
            }
        }

        public void Construct(int id)
        {
            _noteId = id;
        }
    }
}