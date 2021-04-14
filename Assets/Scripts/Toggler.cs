using UnityEngine;
using UnityEngine.UI;

public class Toggler : MonoBehaviour
{
    public bool value;
    public GameObject DeviceAlert;
    [SerializeField] private Animator animator;
    private static readonly int Value = Animator.StringToHash(name: "Value");
    private void Awake()
    {
        if (this.animator == null) this.animator = GetComponent<Animator>();

        this.animator.SetBool(id: Value, this.value);
        
    }

    public void isClick()
    {
        DeviceAlert.SetActive(true);
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
