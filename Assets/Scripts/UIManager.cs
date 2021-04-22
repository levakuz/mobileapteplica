using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ClientLogic;
using static GameManager;
using static NewGameManager;
public class UIManager : MonoBehaviour
{

    public RectTransform containersScrollView;
    public GameObject containerPrefab;
    public NewGameManager newGameManager;

    public GameObject clientServerWindow;
    public GameObject authorizationPanel;

    public ClientLogic clientLogic;
    public InputField loginInputField, passwordInputField;
    public GameObject getConfigurationsPanel;

    public GameObject containerItemsPanel;

    public RectTransform containerItemsScrollView;
    public GameObject itemFromContainerPrefab;
    public GameObject LoginAlert;
    //панели 
    public GameObject sensorFromContainerItemPanel;
    public GameObject deviceFromContainerItemPanel;

    //текста
    public Text sensorFromContainerItemCount;

    public Text sensorFromContainerItemPeriod;


    public string nickname;
    public GameObject textDisplay;


    public Text deviceFromContainerItemCount;
    public Text deviceFromContainerItemPeriod;
    //префабы структур 
    public GameObject itemSenserStructurePrefab;
    public GameObject itemDeviceStructurePrefab;
    //контенты скролл вьюшек
    public RectTransform itemsSenserStructureScrollView;
    public RectTransform itemsDeviceStructureScrollView;
    //списки айтемов
    public List<GameObject> containerItemsScrollViewList;
    public List<GameObject> itemsSenserStructureScrollViewList;
    public List<GameObject> itemsDeviceStructureScrollViewList;
    public List<GameObject> itemsContainersScrollViewList;
    public GameObject[] gameObjects;
    public GameObject[] gameObjects1;
    public byte[] bytesSend;
    public List<Button> gameObjectsButtonsSend;
    public GameObject changeContainersButton;
    public GameObject platePrefabOnRegister;
    public GameObject platePrefabOnOnAutentification;
    public RectTransform plateScrollViewOnRegister;
    public RectTransform plateScrollViewOnAutentification;
    public List<GameObject> platesInScrollViewOnRegister;
    public List<GameObject> platesInScrollViewOnAutentification;
    public GameObject registationPlatesPanel;
    public GameObject autentificationPlatesPanel;
    public List<Image> panels;
    public Toggle rememberToggle;
    public bool isRememberAutentification;
    public Button currentContainerSend;
    public Button currentSensorSend;
    public Button currentDeviceSend;
    public Button sendRegButton;
    public Button updateButton;

    public bool Logged = false;
    public bool isChange;
    public MicroController microController;
    //очистка предметов в скролл вью в панели контейнеров
    public void ClearItemsInContainersScrollView()
    {
        for (int i = 0; i < itemsContainersScrollViewList.Count; i++)
        {
            Destroy(itemsContainersScrollViewList[i]);
        }
        itemsContainersScrollViewList.Clear();
        containersScrollView.sizeDelta = new Vector2(0, 0);
    }
    //добавка предмета в скролл вью в панели контейнеров
    public void AddItemInContainersScrollView(string name)
    {
        if (itemsContainersScrollViewList == null)
        {
            itemsContainersScrollViewList = new List<GameObject>();
        }
        GameObject gO = Instantiate(containerPrefab, containersScrollView);
        itemsContainersScrollViewList.Add(gO);
        Rect rect;
        rect = containersScrollView.rect;
        rect.height += gO.GetComponent<RectTransform>().rect.height + 40;
        containersScrollView.sizeDelta = new Vector2(0, rect.height);
        gO.GetComponent<ConfigJSON>().Init(name, clientLogic);
    }
    //открытие клиент серверного окна(авторизация и прочее)
    public void OpenClientServerWindow()
    {
        clientServerWindow.SetActive(true);
        if (isRememberAutentification)
        {
            authorizationPanel.SetActive(false);
            getConfigurationsPanel.SetActive(false);
        }
        else
        {
            authorizationPanel.SetActive(true);
            getConfigurationsPanel.SetActive(false);
        }
        sensorFromContainerItemPanel.SetActive(false);
        deviceFromContainerItemPanel.SetActive(false);
        containerItemsPanel.SetActive(false);
    }
    public void OpenContainerList()
    {
        if (Logged == true)
        {
            clientLogic.RequestGetContainers();
            clientServerWindow.SetActive(true);
            authorizationPanel.SetActive(false);
            getConfigurationsPanel.SetActive(true);
            sensorFromContainerItemPanel.SetActive(false);
            deviceFromContainerItemPanel.SetActive(false);
            containerItemsPanel.SetActive(false);
            
        }
        else 
        {
            LoginAlert.SetActive(true);
        }
    }

    //метод вызываемый при клике на кнопку "Войти или Авторизация"
    public void AuthorizationStart()
    {
        isRememberAutentification = rememberToggle.isOn;
        clientLogic.RequestAuthorization(loginInputField.text, passwordInputField.text);
    }
    //метод вызываемый при успехе авторизации
    public void AuthorizationSucces()
    {
        //getConfigurationsPanel.SetActive(true);
        //authorizationPanel.SetActive(false);
        Logged = true;
        newGameManager.ClickOnSettings();
        
    }
    //очистка предметов в скролл вью в панели контейнера(одного!)
    public void ClearItemsInContainerItemsScrollView()
    {
        for (int i = 0; i < containerItemsScrollViewList.Count; i++)
        {
            Destroy(containerItemsScrollViewList[i]);
        }
        containerItemsScrollViewList.Clear();
        containerItemsScrollView.sizeDelta = new Vector2(0, 0);
    }
    //открытие окна с содержимым контейнера
    public void OnClickOpenContainerItemsPanel(ConfigJson configJson)
    {
        var currentConfigJson = clientLogic.downloadedJsons.Find(item => item.name.container == clientLogic.currentConfigJson.name.container);
        if (currentConfigJson.isSended)
        {
            currentContainerSend.interactable = false;
        }
        else
        {
            currentContainerSend.interactable = true;
        }
        containerItemsPanel.SetActive(true);
        ClearItemsInContainerItemsScrollView();
        for (int i = 0; i < clientLogic.getContainerResponse.body.sensors.Length; i++)
        {
            AddItemInContainerItemsScrollView("Sensor" + i, ContainerItem.TypeItem.sensor, sensorJson: currentConfigJson.sensors[i]);
            currentConfigJson.sensors[i].itemName = "Sensor" + i;
        }
        for (int i = 0; i < clientLogic.getContainerResponse.body.devices.Length; i++)
        {
            AddItemInContainerItemsScrollView("Device" + i, ContainerItem.TypeItem.device, deviceJson: currentConfigJson.devices[i]);
            currentConfigJson.devices[i].itemName = "Device" + i;
        }
    }
    //добавление предмета в скролл вью панели содержимого контейнера
    public void AddItemInContainerItemsScrollView(string name, ContainerItem.TypeItem typeItem, SensorJson sensorJson = null, DeviceJson deviceJson = null)
    {
        if (containerItemsScrollViewList == null)
        {
            containerItemsScrollViewList = new List<GameObject>();
        }
        GameObject gO = Instantiate(itemFromContainerPrefab, containerItemsScrollView);
        containerItemsScrollViewList.Add(gO);
        Rect rect;
        rect = containerItemsScrollView.rect;
        rect.height += gO.GetComponent<RectTransform>().rect.height + 40;
        containerItemsScrollView.sizeDelta = new Vector2(0, rect.height);
        gO.GetComponent<ContainerItem>().Init(name, typeItem, this, sensorJson, deviceJson);
    }
    //открытие окна с содержимым сенсора
    public void OpenSensorFromContainerItemPanel(SensorJson sensorJson)
    {
        var currentConfigJson = clientLogic.downloadedJsons.Find(item => item.name.container == clientLogic.currentConfigJson.name.container);
        sensorJson.CheckCountSends();
        if (!currentContainerSend.interactable)
        {
            if (sensorJson.isSended)
            {
                currentSensorSend.interactable = false;
            }
            else
            {
                currentSensorSend.interactable = true;
            }
        }
        else
        {
            currentSensorSend.interactable = false;
        }
        clientLogic.currentDeviceJson = null;
        clientLogic.currentSensorJson = sensorJson;
        clientLogic.currentSensorOrDevice = sensorJson;
        sensorFromContainerItemPanel.SetActive(true);
        sensorFromContainerItemCount.text = "count : " + sensorJson.count;
        sensorFromContainerItemPeriod.text = "period : " + sensorJson.period;
        ClearItemsInSensorFromContainerItemPanelScrollView();
        for (int i = 0; i < sensorJson.structure.Length; i++)
        {
            AddItemInSensorFromContainerItemPanelScrollView(sensorJson.structure[i]);
        }
    }
    //открытие окна с содержимым девайса
    public void OpenDeviceFromContainerItemPanel(DeviceJson deviceJson)
    {
        var currentConfigJson = clientLogic.downloadedJsons.Find(item => item.name.container == clientLogic.currentConfigJson.name.container);
        deviceJson.CheckCountSends();
        if (!currentContainerSend.interactable)
        {
            if (deviceJson.isSended)
            {
                currentDeviceSend.interactable = false;
            }
            else
            {
                currentDeviceSend.interactable = true;
            }
        }
        else
        {
            currentDeviceSend.interactable = false;
        }
        clientLogic.currentDeviceJson = deviceJson;
        clientLogic.currentSensorJson = null;
        clientLogic.currentSensorOrDevice = deviceJson;
        deviceFromContainerItemPanel.SetActive(true);
        deviceFromContainerItemCount.text = "count : " + deviceJson.count;
        deviceFromContainerItemPeriod.text = "period : " + deviceJson.period;
        ClearItemsInDeviceFromContainerItemPanelScrollView();
        for (int i = 0; i < deviceJson.structure.Length; i++)
        {
            AddItemInDeviceFromContainerItemPanelScrollView(deviceJson.structure[i]);
        }
    }
    //очистка содержимого скролл вью окна содержимого сенсора из контейнера
    public void ClearItemsInSensorFromContainerItemPanelScrollView()
    {
        for (int i = 0; i < itemsSenserStructureScrollViewList.Count; i++)
        {
            Destroy(itemsSenserStructureScrollViewList[i]);
        }
        itemsSenserStructureScrollViewList.Clear();
        itemsSenserStructureScrollView.sizeDelta = new Vector2(0, 0);
    }
    //добавление предмета в скролл вью содержимого сенсора
    public void AddItemInSensorFromContainerItemPanelScrollView(SensorStructureJson sensorStructureJson)
    {
        if (itemsSenserStructureScrollViewList == null)
        {
            itemsSenserStructureScrollViewList = new List<GameObject>();
        }
        GameObject gO = Instantiate(itemSenserStructurePrefab, itemsSenserStructureScrollView);
        itemsSenserStructureScrollViewList.Add(gO);
        Rect rect;
        rect = itemsSenserStructureScrollView.rect;
        rect.height += gO.GetComponent<RectTransform>().rect.height + 40;
        itemsSenserStructureScrollView.sizeDelta = new Vector2(0, rect.height);
        gO.GetComponent<ItemSenserStructure>().Init(sensorStructureJson.type, sensorStructureJson.count.ToString());
    }
    //очистка содержимого скролл вью окна содержимого девайса из контейнера
    public void ClearItemsInDeviceFromContainerItemPanelScrollView()
    {
        for (int i = 0; i < itemsDeviceStructureScrollViewList.Count; i++)
        {
            Destroy(itemsDeviceStructureScrollViewList[i]);
        }
        itemsDeviceStructureScrollViewList.Clear();
        itemsDeviceStructureScrollView.sizeDelta = new Vector2(0, 0);
    }
    //добавление предмета в скролл вью содержимого девайса
    public void AddItemInDeviceFromContainerItemPanelScrollView(DeviceStructureJson deviceStructureJson)
    {
        if (itemsDeviceStructureScrollViewList == null)
        {
            itemsDeviceStructureScrollViewList = new List<GameObject>();
        }
        GameObject gO = Instantiate(itemDeviceStructurePrefab, itemsDeviceStructureScrollView);
        itemsDeviceStructureScrollViewList.Add(gO);
        Rect rect;
        rect = itemsDeviceStructureScrollView.rect;
        rect.height += gO.GetComponent<RectTransform>().rect.height + 40;
        itemsDeviceStructureScrollView.sizeDelta = new Vector2(0, rect.height);
        gO.GetComponent<ItemDeviceStructure>().Init(deviceStructureJson);
    }
    //возврат назад из окна контейнера
    public void BackContainerItemsPanel()
    {
        getConfigurationsPanel.SetActive(true);
        containerItemsPanel.SetActive(false);
        for (int i = 0; i < containerItemsScrollViewList.Count; i++)
        {
            Destroy(containerItemsScrollViewList[i]);
        }
        Rect rect;
        rect = containerItemsScrollView.rect;
        rect.height = 0;
        containerItemsScrollView.sizeDelta = new Vector2(0, rect.height);
        ChangeContainer();
    }
    //возврат назад из окна содержимого сенсора
    public void BackSensorFromContainerItemPanel()
    {
        containerItemsPanel.SetActive(true);
        sensorFromContainerItemPanel.SetActive(false);
        for (int i = 0; i < itemsSenserStructureScrollViewList.Count; i++)
        {
            Destroy(itemsSenserStructureScrollViewList[i]);
        }
        Rect rect;
        rect = itemsSenserStructureScrollView.rect;
        rect.height = 0;
        itemsSenserStructureScrollView.sizeDelta = new Vector2(0, rect.height);
    }
    //возврат назад из окна содержимого девайса
    public void BackDeviceFromContainerItemPanel()
    {
        containerItemsPanel.SetActive(true);
        deviceFromContainerItemPanel.SetActive(false);
        for (int i = 0; i < itemsDeviceStructureScrollViewList.Count; i++)
        {
            Destroy(itemsDeviceStructureScrollViewList[i]);
        }
        Rect rect;
        rect = itemsDeviceStructureScrollView.rect;
        rect.height = 0;
        itemsDeviceStructureScrollView.sizeDelta = new Vector2(0, rect.height);
    }
    ////возврат назад из окна содержимого json'a в виде контейнеров
    public void BackGetConfigurationsPanel()
    {
        getConfigurationsPanel.SetActive(false);
        clientServerWindow.SetActive(false);
        for (int i = 0; i < itemsContainersScrollViewList.Count; i++)
        {
            Destroy(itemsContainersScrollViewList[i]);
        }
        Rect rect;
        rect = containersScrollView.rect;
        rect.height = 0;
        containersScrollView.sizeDelta = new Vector2(0, rect.height);
    }
    //отправка сенсора на мк
    public void SendToMCSensor()
    {
        microController.SendToMCSensor();
        StartCoroutine(microController.SendToMCSensorWaitResponse());
    }
    //отправка девайса на мk
    public void SendToMCDevice()
    {
        microController.SendToMCDevice();
        StartCoroutine(microController.SendToMCDeviceWaitResponse());
    }
    //отправка контейнера на мк
    public void SendToMCContainer()
    {
        microController.SendToMCContainer();
        StartCoroutine(microController.SendToMCContainerWaitResponse());
    }
    //отправка плат на регистрацию на мк
    public void SendToMCPlatesToRegister()
    {
        
        microController.SendToMCPlatesToRegister();
        StartCoroutine(microController.SendToMCPlatesToRegisterWaitResponse());
        UpdateAuth();
        //StartCoroutine(microController.UpdateAuthCor());

    }
    //выход из аккаунта
    public void OnClickExitFromAccount()
    {
        getConfigurationsPanel.SetActive(false);
        authorizationPanel.SetActive(true);
        rememberToggle.isOn = false;
        isRememberAutentification = false;
        textDisplay.GetComponent<Text>().text = "";
    }
    //отправка запроса на обновление данных на мк
    public void UpdateAuth()
    {
        /*if (platesInScrollViewOnAutentification.Count == 0)
        {
            updateButton.interactable = false;
        }
        else
        {
            updateButton.interactable = true;
        }
        if (platesInScrollViewOnAutentification.Count == 0) return;*/
        microController.UpdateAuth();
        StartCoroutine(microController.UpdateAuthCor());
    }
    //смена содержимого окна с скролл вью содержащего контейнера
    public void OnClickChangeContainersButton()
    {
        var currentConfigJson = clientLogic.downloadedJsons.Find(item => item.name.container == clientLogic.currentConfigJson.name.container);
        if (currentConfigJson == null) return;

        if (currentConfigJson.isSended)
        {
            if (clientLogic.currentConfigJsonButton == null) return;
            Rect rect;
            foreach (var item in itemsContainersScrollViewList)
            {
                containerItemsScrollView.sizeDelta = new Vector2(0, 0);
                if (isChange)
                {
                    item.SetActive(true);
                    rect = containerItemsScrollView.rect;
                    rect.height -= item.GetComponent<RectTransform>().rect.height + 40;
                    containerItemsScrollView.sizeDelta = new Vector2(0, rect.height);
                }
                else
                {
                    item.SetActive(false);
                    rect = containerItemsScrollView.rect;
                    rect.height += item.GetComponent<RectTransform>().rect.height - 40;
                    containerItemsScrollView.sizeDelta = new Vector2(0, rect.height);
                }
            }
            clientLogic.currentConfigJsonButton.SetActive(true);

            rect = containerItemsScrollView.rect;
            rect.height += clientLogic.currentConfigJsonButton.GetComponent<RectTransform>().rect.height - 40;
            containerItemsScrollView.sizeDelta = new Vector2(0, rect.height);
            isChange = !isChange;
        }
    }
    public void ChangeContainer()
    {
        var currentConfigJson = clientLogic.downloadedJsons.Find(item => item.name.container == clientLogic.currentConfigJson.name.container);
        if (currentConfigJson == null) return;

        if (currentConfigJson.isSended)
        {
            if (clientLogic.currentConfigJsonButton == null) return;
            Rect rect;
            foreach (var item in itemsContainersScrollViewList)
            {
                containerItemsScrollView.sizeDelta = new Vector2(0, 0);

                item.SetActive(false);
                rect = containerItemsScrollView.rect;
                rect.height += item.GetComponent<RectTransform>().rect.height - 40;
                containerItemsScrollView.sizeDelta = new Vector2(0, rect.height);
            }
            clientLogic.currentConfigJsonButton.SetActive(true);

            rect = containerItemsScrollView.rect;
            rect.height += clientLogic.currentConfigJsonButton.GetComponent<RectTransform>().rect.height - 40;
            containerItemsScrollView.sizeDelta = new Vector2(0, rect.height);
            isChange = true;
        }
    }

    public void StoreName()
    {
        nickname = loginInputField.text;
        textDisplay.GetComponent<Text>().text = nickname;
    }
    //инициализация
    public void Start()
    {
        foreach (var item in gameObjectsButtonsSend)
        {
            item.interactable = false;
        }
    }
}