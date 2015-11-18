using UnityEngine;
using System.Collections;

namespace Utils
{
    public class FPSText : MonoBehaviour
    {
        void Update()
        {
            GetComponent<UnityEngine.UI.Text>().text = GameUtils.FPS.ToString();
        }
    }
}