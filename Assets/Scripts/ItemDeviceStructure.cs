using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ClientLogic;

public class ItemDeviceStructure : MonoBehaviour
{
    public Text typeText, countText, timeTypeText, frequencyText, periodText, biasText;

    public void Init(DeviceStructureJson deviceStructureJson)
    {
        typeText.text = "type:" + deviceStructureJson.type;
        countText.text = "count:" + deviceStructureJson.count.ToString();
        timeTypeText.text = "time_type:" + deviceStructureJson.time_type;
        frequencyText.text = "frequency:" + deviceStructureJson.frequency.ToString();
        periodText.text = "period:" + deviceStructureJson.period.ToString();
        biasText.text = "bias:" + deviceStructureJson.bias.ToString();
    }
}
