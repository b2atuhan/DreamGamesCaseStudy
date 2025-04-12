using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum RocketDirection
{
    Horizontal,
    Vertical
}

public class Rocket : CubeItem
{
    [SerializeField] ParticleSystem Smoke1;
    [SerializeField] ParticleSystem Smoke2;
    [SerializeField] ParticleSystem SolidParticles;
    [SerializeField] int SmokePeriod;
    public GameObject comboRocketPartPrefab;
    public RocketDirection direction;
    [SerializeField] private GameObject flyingRocketPrefab;
    private bool isExploded = false;


    private static int onMouseDownCount = 0;
    private static int triggerComboCount = 0;
    private static int blastCrossCount = 0;
    private static int damageOrBlastCount = 0;
    private static int hasAdjacentRocketCount = 0;
    private static int explodeCount = 0;
    private static int launchRocketPartsCount = 0;
    private static int moveRocketPartCount = 0;
    private static int createRocketTrailCount = 0;

    void OnMouseDown()
    {
        onMouseDownCount++;
        Debug.Log($"[OnMouseDown #{onMouseDownCount}] → Checking input");

        if (!InputBlocker.IsInputEnabled) return;

        Debug.Log($"[OnMouseDown #{onMouseDownCount}] → Checking for adjacent rockets");

        if (HasAdjacentRocket())
        {
            Debug.Log($"[OnMouseDown #{onMouseDownCount}] → Calling TriggerCombo");
            TriggerCombo();
        }
        else
        {
            Debug.Log($"[OnMouseDown #{onMouseDownCount}] → Calling Explode");
            Explode();
        }

        MoveManager.Instance.UseMove();
    }

    private void TriggerCombo()
    {
        triggerComboCount++;
        Debug.Log($"[TriggerCombo #{triggerComboCount}] → TriggerCombo called at: {GridPosition}");

        Vector2Int center = GridPosition;
        StartCoroutine(BlastCross(center));
    }

    private IEnumerator BlastCross(Vector2Int center)
    {
        blastCrossCount++;
        Debug.Log($"[BlastCross #{blastCrossCount}] → Starting combo blast at: {center}");

        int width = GridManager.width;
        int height = GridManager.height;

        HashSet<Vector2Int> positionsToBlast = new();

        for (int dy = -1; dy <= 1; dy++)
        {
            int y = center.y + dy;
            if (y < 0 || y >= height) continue;

            for (int x = 0; x < width; x++)
            {
                positionsToBlast.Add(new Vector2Int(x, y));
            }
        }

        for (int dx = -1; dx <= 1; dx++)
        {
            int x = center.x + dx;
            if (x < 0 || x >= width) continue;

            for (int y = 0; y < height; y++)
            {
                positionsToBlast.Add(new Vector2Int(x, y));
            }
        }

        positionsToBlast.Remove(center);

        foreach (var pos in positionsToBlast)
        {
            if (GridUtility.IsInBounds(pos))
            {
                Debug.Log($"[BlastCross #{blastCrossCount}] → Blasting at {pos}");
                DamageOrBlast(pos);
                yield return new WaitForSeconds(0.01f);
            }
        }

        yield return new WaitForSeconds(0.2f);

        Debug.Log($"[BlastCross #{blastCrossCount}] → Final explosion at {center}");
        GridUtility.RemoveCube(center);
        Destroy(gameObject);

        CubeManager.Instance.DropAndRefill();
    }

    private void DamageOrBlast(Vector2Int pos)
    {
        damageOrBlastCount++;
        Debug.Log($"[DamageOrBlast #{damageOrBlastCount}] → Processing {pos}");

        CubeItem item = GridUtility.GetCubeAt(pos);

        if (item == null)
        {
            Debug.LogWarning($"[DamageOrBlast #{damageOrBlastCount}] → No item at {pos}");
            return;
        }

        if (item is ObstacleBase obstacle)
        {
            obstacle.TakeDamage();
        }
        else if (item is Cube cube)
        {
            GridUtility.RemoveCube(pos);
            Destroy(cube.gameObject);
        }
        else if (item is Rocket rocket)
        {
            rocket.Explode();
        }
    }

    private bool HasAdjacentRocket()
    {
        hasAdjacentRocketCount++;
        Debug.Log($"[HasAdjacentRocket #{hasAdjacentRocketCount}] → Checking around {transform.position}");

        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        foreach (Vector2 dir in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + (Vector3)dir, Vector2.zero);
            if (hit.collider != null && hit.collider.GetComponent<Rocket>() != null)
            {
                return true;
            }
        }

        return false;
    }

    public void Explode()
{
    if (isExploded)
    {
        Debug.Log($"[Explode] Rocket at {GridPosition} already exploded. Skipping.");
        return;
    }

    isExploded = true;

    explodeCount++;
    Debug.Log($"[Explode #{explodeCount}] → Rocket Explode() called at: {GridPosition}");
    StartCoroutine(LaunchRocketParts());
}


    private void OnEnable()
{
    isExploded = false;
}

    private IEnumerator LaunchRocketParts()
    {
        launchRocketPartsCount++;
        Debug.Log($"[LaunchRocketParts #{launchRocketPartsCount}] → Launching parts");

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        Vector2Int[] dirs = direction == RocketDirection.Horizontal
            ? new[] { Vector2Int.left, Vector2Int.right }
            : new[] { Vector2Int.up, Vector2Int.down };

        int completed = 0;

        foreach (var dir in dirs)
        {
            StartCoroutine(MoveRocketPart(dir, () => completed++));
        }

        while (completed < dirs.Length)
        {
            yield return null;
        }

        GridUtility.RemoveCube(GridPosition);
        Destroy(gameObject);
        CubeManager.Instance.DropAndRefill();
    }

    private IEnumerator MoveRocketPart(Vector2Int dir, System.Action onComplete)
    {
        moveRocketPartCount++;
        Debug.Log($"[MoveRocketPart #{moveRocketPartCount}] → Moving in {dir}");

        Vector2Int currentPos = GridPosition;
        GameObject visualRocket = Instantiate(flyingRocketPrefab);
        visualRocket.transform.position = new Vector3(currentPos.x, currentPos.y, -1);

        float angle = dir == Vector2Int.up ? 0f : dir == Vector2Int.right ? -90f : dir == Vector2Int.down ? 180f : 90f;
        visualRocket.transform.rotation = Quaternion.Euler(0, 0, angle);

        float durationPerCell = 0.16f;
        List<Vector3> path = new();

        while (true)
        {
            currentPos += dir;
            if (!GridUtility.IsInBounds(currentPos)) break;

            Vector3 nextWorldPos = new Vector3(currentPos.x, currentPos.y, -1);
            path.Add(nextWorldPos);

            CubeItem item = GridUtility.GetCubeAt(currentPos);
            if (item != null)
            {
                if (item is ObstacleBase obstacle) obstacle.TakeDamage();
                else if (item is Cube cube)
                {
                    GridUtility.RemoveCube(currentPos);
                    Destroy(cube.gameObject);
                }
                else if (item is Rocket rocket) rocket.Explode();
            }
        }

        if (path.Count > 0)
        {
            visualRocket.transform
                .DOPath(path.ToArray(), path.Count * durationPerCell, PathType.Linear)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    Destroy(visualRocket);
                    onComplete?.Invoke();
                });
        }
        else
        {
            Destroy(visualRocket);
            onComplete?.Invoke();
        }

        yield return null;
    }

    private void CreateRocketTrail(Vector2Int pos, Vector2Int direction)
    {
        createRocketTrailCount++;
        Debug.Log($"[CreateRocketTrail #{createRocketTrailCount}] → Creating trail at {pos}");

        Vector3 spawnPos = new Vector3(pos.x, pos.y, -1);
        Quaternion rotation = direction == Vector2Int.down ? Quaternion.Euler(0, 0, 0) :
                              direction == Vector2Int.up ? Quaternion.Euler(0, 0, 180) :
                              direction == Vector2Int.right ? Quaternion.Euler(0, 0, 90) :
                              Quaternion.Euler(0, 0, -90);

        if (Smoke1 != null) Destroy(Instantiate(Smoke1, spawnPos, rotation).gameObject, Smoke1.main.duration);
        if (Smoke2 != null) Destroy(Instantiate(Smoke2, spawnPos, rotation).gameObject, Smoke2.main.duration);
        if (SolidParticles != null) Destroy(Instantiate(SolidParticles, spawnPos, rotation).gameObject, SolidParticles.main.duration);
    }
}
