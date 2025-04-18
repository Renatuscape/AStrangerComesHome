using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingTitleAnim : MonoBehaviour
{
    TextMeshProUGUI mesh;
    float delay = 0.4f;
    void Start()
    {
        mesh = GetComponent<TextMeshProUGUI>();
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
