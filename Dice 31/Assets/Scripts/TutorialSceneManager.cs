using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialSceneManager : MonoBehaviour
{
    [SerializeField] private Image[] diceFaceImages;
    [SerializeField] private Text diceNameText;
    [SerializeField] private Text diceSummaryText;
    [SerializeField] private Text[] diceFaceDescriptionTexts;
    [SerializeField] private string mainSceneName;
    [SerializeField] private string diceDataPath;
    [SerializeField] private GameObject diceParent;
    
    private DiceInfoData diceInfoData;

    public void Start()
    {
        diceInfoData = JsonUtility.FromJson<DiceInfoData>(Resources.Load<TextAsset>(diceDataPath).text);
        var error = diceInfoData.GetError();
        Debug.Assert(error.Length == 0, error);
        HandleSelectDiceClick(0);
    }

    public void HandleSelectDiceClick(int i)
    {
        Debug.Assert(i < diceInfoData.data.Length);
        if (i >= diceInfoData.data.Length)
            return;
        
        var diceInfo = diceInfoData.data[i];
        diceNameText.text = diceInfo.name;
        diceSummaryText.text = diceInfo.summary;
        for (var j = 0; j < diceFaceDescriptionTexts.Length; j++)
        {
            diceFaceDescriptionTexts[j].text = diceInfo.faces[j].description;
        }
        for (var j = 0; j < diceFaceImages.Length; j++)
        {
            diceFaceImages[j].sprite = Resources.Load<Sprite>(diceInfo.faces[j].image);
        }
        Destroy(diceParent.transform.GetChild(0).gameObject);
        var prefab = Resources.Load<GameObject>(diceInfo.prefab);
        Instantiate(prefab, diceParent.transform);
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
    public string name = "";
    public string summary = "";
    public FaceInfo[] faces;

    public string GetError()
    {
        if (prefab.Length == 0)
            return "prefab is empty";
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