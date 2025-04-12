using UnityEngine;
using System.Collections.Generic;
public class InputHandler : MonoBehaviour
{
    void Update()
    {
        if (!CubeManager.Instance.PossibleMoveExists())
        {
            Debug.Log("❌ No possible moves left");
            CubeManager.Instance.Shuffle();
        }
        if (Input.GetMouseButtonDown(0)) // For mobile: Input.touchCount > 0
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                GameObject clickedObject = hit.collider.gameObject;

                // 🚫 Ignore visual-only elements like ComboRocketPart
                if (clickedObject.CompareTag("ComboRocketPart"))
                {
                    Debug.Log("⚠️ Ignored click on ComboRocketPart");
                    return;
                }

                CubeItem cubeItem = clickedObject.GetComponent<CubeItem>();

                if (cubeItem == null)
                {
                    Debug.LogWarning($"⚠️ Clicked object '{clickedObject.name}' has no CubeItem component");
                }
                else
                {
                    Debug.Log($"✅ Clicked on CubeItem: {cubeItem.name} ({cubeItem.GetType()})");
                    cubeItem.OnTapped();
                }
            }
        }
    }


}
