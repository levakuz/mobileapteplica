using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCSimulator : MonoBehaviour
{
    public static List<BasicDevice> BasicDevices;
    public static List<BasicDevice> BasicDevicesRegister;
    [System.Serializable]
    public class BasicDevice
    {
        public int id;
        public TypeBasicDevice typeBasicDevice;
    }
    [System.Serializable]
    public class Sensor: BasicDevice
    {
        public byte type;
        public float data; 
    }
    [System.Serializable]
    public class Device: BasicDevice
    {
        public byte type;
        public short data;
    }
    [System.Serializable]
    public enum TypeBasicDevice
    {
        sensor,
        device
    }
    void Start()
    {
        BasicDevices = new List<BasicDevice>();
        BasicDevicesRegister = new List<BasicDevice>();
        byte[] arr = BitConverter.GetBytes(100);
        string s = "array( ";
        for (int i = 0; i < arr.Length; i++)
        {
            s += arr[i] + " ";
        }
        print(s);
        for (int i = 0; i < 5; i++)
        {
            Sensor sensor = new Sensor();
            sensor.typeBasicDevice = TypeBasicDevice.sensor;
            sensor.id = i;
            sensor.data = UnityEngine.Random.Range(0f, 100f);
            sensor.type = (byte)UnityEngine.Random.Range(0x02, 0x0A);
            BasicDevices.Add(sensor);
        }
        for (int i = 5; i < 10; i++)
        {
            Device device = new Device();
            device.typeBasicDevice = TypeBasicDevice.device;
            device.id = i;
            
            device.type = (byte)UnityEngine.Random.Range(0x02, 0x05);
            if(device.type == 0x02 | device.type == 0x05)
                device.data = (short)UnityEngine.Random.Range(0, 4096);
            else
                device.data = (short)UnityEngine.Random.Range(0, 2);
            BasicDevices.Add(device);
        }
    }
    public static byte[] message;

    public static byte[] GetMessage103()
    {
        message = new byte[]
        {
            103
        };
        return message;
    }
    public static byte[] GetMessage106()
    {
        List<byte> bytes = new List<byte>();
        bytes.Add(106);
        bytes.Add(5);
        for (int i = 0; i < 5; i++)
        {
            byte[] bytesFromAdress = BitConverter.GetBytes(BasicDevices[i].id);
            for (int j = 0; j < bytesFromAdress.Length; j++)
            {
                bytes.Add(bytesFromAdress[j]);
            }
        }
        return bytes.ToArray();
    }
    public static byte[] GetMessage108()
    {
        List<byte> bytes = new List<byte>();
        bytes.Add(108);
        bytes.Add(5);
        for (int i = 5; i < 10; i++)
        {
            byte[] bytesFromAdress = BitConverter.GetBytes(BasicDevices[i].id);
            for (int j = 0; j < bytesFromAdress.Length; j++)
            {
                bytes.Add(bytesFromAdress[j]);
            }
        }
        return bytes.ToArray();
    }
    public static void SetMessage109(byte[] arr)
    {

        for (int i = 1; i < arr.Length; i+=4)
        {

            int id = BitConverter.ToInt32(arr, i);
            BasicDevice bD = BasicDevices.Find(item => item.id == id);
            if(bD != null)
                BasicDevicesRegister.Add(bD);
        }
    }
    public static byte[] GetMessage109(int count)
    {
        List<byte> bytes = new List<byte>();
        bytes.Add(109);
        
        for (int i = 0; i < count; i++)
        {
            bytes.Add(0);
        }
        return bytes.ToArray();
    }

    public static byte[] GetMessage110(int count)
    {
        //message = new byte[14];
        //message[0] = 110;

        //message[1] = 0x01;
        //message[2] = 0x02;
        //byte[] arr = BitConverter.GetBytes(89f);
        //message[3] = arr[0];
        //message[4] = arr[1];
        //message[5] = arr[2];
        //message[6] = arr[3];

        //message[7] = 0x02;
        //message[8] = 0x02;
        //arr = BitConverter.GetBytes((short)45);
        //message[9] = arr[0];
        //message[10] = arr[1];


        //message[11] = 0x02;
        //message[12] = 0x03;
        //message[13] = 1;


        List<byte> bytes = new List<byte>();
        bytes.Add(110);

        for (int i = 0; i < BasicDevicesRegister.Count; i++)
        {
            byte[] data;
            switch (BasicDevicesRegister[i].typeBasicDevice)
            {
                case TypeBasicDevice.sensor:
                    Sensor sensor = (Sensor)BasicDevicesRegister[i];
                    bytes.Add(1);
                    bytes.Add(sensor.type);
                    data = BitConverter.GetBytes(sensor.data);
                    for (int j = 0; j < data.Length; j++)
                    {
                        bytes.Add(data[j]);
                    }
                    break;
                case TypeBasicDevice.device:
                    Device device = (Device)BasicDevicesRegister[i];
                    bytes.Add(2);
                    bytes.Add(device.type);
                    if (device.type == 0x02 | device.type == 0x05)
                    {
                        data = BitConverter.GetBytes(device.data);
                        for (int j = 0; j < data.Length; j++)
                        {
                            bytes.Add(data[j]);
                        }
                    }
                    else
                    {
                        bytes.Add((byte)device.data);
                    }
                    break;
            }
            //bytes.Add(0);
        }
        return bytes.ToArray();
    }
}
