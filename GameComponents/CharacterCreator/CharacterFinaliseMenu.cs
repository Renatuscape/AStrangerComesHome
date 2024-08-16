using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterFinaliseMenu: MonoBehaviour
{
    public DataManagerScript dataManager;
    public TextMeshProUGUI detailsText;
    public Button btnConfirm;
    public Button btnCancel;

    private void Start()
    {
        btnCancel.onClick.AddListener(() => gameObject.SetActive(false));
        btnConfirm.onClick.AddListener(() => CompleteCharacterCreation());
    }

    public void Finalise(CharacterCreator creator)
    {
        GlobalSettings.SaveSettings();
        Character player = Characters.FindByTag("Traveller", gameObject.name);

        dataManager.playerName = creator.characterName.text;
        player.trueName = dataManager.playerName;
        player.hexColour = dataManager.playerNameColour;
        player.NameSetup();
        DialogueTagParser.UpdateTags(dataManager);

        if (player != null)
        {
            string playerName = $"<b>{player.ForceTrueNamePlate()}</b>,";
            string cleanName = playerName.Replace("\u200B", "").Replace("\u00A0", "").Trim();

            detailsText.text = $"In this incarnation, my true name is {cleanName}" +
                $" but those who respect the old ways will refer to me as <b>{DialogueTagParser.ParseText("|the Traveller|")}</b> - at least until our bonds grow stronger.\r\n\r\n" +
                $"My pronouns are <b>{dataManager.pronounSub.ToLower()}</b>/<b>{dataManager.pronounObj.ToLower()}</b>/<b>{dataManager.pronounGen.ToLower()}</b>, and ";

            if (dataManager.playerGender == "Male")
            {
                detailsText.text += "I identify as male.";
            }
            else if (dataManager.playerGender == "Female")
            {
                detailsText.text += "I identify as female.";
            }
            else if (dataManager.playerGender == "Other")
            {
                detailsText.text += "I do not identify with a binary gender.";
            }
            else
            {
                detailsText.text += "my gender is inconsequential.";
            }
        }
        else
        {
            detailsText.text = "Player data has not yet loaded properly. Please try again.";
        }

        gameObject.SetActive(true);
    }

    void CompleteCharacterCreation()
    {
        TransientDataScript.ReturnToOverWorld("Character Creator", gameObject);
    }
}
