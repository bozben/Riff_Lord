using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InputReceiver : MonoBehaviour
{
    [Header("Visuals")]
    public GameObject hitEffectPrefab;
    public Transform effectContainer;

    private List<GameObject> notesInTrigger = new List<GameObject>();

    [SerializeField] private ComboManager comboManager;

    private void OnEnable()
    {
        if (InputManager.instance != null)
        {
            var rhythmActions = InputManager.instance.inputActions.RhythmGame;

            rhythmActions.Lane1.performed += OnLane1Pressed;
            rhythmActions.Lane2.performed += OnLane2Pressed;
            rhythmActions.Lane3.performed += OnLane3Pressed;
            rhythmActions.Lane4.performed += OnLane4Pressed;
            rhythmActions.Lane5.performed += OnLane5Pressed;
        }
    }

    private void OnDisable()
    {
        if (InputManager.instance != null)
        {
            var rhythmActions = InputManager.instance.inputActions.RhythmGame;

            rhythmActions.Lane1.performed -= OnLane1Pressed;
            rhythmActions.Lane2.performed -= OnLane2Pressed;
            rhythmActions.Lane3.performed -= OnLane3Pressed;
            rhythmActions.Lane4.performed -= OnLane4Pressed;
            rhythmActions.Lane5.performed -= OnLane5Pressed;
        }
    }

    private void OnLane1Pressed(InputAction.CallbackContext ctx) => OnLanePressed(0);
    private void OnLane2Pressed(InputAction.CallbackContext ctx) => OnLanePressed(1);
    private void OnLane3Pressed(InputAction.CallbackContext ctx) => OnLanePressed(2);
    private void OnLane4Pressed(InputAction.CallbackContext ctx) => OnLanePressed(3);
    private void OnLane5Pressed(InputAction.CallbackContext ctx) => OnLanePressed(4);



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
        GameObject noteToHit = FindNoteInLane(laneIndex);

        if (noteToHit != null)
        {
            if (hitEffectPrefab != null)
            {
                Color noteColor = noteToHit.GetComponent<UnityEngine.UI.Image>().color;

                GameObject effectGO = Instantiate(hitEffectPrefab, effectContainer);
                effectGO.transform.position = noteToHit.transform.position;

                effectGO.GetComponent<HitEffect>().Setup(noteColor);
            }

            notesInTrigger.Remove(noteToHit);
            Destroy(noteToHit);

            if (comboManager != null)
            {
                comboManager.NoteHit();
            }

            ScoreManager.instance.NoteHit(5);
        }
        else
        {
            if (comboManager != null)
            {
                comboManager.NoteMissed();
            }

            ScoreManager.instance.NoteMissed(2);
        }
    }

    private GameObject FindNoteInLane(int targetLaneIndex)
    {
        foreach (GameObject note in notesInTrigger)
        {
            if (note == null) continue;

            var noteScript = note.GetComponent<NoteObject>();
            if (noteScript != null && noteScript.GetLaneIndex() == targetLaneIndex)
            {
                return note;
            }
        }
        return null;
    }
}