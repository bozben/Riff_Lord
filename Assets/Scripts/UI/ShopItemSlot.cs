using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemSlot : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI nameText;
    public GameObject lockIcon;

    [Header("Slot Buttons")]
    public Button btnSlot1;
    public Button btnSlot2;
    public Button btnSlot3;
    public Image[] slotImages;

    private CharacterData_SO myCharacter;
    private Shop_UI shopManager;

    private Color selectedColor = Color.green;
    private Color defaultColor = Color.white;
    private Color disabledColor = Color.gray;

    public void Setup(CharacterData_SO data, Shop_UI manager)
    {
        myCharacter = data;
        shopManager = manager;

        if (data.isUnlocked)
        {
            nameText.text = data.characterName + " (Lv." + data.level + ")";
        }
        else
        {
            nameText.text = "???";
        }

        lockIcon.SetActive(!data.isUnlocked);

        btnSlot1.gameObject.SetActive(data.isUnlocked);
        btnSlot2.gameObject.SetActive(data.isUnlocked);
        btnSlot3.gameObject.SetActive(data.isUnlocked);

        btnSlot1.onClick.RemoveAllListeners();
        btnSlot2.onClick.RemoveAllListeners();
        btnSlot3.onClick.RemoveAllListeners();

        btnSlot1.onClick.AddListener(() => shopManager.OnSlotButtonClicked(0, myCharacter));
        btnSlot2.onClick.AddListener(() => shopManager.OnSlotButtonClicked(1, myCharacter));
        btnSlot3.onClick.AddListener(() => shopManager.OnSlotButtonClicked(2, myCharacter));

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => shopManager.DisplayChampionDetails(myCharacter));

        UpdateSlotVisuals();
    }
    public void SetupSong(SongChart_SO songData, Shop_UI manager, bool isSelected)
    {
        shopManager = manager;

        if (songData.isUnlocked)
        {
            nameText.text = songData.spellName + " (Lv." + songData.level + ")";
            if (lockIcon != null) lockIcon.SetActive(false);
        }
        else
        {
            nameText.text = "???";
            if (lockIcon != null) lockIcon.SetActive(true);
        }

        if (btnSlot1 != null) btnSlot1.gameObject.SetActive(false);
        if (btnSlot2 != null) btnSlot2.gameObject.SetActive(false);
        if (btnSlot3 != null) btnSlot3.gameObject.SetActive(false);

        Button mainButton = GetComponent<Button>();
        mainButton.onClick.RemoveAllListeners();
        mainButton.onClick.AddListener(() => {
            shopManager.DisplaySongDetails(songData);
        });

        Color activeColor = new Color(142f / 255f, 172f / 255f, 47f / 255f, 1f);
        Color lockedSelectedColor = new Color(1f, 0.6f, 0f, 1f); 
        Color normalColor = Color.white;

        if (isSelected)
        {
            if (songData.isUnlocked)
                mainButton.image.color = activeColor;
            else
                mainButton.image.color = lockedSelectedColor;
        }
        else
        {
            mainButton.image.color = normalColor;
        }
    }

    public void UpdateSlotVisuals()
    {

        var team = GameManager.instance.selectedTeam;

        if (team.Count > 0 && team[0] == myCharacter)
            slotImages[0].color = selectedColor;
        else
            slotImages[0].color = defaultColor;

        if (team.Count > 1 && team[1] == myCharacter)
            slotImages[1].color = selectedColor;
        else
            slotImages[1].color = defaultColor;

        if (team.Count > 2 && team[2] == myCharacter)
            slotImages[2].color = selectedColor;
        else
            slotImages[2].color = defaultColor;
    }
}