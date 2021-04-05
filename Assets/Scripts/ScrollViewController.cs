using UnityEngine;
using System.Collections;

public class ScrollViewController : MonoBehaviour
{
    public GameObject prefab;
    public Transform content;

    void respawn()
    {
        Instantiate(prefab, content);
    }
}
