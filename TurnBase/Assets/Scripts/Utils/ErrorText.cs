using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorText : MonoBehaviour
{
    [SerializeField] private TMP_Text errorText;

    public void ShowErrorText(string contents)
    {
        StartCoroutine(ShowErrorTextAsync(contents));
    }

    private IEnumerator ShowErrorTextAsync(string contents)
    {
        errorText.gameObject.SetActive(true);

        errorText.text = contents;
    
        yield return new WaitForSeconds(3);

        errorText.gameObject.SetActive(false);
    }
}
