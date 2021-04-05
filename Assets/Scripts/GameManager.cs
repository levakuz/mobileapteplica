using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static float convert(float value, float From1, float From2, float To1, float To2)
    {
        return (value - From1) / (From2 - From1) * (To2 - To1) + To1;
    }

    public class TestPlugin//класс для работы с джава плагином
    {
        private const string NAME_CLASS = "com.example.bluetoothplugin.BluetoothPlugin";
        private static AndroidJavaClass jC;
        public static void Init()//инициализация
        {
            jC = new AndroidJavaClass(NAME_CLASS);
            jC.CallStatic("Init");
        }
        public static void Show()//показываем поиск устройств
        {

            if (jC != null)
            {
                jC.CallStatic("searchDevices");
            }
        }
        public static void ShowDevaces()//показываем все девайсы
        {
            jC.CallStatic("showListDevices");
        }
        public static byte[] GetMessage(int len)//поулчаем месседж(для будущего затычка)
        {
            return jC.CallStatic<byte[]>("getMessage",len);
        }
        public static int GetLenght()//получаем длину массива
        {
            return jC.CallStatic<int>("getLenght");
        }
        public static void ShowDeviceName(Text log)//получаем имя устройства
        {
            log.text = jC.CallStatic<string>("getNameDevice", 0);
        }
        static public byte checkbit(byte value, int position)//чекаем бит
        {
            byte result;
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

        static public byte setbit(byte value, int position)//устанавливаем бит в 1
        {
            return (byte)(value | (1 << position));
        }

        static public byte unsetbit(byte value, int position)//устанавливаем бит в 0
        {
            return (byte)(value & ~(1 << position));
        }
        public static void SetMessage(byte[] m)//отправляем смску 
        {
            jC.CallStatic("setMessage", m);
        }
        public static void ShowLog(string s)//отправляем смску 
        {
            jC.CallStatic("showToastMessage", s);
        }
        public static void ShowWebView(string url)//отправляем смску 
        {

            jC.CallStatic("ShowWebView", url);
        }
        public static void ClearStream()//отправляем смску 
        {

            jC.CallStatic("clearStream");
        }

        public static void Recconnect()//переподключение
        {
            jC.CallStatic("Recconnect");
        }
        public static void CloseConnetion()
        {
            jC.CallStatic("closeConnection");
        }
        internal static void GetStatus(Image image, Sprite red, Sprite green)//статус устройства
        {
            if (jC == null) return;
            try
            {
                Debug.Log("void GetStatus unity START");
                if (jC.CallStatic<string>("GetStatus") == "Подключение прошло успешно!")
                {
                    Debug.Log("void GetStatus unity GREEN STATUS");
                    image.sprite = green;
                    //image.color = Color.green;
                    //string mac = jC.CallStatic<string>("GetMac");
                    //PlayerPrefs.SetString("Mac", mac);
                    //PlayerPrefs.Save();
                }
                else
                {
                    Debug.Log("void GetStatus unity RED STATUS");
                    image.sprite = red;
                    //image.color = Color.red;
                }
                Debug.Log("void GetStatus unity END");
            }
            catch (Exception e)
            {

                Debug.LogError(e.Message);
                Debug.LogError("void GetStatus unity HDE TO");
            }
            
        }
    }
}
