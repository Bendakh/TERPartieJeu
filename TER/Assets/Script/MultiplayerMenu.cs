using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class MultiplayerMenu : MonoBehaviour
{
    private readonly string pseudo = "_PSEUDO";

    [SerializeField]
    private TextMeshProUGUI pseudoZone;
    [SerializeField]
    private GameObject nameSetter;
    [SerializeField]
    private TMP_InputField nameInput;
    [SerializeField]
    private GameObject mainPanel;

    // Start is called before the first frame update
    void Start()
    {
        //PlayerPrefs.DeleteAll();
        if (PlayerPrefs.GetString(pseudo) == "")
        {
            DisplayNameSetterPanel();
        }
        else
        {
            pseudoZone.SetText("Pseudo: " + PlayerPrefs.GetString(pseudo));
        }
    }

    private void DisplayNameSetterPanel()
    {
        nameSetter.SetActive(true);
        for(int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).tag == "NameSetter")
                continue;

            if(this.transform.GetChild(i).GetComponent<Button>() != null)
            {
                this.transform.GetChild(i).GetComponent<Button>().interactable = false;
            }
        }
    }

    public void ValidateName()
    {
        //Taille max nom 20
        if(nameInput.text.Length <= 0 || nameInput.text.Length > 20)
        {
            Debug.Log("Error in name");

            return;
        }
        else
        {
            PlayerPrefs.SetString(pseudo, nameInput.text);
            pseudoZone.SetText("Pseudo: " + PlayerPrefs.GetString(pseudo));
        }

        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).tag == "NameSetter")
                continue;

            if (this.transform.GetChild(i).GetComponent<Button>() != null)
            {
                this.transform.GetChild(i).GetComponent<Button>().interactable = true;
            }
        }
        nameSetter.SetActive(false);
    }

    public void ReturnButton()
    {
        mainPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void ChangeNameButton()
    {
        DisplayNameSetterPanel();
    }

    public void ConnectButton()
    {
        //Abstract method to connect to server and get id and shit 
    }
}
