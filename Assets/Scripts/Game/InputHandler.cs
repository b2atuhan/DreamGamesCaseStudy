using UnityEngine;
using System.Collections.Generic;

public class InputHandler : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if (!CubeManager.Instance.PossibleMoveExists())
        {
            Debug.Log("❌ No possible moves left");
            CubeManager.Instance.Shuffle();
        }

        if (Input.GetMouseButtonDown(0)) // For mobile: Input.touchCount > 0
        {
            Vector2 worldPoint = mainCam.ScreenToWorldPoint(Input.mousePosition); // Cached!
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                GameObject clickedObject = hit.collider.gameObject;

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
