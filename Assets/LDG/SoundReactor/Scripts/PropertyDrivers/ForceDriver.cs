// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.SoundReactor
{
    public enum ForceType { Force, Torque, RelativeForce, RelativeTorque };

    public class ForceDriver : PropertyDriver
    {
        private Rigidbody rb;

        public ForceMode forceMode = ForceMode.Force;
        public ForceType forceType = ForceType.Force;
        public float maxAngularVelocity = 7.0f; // Unity's default

        private void Start()
        {
            if(!(rb = GetComponent<Rigidbody>()))
            {
                Debug.LogWarning("ForceDriver can't find a RigidBody.", this);
                componentMissing = true;
            }
        }

        protected override void DoLevel()
        {
            ForceDriver forceDriver = (sharedDriver != null) ? (ForceDriver)sharedDriver : this;

            Vector3 level = LevelVector();

            switch (forceDriver.forceType)
            {
                case ForceType.Force:
                    rb.AddForce(level, forceDriver.forceMode);
                    break;

                case ForceType.RelativeForce:
                    rb.AddRelativeForce(level, forceDriver.forceMode);
                    break;

                case ForceType.Torque:
                    rb.maxAngularVelocity = forceDriver.maxAngularVelocity;
                    rb.AddTorque(level, forceDriver.forceMode);
                    break;

                case ForceType.RelativeTorque:
                    rb.maxAngularVelocity = forceDriver.maxAngularVelocity;
                    rb.AddRelativeTorque(level, forceDriver.forceMode);
                    break;
            }
        }
    }
}
