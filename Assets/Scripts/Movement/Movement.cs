using UnityEngine;
using System.Collections;

namespace Movement
{
    public class Movement : MonoBehaviour
    {
        public float moveSensitivity = 1f;
        public float lookSensitivity = 5f;

        private MovementType move;

        void Start()
        {
            move = new NoclipMovement(moveSensitivity, lookSensitivity);
        }

        void Update()
        {
            move.UpdateView(transform);
            move.UpdatePosition(transform);
        }
    }

    abstract class MovementType
    {
        public abstract void UpdateView(Transform t);
        public abstract void UpdatePosition(Transform t);
    }
}