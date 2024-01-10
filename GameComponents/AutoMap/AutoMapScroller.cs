using UnityEngine;

public class AutoMapScroller
{
    float scrollSpeed = 0.05f;
    public int xCutOff = 6;
    public int yCutOff = 4;

    Transform map;

    public AutoMapScroller(AutoMap autoMap)
    {
        map = autoMap.mapContainer.transform;
    }

    public void ScrollMapUp()
    {
        if (map.position.y < yCutOff)
        {
            map.position = new Vector3(
                map.position.x,
                map.position.y + scrollSpeed,
                map.position.z);
        }
    }
    public void ScrollMapDown()
    {
        if (map.position.y > yCutOff * -1)
        {
            map.position = new Vector3(
                map.position.x,
                map.position.y - scrollSpeed,
                map.position.z);
        }
    }
    public void ScrollMapLeft()
    {
        if (map.position.x > xCutOff * -1)
        {
            map.position = new Vector3(
            map.position.x - scrollSpeed,
            map.position.y,
            map.position.z);
        }
    }
    public void ScrollMapRight()
    {
        if (map.position.x < xCutOff)
        {
            map.position = new Vector3(
            map.position.x + scrollSpeed,
            map.position.y,
            map.position.z);
        }
    }
}