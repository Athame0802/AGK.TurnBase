using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFields : MonoBehaviour
{
    public static InputFields Instance;
    public TMP_InputField DigitInput => _digitInput;
    [SerializeField] private TMP_InputField _digitInput;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
