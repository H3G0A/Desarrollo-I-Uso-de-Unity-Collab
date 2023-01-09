using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] Image[] hearts;
    [SerializeField] Sprite fullHeart;
    [SerializeField] Sprite emptyHeart;
    [SerializeField] TextMeshProUGUI keyText;

    public void SetHealth(int health)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i <= health - 1)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }

    public void SetKeys(int keys)
    {
        keyText.SetText(keys.ToString() + " / 6");
    }
}
