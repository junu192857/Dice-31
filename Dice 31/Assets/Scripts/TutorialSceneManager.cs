using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialSceneManager : MonoBehaviour
{
    private GameObject diceObject;
    [SerializeField] private Image[] diceFaceImages;
    [SerializeField] private Text diceNameText;
    [SerializeField] private Text diceSummaryText;
    [SerializeField] private Text[] diceFaceDescriptionTexts;
    [SerializeField] private string mainSceneName;
    [SerializeField] private string diceDataPath;
    [SerializeField] private GameObject selectButtons;

    private DiceInfoData diceInfoData;

    public void Start()
    {
        diceInfoData = JsonUtility.FromJson<DiceInfoData>(Resources.Load<TextAsset>(diceDataPath).text);
        var error = diceInfoData.GetError();
        Debug.Assert(error.Length == 0, error);
        Debug.Assert(selectButtons.transform.childCount == diceInfoData.data.Length);
        for (var i = 0; i < selectButtons.transform.childCount; i++)
        {
            var button = selectButtons.transform.GetChild(i).GetComponent<Button>();
            button.GetComponent<Image>().sprite = Resources.Load<Sprite>(diceInfoData.data[i].image);
        }
        HandleSelectDiceClick(0);
    }

    public void HandleSelectDiceClick(int i)
    {
        Debug.Assert(i < diceInfoData.data.Length);
        if (i >= diceInfoData.data.Length)
            return;
    }

    public void HandleBackClick()
    {
        SceneManager.LoadScene(mainSceneName);
    }
}

[Serializable]
class DiceInfoData
{
    public DiceInfo[] data;

    public string GetError()
    {
        if (data == null)
            return "data is null";
        if (data.Length != 11)
            return "data length is not 11";
        for (var i = 0; i < data.Length; i++)
        {
            var error = data[i].GetError();
            if (error.Length > 0)
                return "data[" + i + "]: " + error;
        }
        return "";
    }
}

[Serializable]
class DiceInfo
{
    public string prefab = "";
    public string image = "";
    public string name = "";
    public string summary = "";
    public FaceInfo[] faces;

    public string GetError()
    {
        if (prefab.Length == 0)
            return "prefab is empty";
        if (image.Length == 0)
            return "image is empty";
        if (name.Length == 0)
            return "name is empty";
        if (summary.Length == 0)
            return "summary is empty";
        if (faces == null)
            return "faces is null";
        if (faces.Length != 6)
            return "faces length is not 6";
        if (faces[0].description.Length == 0)
            return "faces[0].description is empty";
        foreach (var faceInfo in faces)
        {
            if (faceInfo.image.Length == 0)
                return "faceInfo.image is empty";
        }
        return "";
    }
}

[Serializable]
class FaceInfo
{
    public string image = "";
    public string description = "";
}