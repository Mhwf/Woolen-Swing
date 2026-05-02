using UnityEngine;
using WoolenSwing.Management;

namespace WoolenSwing.Environment
{
    public class WinBasket : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && GameManager.Instance.CurrentState == GameState.Playing)
            {
                GameManager.Instance.WinGame();
            }
        }
    }
}