using UnityEngine;

public class Toggler : MonoBehaviour
{
    public bool value;
    [SerializeField] private Animator animator;

    private static readonly int Value = Animator.StringToHash(name: "Value");
    private void Awake()
    {
        if (this.animator == null) this.animator = GetComponent<Animator>();

        this.animator.SetBool(id: Value, this.value);
    }

    public void Togler()
    {
        this.value = !this.value;

        this.animator.SetBool(id: Value, this.value);
    }
}
