using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OldWorldRuinManager : MonoBehaviour
{
    public enum RuinResourceType
    {
        key,
        clue,
        glyph,
        light,
        tool,
        medicine,
        food,
        drink,
        wit,
        rumination,
        mystique
    }

    public enum RuinTheme
    {
        none,
        forest,
        plain,
        town,
        fortress,
        swamp,
        mountain,
        beach,
        random,
        mixed
    }

    public GameObject ruinInterface;
    public Sprite interfaceBackground;
    public List<OldWorldRuin> ruinCollection;

    public List<RuinEvent> premadeEvents;
    public List<RuinPath> premadePaths;
    public List<RuinRoom> premadeRooms;
    public List<RuinChoice> premadeChoices;

    void Start()
    {
        SetUpRuinMenu();

        RuinA();

        RandomRuin(3);
        RandomRuin(1);
        RandomRuin(8);
    }

    void SetUpRuinMenu()
    {
        //CANVAS
        var ruinCanvas = this.AddComponent<Canvas>();
        ruinCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var ruinCanvasScaler = this.AddComponent<CanvasScaler>();
        ruinCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;


        ruinCanvas.pixelPerfect = true;
        ruinCanvasScaler.scaleFactor = 2;
        ruinCanvasScaler.referencePixelsPerUnit = 32;

        //INTERFACE
        ruinInterface = new GameObject();
        ruinInterface.name = "RuinInterface";
        ruinInterface.transform.parent = ruinCanvas.transform;

        var ruinImage = ruinInterface.AddComponent<Image>();
        ruinImage.sprite = interfaceBackground;
        ruinImage.type = Image.Type.Tiled;

        var interfaceRect = ruinInterface.GetComponent<RectTransform>();
        interfaceRect.localPosition = Vector3.zero;
        interfaceRect.sizeDelta = new Vector2(250, 300);
    }

    [System.Serializable]
    public struct OldWorldRuin
    {
        public string printName;
        public int difficulty;
        public int depth;
        public List<RuinRoom> rooms;
        public List<MoDataSet> completionReward;
    }

    [System.Serializable]
    public struct MoDataSet
    {
        public MotherObject rewardType;
        public int amount;
    }

    [System.Serializable]
    public struct RuinRoom
    {
        public string roomDescription;
        public RuinEvent roomEvent;
        public List<RuinResourceDataSet> roomTreasure;

        public RuinPath northExit;
        public RuinPath southExit;
        public RuinPath westExit;
        public RuinPath eastExit;
        //make sure that the door the player came through is marked originDoor. Choosing this returns to the surface
    }

    [System.Serializable]
    public struct RuinPath
    {
        public enum PathType
        {
            openHallway,
            collapsedHallway, //opened with tools and/or other resources
            lockedDoor, //opened with keys or wit(RNG)
            puzzleDoor, //opened with clues or rumination(RNG)
            magicDoor, //opened with glyphs or mystique(RNG)
            wall,
            originDoor
        }

        public PathType pathType;
        public string doorDescription;
        public string successText;
        public string failureText;
        public List<RuinResourceDataSet> resourcesNeeded;
        public bool canBeForced; //if the room has no open hallway, one door must be forcible. Exclude originDoor
    }

    [System.Serializable]
    public struct RuinEvent
    {
        public enum RuinEventType
        {
            story,
            hazard
        }
        
        public RuinEventType eventType;
        public List<RuinEventStage> eventStages;
    }

    [System.Serializable]
    public struct RuinEventStage
    {
        public string eventStageText;
        public List<RuinChoice> eventChoices;
    }

    //CHOICES, CHECKS AND REWARD
    [System.Serializable]
    public struct RuinChoice
    {

        public string choiceDescription;
        public string successText;
        public string failureText;
        public bool notRepeatable;

        public List<RuinResourceDataSet> checkList;
        public List<RuinResourceDataSet> rewardList;
    }

    [System.Serializable]
    public struct RuinResourceDataSet
    {
        public RuinResourceType type;
        public int checkLevel;
        public bool rollRandom;
        public int randomRange; //roll 0 - 100. Success if result > randomRange.
    }

    //METHODS FOR CREATING RUINS
    public void RandomRuin(int difficulty)
    {
        var newRuin = new OldWorldRuin();

        if (difficulty > 5)
            difficulty = 5;
        else if (difficulty < 1)
            difficulty = 1;

        newRuin.printName = "RandomDungeonName";
        newRuin.difficulty = difficulty;
        newRuin.depth = difficulty * 5;
        newRuin.rooms = new List<RuinRoom>();

        //Add room for each depth level
        for (int i = 0; i < newRuin.depth; i++)
        {
            var newRoom = new RuinRoom();
            newRoom.roomDescription = "This is a randomly created room.";

            //further room configuration. Or grab a room from a list of complete rooms?

            newRuin.rooms.Add(newRoom);
        }

        ruinCollection.Add(newRuin);
    }

    public void RuinA()
    {
        var newRuin = new OldWorldRuin();

        newRuin.printName = "Thalik-Ra";
        newRuin.difficulty = 3;
        newRuin.depth = 15;
        newRuin.rooms = new List<RuinRoom>();

        //Room 1
        var newRoom = new RuinRoom();
        newRoom.roomDescription = "I enter the ruins of Thalik-Ra through gates of wrought iron. Mist hangs densely in the air and gathers along the earthern floor, coiling its pale, white arms around overgrown granite stones that mark long forgotten graves. Ahead of me to the north is an open iron gate.";
        
        newRoom.northExit.pathType = RuinPath.PathType.openHallway;
        newRoom.southExit.pathType = RuinPath.PathType.originDoor;
        newRoom.eastExit.pathType = RuinPath.PathType.wall;
        newRoom.westExit.pathType = RuinPath.PathType.wall;

        newRoom.northExit.canBeForced = true;

        newRuin.rooms.Add(newRoom);


        ruinCollection.Add(newRuin);
    }

    public void ConfigurePathTypes(RuinRoom room, RuinPath.PathType north, RuinPath.PathType south, RuinPath.PathType east, RuinPath.PathType west)
    {
        room.northExit.pathType = north;
        room.southExit.pathType = south;
        room.eastExit.pathType = east;
        room.westExit.pathType = west;
    }
}
