using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class CollectableBase : MonoBehaviour, ICollectable
{

    [SerializeField] protected GameObject uiCoinPrefab;
    [SerializeField] protected float flyDuration = 0.6f;
    protected Canvas targetCanvas;
    protected RectTransform targetIcon;
    [SerializeField] protected int value = 1;
    protected bool collected;

    public int Value => value;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected) return;

        if (collision.CompareTag("player") || collision.CompareTag("car") || collision.CompareTag("tire"))
        {
            // Debug.Log("Collectable Triggered by " + this.gameObject.name);
            Collect(collision.gameObject);
        }
    }

    public virtual void Collect(GameObject collector)
    {
        collected = true;
        HandleCollect(collector);
        Destroy(gameObject);
    }

    // 🔑 Each collectable defines its own behavior
    protected abstract void HandleCollect(GameObject collector);
}
