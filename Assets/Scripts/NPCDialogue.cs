using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Yarn.Unity;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum PlayerConstellationState {
    PERSEUS,
    LOVERS,
    TRICKSTER,
    DIONYSUS,
    DRACO,
    CASSIOPEIA,
    GUN,
    MINOR1,
    MINOR2
}

public class NPCDialogue : MonoBehaviour
{
    private State currentState;
    public State CurrentState
    {
        get
        {
            return currentState;
        }
    }

    private bool canTalk;

    public DialogueRunner dialogueRunner;

    // Name of status variable to get from Dialog scripts
    [Header("Dialogue Script Status Variable")]
    [SerializeField] public string statusVar;

    [Header("Dialogue Script names")]
    // File names of the yarn spinner scripts (ex. LoversNPC)
    [SerializeField] public string idleStateDialogueTitle;
    [SerializeField] public string taskInProgressStateDialogueTitle;
    [SerializeField] public string taskCompleteDialogueTitle;
    [SerializeField] public string postCompletionDialogueTitle;

    [SerializeField] public CharacterImageView characterImageView;

    // Start is called before the first frame update
    void Start()
    {
        currentState = new IdleState(dialogueRunner, idleStateDialogueTitle);
        currentState.OnEnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (canTalk && Input.GetKeyDown(KeyCode.Space))
        {
            //dialogueRunner.StartDialogue("LoversNPC");
            string dialogueAnswer;
            dialogueRunner.VariableStorage.TryGetValue($"${statusVar}", out dialogueAnswer);

            if (dialogueAnswer.Equals("Affirmative"))
            {
                ChangeDialogueState(new IncompleteTaskState(dialogueRunner, taskInProgressStateDialogueTitle));
            }
            else if (dialogueAnswer.Equals("TalkToNPCAgain"))
            {
                ChangeDialogueState(new CompletedTaskState(dialogueRunner, taskCompleteDialogueTitle));
            }
            else if (dialogueAnswer.Equals("FinalState"))
            {
                ChangeDialogueState(new AllFinishedState(dialogueRunner, postCompletionDialogueTitle));
            }
            else if (dialogueAnswer.Equals("Beginning"))
            {
                ChangeDialogueState(new IdleState(dialogueRunner, idleStateDialogueTitle));
            }
            currentState.OnExecuteState(this);
        }
    }

    public void ChangeDialogueState(State newState)
    {
        currentState.OnExitState(this);
        currentState = newState;
        newState.OnEnterState(this);
        Debug.Log($"STATE: {currentState.GetType()}");
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") {
            canTalk = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") {
            canTalk = false;   
        }
    }

    [YarnCommand("show_image")]
    public void ShowImage(string filepath)
    {
        if (!filepath.Equals("NO SPRITE"))
        {

            characterImageView.characterDialogueImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filepath);
            Debug.Log($"SPRITE: {characterImageView.characterDialogueImage.sprite}");

        }
        else
        {
            characterImageView.characterDialogueImage.sprite = null;
            Debug.Log($"SPRITE: {characterImageView.characterDialogueImage.sprite}");
        }
    }
}
