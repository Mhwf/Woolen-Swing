using UnityEngine;

namespace WoolenSwing.UI
{
    public class MenuInteraction : MonoBehaviour
    {
        [SerializeField] private Camera menuCamera;

        void Update()
        {

            if (Cursor.visible && UnityEngine.Input.GetMouseButtonDown(0))
            {
                Ray ray = menuCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 10f))
                {
                    if (hit.collider.name == "Play")
                    {
                        Management.GameManager.Instance.StartGame();
                    }
                    else if (hit.collider.name == "Exit")
                    {
                        Management.GameManager.Instance.ExitGame();
                    }
                }
            }
        }
    }
}