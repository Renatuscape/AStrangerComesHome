using System;

[Serializable]
public class PlayerSpriteData
{
    public string hairID;
    public string bodyID;
    public string headID;
    public string eyesID;

    public string hairHexColour;
    public string hairAccentHexColour;
    public string accessoryHexColour;

    public string eyesHexColour;
    public string lipTintHexColour;

    public string cloakHexColour;
    public string vestHexColour;
    public string tightsHexColour;

    public float lipTintTransparency;
    public bool enableAccessory;
    public bool enableAccent;

    public void ResetValues()
    {
        hairID = "HairA0";
        bodyID = "BodyA0";
        headID = "Default";
        eyesID = "EyesA0";

        hairHexColour = "83695C";
        hairAccentHexColour = "E0C49F";
        eyesHexColour = "88DA69";
        accessoryHexColour = "A8AB98";
        lipTintHexColour = "CAAB80";
        cloakHexColour = "808E94";
        vestHexColour = "BDA8A7";
        tightsHexColour = "919F98";

        lipTintTransparency = 0;
        enableAccessory = false;
        enableAccent = false;
    }


    public PlayerSpriteData Clone()
    {
        return new PlayerSpriteData
        {
            hairID = this.hairID,
            bodyID = this.bodyID,
            headID = this.headID,
            eyesID = this.eyesID,
            hairHexColour = this.hairHexColour,
            hairAccentHexColour = this.hairAccentHexColour,
            accessoryHexColour = this.accessoryHexColour,
            eyesHexColour = this.eyesHexColour,
            lipTintHexColour = this.lipTintHexColour,
            cloakHexColour = this.cloakHexColour,
            vestHexColour = this.vestHexColour,
            tightsHexColour = this.tightsHexColour,
            lipTintTransparency = this.lipTintTransparency,
            enableAccessory = this.enableAccessory,
            enableAccent = this.enableAccent
        };
    }
}
