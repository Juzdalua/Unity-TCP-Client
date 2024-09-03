using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatCell : MonoBehaviour
{
   public ChatType ChatType { get; set; }

    public void Setup(ChatType type, Color color, string textData)
    {
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();

        ChatType = type;
        text.color = color;
        text.text = textData; 
    }
}
