using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ClientLogic;
using static GameManager;

public class MicroController : MonoBehaviour
{
    //ссылка на менеджер интерфейса
    public UIManager uIManager;
    //корутина получения ответа от мк и дальнейшее преобразование его в тип ответа для сенсора
    public void SendToMCSensor()
    {
        #if UNITY_EDITOR

#elif UNITY_ANDROID
        uIManager.bytesSend = uIManager.clientLogic.currentSensorJson.GetByteArray(new List<byte>()).ToArray();
        TestPlugin.SetMessage(uIManager.bytesSend);
#endif
    }
    public IEnumerator SendToMCSensorWaitResponse()
    {
        string i = "0";
        float timeValue = 0;
#if UNITY_EDITOR
        yield return new WaitForEndOfFrame();
#elif UNITY_ANDROID
        
        while (i == "0" & timeValue < 2f)
        {
            yield return new WaitForEndOfFrame();
            timeValue += Time.deltaTime;
            i = TestPlugin.GetLenght().ToString();
        }
#endif

        try
        {
            byte[] bA;
#if UNITY_EDITOR
            bA = MCSimulator.GetMessage106();
#elif UNITY_ANDROID
            bA = TestPlugin.GetMessage(Convert.ToInt32(i));
#endif
            if (bA != null)//gameObjectsButtonsSend
            {
                if (bA.Length > 0)
                {
                    if (bA[0] == 106)
                    {
                        for (int k = 0; k < uIManager.platesInScrollViewOnRegister.Count; k++)
                        {
                            Destroy(uIManager.platesInScrollViewOnRegister[k]);
                        }

                        uIManager.platesInScrollViewOnRegister.Clear();
                        uIManager.platesInScrollViewOnRegister = new List<GameObject>();
                        uIManager.plateScrollViewOnRegister.sizeDelta = new Vector2(0, 0);
                        var currentConfigJson = uIManager.clientLogic.downloadedJsons.Find(item => item.name.container == uIManager.clientLogic.currentConfigJson.name.container);
                        SensorJson sensorJson = null;
                        if (currentConfigJson != null)
                        {
                            foreach (var item in currentConfigJson.sensors)
                            {
                                if (item == (uIManager.clientLogic.currentSensorOrDevice as SensorJson))
                                {
                                    sensorJson = item;
                                    Debug.Log(item.count);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            
                        }
                        for (int j = 2; j < bA[1] * 4 + 2; j += 4)
                        {
                            int adress = BitConverter.ToInt32(new byte[] { bA[j], bA[j + 1], bA[j + 2], bA[j + 3] }, 0);

                            SensorMC authSensor = sensorJson.sensorMCAuthorizations.Find(item => item.adress == adress);
                            SensorMC regSensor = sensorJson.sensorMCRegistrations.Find(item => item.adress == adress);
                            SensorMC tempSensor = currentConfigJson.itemMCs.Find(item => item.adress == adress & item.itemName == sensorJson.itemName) as SensorMC;
                            if (authSensor == null)
                            {
                                GameObject gO = Instantiate(uIManager.platePrefabOnRegister, uIManager.plateScrollViewOnRegister);
                                uIManager.platesInScrollViewOnRegister.Add(gO);
                                Rect rect;
                                rect = uIManager.plateScrollViewOnRegister.rect;
                                rect.height += gO.GetComponent<RectTransform>().rect.height + 40;
                                uIManager.plateScrollViewOnRegister.sizeDelta = new Vector2(0, rect.height);
                                SensorMC sensorMC = new SensorMC(adress, sensorJson, sensorJson.itemName);
                                if (regSensor == null)
                                {
                                    sensorJson.sensorMCRegistrations.Add(sensorMC);
                                }
                                bool isfasdg = false;
                                if (tempSensor != null)
                                    isfasdg = true;
                                gO.GetComponent<ItemPlate>().Init(uIManager.clientLogic, adress, "Sensor", sensorMC, uIManager, isfasdg, sensorMC);
                            }
                        }

                        uIManager.registationPlatesPanel.SetActive(true);
                        uIManager.sendRegButton.interactable = true;
                        uIManager.clientServerWindow.SetActive(false);
                        TestPlugin.ShowLog("Успешно отправлен");
                    }
                }
            }
        }
        catch (Exception e)
        {
            TestPlugin.ShowLog(e.Message);
            TestPlugin.ShowLog(e.StackTrace);
        }
    }
    public void SendToMCDevice()
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        uIManager.bytesSend = uIManager.clientLogic.currentDeviceJson.GetByteArray(new List<byte>()).ToArray();
        TestPlugin.SetMessage(uIManager.bytesSend);
#endif
    }
    //корутина получения ответа от мк и дальнейшее преобразование его в тип ответа для девайса
    public IEnumerator SendToMCDeviceWaitResponse()
    {
        string i = "0";
        float timeValue = 0;
#if UNITY_EDITOR
        yield return new WaitForEndOfFrame();
#elif UNITY_ANDROID

        while (i == "0" & timeValue < 2f)
        {
            yield return new WaitForEndOfFrame();
            timeValue += Time.deltaTime;
            i = TestPlugin.GetLenght().ToString();
        }
#endif
        try
        {
            byte[] bA;
#if UNITY_EDITOR
            bA = MCSimulator.GetMessage108();
#elif UNITY_ANDROID
            bA = TestPlugin.GetMessage(Convert.ToInt32(i));
#endif
            if (bA != null)
            {
                if (bA.Length > 0)
                {
                    if (bA[0] == 108)
                    {
                        for (int k = 0; k < uIManager.platesInScrollViewOnRegister.Count; k++)
                        {
                            Destroy(uIManager.platesInScrollViewOnRegister[k]);
                        }

                        uIManager.platesInScrollViewOnRegister.Clear();
                        uIManager.platesInScrollViewOnRegister = new List<GameObject>();
                        uIManager.plateScrollViewOnRegister.sizeDelta = new Vector2(0, 0);
                        var currentConfigJson = uIManager.clientLogic.downloadedJsons.Find(item => item.name.container == uIManager.clientLogic.currentConfigJson.name.container);
                        DeviceJson deviceJson = null;
                        if (currentConfigJson != null)
                        {
                            foreach (var item in currentConfigJson.devices)
                            {
                                if (item == (uIManager.clientLogic.currentSensorOrDevice as DeviceJson))
                                {
                                    deviceJson = item;
                                    Debug.Log(item.count);
                                }
                            }
                        }
                        for (int j = 2; j < bA[1] * 4 + 2; j += 4)
                        {
                            int adress = BitConverter.ToInt32(new byte[] { bA[j], bA[j + 1], bA[j + 2], bA[j + 3] }, 0);

                            DeviceMC authDevice = deviceJson.deviceMCAuthorizations.Find(item => item.adress == adress);
                            DeviceMC regDevice = deviceJson.deviceMCRegistrations.Find(item => item.adress == adress);
                            DeviceMC tempDevice = currentConfigJson.itemMCs.Find(item => item.adress == adress & item.itemName == deviceJson.itemName) as DeviceMC;
                            if (authDevice == null)
                            {
                                GameObject gO = Instantiate(uIManager.platePrefabOnRegister, uIManager.plateScrollViewOnRegister);
                                uIManager.platesInScrollViewOnRegister.Add(gO);
                                Rect rect;
                                rect = uIManager.plateScrollViewOnRegister.rect;
                                rect.height += gO.GetComponent<RectTransform>().rect.height + 40;
                                uIManager.plateScrollViewOnRegister.sizeDelta = new Vector2(0, rect.height);
                                DeviceMC deviceMC = new DeviceMC(adress, deviceJson, deviceJson.itemName);
                                if (regDevice == null)
                                {
                                    deviceJson.deviceMCRegistrations.Add(deviceMC);
                                }
                                bool isfasdg = false;
                                if (tempDevice != null)
                                    isfasdg = true;
                                gO.GetComponent<ItemPlate>().Init(uIManager.clientLogic, adress, "Device", deviceMC, uIManager, isfasdg, deviceMC);
                            }
                        }
                        uIManager.registationPlatesPanel.SetActive(true);
                        uIManager.sendRegButton.interactable = true;
                        uIManager.clientServerWindow.SetActive(false);
                        TestPlugin.ShowLog("Успешно отправлен");
                    }
                }
            }
        }
        catch (Exception e)
        {
            TestPlugin.ShowLog(e.Message);
            TestPlugin.ShowLog(e.StackTrace);
        }
    }
    public void SendToMCPlatesToRegister() //отправка на регистрацию 
    {
        List<byte> bL = new List<byte>();
        var currentConfigJson = uIManager.clientLogic.downloadedJsons.Find(item => item.name.container == uIManager.clientLogic.currentConfigJson.name.container);
        bL.Add(109);

        for (int i = 0; i < currentConfigJson.itemMCs.Count; i++)
        {
            switch (currentConfigJson.itemMCs[i].type)
            {
                case ItemMC.Type.sensor:
                    bL = (currentConfigJson.itemMCs[i] as SensorMC).GetListBytes(bL);
                    break;
                case ItemMC.Type.device:
                    bL = (currentConfigJson.itemMCs[i] as DeviceMC).GetListBytes(bL);
                    break;
            }
        }
        print("SendToMCPlatesToRegister(bL) = " + bL.Count);
#if UNITY_EDITOR
        MCSimulator.SetMessage109(bL.ToArray());
#elif UNITY_ANDROID
        TestPlugin.SetMessage(bL.ToArray());
#endif
    }
    //корутина получения ответа от мк и дальнейшее преобразование его в тип ответа для регистрации айтемов
    public IEnumerator SendToMCPlatesToRegisterWaitResponse()
    {
        string i = "0";
        float timeValue = 0;
#if UNITY_EDITOR
        yield return new WaitForEndOfFrame();
#elif UNITY_ANDROID

        while (i == "0" & timeValue < 2f)
        {
            yield return new WaitForEndOfFrame();
            timeValue += Time.deltaTime;
            i = TestPlugin.GetLenght().ToString();
        }
#endif
        try
        {
            byte[] bA;
            var currentConfigJson = uIManager.clientLogic.downloadedJsons.Find(item => item.name.container == uIManager.clientLogic.currentConfigJson.name.container);
#if UNITY_EDITOR

            bA = MCSimulator.GetMessage109(currentConfigJson.itemMCs.Count);
#elif UNITY_ANDROID
            bA = TestPlugin.GetMessage(Convert.ToInt32(i));
#endif
            if (bA != null)
            {
                if (bA.Length > 0)
                {
                    TestPlugin.ShowLog("SendToMCPlatesToRegisterWaitResponse");
                    for (int k = 0; k < bA.Length; k++)
                    {
                        TestPlugin.ShowLog("bA[" + k + "]=" + bA[k]);
                    }

                    if (bA[0] == 109)
                    {
                        Rect rect;
                        int iterator = 1;
                        for (int j = 1, jj = 0; j < bA.Length; j += iterator, jj++)
                        {
                            if (bA[j] == 0)
                            {


                                GameObject gO = null;
                                switch (currentConfigJson.itemMCs[jj].type)
                                {
                                    case ItemMC.Type.sensor:
                                        SensorMC sensorMC = currentConfigJson.itemMCs[jj] as SensorMC;
                                        sensorMC.sensorJson.sensorMCAuthorizations.Add(sensorMC);
                                        gO = Instantiate(uIManager.platePrefabOnOnAutentification, uIManager.plateScrollViewOnAutentification);
                                        rect = uIManager.plateScrollViewOnAutentification.rect;
                                        rect.height += gO.GetComponent<RectTransform>().rect.height + 10;
                                        rect.width += gO.GetComponent<RectTransform>().rect.width + 30;
                                        uIManager.plateScrollViewOnAutentification.sizeDelta = new Vector2(rect.width, rect.height);
                                        gO.GetComponent<PlatePrefabOnOnAutentification>().Init(sensorMC.adress.ToString(), "Sensor", currentConfigJson.name.container, sensorMC.itemName);
                                        TestPlugin.ShowLog("Sensor:" + sensorMC.adress + ", success");
                                        break;
                                    case ItemMC.Type.device:
                                        DeviceMC deviceMC = currentConfigJson.itemMCs[jj] as DeviceMC;
                                        deviceMC.deviceJson.deviceMCAuthorizations.Add(deviceMC);
                                        gO = Instantiate(uIManager.platePrefabOnOnAutentification, uIManager.plateScrollViewOnAutentification);
                                        rect = uIManager.plateScrollViewOnAutentification.rect;
                                        rect.height += gO.GetComponent<RectTransform>().rect.height + 20;
                                        rect.width += gO.GetComponent<RectTransform>().rect.width + 30;
                                        uIManager.plateScrollViewOnAutentification.sizeDelta = new Vector2(rect.width, rect.height);
                                        gO.GetComponent<PlatePrefabOnOnAutentification>().Init(deviceMC.adress.ToString(), "Device", currentConfigJson.name.container, deviceMC.itemName);
                                        TestPlugin.ShowLog("Device:" + deviceMC.adress + ", success");
                                        break;
                                }
                                uIManager.platesInScrollViewOnAutentification.Add(gO);
                            }
                            else
                            {
                                switch (currentConfigJson.itemMCs[jj].type)
                                {
                                    case ItemMC.Type.sensor:
                                        SensorMC sensorMC = currentConfigJson.itemMCs[jj] as SensorMC;
                                        sensorMC.sensorJson.currentCount--;
                                        TestPlugin.ShowLog("Sensor:" + sensorMC.adress + ", error");
                                        break;
                                    case ItemMC.Type.device:
                                        DeviceMC deviceMC = currentConfigJson.itemMCs[jj] as DeviceMC;
                                        deviceMC.deviceJson.currentCount--;
                                        TestPlugin.ShowLog("Device:" + deviceMC.adress + ", error");
                                        break;
                                }
                            }

                        }
                        uIManager.plateScrollViewOnAutentification.sizeDelta = new Vector2(0, 0);
                        foreach (var item in uIManager.platesInScrollViewOnAutentification)
                        {
                            PlatePrefabOnOnAutentification platePrefabOnOnAutentification = item.GetComponent<PlatePrefabOnOnAutentification>();
                            if (platePrefabOnOnAutentification.fromContainer == currentConfigJson.name.container)
                            {
                                item.SetActive(true);
                                rect = uIManager.plateScrollViewOnAutentification.rect;
                                rect.height += item.GetComponent<RectTransform>().rect.height + 40;
                                uIManager.plateScrollViewOnAutentification.sizeDelta = new Vector2(0, rect.height);
                            }
                            else
                            {
                                item.SetActive(false);
                            }
                        }
                        currentConfigJson.itemMCs.Clear();
                        uIManager.plateScrollViewOnRegister.sizeDelta = new Vector2(0, 0);
                        uIManager.registationPlatesPanel.SetActive(false);
                        uIManager.autentificationPlatesPanel.SetActive(true);
                        UpdateAuth();
                        StartCoroutine(UpdateAuthCor());
                        uIManager.getConfigurationsPanel.SetActive(false);
                        if (uIManager.platesInScrollViewOnAutentification.Count == 0)
                        {
                            uIManager.updateButton.interactable = false;
                        }
                        else
                        {
                            uIManager.updateButton.interactable = true;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            TestPlugin.ShowLog(e.Message);
            TestPlugin.ShowLog(e.StackTrace);
        }
    }
    public void SendToMCContainer()
    {

#if UNITY_EDITOR

#elif UNITY_ANDROID
        uIManager.bytesSend = uIManager.clientLogic.currentConfigJson.GetByteArray().ToArray();
        TestPlugin.SetMessage(uIManager.bytesSend);
#endif
    }
    //корутина получения ответа от мк и дальнейшее преобразование его в тип ответа для контейнера
    public IEnumerator SendToMCContainerWaitResponse()
    {
        byte[] bA;
#if UNITY_EDITOR
        yield return new WaitForEndOfFrame();
        bA = MCSimulator.GetMessage103();
#elif UNITY_ANDROID
        string i = "0";
        float timeValue = 0;
        while (i == "0" & timeValue < 2f)
        {
            yield return new WaitForEndOfFrame();
            timeValue += Time.deltaTime;
            i = TestPlugin.GetLenght().ToString();
        }
        bA = TestPlugin.GetMessage(1);
#endif
        if (bA != null)
        {
            if (bA.Length > 0)
            {
                if (bA[0] == 103)
                {
                    foreach (var item in uIManager.gameObjectsButtonsSend)
                    {
                        item.interactable = true;
                    }
                    uIManager.changeContainersButton.SetActive(true);
                    uIManager.isChange = false;
                    uIManager.ChangeContainer();
                    uIManager.currentContainerSend.interactable = false;

                    var currentConfigJson = uIManager.clientLogic.downloadedJsons.Find(item => item.name.container == uIManager.clientLogic.currentConfigJson.name.container);
                    //currentConfigJson.Clear();
                    foreach (var item in uIManager.clientLogic.downloadedJsons)
                    {
                        if (currentConfigJson != item)
                        {
                            item.Clear();
                            uIManager.plateScrollViewOnAutentification.sizeDelta = new Vector2(0, 0);
                            List<GameObject> temp = new List<GameObject>();
                            foreach (var gO in uIManager.platesInScrollViewOnAutentification)
                            {
                                PlatePrefabOnOnAutentification platePrefabOnOnAutentification = gO.GetComponent<PlatePrefabOnOnAutentification>();
                                if (platePrefabOnOnAutentification.fromContainer == item.name.container)
                                {
                                    Destroy(gO);

                                }
                                else
                                {
                                    temp.Add(gO);
                                }
                            }
                            uIManager.platesInScrollViewOnAutentification.Clear();
                            foreach (var gO in temp)
                            {
                                uIManager.platesInScrollViewOnAutentification.Add(gO);
                            }
                        }
                    }
                    currentConfigJson.isSended = true;

                    TestPlugin.ShowLog("Успешно отправлен");
                }
            }
        }
        else
        {
            TestPlugin.ShowLog("Сбой отправки");
        }
    }
    public void UpdateAuth()
    {
        List<byte> bL = new List<byte>();
        byte[] bA;
        bL.Add(110);
        foreach (var item in uIManager.platesInScrollViewOnAutentification)
        {
            bA = BitConverter.GetBytes(item.GetComponent<PlatePrefabOnOnAutentification>().adressValue);
            for (int i = 0; i < bA.Length; i++)
            {
                bL.Add(bA[i]);
            }

        }
#if UNITY_EDITOR

#elif UNITY_ANDROID
        TestPlugin.SetMessage(bL.ToArray());
#endif
    }
    //корутина получения ответа от мк и дальнейшее преобразование его в тип ответа для обновления зарегистрированных айтемов
    public IEnumerator UpdateAuthCor()
    {
        string i = "0";
        float timeValue = 0;
#if UNITY_EDITOR
        yield return new WaitForEndOfFrame();
#elif UNITY_ANDROID

        while (i == "0" & timeValue < 2f)
        {
            yield return new WaitForEndOfFrame();
            timeValue += Time.deltaTime;
            i = TestPlugin.GetLenght().ToString();
        }
#endif
        try
        {
            byte[] bA;
            var currentConfigJson = uIManager.clientLogic.downloadedJsons.Find(item => item.name.container == uIManager.clientLogic.currentConfigJson.name.container);
#if UNITY_EDITOR
            bA = MCSimulator.GetMessage110(currentConfigJson.itemMCs.Count);
#elif UNITY_ANDROID
            bA = TestPlugin.GetMessage(Convert.ToInt32(i));
#endif
            if (bA != null)
            {
                if (bA.Length > 0)
                {
                    TestPlugin.ShowLog("SendToMCPlatesToRegisterWaitResponse");
                    for (int k = 0; k < bA.Length; k++)
                    {
                        TestPlugin.ShowLog("bA[" + k + "]=" + bA[k]);
                    }

                    if (bA[0] == 110)
                    {
                        int iterator = 1;
                        for (int j = 1, jj = 0; j < bA.Length; j += iterator, jj++)
                        {
                            GameObject gO = null;
                            byte typeData = bA[j + 1];
                            float dataSensor = 0;
                            short dataDevice = 0;
                            byte[] array;
                            PlatePrefabOnOnAutentification platePrefabOnOnAutentification = uIManager.platesInScrollViewOnAutentification[jj].GetComponent<PlatePrefabOnOnAutentification>();
                            if (bA[j] == 0x01)
                            {
                                iterator = 6;
                                array = new byte[4];
                                array[0] = bA[j + 2];
                                array[1] = bA[j + 3];
                                array[2] = bA[j + 4];
                                array[3] = bA[j + 5];
                                dataSensor = BitConverter.ToSingle(array, 0);
                                platePrefabOnOnAutentification.UpdateData(typeData, dataSensor);
                            }
                            else if (bA[j] == 0x02)
                            {

                                switch (typeData)
                                {
                                    case 0x02:
                                        iterator = 4;
                                        array = new byte[2];
                                        array[0] = bA[j + 2];
                                        array[1] = bA[j + 3];
                                        dataDevice = BitConverter.ToInt16(array, 0);
                                        platePrefabOnOnAutentification.UpdateData(typeData, dataDevice);
                                        break;
                                    case 0x03:
                                        iterator = 3;
                                        platePrefabOnOnAutentification.UpdateData(typeData, bA[j + 2]);
                                        break;
                                    case 0x04:
                                        iterator = 3;
                                        platePrefabOnOnAutentification.UpdateData(typeData, bA[j + 2]);
                                        break;
                                    case 0x05:
                                        iterator = 4;
                                        array = new byte[2];
                                        array[0] = bA[j + 2];
                                        array[1] = bA[j + 3];
                                        dataDevice = BitConverter.ToInt16(array, 0);
                                        platePrefabOnOnAutentification.UpdateData(typeData, dataDevice);

                                        break;
                                }

                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            TestPlugin.ShowLog(e.Message);
            TestPlugin.ShowLog(e.StackTrace);
        }
    }
}
