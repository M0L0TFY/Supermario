using UnityEngine;

public class PowerUps : MonoBehaviour
{
    public enum Type
    {
        Coin,
        Mushroom,
    }

    public Type type;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Player player)) {
            Collect(player);
        }
    }

    private void Collect(Player player)
    {
        switch (type)
        {
            case Type.Coin:
                GameManager.Instance.AddCoin();
                break;

            case Type.Mushroom:
                player.Grow();
                break;
        }

        Destroy(gameObject);
    }

}