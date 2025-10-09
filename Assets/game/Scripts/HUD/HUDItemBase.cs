using UnityEngine;
using UnityEngine.UI;
using System;

public abstract class HUDItemBase : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] protected Text txtItemCount;

    [Header("HUD Type")]
    [SerializeField] protected HUDItem itemType = HUDItem.COIN;

    private int value;
    public int Value
    {
        get => value;
        protected set
        {
            if (this.value == value) return;

            this.value = Mathf.Max(0, value);
            UpdateDisplay();
            // OnValueChanged?.Invoke(this.value);
        }
    }

    public HUDItem ItemType => itemType;

    /// <summary>
    /// Called whenever this HUD item’s value changes.
    /// </summary>
    public event Action<int> OnValueChanged;

    // Called when enabled or instantiated
    protected virtual void OnEnable()
    {
        ResetText();
    }

    /// <summary>
    /// Resets HUD item value and display to 0.
    /// </summary>
    public virtual void ResetText()
    {
        Value = 0;
        UpdateDisplay();
    }

    /// <summary>
    /// Updates the displayed text (override for animations, etc.)
    /// </summary>
    protected virtual void UpdateDisplay()
    {
        if (txtItemCount != null)
            txtItemCount.text = Value.ToString();
    }

    /// <summary>
    /// Called when item is collected (e.g., coin pickup).
    /// </summary>
    public virtual void Collect(EventItemCollected eventItemCollected)
    {
        if (eventItemCollected.item != itemType) return;
        AddValue(eventItemCollected.value);
    }

    /// <summary>
    /// Increase item count.
    /// </summary>
    public virtual void AddValue(int amount)
    {
        if (amount <= 0) return;
        Value += amount;
    }
}

public enum HUDItem
{
    COIN,
    GEM,
    FUEL
}
