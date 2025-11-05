using UnityEngine;
using UnityEngine.InputSystem; // Yeni Input System'i kullanmak için bu satýr þart!
using System.Collections.Generic;

public class InputReceiver : MonoBehaviour
{
    private Inputs playerInputs;

    private List<GameObject> notesInTrigger = new List<GameObject>();

   // private ComboManager comboManager;

    private void Awake()
    {
        playerInputs = new Inputs();

        //comboManager = FindObjectOfType<ComboManager>();
    }

    private void OnEnable()
    {
        playerInputs.RhythmGame.Enable();

        playerInputs.RhythmGame.Lane1.performed += context => OnLanePressed(0);
        playerInputs.RhythmGame.Lane2.performed += context => OnLanePressed(1);
        playerInputs.RhythmGame.Lane3.performed += context => OnLanePressed(2);
        playerInputs.RhythmGame.Lane4.performed += context => OnLanePressed(3);
        playerInputs.RhythmGame.Lane5.performed += context => OnLanePressed(4);
    }

    private void OnDisable()
    {
        playerInputs.RhythmGame.Disable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Note"))
        {
            notesInTrigger.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Note"))
        {
            notesInTrigger.Remove(other.gameObject);
        }
    }

    private void OnLanePressed(int laneIndex)
    {
    //    GameObject noteToHit = FindNoteInLane(laneIndex);

    //    if (noteToHit != null)
    //    {
    //        // --- BAÞARILI VURUÞ ---
    //        Debug.Log("Lane " + (laneIndex + 1) + " -> Success");

    //        notesInTrigger.Remove(noteToHit);
    //        Destroy(noteToHit);

    //        if (comboManager != null)
    //        {
    //            // comboManager.NoteHit(); // Bu satýrý ComboManager'ý yazýnca aktive edeceðiz.
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("Lane " + (laneIndex + 1) + " -> Fail");

    //        if (comboManager != null)
    //        {
    //            // comboManager.NoteMissed(); // Bu satýrý ComboManager'ý yazýnca aktive edeceðiz.
    //        }
    //    }
    }

    private GameObject FindNoteInLane(int targetLaneIndex)
    {

        if (notesInTrigger.Count > 0)
        {
            // TODO: Gelen notanýn gerçekten doðru lane'de olup olmadýðýný kontrol et.
            return notesInTrigger[0];
        }
        return null;
    }
}