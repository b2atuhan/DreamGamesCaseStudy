public class StoneObstacle : ObstacleBase
{
    public override bool CanFall => false;

    public override void TakeDamage()
    {
        // Later: restrict only to Rocket source
        base.TakeDamage();
    }
}
