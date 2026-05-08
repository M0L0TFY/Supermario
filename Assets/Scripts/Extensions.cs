using UnityEngine;

public static class Extensions
{
    private static LayerMask layerMask = LayerMask.GetMask("Default");

    public static bool Raycast(this Rigidbody2D rigidbody, Vector2 direction)
    {
        if (rigidbody.bodyType == RigidbodyType2D.Kinematic) {
            return false;
        }

        Vector2 edge = rigidbody.ClosestPoint(rigidbody.position + direction);
        float radius = (edge - rigidbody.position).magnitude / 2f;
        float distance = radius + 0.125f;

        Vector2 point = rigidbody.position + (direction.normalized * distance);
        Collider2D collider = Physics2D.OverlapCircle(point, radius, layerMask);
        return collider != null && collider.attachedRigidbody != rigidbody;
    }


    public static bool DotTest(this Transform transform, Transform other, Vector2 testDirection)
    {
        Vector2 direction = other.position - transform.position;
        return Vector2.Dot(direction.normalized, testDirection) > 0.5f;
    }

}