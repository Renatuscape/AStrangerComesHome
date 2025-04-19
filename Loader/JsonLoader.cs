using System;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public abstract class JsonLoader
{
    public string path = Application.streamingAssetsPath;
    public string displayName = "data";
    public abstract Task StartLoading();
}
