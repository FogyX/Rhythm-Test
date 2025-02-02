using UnityEngine;

namespace LDG.SoundReactor
{
    using LDG.Core;

    public class PianoRollNote : MonoBehaviourParticle
    {
        public struct Parameters
        {
            public Vector3 Velocity;
            public Vector3 Position;
            public Vector3 Size;
            public float LifeTime;
            public Transform Parent;
            public int Track;
        }

        private Vector3 velocity;

        new void Awake()
        {
            base.Awake();
        }

        new void Update()
        {
            base.Update();

            transform.localPosition += velocity * Time.deltaTime;
        }

        override public void Initialize(object parameters)
        {
            Parameters p = (Parameters)parameters;

            if (p.Parent)
            {
                transform.SetParent(p.Parent);
            }

            p.Position.y += (float)p.Track * 0.01f;

            transform.localPosition = p.Position;
            transform.localScale = p.Size;

            velocity = p.Velocity;

            Emit(p.LifeTime);
        }
    }
}