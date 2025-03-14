using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SingleGlyph : MonoBehaviour
{
    Glyph glyph;
    public Glyph Glyph => glyph;

    public Vector2 currentVector;

    [SerializeField]
    Image image;

    private void Update()
    {
        image.sprite = glyph.sprite;
        currentVector = transform.localPosition * 2f;
    }

    public void SetGlyph(Glyph glyph)
    {
        this.glyph = glyph;
        image.sprite = glyph.sprite;
    }

    private void OnEnable()
    {
        DOTween.To(() => image.fillAmount, f => image.fillAmount = f,
    1f, .5f);
    }

    public void DestroyThis()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => image.fillAmount, f => image.fillAmount = f,
            0f, .2f
            ));
        sequence.AppendCallback(() => GameObject.Destroy(gameObject));
    }


}
