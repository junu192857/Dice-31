using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class GameManagerDisplay : EditorWindow
{
    [MenuItem("Window/UI Toolkit/GameManagerDisplay")]
    public static void ShowGameManagerDisplay()
    {
        GameManagerDisplay wnd = GetWindow<GameManagerDisplay>();
        wnd.titleContent = new GUIContent("GameManagerDisplay");
    }
    
    Label gameStateLabel, currentCountLabel, roundCountLabel, matchCountLabel, bombDiceNumberLabel, corruptCountLabel;

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Resources/GameManagerDisplay.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        gameStateLabel = root.Q<Label>("GameState");
        currentCountLabel = root.Q<Label>("CurrentCount");
        roundCountLabel = root.Q<Label>("RoundCount");
        matchCountLabel = root.Q<Label>("MatchCount");
        bombDiceNumberLabel = root.Q<Label>("BombDiceNumber");
        corruptCountLabel = root.Q<Label>("CorruptCount");
    }

    public void Update()
    {
        if (GameManager.Inst == null) return;
        gameStateLabel.text = GameManager.Inst.gsm.State.ToString();
        currentCountLabel.text = GameManager.Inst.pm.curCount.ToString();
        roundCountLabel.text = GameManager.Inst.pm.roundCount.ToString();
        matchCountLabel.text = GameManager.Inst.pm.matchCount.ToString();
        bombDiceNumberLabel.text = GameManager.Inst.pm.bombDiceNum.ToString();
        corruptCountLabel.text = GameManager.Inst.pm.corruptStack.ToString();
    }
}