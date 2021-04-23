using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class NewGameManager : MonoBehaviour
{
    public GameObject settingsPanel,deviceListPanel, registerPanel, currentPanel;
    public Sprite[] iconsNS,iconsS;
    public Image[] imagesB;

    public Text nameMC;
    public Text nickname;
    public GameObject Mconnection;

    public Button[] buttons;
    public GameObject LoginAlert, ExitAlert, DeviceAlert;

    public GameObject closeConnectionButton, findDevicesButton;

    public GameObject webPanel;
    public Button sendRegButton;
    public UIManager uIManager;
    public bool DeviceAlert_exist;

    //аналог map из c++
    public float Convert(float value, float From1, float From2, float To1, float To2)
    {
        return (value - From1) / (From2 - From1) * (To2 - To1) + To1;
    }
    //получение имени МК
    public void GetNameMC()
    {
        closeConnectionButton.SetActive(true);
        findDevicesButton.SetActive(false);
        StartCoroutine(GetNameMicroC());
        nickname.text = nameMC.text;
        nickname.text = Mconnection.GetComponent<Text>().text;
    }


    //корутина для получения имени МК
    private IEnumerator GetNameMicroC()
    {
#if UNITY_EDITOR
        yield return new WaitForEndOfFrame();
        nameMC.text = "Вы подключены к MCSimulator";
        nickname.text = "Устройство: MCSimulator";
        foreach (var item in buttons)
        {
            item.interactable = true;
        }
        
#elif UNITY_ANDROID
        string i = "0";
        yield return new WaitForEndOfFrame();
        byte[] bytes = new byte[1];
        int value;
        //bytes[0] = 1;
        bytes[0] = 0x02 + 100;
        TestPlugin.SetMessage(bytes);
        i = "0";
        while (i == "0")
        {
            yield return new WaitForEndOfFrame();
            value = TestPlugin.GetLenght();
            i = value.ToString();
        }
        int l = int.Parse(i);
        byte[] iA;
        iA = TestPlugin.GetMessage(l);
        nameMC.text = "Вы подключены к ";
        nickname.text = "Устройство: "
        for (int j = 0; j < iA.Length; j++)
        {
            nameMC.text += (char)iA[j];
            nickname.text += (char)iA[j];
        }
        foreach (var item in buttons)
        {
            item.interactable = true;
        }
         
#endif
    }

    //проверка бита в байте на определённой позиции
    static public int checkbit(int value, int position)//чекаем бит
    {
        int result;
        if ((value & (1 << position)) == 0)
        {
            result = 0;
        }
        else
        {
            result = 1;
        }
        return result;
    }
    //установка бита в 1 на определённой позиции в байте
    static public int setbit(int value, int position)//устанавливаем бит в 1
    {
        return (value | (1 << position));
    }
    //установка бита в 0 на определённой позиции в байте
    static public int unsetbit(int value, int position)//устанавливаем бит в 0
    {
        return (value & ~(1 << position));
    }

    //выполняется при инициализации сцены - является инициализатором скрипта
    public void Awake()
    {
        Init();
    }
    //инициализатор джава класса
    public void Init()
    {
        TestPlugin.Init();//инициализируем джава класс
    }
    //поиск устройств
    public void SearchDevices()
    {
#if UNITY_EDITOR
        GetNameMC();
#elif UNITY_ANDROID
        TestPlugin.Show();
#endif
    }
    public void CloseConnection() 
    {
        closeConnectionButton.SetActive(false);
        findDevicesButton.SetActive(true);
        StopCoroutine(GetNameMicroC());
        nickname.text = "";
        nameMC.text = "";
        Debug.Log("here");
    }
    //вызывается при клике кнопки "список устройств"
    public void ClickOnDeviceList()
    {
        CurrentPanelSetActive(deviceListPanel);
        //SettingSpriteBackgroundsButtons(1);
    }
    //вызывается при клике кнопки "настройки"
    public void ClickOnSettings()
    {
        CurrentPanelSetActive(settingsPanel);
        //SettingSpriteBackgroundsButtons(2);
    }

    //вызывается при клике кнопки "список не зарегистрированных устройств"
    public void ClickOnRegister()
    {

        CurrentPanelSetActive(registerPanel);
        //SettingSpriteBackgroundsButtons(0);
        sendRegButton.interactable = false;

        foreach (var item in uIManager.platesInScrollViewOnRegister)
        {
            Destroy(item);
        }
        uIManager.platesInScrollViewOnRegister.Clear();
    }
    //вызывается при клике кнопки "вебвью"
    public void ClickOnWebView()
    {
        CurrentPanelSetActive(webPanel);
        //SettingSpriteBackgroundsButtons(3);
    }
    public void ClickOnContainerList()
    {
        uIManager.OpenContainerList();
        //if (uIManager.Logged == true) 
        //{
        CurrentPanelSetActive(webPanel);
       // }
    }
    public void ClickOnExitAccount()
    {
        if (uIManager.Logged == true)
        {
            ExitAlert.SetActive(true);
        }
    }
    public void CurrentPanelSetActive(GameObject panel)
    {
        if(currentPanel !=null)
        {
            currentPanel.SetActive(false);
        }
        currentPanel = panel;
        currentPanel.SetActive(true);
    }
    public void AlertClose(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
    public void AlertCloseDelete(GameObject gameObject)
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
    public void SettingSpriteBackgroundsButtons(int index)
    {
        for (int i = 0; i < 4; i++)
        {
            imagesB[i].sprite = iconsNS[i];
        }
        imagesB[index].sprite = iconsS[index];
    }

}
