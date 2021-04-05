using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSenserStructure : MonoBehaviour
{
    public Text typeText, countText;

    public void Init(string type,string count)
    {
        typeText.text = "type:" + type;
        countText.text = "count:" + count;
    }
}
