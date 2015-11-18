using UnityEngine;
using System.Collections;

namespace Utils
{
    public static class GameUtils
    {
        public static float FPS { get { return 1f / Time.smoothDeltaTime; } }
    }
}