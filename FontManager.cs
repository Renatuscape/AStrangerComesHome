using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class FontManager : MonoBehaviour
{
    public List<TMP_FontAsset> headerFonts;
    public List<TMP_FontAsset> subtitleFonts;
    public List<TMP_FontAsset> bodyFonts;
    public List<TMP_FontAsset> scriptFonts;

    public TextMeshProUGUI header;
    public TextMeshProUGUI subtitle;
    public TextMeshProUGUI body;
    public TextMeshProUGUI script;

    public bool isScriptEnabled = true;
}
