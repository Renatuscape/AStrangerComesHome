using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Synthesiser
{
    SynthesiserA,
    SynthesiserB,
    SynthesiserC
}
public enum SynthState
{
    Inactive, //unlit graphic
    Active, //bright pulsing graphic (teal)
    Paused, //low, constant light (purple)
    Complete //bright constant light (seagreen)
}

/*TO DO LIST
 * Show recipe name properly (if r.dataValue < 1, show ???)
 * View known recipes
 * Test for discoverable and non-discoverable recipes
 * Test for recipe level against alchemy level
 * Add yield adjustments for synth results
 * Add pause button functionality
 * Disable item select during synthesis
 * Add indicator for current synthState
 */

public class SynthMenu : MonoBehaviour
{
    public DataManagerScript dataManager;
    public TransientDataScript transientData;
    public Synthesiser thisSynthesiser;
    public Item failedExperiment;
    public List<Recipe> recipeList;



    public Canvas dInvenCanvas;
    public DynamicInventory dInven;

    public Item ingredientA;
    public TextMeshProUGUI ingNameA;
    public Item ingredientB;
    public TextMeshProUGUI ingNameB;
    public Item ingredientC;
    public TextMeshProUGUI ingNameC;

    public GameObject btnCreate;
    public GameObject btnPause;
    public GameObject btnCollect;

    public Item activeItem;

    private void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }
    private void Update()
    {
        if (thisSynthesiser == Synthesiser.SynthesiserA)
            ButtonChange(ref dataManager.isSynthActiveA, ref dataManager.progressSynthA, dataManager.synthItemA);
        else if (thisSynthesiser == Synthesiser.SynthesiserB)
            ButtonChange(ref dataManager.isSynthActiveB, ref dataManager.progressSynthB, dataManager.synthItemB);
        else if (thisSynthesiser == Synthesiser.SynthesiserC)
            ButtonChange(ref dataManager.isSynthActiveC, ref dataManager.progressSynthC, dataManager.synthItemC);

        if (dInven.gameObject.activeInHierarchy == true)
        {
            activeItem = dInven.activeItem;
        }
    }

    private void OnEnable()
    {

        if (transientData.objectIndex != null)
        {
            foreach (MotherObject mo in transientData.objectIndex)
            {
                if (mo is Recipe)
                {
                    var x = (Recipe)mo;
                    recipeList.Add(x);
                }
            }
        }
    }
    private void OnDisable()
    {
        recipeList.Clear();
    }

    private void ButtonChange(ref bool synthActive, ref float synthProgress, Item i)
    {
        if (i != null)
        {
            if (synthActive && synthProgress < i.recipe.maxSynth)
            {
                btnCreate.SetActive(false);
                btnPause.SetActive(true);
                btnCollect.SetActive(false);
            }
            else if (synthActive && synthProgress >= i.recipe.maxSynth)
            {
                btnCreate.SetActive(false);
                btnPause.SetActive(false);
                btnCollect.SetActive(true);
            }
            else
            {
                btnCreate.SetActive(true);
                btnPause.SetActive(false);
                btnCollect.SetActive(false);
            }
        }
        else
        {
            btnCreate.SetActive(true);
            btnPause.SetActive(false);
            btnCollect.SetActive(false);
        }
    }

    //INGREDIENT SELECTION
    public void IngSlotClick(GameObject obj)
    {
        if (activeItem != null && dInvenCanvas.gameObject.activeInHierarchy == true)
        {
            obj.GetComponent<Image>().sprite = activeItem.sprite;

            if (obj.name == "IngBoxA")
            {
                ingredientA = activeItem;
                ingNameA.text = activeItem.printName;
            }
            else if (obj.name == "IngBoxB")
            {
                ingredientB = activeItem;
                ingNameB.text = activeItem.printName;
            }
            else if (obj.name == "IngBoxC")
            {
                ingredientC = activeItem;
                ingNameC.text = activeItem.printName;
            }
            else
                Debug.Log("Ingredient box matched no names. No ingredient set.");
        }
        else
        {
            dInven.displayCatalysts = true;
            dInven.displayMaterials = true;
            dInven.displayMisc = false;
            dInven.displayPlants = true;
            dInven.displaySeeds = false;
            dInven.displayTrade = false;
            dInven.displayTreasures = true;

            dInvenCanvas.gameObject.SetActive(true);
            dInven.PopulateItemContainer(DynamicInventoryPage.Catalysts);
        }
    }

    //RECIPE SELECTION
    public void CheckRecipe()
    {
        bool noMatchFound = true;

        foreach (Recipe r in recipeList)
        {
            HashSet<Item> recipeIngredients = new HashSet<Item>() { r.ingredientA, r.ingredientB, r.ingredientC };
            HashSet<Item> playerIngredients = new HashSet<Item>() { ingredientA, ingredientB, ingredientC };

            if (recipeIngredients.SetEquals(playerIngredients))
            {
                noMatchFound = false;
                CheckIngredientAmount(r);
                break; // Exit the loop if a match is found
            }
        }

        if (noMatchFound == true)
            FailedExperiment(ingredientA, ingredientB, ingredientC);
    }
    void FailedExperiment(Item a, Item b, Item c) //Start synthesis with failed experiment
    {
        a.dataValue--;
        b.dataValue--;
        c.dataValue--;

        if (a.dataValue < 0 || b.dataValue < 0 || c.dataValue < 0)
            Debug.Log("I think this recipe could work if I have more of the ingredients.");
        else
            StartSynth(failedExperiment);
    }
    void CheckIngredientAmount(Recipe r)
    {
        r.ingredientA.dataValue -= r.ingredientAmountA;
        r.ingredientB.dataValue -= r.ingredientAmountB;
        r.ingredientC.dataValue -= r.ingredientAmountC;

        //Check if any dataValues are below 0 after subtraction. This way you account for recipes with duplicate ingredients
        if (r.ingredientA.dataValue < 0|| r.ingredientB.dataValue < 0 || ingredientC.dataValue <0)
        {
            Debug.Log("I think this recipe could work if I have more of the ingredients.");
            r.ingredientA.dataValue += r.ingredientAmountA;
            r.ingredientB.dataValue += r.ingredientAmountB;
            r.ingredientC.dataValue += r.ingredientAmountC;
        }
        else
            StartSynth(r.createsItem); //Start a successful synthesis
    }
    void StartSynth(Item i)
    {
        if (thisSynthesiser == Synthesiser.SynthesiserA)
        {
            dataManager.synthItemA = i;
            dataManager.progressSynthA = 0;
            dataManager.isSynthActiveA = true;
            dataManager.isSynthPausedA = false;
        }
        else if (thisSynthesiser == Synthesiser.SynthesiserB)
        {
            dataManager.synthItemB = i;
            dataManager.progressSynthB = 0;
            dataManager.isSynthActiveB = true;
            dataManager.isSynthPausedB = false;
        }
        else if (thisSynthesiser == Synthesiser.SynthesiserC)
        {
            dataManager.synthItemC = i;
            dataManager.progressSynthC = 0;
            dataManager.isSynthActiveC = true;
            dataManager.isSynthPausedC = false;
        }

        if (i.recipe.dataValue < 1 && i.recipe.isDiscoverable)
            i.recipe.dataValue = 1;
        dInvenCanvas.gameObject.SetActive(false);
    }

    //COLLECT FINISHED PRODUCT
    public void CollectSynthItem()
    {
        if (thisSynthesiser == Synthesiser.SynthesiserA)
            UpdateDataManager(ref dataManager.isSynthActiveA, ref dataManager.synthItemA);
        else if (thisSynthesiser == Synthesiser.SynthesiserB)
            UpdateDataManager(ref dataManager.isSynthActiveB, ref dataManager.synthItemB);
        else if (thisSynthesiser == Synthesiser.SynthesiserC)
            UpdateDataManager(ref dataManager.isSynthActiveC, ref dataManager.synthItemC);
    }
    void UpdateDataManager(ref bool active, ref Item item)
    {
        if (item.dataValue < item.maxValue)
        {
            active = false;
            item.dataValue += 1;
            Debug.Log($"I successfully synthesised <b>{item.printName}</b>!");
        }
        else
            Debug.Log("Seems like I have too many of these right now.");
    }
}
