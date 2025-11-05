using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [HideInInspector] 
    public float speed = 500f;


    //private ComboManager comboManager;

   /*
    public void Initialize(ComboManager cm)
    {
        comboManager = cm;
    }
    void Update()
    {
        // Notayý her saniye sola doðru hareket ettir.
        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);

        // Eðer nota vuruþ bölgesini geçip ekranýn çok soluna gittiyse,
        // bu "Miss" (Kaçýrýldý) anlamýna gelir.
        // Bu pozisyon deðerini kendi HitZone pozisyonuna göre ayarlaman gerekebilir.
        if (transform.position.x < -1000f) // Örnek bir ekran dýþý deðeri
        {
            // Kaçýrýldýðýnda ComboManager'a haber ver.
            if (comboManager != null)
            {
                comboManager.NoteMissed();
            }

            // Kendini yok et.
            Destroy(gameObject);
        }
    }
   */
}