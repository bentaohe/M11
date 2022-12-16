using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Net;
using System.IO;
using System.Text;
using AddressFamily = System.Net.Sockets.AddressFamily;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using System.Net.Sockets;

public class IpAddresses : MonoBehaviour
{
    public string localIp = "?.?.?.?";
    public string globalIp = "?.?.?.?";
    public string chosenIp = "";

    public GameObject scrollContent;
    public GameObject btnTemplate;
    public Button iPButtionPrefab;
    public event Action<String> IpChosen;
    private void Start()
    {
        btnTemplate.gameObject.SetActive(false);
        localIp = GetLocalIp();
        globalIp = GetGlobalIp();


        AddButton(localIp, "local");
        AddButton(globalIp, "global");
        AddButton("127.0.0.1", "localhost");
        AddButton("0,0,0,0", "all");
        

    }
    private void AddButton(string ip, string label)
    {
        GameObject newButton = Instantiate(btnTemplate);
        newButton.transform.SetParent(scrollContent.transform, false);
        newButton.gameObject.SetActive(true);
        Button b = newButton.transform.Find("IpButton").GetComponent<Button>();
        newButton.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = label;
        b.transform.Find("ButtonText").GetComponent<TextMeshProUGUI>().text = ip;
        b.onClick.AddListener(delegate { OnIpButtonClicked(ip); });
    }
    private void OnIpButtonClicked(string ip)
    {
        IpChosen.Invoke(ip);    
    }

    public string GetGlobalIp()
    {
        string toReturn = "?.?.?.?";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.ipify.org");
        request.Method = "GET";
        request.Timeout = 1000; //time in ms
        try
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
               
            } //if
            else
            {
                Debug.LogError("Timed out? " + response.StatusDescription);
               
            } //else
        } //try
        catch (WebException ex)
        {
            Debug.Log("Likely no internet connection: " + ex.Message);
           
        }
        return toReturn;//catch
        //myAddressGlobal=new System.Net.WebClient().DownloadString("https://api.ipify.org"); //single-line solution for the global IP, but long time-out when there is no internet connection, so I prefer to do the method above where I can set a short time-out time
    } //Start
    public string GetLocalIp()
    {
        string toReturn = "?.?.?.?";
        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in hostEntry.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                toReturn = ip.ToString();
                break;
            }
        }
    }
}
   
   

