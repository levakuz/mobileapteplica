using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ClientLogic;

public class ContainerItem : MonoBehaviour
{
    public string nameItem;
    public Text nameText;
    public SensorJson sensorJson;
    public DeviceJson deviceJson;
    public TypeItem typeItem;
    public UIManager uIManager;
    public enum TypeItem
    {
        sensor,
        device
    }
    public void Init(string name, TypeItem typeItem, UIManager uIManager, SensorJson sensorJson = null, DeviceJson deviceJson = null)
    {
        nameItem = name;
        nameText.text = name;
        this.sensorJson = sensorJson;
        this.deviceJson = deviceJson;
        this.typeItem = typeItem;
        this.uIManager = uIManager;
    }
    public void OnClick()
    {
        switch (typeItem)
        {
            case TypeItem.sensor:
                uIManager.OpenSensorFromContainerItemPanel(sensorJson);
                break;
            case TypeItem.device:
                uIManager.OpenDeviceFromContainerItemPanel(deviceJson);
                break;
        }
        
    }
}
