using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatePrefabOnOnAutentification : MonoBehaviour
{
    public Text adress;
    public string fromContainer;
    public Text typeData;
    public Text data;
    public Text type;
    public Type typeItem;
    public int adressValue;
    public enum Type
    {
        device,
        sensor
    }
    public string[] ConvertTypeDataSensorByteToString(byte typeData)
    {
        string[] value = { "DefaultType", "%" };
        switch (typeData)
        {
            case 0x02:
                value[0] = "Заряд аккумулятора";
                value[1] = "%";
                break;
            case 0x03:
                value[0] = "Влажность воздуха";
                value[1] = "%";
                break;
            case 0x04:
                value[0] = "Температура воздуха";
                value[1] = "°C";
                break;
            case 0x05:
                value[0] = "Температура воды";
                value[1] = "°C";
                break;
            case 0x06:
                value[0] = "Уровень освещённости";
                value[1] = "Люкс";
                break;
            case 0x07:
                value[0] = "Мощность ламп";
                value[1] = "Ватт";
                break;
            case 0x08:
                value[0] = "Мощность насосов";
                value[1] = "Ватт";
                break;
            case 0x09:
                value[0] = "Показатель pH";
                value[1] = "";
                break;
            case 0x0B:
                value[0] = "Показатель eCO2";
                value[1] = "ppm";
                break;
            case 0x0C:
                value[0] = "Показатель nTVOC";
                value[1] = "ppb";
                break;
            case 0x0A:
                value[0] = "Показатель ЕС";
                value[1] = "mS";
                break;
        }
        return value;
    }
    public string ConvertTypeDataDeviceByteToString(byte typeData)
    {
        string value = "DefaultType";
        switch (typeData)
        {
            case 0x02:
                value = "Вентилятор (ШИМ)";
                break;
            case 0x03:
                value = "Насосная система (Цифровой сигнал)";
                break;
            case 0x04:
                value = "Фитолампа(Цифровой сигнал)";
                break;
            case 0x05:
                value = "Фитолампа(ШИМ)";
                break;
        }
        return value;
    }
    public void Init(string adress, string type, string fromContainer, string nameItem)
    {
        this.adress.text = "ID:"+adress;
        this.fromContainer = fromContainer;
        adressValue = Convert.ToInt32(adress);

    }

    public void UpdateData(byte typeData, float data)
    {
        this.typeData.text = "Параметр:" + ConvertTypeDataSensorByteToString(typeData)[0];
        this.data.text = "Значение:" + data.ToString() + ConvertTypeDataSensorByteToString(typeData)[1];
        this.type.text = "Тип: Сенсор";
    }
    public void UpdateData(byte typeData, short data)
    {
        this.typeData.text = "Параметр:" + ConvertTypeDataDeviceByteToString(typeData);
        this.data.text = "Значение:" + data.ToString();
        this.type.text = "Тип: Устройство";
    }
}
