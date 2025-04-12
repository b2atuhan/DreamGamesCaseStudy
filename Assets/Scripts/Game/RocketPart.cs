using UnityEngine;

public class RocketPart : MonoBehaviour
{
    public float damageRadius = 1.5f;

    void Update()
    {
        // You can update damage on a timer or from external trigger
        DamageNearbyCells();
    }

    void DamageNearbyCells()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, damageRadius);
        foreach (Collider2D col in hits)
        {
            ObstacleBase obstacle = col.GetComponent<ObstacleBase>();
            if (obstacle != null)
            {
                obstacle.TakeDamage();
            }

            CubeItem cube = col.GetComponent<CubeItem>();
            if (cube != null)
            {
                Destroy(cube.gameObject); // or call a blast animation
            }
        }
    }
}
