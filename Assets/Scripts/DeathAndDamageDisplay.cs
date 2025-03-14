using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathAndDamageDisplay : MonoBehaviour
{
    [SerializeField]
    Affected player;
    [SerializeField]
    CanvasGroup canvasGroup;
    [SerializeField]
    CanvasGroup damage;
    [SerializeField]
    TextMeshProUGUI text;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.OnTakeDamage += TakeDamage;
        player.OnDie += Die;
    }

    void Die()
    {
        sequence?.Kill();
        damage.alpha = 0f;
        canvasGroup.DOFade(1f, 1f);
        canvasGroup.alpha = 1f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        text.text = $"You died to {lastDamage}\nYou discovered " + SpellDisplay.Instance.SpellsEarned + " spells";
    }

    string lastDamage;
    Sequence sequence;
    void TakeDamage(float amount, string thing)
    {
        lastDamage = thing;
        damage.alpha += amount / 20f;
        sequence?.Kill();
        sequence = DOTween.Sequence();
        sequence.Append(damage.DOFade(0f, 1.5f));
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
