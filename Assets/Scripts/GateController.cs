using System.Collections;
using UnityEngine;

public class GateController : MonoBehaviour
{
    [Header("Settings")]
    public float openSpeed = 2.0f;

    private Vector3 closedScale;
    private Vector3 openScale;

    void Awake()
    {
        closedScale = transform.localScale;
        openScale = new Vector3(closedScale.x, 0f, closedScale.z);
    }

    public IEnumerator OpenGateRoutine()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * openSpeed;
            transform.localScale = Vector3.Lerp(closedScale, openScale, t);
            yield return null;
        }
        transform.localScale = openScale; 
    }

    public IEnumerator CloseGateRoutine()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * openSpeed;
            transform.localScale = Vector3.Lerp(openScale, closedScale, t);
            yield return null;
        }
        transform.localScale = closedScale;
    }
}