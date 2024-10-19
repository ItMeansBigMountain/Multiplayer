using TMPro;
using UnityEngine;

public class ForceKeyboard : MonoBehaviour
{
    public TMP_InputField inputField;

    public void OnSelectInputField()
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }
}
