using System.Collections;
using TMPro;
using UnityEngine;

public class EnhancedText : MonoBehaviour
{
    private string content = "Hola <b> Erito </b> lo estoy haciendo <color=#FF0000>todo de nuevo</color>" +
        "y para la talla de mostrarse uno por uno hice uso de un <color=#0000FF>Enumerador</color> con un WaitForSeconds() en el yield." +
        "<color=#00FF00>Ahora se ve mas limpia la impresion</color>"; 
    public string Name;
    public TMP_Text outputText;
    public TMP_TextInfo textInfo;
    private readonly float velocity = 0.05f;

    private void Start()
    {
        outputText = GetComponent<TMP_Text>();
        outputText.richText = true;
        outputText.text = content;
        outputText.ForceMeshUpdate();
        textInfo = outputText.textInfo;
        HideChars();
        StartCoroutine(Display());
    }

    public void HideChars()
    {
        // pone invisibles todos los caracteres
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int mesh = textInfo.characterInfo[i].materialReferenceIndex;
            Color32[] vertexColors = textInfo.meshInfo[mesh].colors32;
            for (int j = 0; j < 4; j++)
            {
                Color32 c = vertexColors[vertexIndex + j];
                c.a = 0;
                vertexColors[vertexIndex + j] = c;
            }
        }
        outputText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    public IEnumerator Display()
    {
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int mesh = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Color32[] vertexColors = textInfo.meshInfo[mesh].colors32;

            for (int j = 0; j < 4; j++)
            {
                Color32 c = vertexColors[vertexIndex + j];
                c.a = 255;
                vertexColors[vertexIndex + j] = c;
            }

            outputText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            yield return new WaitForSeconds(velocity);
        }
    }

}