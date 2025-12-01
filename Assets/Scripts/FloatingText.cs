using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float destroyTime = 1f;
    public TextMeshPro textMesh;

    private Color textColor;

    public void Setup(int amount, bool isHeal)
    {
        if (isHeal)
        {
            textMesh.text = "+" + amount.ToString();
            textMesh.color = Color.green;
            textMesh.fontSize = 6;
        }
        else
        {
            textMesh.text = amount.ToString();
            textMesh.color = Color.yellow;
            textMesh.fontSize = 5;
        }

        textColor = textMesh.color;
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        textColor.a -= Time.deltaTime / destroyTime;
        textMesh.color = textColor;
    }
}