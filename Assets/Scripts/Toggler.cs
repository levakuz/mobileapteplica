using UnityEngine;
using UnityEngine.UI;
public class Toggler : MonoBehaviour
{
    public bool value;
    public GameObject Canvas;
    public GameObject NewGameManager;
    public GameObject DeviceAlert;
    private GameObject newParent;
    [SerializeField] private Animator animator;
    private static readonly int Value = Animator.StringToHash(name: "Value");
    private void Awake()
    {
        if (this.animator == null) this.animator = GetComponent<Animator>();

        this.animator.SetBool(id: Value, this.value);
        
    }

    public void isClick()
    {

        GameObject go = Instantiate(DeviceAlert, new Vector2(-30, -60), Quaternion.identity) as GameObject;
        go.transform.SetParent (this.transform, false);
        //GameObject enemy = GameObject.Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity, GameObject.FindGameObjectWithTag("Canvas").transform);
        go.SetActive(true);

    }

    public void ConfirmAlert(GameObject go)
    {         
        var hah = go.GetComponentInParent<Toggler>();
        hah.isChangedToggler();
        go.SetActive(false);
        
    
    }

    public void isChangedToggler()
    {
        Togler();
    }

    public void Togler()
    {
        this.value = !this.value;
        this.animator.SetBool(id: Value, this.value);
    }

}
