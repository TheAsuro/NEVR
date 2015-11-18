using UnityEngine;

namespace Movement
{
    class NoclipMovement : MovementType
    {
        private float moveSensitivity = 1f;

        private enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
        private RotationAxes axes = RotationAxes.MouseXAndY;
        private float sensitivityX = 15F;
        private float sensitivityY = 15F;

        private float minimumX = -360F;
        private float maximumX = 360F;

        private float minimumY = -60F;
        private float maximumY = 60F;

        private bool invertY = false;

        private float rotationY = 0F;

        public NoclipMovement(float moveSens, float lookSens)
        {
            moveSensitivity = moveSens;
            sensitivityX = lookSens;
            sensitivityY = lookSens;
        }

        public override void UpdatePosition(Transform t)
        {
            t.Translate(new Vector3(GetSideways() * moveSensitivity, 0f, GetForward() * moveSensitivity), Space.Self);
        }

        public override void UpdateView(Transform t)
        {
            float ySens = sensitivityY;
            if (invertY) { ySens *= -1f; }

            if (axes == RotationAxes.MouseXAndY)
            {
                float rotationX = t.localEulerAngles.y + GetMouseX() * sensitivityX;

                rotationY += GetMouseY() * ySens;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                t.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
            }
            else if (axes == RotationAxes.MouseX)
            {
                t.Rotate(0, GetMouseX() * sensitivityX, 0);
            }
            else
            {
                rotationY += GetMouseY() * ySens;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                t.localEulerAngles = new Vector3(-rotationY, t.localEulerAngles.y, 0);
            }
        }

        float GetMouseX()
        {
            return Input.GetAxis("Mouse X");
        }

        float GetMouseY()
        {
            return Input.GetAxis("Mouse Y");
        }

        float GetForward()
        {
            return Input.GetAxis("Vertical");
        }

        float GetSideways()
        {
            return Input.GetAxis("Horizontal");
        }
    }
}