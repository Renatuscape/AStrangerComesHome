using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Slider bar;
    public TextMeshProUGUI contextMesh;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        GameManagerScript.loadingCanvas = this;
    }

    public void Simplify()
    {
        contextMesh.text = "";
        bar.gameObject.SetActive(false);
    }
}