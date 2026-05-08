using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rigbody;
    public float speed = 2f;

    public Vector2 direction = Vector2.left;

    private void Awake()
    {
        rigbody = GetComponent<Rigidbody2D>();
        enabled = false;
    }

    private void OnBecameVisible()
    {
        enabled = true;
    }

    private void OnBecameInvisible()
    {
        enabled = false;
    }

    private void OnEnable()
    {
        rigbody.WakeUp();
    }

    private void OnDisable()
    {
        rigbody.linearVelocity = Vector2.zero;
        rigbody.Sleep();
    }

    private void FixedUpdate()
    {
        rigbody.linearVelocity = new Vector2(direction.x * speed, rigbody.linearVelocity.y);
        rigbody.linearVelocity += new Vector2(0f, Physics2D.gravity.y * Time.fixedDeltaTime);

        rigbody.MovePosition(rigbody.position + rigbody.linearVelocity * Time.fixedDeltaTime);

        //Prevent gravity from building up while enemy is on the ground
        if (rigbody.Raycast(Vector2.down)) {
            rigbody.linearVelocity = new Vector2(rigbody.linearVelocity.x, Mathf.Max(rigbody.linearVelocity.y, 0f));
        }

        //Move in opposite direction if enemy hits a wall
        if (rigbody.Raycast(direction)) {
            direction = -direction;
        }

        //Flip sprite based on movement direction
        if (direction.x > 0f) {
            transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        } else if (direction.x < 0f) {
            transform.localEulerAngles = Vector3.zero;
        }
    }

}