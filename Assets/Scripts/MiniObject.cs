using UnityEngine;

public class MiniObject : MonoBehaviour
{
    [SerializeField]
    new private ParticleSystem particleSystem;

    private void Start()
    {
        if (transform.parent != null)
        {
            particleSystem.transform.localScale = transform.parent.localScale;
        }
    }

    public void ApplyColor(Gradient gradient)
    {
        var mainModule = particleSystem.main;
        var newColor = new ParticleSystem.MinMaxGradient(gradient)
        {
            mode = ParticleSystemGradientMode.RandomColor
        };
        mainModule.startColor = newColor;
    }

    public void DestroyThis()
    {
        particleSystem.transform.parent = null;
        var e = particleSystem.emission;
        e.enabled = false;

        GameObject.Destroy(this);
    }

}
