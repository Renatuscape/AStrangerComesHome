
using UnityEngine;

public class CardFloatAnimator : MonoBehaviour
{
    public GameObject outline;
    public GameObject shadow;
    public int positionIndex;
    public float startPosition;
    float bobTarget = 6;
    float bobMagnitude = 1f;
    public bool isGoingDown = false;
    public float timer = 0;

    void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, startPosition, 0);
    }
    void Update()
    {
        timer += 1 * Time.deltaTime;

        if (timer >= 0.12f)
        {
            timer = 0;
            Animate();
        }
    }

    void Animate()
    {
        if (isGoingDown && transform.localPosition.y > 0)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - bobMagnitude, 0);
        }
        else if (!isGoingDown && transform.localPosition.y < bobTarget)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + bobMagnitude, 0);
        }
        if (transform.localPosition.y >= bobTarget)
        {
            isGoingDown = true;
        }
        else if (transform.localPosition.y <= 0)
        {
            isGoingDown = false;
        }

        outline.transform.position = transform.position;

        shadow.transform.position = new Vector3(transform.position.x + 15, transform.position.y - 15, 0);
    }

    public void IncrementIndex()
    {
        positionIndex--;
        if (positionIndex < 0 )
        {
            positionIndex = System.Enum.GetValues(typeof(JournalMainPage)).Length - 1;
        }
    }
}
