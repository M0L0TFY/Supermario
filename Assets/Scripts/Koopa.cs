using UnityEngine;

public class Koopa : MonoBehaviour
{
    public Sprite shellSprite;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent(out Player player))
        {
            if (collision.transform.DotTest(transform, Vector2.down)) {
                EnterShell();
             } else {
                player.Hit();
            }
        }
    }

    private void EnterShell()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<EnemyMovement>().enabled = false;
        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<SpriteRenderer>().sprite = shellSprite;
        Destroy(gameObject, 0.5f);
    }

}