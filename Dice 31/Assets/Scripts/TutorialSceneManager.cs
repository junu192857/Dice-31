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
    [SerializeField] private GameObject buttonsParent;
    [SerializeField] private float rotationSpeed = 30f;
    
    private DiceInfoData diceInfoData;

    private int page;
    [SerializeField] private Text pageText;
    [SerializeField] private List<GameObject> GeneralInfos;
    [SerializeField] private GameObject DiceInfo;
    private void Start()
    {
        diceInfoData = JsonUtility.FromJson<DiceInfoData>(Resources.Load<TextAsset>(diceDataPath).text);
        var error = diceInfoData.GetError();
        Debug.Assert(error.Length == 0, error);
        for (var i = 0; i < diceInfoData.data.Length; i++)
        {
            var button = buttonsParent.transform.GetChild(i).GetComponent<Button>();
            var index = i;
            button.onClick.AddListener(() => HandleSelectDiceClick(index));
            var image = Resources.Load<Sprite>(diceInfoData.data[i].image);
            var imageComp = button.transform.GetChild(0).GetComponent<Image>();
            imageComp.sprite = image;
        }
        page = 0;
        ShowHelpPage(0);
    }

    private void Update()
    {
        diceParent.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void HandleSelectDiceClick(int i)
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
        var oldDice = diceParent.transform.GetChild(0);
        var rotation = oldDice.localRotation;
        Destroy(oldDice.gameObject);
        var prefab = Resources.Load<GameObject>(diceInfo.prefab);
        var dice = Instantiate(prefab, diceParent.transform);
        dice.transform.localPosition = Vector3.zero;
        dice.transform.localRotation = rotation;
        dice.transform.localScale = Vector3.one * 0.15f;
    }
    public void ShowHelpPage(int currentPage) {
        page = currentPage;
        for (int i = 0; i < GeneralInfos.Count; i++)
        {
            if (i == currentPage) { 
                GeneralInfos[i].gameObject.SetActive(true);
            }
            else GeneralInfos[i].gameObject.SetActive(false);
        }
        DiceInfo.gameObject.SetActive(false);
        pageText.text = $"{currentPage + 1} / {GeneralInfos.Count}";
    }
    public void HandleMovingPage(bool left) {
        if (left)
        {
            ShowHelpPage(--page);
        }
        else ShowHelpPage(++page);
    }

    public void ShowDiceInformation() {
        foreach (var panel in GeneralInfos) {
            panel.gameObject.SetActive(false);
        }
        DiceInfo.gameObject.SetActive(true);
        pageText.text = "";
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