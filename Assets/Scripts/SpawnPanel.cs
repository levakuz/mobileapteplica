using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPanel : MonoBehaviour
{
    public GameObject SwitchPanel;
    public Transform obj;
    public int k = 0;
    public float yPos = -580;
    void Start()
    {
        GameObject panel = SwitchPanel.GetComponent<GameObject>();
    }
    public void isClick()
    {
        var xLeftPos = -340;
        var xRightPos = 380;
        var go = Instantiate(SwitchPanel, obj);
        if (k % 2 == 0)
        {
            go.transform.localPosition = new Vector2(xLeftPos, yPos);
            k++;
        }
        else
        {
            go.transform.localPosition = new Vector2(xRightPos, yPos);
            k++;
            yPos -= 1230;
        }
        Debug.Log(yPos);
    }
}
