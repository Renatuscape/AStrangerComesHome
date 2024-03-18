using UnityEngine;

public class AlchemyProgressBar
{
    public SynthesiserData synthData;
    public GameObject fillBar;
    public float fullCoordinate;
    public float emptyCoordinate;

    public void Initialise(SynthesiserData synthData)
    {
        this.synthData = synthData;
    }
}