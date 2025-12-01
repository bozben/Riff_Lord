using UnityEngine;
using UnityEngine.UI;

public class NoteObject : MonoBehaviour
{
    public float speed = 0f;

    private int laneIndex;
    private ComboManager comboManager;

    private bool isMissed = false;

    private float missThresholdX = -950f; 
    private float destroyThresholdX = -960f;

    public void Initialize(ComboManager cm, int lane, NoteData data = null)
    {
        comboManager = cm;
        laneIndex = lane;
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);

        if (!isMissed && transform.position.x < missThresholdX)
        {
            isMissed = true;

            Image noteImage = GetComponent<Image>();
            if (noteImage != null)
            {
                noteImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }

            if (comboManager != null) comboManager.NoteMissed();
            ScoreManager.instance.NoteMissed(5);
        }

        if (transform.position.x < destroyThresholdX)
        {
            Destroy(gameObject);
        }
    }

    public int GetLaneIndex()
    {
        return laneIndex;
    }
}