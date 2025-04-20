using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingTitleAnim : MonoBehaviour
{
    TextMeshProUGUI mesh;
    float delay = 0.4f;

    private void Awake()
    {
        mesh = GetComponent<TextMeshProUGUI>();
    }
    void OnEnable()
    {
        StartCoroutine("AnimateLoadingTitle");
    }

    IEnumerator AnimateLoadingTitle()
    {
        while (true)
        {
            mesh.text = "Loading";
            yield return new WaitForSeconds(delay);
            mesh.text = ". Loading .";
            yield return new WaitForSeconds(delay);
            mesh.text = ".. Loading ..";
            yield return new WaitForSeconds(delay);
            mesh.text = "... Loading ...";
            yield return new WaitForSeconds(delay);
        }
    }
}
