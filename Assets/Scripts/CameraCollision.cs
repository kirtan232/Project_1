using UnityEngine;

// Attach to the Main Camera (a child of the Player).
// If a solid collider is between the player and the camera, the camera moves.
// closer to the player so it never ends up inside or behind a wall.
public class CameraCollision : MonoBehaviour
{
    public float pivotHeight = 1.5f;     // point above the player's center the camera looks from.
    public float radius = 0.3f;          // thickness of the camera "ball" used to detect walls.
    public float wallPadding = 0.1f;     // gap kept between the camera and the wall.
    public float returnSpeed = 10f;      // how fast the camera slides back out after the wall is gone.

    Vector3 defaultLocalPos;             // where the camera sits when nothing blocks it.
    Transform player;
    float currentDistance;

    void Start()
    {
        player = transform.parent;
        defaultLocalPos = transform.localPosition;
        currentDistance = Vector3.Distance(PivotPoint(), transform.parent.TransformPoint(defaultLocalPos));
    }

    Vector3 PivotPoint()
    {
        return player.position + Vector3.up * pivotHeight;
    }

    // LateUpdate runs after the player has moved this frame, so the camera never lags behind.
    void LateUpdate()
    {
        Vector3 pivot = PivotPoint();
        Vector3 desired = player.TransformPoint(defaultLocalPos);
        Vector3 dir = desired - pivot;
        float maxDistance = dir.magnitude;
        dir /= maxDistance;

        // Closest solid hit between the player and the desired camera spot (triggers and the player are ignored).
        float allowed = maxDistance;
        RaycastHit[] hits = Physics.SphereCastAll(pivot, radius, dir, maxDistance, ~0, QueryTriggerInteraction.Ignore);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.IsChildOf(player)) continue;
            if (hit.distance <= 0f) continue;   // started inside something, nothing sensible to zoom to.
            allowed = Mathf.Min(allowed, hit.distance - wallPadding);
        }
        allowed = Mathf.Max(allowed, 0.5f);

        // Snap in immediately when blocked, ease back out when clear.
        if (allowed < currentDistance)
            currentDistance = allowed;
        else
            currentDistance = Mathf.MoveTowards(currentDistance, allowed, returnSpeed * Time.deltaTime);

        transform.position = pivot + dir * currentDistance;
    }
}
