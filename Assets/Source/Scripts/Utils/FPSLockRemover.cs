using UnityEngine;

namespace Source.Scripts.Utils
{
    public class FPSLockRemover : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 120;
        }
    }
}