using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigJSON : MonoBehaviour
{
    public string nameContainer;
    public Text nameTextConfig;
    public ClientLogic clientLogic;
    public void Init(string name,ClientLogic clientLogic)
    {
        nameTextConfig.text = name;
        nameContainer = name;
        this.clientLogic = clientLogic;
    }
    public void OnClick()
    {
        clientLogic.RequestGetContainer(nameContainer,this.gameObject);
    }
}
