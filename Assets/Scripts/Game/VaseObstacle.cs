using UnityEngine;

public class VaseObstacle : ObstacleBase
{
    private void Awake()
    {
        health = 2;
    }

    public override void TakeDamage()
    {
        base.TakeDamage();
        // Later: Add vase cracked animation etc.
    }
}
