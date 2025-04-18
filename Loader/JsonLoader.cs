using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;

public abstract class JsonLoader : MonoBehaviour
{
    public string path = Application.streamingAssetsPath;
    public string displayName = "Data";
    public abstract Task StartLoading();
}
