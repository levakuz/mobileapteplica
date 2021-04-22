using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientLogic : MonoBehaviour
{
    //запросы и ответы от сервера
    public AuthorizationRequest authorizationRequest;
    public AuthorizationResponse authorizationResponse;

    public GetContainersRequest getContainersRequest;
    public GetContainersResponse getContainersResponse;

    public GetContainerRequest getContainerRequest;
    public GetContainerResponse getContainerResponse;
    //сокет
    public WebSocket webSocket;
    //месседж
    public string returnMSG;
    //юай менеджер и джисоны 
    public UIManager uiManager;
    public ConfigJson currentConfigJson;
    public SensorJson currentSensorJson;
    public DeviceJson currentDeviceJson;
    //кнопка 
    public GameObject currentConfigJsonButton;

    public List<GameObject> sendPlatesOnRegistartion;

    public List<ItemPlate> itemPlatesToRegister;

    public List<ConfigJson> downloadedJsons;

    public CurrentClientServerWindow currentClientServerWindow;
    public object currentSensorOrDevice;
    public static string logMessage;
    public enum CurrentClientServerWindow
    {
        authorization,
        containers,
        container,
        sensor,
        device
    }
    //запрос авторизации
    public void RequestAuthorization(string name,string password)
    {
        authorizationRequest.head = "Q-Login";
        authorizationRequest.body = new AuthorizationRequest.Body(name, password);
        string msg = JsonUtility.ToJson(authorizationRequest);
        var taskWebConnect = Task.Run(() => webSocket.Reqest(msg));
        taskWebConnect.Wait();
        returnMSG = taskWebConnect.Result;
        authorizationResponse = JsonUtility.FromJson<AuthorizationResponse>(returnMSG);
        if(authorizationResponse.body.success)
        {
            uiManager.AuthorizationSucces();
            //RequestGetContainers();
        }
    }

    //запрос на получение контейнеров
    public void RequestGetContainers()
    {
        getContainersRequest.head = "Q-Containers";
        getContainersRequest.body = new GetContainersRequest.Body(authorizationResponse.body.session);
        string msg = JsonUtility.ToJson(getContainersRequest);
        Debug.Log(msg);
        var taskWebConnect = Task.Run(() => webSocket.Reqest(msg));
        taskWebConnect.Wait();
        returnMSG = taskWebConnect.Result;
        getContainersResponse = JsonUtility.FromJson<GetContainersResponse>(returnMSG);
        uiManager.ClearItemsInContainersScrollView();
        foreach (var item in getContainersResponse.body)
        {
            Debug.Log(item);
            uiManager.AddItemInContainersScrollView(item);
        }
    }
    //запрос на получение контейнера (с детальной инфой)
    public void RequestGetContainer(string name,GameObject button)
    {
        getContainerRequest.head = "Q-Container";
        getContainerRequest.body = new GetContainerRequest.Body(authorizationResponse.body.session, name);
        string msg = JsonUtility.ToJson(getContainerRequest);
        Debug.Log(msg);
        var taskWebConnect = Task.Run(() => webSocket.Reqest(msg));
        taskWebConnect.Wait();
        returnMSG = taskWebConnect.Result;
        Debug.Log(returnMSG);

        getContainerResponse = JsonUtility.FromJson<GetContainerResponse>(returnMSG);
        Debug.Log(getContainerResponse.body);

        currentConfigJson = getContainerResponse.body;
        Debug.Log("currentConfigJson.sensors");
        Debug.Log(currentConfigJson);
        currentConfigJsonButton = button;
        Debug.Log(downloadedJsons);
        Debug.Log(currentConfigJson.sensors[0]);
        if (downloadedJsons == null)
        {
            downloadedJsons = new List<ConfigJson>();
            Debug.Log(downloadedJsons);
        }
        ConfigJson findConfigJson = downloadedJsons.Find(item => item.name.container == currentConfigJson.name.container);
        Debug.Log(findConfigJson);
        Debug.Log(currentConfigJson);
        if (findConfigJson == null)
        {
            foreach (var item in currentConfigJson.sensors)
            {
                item.sensorMCAuthorizations = new List<SensorMC>();
                item.sensorMCRegistrations = new List<SensorMC>();
            }
            foreach (var item in currentConfigJson.devices)
            {
                item.deviceMCAuthorizations = new List<DeviceMC>();
                item.deviceMCRegistrations = new List<DeviceMC>();
            }
            downloadedJsons.Add(currentConfigJson);
            uiManager.OnClickOpenContainerItemsPanel(currentConfigJson);
        }
        else
        {
            uiManager.OnClickOpenContainerItemsPanel(findConfigJson);
        }
    }
    //классы запросов и ответов, всё просто, как обжекты в js конвертируются в/из джисона
    [System.Serializable]
    public class AuthorizationRequest
    {
        public string head;
        public Body body;
        [System.Serializable]
        public class Body
        {
            public Body(string name, string password)
            {
                this.name = name;
                this.password = password;
            }
            public string name;
            public string password;
        }
    }
    [System.Serializable]
    public class AuthorizationResponse
    {
        public string head;
        public Body body;
        [System.Serializable]
        public class Body
        {
            public bool success;
            public string session;
        }
    }
    [System.Serializable]
    public class GetContainersRequest
    {
        public string head;
        public Body body;
        [System.Serializable]
        public class Body
        {
            public Body(string session)
            {
                this.session = session;
            }
            public string session;
        }
    }
    [System.Serializable]
    public class GetContainersResponse
    {
        public string head;
        public string[] body;
    }
    [System.Serializable]
    public class GetContainerRequest
    {
        public string head;
        public Body body;
        [System.Serializable]
        public class Body
        {
            public Body(string session,string container)
            {
                this.session = session;
                this.container = container;
            }
            public string session;
            public string container;
        }
    }
    [System.Serializable]
    public class GetContainerResponse
    {
        public string head;
        public ConfigJson body;
    }
    [System.Serializable]
    public class Wifi
    {
        public string name;
        public string pass;
        public string ip;
        //получение байтового массива из данных объекта данного класса
        public List<byte> GetByteArray(List<byte> bytes)
        {
            logMessage += "wifi start 114 " + "\n";
            bytes.Add(114);//отправка wifi
            logMessage += "code 115(name)";
            bytes.Add(115);//отправка wifi
            logMessage += "name(length) = " + name.Length + " ";
            bytes.Add((byte)name.Length);
            logMessage += "name(value) = (";
            for (int i = 0; i < name.Length; i++)
            {
                logMessage += (byte)name[i] + (i < name.Length - 1 ? "," : ")\n"); ;
                bytes.Add((byte)name[i]);
            }

            logMessage += "code 116(pass) ";
            logMessage += "pass(length) = " + pass.Length + " ";
            logMessage += "pass(value) = (";
            bytes.Add(116);//отправка wifi
            bytes.Add((byte)pass.Length);
            for (int i = 0; i < pass.Length; i++)
            {
                logMessage += (byte)pass[i] + (i < pass.Length - 1 ? "," : ")\n"); ;
                bytes.Add((byte)pass[i]);
            }
            bytes.Add(117);//отправка wifi
            bytes.Add((byte)ip.Length);
            logMessage += "code 117(ip) ";
            logMessage += "ip(length) = " + ip.Length + " ";
            logMessage += "ip(value) = (";
            for (int i = 0; i < ip.Length; i++)
            {
                logMessage += (byte)ip[i] + (i < ip.Length - 1 ? "," : ")\n"); ;
                bytes.Add((byte)ip[i]);
            }
            return bytes;
        }
    }
    [System.Serializable]
    public class ConfigJson
    {
        public Name name;
        public int address;
        public int channel;

        public List<ItemMC> itemMCs;
        public SensorJson[] sensors;
        public DeviceJson[] devices;
        public Wifi wifi;
        public bool isSended;
        List<byte> bytes = new List<byte>();
        [System.Serializable]
        public class Name
        {
            public string session;
            public string container;
        }
        //чистка отправки и т.д. текущего конфига
        public void Clear()
        {
            isSended = false;
            itemMCs.Clear();
            foreach (var item in sensors)
            {
                item.Clear();
            }
        }
        //получение байтового массива из данных объекта данного класса
        public List<byte> GetByteArray()
        {
            bytes = new List<byte>();
            logMessage += "\nstart config\ncode 103" + "\n";
            bytes.Add((byte)103);//отправка конфига
            logMessage += "code 104(adress)\n";
            logMessage += "address(value) = (\n";
            byte[] bA = BitConverter.GetBytes((short)address);
            for (int i = 0; i < bA.Length; i++)
            {
                logMessage += bA[i] + (i < bA.Length-1 ? "," : "\n);\n");
            }
            bytes.Add((byte)104);//отправка адреса
            bytes = IntConvertInBytes(address, bytes);
            logMessage += " code 105(channel) ";
            logMessage += "channel(value) = (";
            bA = BitConverter.GetBytes((short)channel);
            for (int i = 0; i < bA.Length; i++)
            {
                logMessage += bA[i] + (i < bA.Length - 1 ? "," : ")\n");
            }
            bytes.Add((byte)105);//отправка канала
            bytes = IntConvertInBytes(channel, bytes);



            foreach (var item in sensors)
            {
                bytes = item.GetByteArray(bytes);
            }
            foreach (var item in devices)
            {
                bytes = item.GetByteArray(bytes);
            }
            bytes = wifi.GetByteArray(bytes);
            Debug.Log("logMessage = " + logMessage);
            return bytes;
        }
    }
    //конвертация инта в байтовый массив
    public static List<byte> IntConvertInBytes(int obj, List<byte> bytes)
    {
        byte[] array = BitConverter.GetBytes((short)obj);
        for (int i = 0; i < array.Length; i++)
        {
            bytes.Add(array[i]);
        }
        return bytes;
    }
    [System.Serializable]
    public class SensorJson
    {
        public int count;
        public int currentCount;
        public int period;
        public bool isSended;
        public string itemName;
        //снова чистка отправки, уже джисона сенсора
        public void Clear()
        {
            isSended = false;
            currentCount = 0;
            sensorMCRegistrations.Clear();
            sensorMCAuthorizations.Clear();
        }
        //сравнение отправленых элементов, если равно заданому в джисоне контейнера для сенсоров, то считаем его отправленным
        public void CheckCountSends()
        {
            if(sensorMCAuthorizations.Count == count)
            {
                isSended = true;
            }
            else
            {
                isSended = false;
            }
        }
        public List<SensorMC> sensorMCRegistrations;
        public List<SensorMC> sensorMCAuthorizations;
        //получение байтового массива из данных объекта данного класса
        public List<byte> GetByteArray(List<byte> bytes)
        {
            bytes.Add((byte)106);//отправка периода
            bytes = IntConvertInBytes(period, bytes);
            logMessage += "start sensor ";
            logMessage += "code 106(period) ";
            logMessage += "period(value) = (";
            byte[] bA = BitConverter.GetBytes((short)period);
            for (int i = 0; i < bA.Length; i++)
            {
                logMessage += bA[i] + (i < bA.Length - 1 ? "," : ")\n");
            }
            foreach (var item in structure)
            {
                bytes = item.GetByteArray(bytes);
            }
            return bytes;
        }

        public SensorStructureJson[] structure;
    }
    [System.Serializable]
    public class ItemMC
    {
        public Type type;
        public string itemName;
        public int adress;
        public enum Type
        {
            sensor,
            device
        }
    }
    [System.Serializable]
    public class SensorMC : ItemMC
    {
        
        public SensorJson sensorJson;
        //получение байтового массива из данных объекта данного класса
        public List<byte> GetListBytes(List<byte> bL)
        {
            byte[] bA = BitConverter.GetBytes(adress);
            for (int i = 0; i < bA.Length; i++)
            {
                bL.Add(bA[i]);
            }
            return bL;
        }
        public SensorMC(int adress, SensorJson sensorJson, string itemName)
        {
            this.adress = adress;
            this.sensorJson = sensorJson;
            this.itemName = itemName;
            type = Type.sensor;
        }
    }
    [System.Serializable]
    public class SensorStructureJson
    {
        public enum Type_sensor : byte
        {
            Analog_signal = 0,
            Discrete_signal,
            Battery_charge,
            Air_humidity,
            Air_temperature,
            Water_temperature,
            Illumination_level,
            Lamp_power,
            Pump_power,
            Indicator_pH,
            Indicator_EC,
            Indicator_eCO2,
            Indicator_nYVOC
        };
        public string type;
        public Type_sensor type_e;
        public int count;
        //конвертация стринги из джисона в тип перечисления
        public void ConvertStringToEnum()
        {
            switch(type)
            {
                case "Analog_signal":type_e = Type_sensor.Analog_signal;
                    break;
                case "Discrete_signal":
                    type_e = Type_sensor.Discrete_signal;
                    break;
                case "Battery_charge":
                    type_e = Type_sensor.Battery_charge;
                    break;
                case "Air_humidity":
                    type_e = Type_sensor.Air_humidity;
                    break;
                case "Air_temperature":
                    type_e = Type_sensor.Air_temperature;
                    break;
                case "Water_temperature":
                    type_e = Type_sensor.Water_temperature;
                    break;
                case "Illumination_level":
                    type_e = Type_sensor.Illumination_level;
                    break;
                case "Lamp_power":
                    type_e = Type_sensor.Lamp_power;
                    break;
                case "Pump_power":
                    type_e = Type_sensor.Pump_power;
                    break;
                case "Indicator_pH":
                    type_e = Type_sensor.Indicator_pH;
                    break;
                case "Indicator_EC":
                    type_e = Type_sensor.Indicator_EC;
                    break;
                case "Indicator_eCO2":
                    type_e = Type_sensor.Indicator_eCO2;
                    break;
                case "Indicator_nYVOC":
                    type_e = Type_sensor.Indicator_nYVOC;
                    break;
            }
        }
        //получение байтового массива из данных объекта данного класса
        public List<byte> GetByteArray(List<byte> bytes)
        {
            ConvertStringToEnum();
            logMessage += "start sensor structure \n";
            logMessage += "code 107(type_e) ";
            logMessage += "type_e(value) = " + (byte)type_e + "\n";
            bytes.Add((byte)107);//отправка типа сенсора
            bytes.Add((byte)type_e);
            return bytes;
        }
    }
    [System.Serializable]
    public class DeviceJson
    {
        public int count;
        public int period;
        public int currentCount;
        public bool isSended;
        public string itemName;
        //снова чистка
        public void Clear()
        {
            isSended = false;
            currentCount = 0;
            deviceMCRegistrations.Clear();
            deviceMCAuthorizations.Clear();
        }
        //и проверка на максимальное отправленное кол-во
        public void CheckCountSends()
        {
            if (deviceMCAuthorizations.Count == count)
            {
                isSended = true;
            }
            else
            {
                isSended = false;
            }
        }
        public List<DeviceMC> deviceMCRegistrations;
        public List<DeviceMC> deviceMCAuthorizations;
        public DeviceStructureJson[] structure;
        //получение байтового массива из данных объекта данного класса
        public List<byte> GetByteArray(List<byte> bytes)
        {
            logMessage += "start device \n";
            logMessage += "code 108(period) ";
            logMessage += "period(value) = (";
            byte[] bA = BitConverter.GetBytes((short)period);
            for (int i = 0; i < bA.Length; i++)
            {
                logMessage += bA[i] + (i < bA.Length - 1 ? "," : ")\n");
            }
            bytes.Add((byte)108);//отправка девайса
            bytes = IntConvertInBytes(period, bytes);
            foreach (var item in structure)
            {
                bytes = item.GetByteArray(bytes);
            }
            return bytes;
        }
    }
    [System.Serializable]
    public class DeviceMC : ItemMC
    {
        public DeviceJson deviceJson;
        //получение байтового массива из данных объекта данного класса
        public List<byte> GetListBytes(List<byte> bL)
        {
            byte[] bA = BitConverter.GetBytes(adress);
            for (int i = 0; i < bA.Length; i++)
            {
                bL.Add(bA[i]);
            }
            return bL;
        }
        public DeviceMC(int adress, DeviceJson deviceJson, string itemName)
        {
            this.adress = adress;
            this.deviceJson = deviceJson;
            type = Type.device;
            this.itemName = itemName;
        }
    }
    [System.Serializable]
    public class DeviceStructureJson
    {
        // enum типов устройств
        public enum Type_device:byte
        {
            Signal_PWM = 0,
            Signal_digital,
            Fan_PWM,
            Pumping_system,
            Phytolamp_digital,
            Phytolamp_PWM
        };

        // enum типов времени
        public enum Period_type : byte
        {
            SEC = 0,
            MIN,
            HOUR
        };

        
        public int count;
        public int currentCount = 0;
        public List<DeviceStructureJson> deviceStructureJsonsRegistrations;
        public List<DeviceStructureJson> deviceStructureJsonsAuthorizations;
        public Type_device type_Device;
        public Period_type time_Type;
        public string type;
        public string time_type;
        public byte frequency;
        public byte period;
        public byte bias;
        //конвертация стринги в энам
        public void ConvertStringToEnumTypeDevice()
        {
            switch (type)
            {
                case "Signal_PWM":
                    type_Device = Type_device.Signal_PWM;
                    break;
                case "Signal_digital":
                    type_Device = Type_device.Signal_digital;
                    break;
                case "Fan_PWM":
                    type_Device = Type_device.Fan_PWM;
                    break;
                case "Pumping_system":
                    type_Device = Type_device.Pumping_system;
                    break;
                case "Phytolamp_digital":
                    type_Device = Type_device.Phytolamp_digital;
                    break;
                case "Phytolamp_PWM":
                    type_Device = Type_device.Phytolamp_PWM;
                    break;
            }
        }
        //тоже что и сверху
        public void ConvertStringToEnumTypePeriod()
        {
            switch (time_type)
            {
                case "sec":
                    time_Type = Period_type.SEC;
                    break;
                case "min":
                    time_Type = Period_type.MIN;
                    break;
                case "hour":
                    time_Type = Period_type.HOUR;
                    break;
            }
        }
        //получение байтового массива из данных объекта данного класса
        public List<byte> GetByteArray(List<byte> bytes)
        {
            ConvertStringToEnumTypeDevice();
            ConvertStringToEnumTypePeriod();

            logMessage += "start device struecture\n";
            logMessage += "code 109(type_Device) ";
            logMessage += "type_Device(value) = " + (byte)type_Device + "\n";
            bytes.Add((byte)109);//отправка типа устройства
            bytes.Add((byte)type_Device);
            logMessage += "code 110(time_Type) ";
            logMessage += "time_Type(value) = " + (byte)time_Type + "\n";
            bytes.Add((byte)110);//отправка типа времени
            bytes.Add((byte)time_Type);
            logMessage += "code 111(frequency) ";
            logMessage += "frequency(value) = " + frequency + "\n";
            bytes.Add((byte)111);//отправка частоты
            bytes.Add(frequency);
            logMessage += "code 112(period) ";
            logMessage += "period(value) = " + period + "\n";
            bytes.Add((byte)112);//отправка периода
            bytes.Add(period);
            logMessage += "code 113(bias) ";
            logMessage += "bias(value) = " + bias + "\n";
            bytes.Add((byte)113);//отправка bias
            bytes.Add((byte)bias);
            return bytes;
        }
    }
}