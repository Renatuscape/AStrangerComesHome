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
    public float minScale;
    public float maxScale;
    public float coachSpeedMultiplier;
    public bool adjustForCoachSpeed;
    public bool isFadeDisabled; // otherwise fade gradually over time
    public bool isGrowing;
    public bool isShrinking;
    public bool randomiseScale;
}