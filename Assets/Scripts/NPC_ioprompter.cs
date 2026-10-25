using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.IO;
using UnityEngine.Assemblies;

public class NPC_ioprompter : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputText;
    [SerializeField] private TMP_Text outputText;
    //Handling Input and output 
    private void OnEnable()
    {
        inputText.onSubmit.AddListener(OnPlayerSubmit);
    }
    private void OnDisable()
    {
        inputText.onSubmit.RemoveListener(OnPlayerSubmit);
    }

    private void OnPlayerSubmit(string input)
    {
        // Process the player's input and generate a response
        if(string.IsNullOrWhiteSpace(input))
        {
            outputText.text = "Whatt?! What did you say?";
            return;
        }
        SendPlayerInput(input);
    }
    
    
    //ollama side
    
}