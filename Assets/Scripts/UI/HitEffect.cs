using UnityEngine;
using UnityEngine.UI;

public class HitEffect : MonoBehaviour
{
    public float animationDuration = 0.3f; 
    public Vector3 targetScale = new Vector3(2f, 2f, 1f); 

    private Image image;

    public void Setup(Color effectColor)
    {
        image = GetComponent<Image>();
        image.color = effectColor;

        StartCoroutine(AnimateEffect());
    }

    private System.Collections.IEnumerator AnimateEffect()
    {
        float timer = 0f;
        Vector3 startScale = transform.localScale;
        Color startColor = image.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float t = timer / animationDuration;

            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            image.color = Color.Lerp(startColor, endColor, t);

            yield return null;
        }

        Destroy(gameObject);
    }
}