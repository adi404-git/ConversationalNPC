using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Text;


public class NPC_ioprompter : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text outputText;
    //Handling Input and output 
    private void OnEnable()
    {
        if(inputField!=null) inputField.onSubmit.AddListener(OnPlayerSubmit);
    }
    private void OnDisable()
    {
        if(inputField!=null) inputField.onSubmit.RemoveListener(OnPlayerSubmit);
    }
    private void OnPlayerSubmit(string input)
    {
        // Process the player's input and generate a response
        if(string.IsNullOrWhiteSpace(input))
        {
            outputText.text = "Whatt?! What did you say?";
            return;
        }
        SendInput(input);
    }
    // setting up ollama : 
    [SerializeField] private string ollamaUrl = "http://localhost:11434";
    [SerializeField] private string modelName = "qwen3.5:4b";
    

    // required data classes for talking with ollama
    [System.Serializable] class ChatMessage
    {
        public string role;
        public string content;
    }
    [System.Serializable] class ChatRequestBody
    {
        public string model;
        public List<ChatMessage> messages;
        public bool stream;

    }
    [System.Serializable] class ChatResponseMessage
    {
        public string role;
        public string content;
        public string thinking ; // just to take the reasoning as well maybe i will use it later ? interesting regardless
    }

    [System.Serializable] class ChatResponse
    {
        public ChatResponseMessage message;
        public bool done;
    }
    
    public void SendInput(string input)
    {
        StartCoroutine(ReplyRoutine(input));
    }
    private IEnumerator ReplyRoutine(string input)
    {
        var messages = new List<ChatMessage>();
        messages.Add(
            new ChatMessage
            {
                role = "user",
                content= inputField.text
            }
        );    
        
        var body = new ChatRequestBody
        {
            model = modelName,
            messages = messages,
            stream = false, // sends the reply back at once  
        };

        string json = JsonUtility.ToJson(body);
        byte[] bodyBytes = Encoding.UTF8.GetBytes(json);
        // networking part now
        string url = ollamaUrl + "/api/chat";
        //Debug.Log("Routine Starts.");
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type","application/json");
            //Debug.Log("Request sent.");
            yield return request.SendWebRequest();
            //Debug.Log("Request Recieved");
            string responseJson = request.downloadHandler.text;
            ChatResponse responseData = JsonUtility.FromJson<ChatResponse>(responseJson);
            //if(responseData==null){Debug.Log("Invalid Ollama response"+responseJson);yield break;}
            string reply = responseData.message.content ; 
            outputText.text = reply;

        }
        

    }

}
