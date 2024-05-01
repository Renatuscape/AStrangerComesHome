[System.Serializable]
public class ParticleData
{
    public string particleID;
    public float particleLife;
    public float velocity;
    public float randomDrag;
    public float verticalAcceleration;
    public float horizontalAcceleration;
    public float gravity;
    public float scatterRange;
    public bool adjustForCoachSpeed;
    public bool isSpinning;
    public bool isSpinningLeft;
    public bool isGrowing;
    public bool isShrinking;
    public bool isFadeDisabled; // otherwise fade gradually over time
}