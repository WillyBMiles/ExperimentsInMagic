using UnityEngine;

public abstract class SpellPart : MonoBehaviour
{
    [SerializeField]
    string partName;
    public string Name => partName;
    [SerializeField]
    string partDescription;

    [SerializeField, Tooltip("Object gets deleted after this amount of time. Set higher than Max duration probably")]
    float timeOut = 10f;

    private void Awake()
    {
        Invoke(nameof(DestroyThis), timeOut);
    }

    void DestroyThis()
    {
        if (gameObject == null)
            return;
        Destroy(gameObject);
    }

}
