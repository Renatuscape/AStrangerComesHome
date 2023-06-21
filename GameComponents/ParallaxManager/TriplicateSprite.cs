using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class TriplicateSprite : MonoBehaviour
{
    public GameObject spriteObject;
    public Vector3 spriteSize;

    void Start()
    {
        spriteSize = spriteObject.GetComponent<SpriteRenderer>().bounds.size;
        TriplicateThis();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TriplicateThis()
    {
        var leftClone = Instantiate(spriteObject);
        leftClone.transform.parent = gameObject.transform;
        leftClone.transform.position = new Vector3(transform.position.x - spriteSize.x, transform.position.y, transform.position.z);

        var rightClone = Instantiate(spriteObject);
        rightClone.transform.parent = gameObject.transform;
        rightClone.transform.position = new Vector3(transform.position.x + spriteSize.x, transform.position.y, transform.position.z);
    }
}
