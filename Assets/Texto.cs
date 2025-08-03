using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class EnhancedText : MonoBehaviour
{
    private string content = "hola soy Lucas";
    public TMP_Text outputText;
    public TMP_TextInfo textInfo;
    private readonly int velocity = 50;

    private void Start()
    {
        outputText = GetComponent<TMP_Text>();
        outputText.richText = true;
        textInfo = outputText.textInfo;
    }
}