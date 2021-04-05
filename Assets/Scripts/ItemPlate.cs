using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ClientLogic;
using static GameManager;

public class ItemPlate : MonoBehaviour
{
    public Image background;
    public ClientLogic clientLogic;
    public Text adressText;
    public int adress;
    public Type type;
    public object currentObject, currentTempObject;
    public UIManager uiManager;
    public enum Type
    {
        sensor,
        device
    }
    Type ConvertStringToType(string type)
    {
        Type type1 = Type.device;
        if(type == "Sensor")
        {
            type1 = Type.sensor;
        }
        else if(type == "Device")
        {
            type1 = Type.device;
        }
        return type1;
    }
    public void Init(ClientLogic clientLogic,int adress, string type, object currentObject, UIManager uiManager, bool isChecked = false, object currentTempObject = null)
    {
        Color color;
        color = background.color;
        this.type = ConvertStringToType(type);
        if (!isChecked)
            color.a = 0;
        else
            color.a = 255;
        background.color = color;
        this.clientLogic = clientLogic;
        adressText.text = "ID:" + adress;
        this.adress = adress;
        
        this.currentObject = currentObject;
        this.currentTempObject = currentTempObject;
        this.uiManager = uiManager;
    }
    public List<byte> GetListBytes(List<byte> list)
    {
        byte[] bA = BitConverter.GetBytes(adress);
        for (int i = 0; i < bA.Length; i++)
        {
            list.Add(bA[i]);
        }
        return list;
    }
    public void OnClick()
    {
        var currentConfigJson = clientLogic.downloadedJsons.Find(item => item.name.container == clientLogic.currentConfigJson.name.container);
        switch (type)
        {
            case Type.sensor:

                SensorJson sensorJson = null;

                if (currentConfigJson != null)
                {
                    foreach (var item in currentConfigJson.sensors)
                    {

                        if (item == (clientLogic.currentSensorOrDevice as SensorJson))
                        {
                            sensorJson = item;
                            Debug.Log(item.count);
                            
                        }
                    }
                    if(sensorJson != null)
                    {
                        if (currentConfigJson.itemMCs.Find(item => item == (currentObject as ItemMC)) != null)
                        {
                            currentConfigJson.itemMCs.Remove((currentObject as SensorMC));
                            sensorJson.sensorMCRegistrations.Add((currentObject as SensorMC));
                            sensorJson.currentCount--;
                            Color color;
                            color = background.color;
                            color.a = 0;
                            background.color = color;
                        }
                        else if(sensorJson.count > sensorJson.currentCount)
                        {
                            currentConfigJson.itemMCs.Add((currentObject as SensorMC));
                            sensorJson.currentCount++;
                            sensorJson.sensorMCRegistrations.Remove((currentObject as SensorMC));
                            Color color;
                            color = background.color;
                            color.a = 255;
                            background.color = color;
                            //uiManager.SendToMCPlatesToRegister();
                        }
                        else if (currentTempObject != null)
                        {
                            var objectC = currentConfigJson.itemMCs.Find(item => (item as SensorMC).adress == (currentTempObject as SensorMC).adress);
                            if (objectC != null)
                            {
                                currentConfigJson.itemMCs.Remove(objectC);
                                sensorJson.sensorMCRegistrations.Add((objectC as SensorMC));
                                sensorJson.currentCount--;
                                Color color;
                                color = background.color;
                                color.a = 0;
                                background.color = color;
                            }
                        }
                    }
                    //currentConfigJson.sensors
                }
                break;
            case Type.device:
                DeviceJson deviceJson = null;

                if (currentConfigJson != null)
                {
                    foreach (var item in currentConfigJson.devices)
                    {

                        if (item == (clientLogic.currentSensorOrDevice as DeviceJson))
                        {
                            deviceJson = item;
                            Debug.Log(item.count);

                        }
                    }
                    if (deviceJson != null)
                    {

                        if (currentConfigJson.itemMCs.Find(item => item == (currentObject as ItemMC)) != null)
                        {
                            currentConfigJson.itemMCs.Remove((currentObject as DeviceMC));
                            deviceJson.deviceMCRegistrations.Add((currentObject as DeviceMC));
                            Color color;
                            color = background.color;
                            color.a = 0;
                            background.color = color;
                            deviceJson.currentCount--;
                        }
                        else if (deviceJson.count > deviceJson.currentCount)
                        {
                            currentConfigJson.itemMCs.Add((currentObject as DeviceMC));
                            deviceJson.currentCount++;
                            deviceJson.deviceMCRegistrations.Remove((currentObject as DeviceMC));
                            //uiManager.SendToMCPlatesToRegister();
                            Color color;
                            color = background.color;
                            color.a = 255;
                            background.color = color;
                        }
                        else if (currentTempObject != null)
                        {
                            var objectC = currentConfigJson.itemMCs.Find(item => (item as DeviceMC).adress == (currentTempObject as DeviceMC).adress);
                            if (objectC != null)
                            {
                                currentConfigJson.itemMCs.Remove(objectC);
                                deviceJson.deviceMCRegistrations.Add((objectC as DeviceMC));
                                Color color;
                                color = background.color;
                                color.a = 0;
                                background.color = color;
                                deviceJson.currentCount--;
                            }
                        }
                    }
                    //currentConfigJson.sensors
                }
                else
                {
                    Debug.Log("Дарова, отец");
                }
                break;
        }
        
    }
    public void OnClick_TwoButton()
    {
        byte[] adressBytes = BitConverter.GetBytes(adress);
        List<byte> listBytes = new List<byte>();
        listBytes.Add(111);
        foreach (var item in adressBytes)
        {
            listBytes.Add(item);
        }
        listBytes.Add(0);
        TestPlugin.SetMessage(listBytes.ToArray());
    }
}
