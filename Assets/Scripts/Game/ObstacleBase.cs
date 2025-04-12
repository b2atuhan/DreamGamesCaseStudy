using UnityEngine;
using System.Collections;

public abstract class ObstacleBase : CubeItem
{
    public int health = 1;

    public virtual void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            // Delay the UI update using external runner
            CoroutineRunner.Instance.RunCoroutine(DelayedUpdateDisplay());
            Destroy(gameObject);
        }
        else
        {
            ObstacleDisplayUI.Instance.UpdateDisplay(); // optional for live updates
        }
    }

    private IEnumerator DelayedUpdateDisplay()
    {
        yield return new WaitForSeconds(0.2f); // let Unity destroy the object
        ObstacleDisplayUI.Instance.UpdateDisplay();
    }

}
