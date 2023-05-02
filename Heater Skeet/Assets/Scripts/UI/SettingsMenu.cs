using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu: MonoBehaviour
{
    private enum PauseMenuState {
        Inactive,
        Opening,
        Idle,
        Transitioning,
        Closing
    }
    
    private enum Directions {
        Up,
        Down,
        Left,
        Right
    }

    private PauseMenuState pauseMenuState = PauseMenuState.Inactive;

    public Transform[] selectionTransforms;
    public Transform cursorTransform;

    private int transitionCounter;
    private const float INTRO_DURATION = 30.0f;
    private const float TRANSITION_DURATION = 15.0f;
    private float ratio;
    private int frameCounter=0;

    private Vector3 cursorStartPosition;
    private Vector3 cursorEndPosition;
    //private readonly Vector3 CURSOR_OFFSET = new Vector3(231.0f, -20.0f, 0.0f);
    private readonly Vector3[] cursorPos = new [] {
    new Vector3(280.0f, 135.0f, 0.0f),
    new Vector3(280.0f, 75.0f, 0.0f),
    new Vector3(280.0f, 15.0f, 0.0f),
    new Vector3(280.0f, -60.0f, 0.0f),
    new Vector3(280.0f, -115.0f, 0.0f),
    new Vector3(60.0f, -300.0f, 0.0f),
    new Vector3(280.0f, -300.0f, 0.0f),

    };
    //cursorOffset[0] = new Vector3 (231.0f, -20.0f, 0.0f);

    private int currentSelection = 0;
    private int lastSelection = -1;

    private readonly Vector3 MENU_SCALE = new Vector3(0.8f, 0.8f, 1.0f);
    private readonly Color INACTIVE_COLOR = new Color(0.12f, 0.12f, 0.12f);
    private readonly Color ACTIVE_COLOR = new Color(1.0f, 0.831f, 0.0f);

    private MainInputActions controls;

    //vars unique to settingsMenu
    //(Order: Up, Down, Left, Right)
    //-1 if no adj node, or INDEX of adj node

    //0=SFX
    //1=Muzak
    //2=Flava
    //3=Pottymouf
    //4=Lengua NEEDS WORK
    //5=OK
    //6=CANCEL
    private readonly int[,] adjList=new int[,] {{-1,1,-1,-1}, {0,2,-1,-1}, {1,3,-1,-1}, {2,4,-1,-1}, {3,5,-1,-1}, {4,-1,-1,6}, {4,-1,5,-1}};

    // Start is called before the first frame update
    void Start()
    {
        //setup cursor offsets. Must be done in function bc it is private
        
        Debug.Log("STARTING SETTINGS!!!!!!!!");
        // Hide menu on start (transform.localScale to (0.0f, 0.0f, 0.0f))
        // Call startOpening()
        transform.localScale=new Vector3(0.0f, 0.0f, 0.0f);
        startOpening();

        // Instantiate and enable controls object
        controls = new MainInputActions();
        controls.Enable();

        // Setup move cursor callback
        controls.Playa.MovePlayer.performed += context => moveCursor(context.ReadValue<Vector2>());

        // Setup cancel button callback
        controls.Playa.ChangeAbility.performed += context => cancelSelection();

        // Setup accept button callback
        controls.Playa.Dodge.performed += context => acceptSelection();
    }

    private void moveCursor(Vector2 direction) {
        // If pauseMenuState isn't Idle, bail
        if (pauseMenuState!=PauseMenuState.Idle)
        {
            Debug.Log("Not inactive");
            return;
        }
        //Go up
        if (direction.y > 0.0f) {
            Debug.Log("UP");
            startTransition(Directions.Up);
        //Go down
        } else if (direction.y < 0.0f) {
            Debug.Log("DOWN");
            startTransition(Directions.Down);
        }
        else if (direction.x<0.0f){
            Debug.Log("LEFT");
            startTransition(Directions.Left);
        }
        else if (direction.x>0.0f){
            Debug.Log("RIGHT");
            startTransition(Directions.Right);
        }
        
    }

    private void cancelSelection() {
        // Will become relevant later
    }

    private void acceptSelection() {
        // Will become relevant later
    }

    private void startOpening() {
        // Set transitionCounter to zero
        transitionCounter=0;
        // Set pauseMenuState to Opening
        pauseMenuState=PauseMenuState.Opening;
    }

     private void startTransition(Directions move) {
        // Bail if can't transition, like trying to transition up when at index 0
        
        Debug.Log("Currently at Node " + currentSelection);
        Debug.Log("value= " + adjList[currentSelection, (int) move]);

        if(adjList[currentSelection, (int) move] == -1){
            Debug.Log("Cant move" + move);
            return;
        }

        // Set lastSelection to currentSelection
        lastSelection=currentSelection;
                
        currentSelection=adjList[currentSelection, (int) move];

        // Set cursorStartPosition to cursorTransform.localPosition
        // Set cursorNextPosition to selectionTransforms[currentSelection].localPosition + CURSOR_OFFSET
        cursorStartPosition=cursorTransform.localPosition;
        //cursorEndPosition=selectionTransforms[currentSelection].localPosition + cursorOffset[0];
        cursorEndPosition=cursorPos[currentSelection];
        // Set transitionCounter to zero
        transitionCounter=0;
        // Set pauseMenuState to Transitioning
        pauseMenuState=PauseMenuState.Transitioning;

    }

    // Update is called once per frame
    void Update() {
        if (pauseMenuState == PauseMenuState.Opening) {
            // Tween transform.localScale from (0.0f, 0.0f, 0.0f) to MENU_SCALE
            if (frameCounter >= INTRO_DURATION){
             pauseMenuState= PauseMenuState.Idle;
             return;    
            };
            frameCounter++;
            transform.localScale=Vector3.Lerp(new Vector3(0.0f, 0.0f, 0.0f), MENU_SCALE, frameCounter/INTRO_DURATION);
            // When done, set state to PauseMenuState.Idle

        } else if (pauseMenuState == PauseMenuState.Idle) {
            // Nothing to do here. Transitioning is accomplished via callbacks
            // (Transition functions are automatically called when player moves joystick,
            // just bail if they try to transition and menu state isn't Idle)
            if (pauseMenuState!=PauseMenuState.Idle){
                return;
            }
        } else if (pauseMenuState == PauseMenuState.Transitioning) {
            // Tween cursorTransform.localPosition from cursorStartPosition to cursorEndPosition
            // Tween selectionTransforms[lastSelection].GetChild(0).GetComponent<Image>().color from ACTIVE_COLOR to INACTIVE_COLOR
            // Tween selectionTransforms[currentSelection].GetChild(0).GetComponent<Image>().color from INACTIVE_COLOR to ACTIVE_COLOR
            
            if(transitionCounter>=TRANSITION_DURATION){
            // When done, set state to PauseMenuState.Idle
            pauseMenuState=PauseMenuState.Idle;
            return;
            }
            transitionCounter++;
            cursorTransform.localPosition=Vector3.Lerp(cursorStartPosition, cursorEndPosition, transitionCounter/TRANSITION_DURATION);

            if (currentSelection==0 || currentSelection==1) {
            selectionTransforms[lastSelection].GetChild(0).GetChild(0).GetComponent<Image>().color=Color.Lerp(ACTIVE_COLOR, INACTIVE_COLOR, transitionCounter/TRANSITION_DURATION);
            selectionTransforms[currentSelection].GetChild(0).GetChild(0).GetComponent<Image>().color=Color.Lerp(INACTIVE_COLOR, ACTIVE_COLOR, transitionCounter/TRANSITION_DURATION);
            }
            else if (currentSelection==2 && lastSelection==1) {
            selectionTransforms[lastSelection].GetChild(0).GetChild(0).GetComponent<Image>().color=Color.Lerp(ACTIVE_COLOR, INACTIVE_COLOR, transitionCounter/TRANSITION_DURATION);
            selectionTransforms[currentSelection].GetChild(0).GetChild(0).GetComponent<Image>().color=Color.Lerp(INACTIVE_COLOR, ACTIVE_COLOR, transitionCounter/TRANSITION_DURATION);
            }
            else if(currentSelection==3 && lastSelection==2){
                selectionTransforms[lastSelection].GetChild(0).GetChild(0).GetComponent<Image>().color=Color.Lerp(ACTIVE_COLOR, INACTIVE_COLOR, transitionCounter/TRANSITION_DURATION);
                selectionTransforms[currentSelection].GetChild(0).GetComponent<Image>().color=Color.Lerp(INACTIVE_COLOR, ACTIVE_COLOR, transitionCounter/TRANSITION_DURATION);
            }
            else if(currentSelection==2 && lastSelection==3){
                selectionTransforms[lastSelection].GetChild(0).GetComponent<Image>().color=Color.Lerp(ACTIVE_COLOR, INACTIVE_COLOR, transitionCounter/TRANSITION_DURATION);
                selectionTransforms[currentSelection].GetChild(0).GetChild(0).GetComponent<Image>().color=Color.Lerp(INACTIVE_COLOR, ACTIVE_COLOR, transitionCounter/TRANSITION_DURATION);
            }
            else if(currentSelection==3 && lastSelection==4){
                //selectionTransforms[lastSelection].GetChild(0).GetComponent<Image>().color=Color.Lerp(ACTIVE_COLOR, INACTIVE_COLOR, transitionCounter/TRANSITION_DURATION);
                selectionTransforms[currentSelection].GetChild(0).GetComponent<Image>().color=Color.Lerp(INACTIVE_COLOR, ACTIVE_COLOR, transitionCounter/TRANSITION_DURATION);
            }
            else if (currentSelection==4 && lastSelection==3){
                selectionTransforms[lastSelection].GetChild(0).GetComponent<Image>().color=Color.Lerp(ACTIVE_COLOR, INACTIVE_COLOR, transitionCounter/TRANSITION_DURATION);
            }
            else if (currentSelection==5 && lastSelection==4){
                selectionTransforms[currentSelection].GetChild(0).GetComponent<Image>().color=Color.Lerp(INACTIVE_COLOR, ACTIVE_COLOR, transitionCounter/TRANSITION_DURATION);
            }
            else if (currentSelection==4 && (lastSelection==6 || lastSelection==5)){
                selectionTransforms[lastSelection].GetChild(0).GetComponent<Image>().color=Color.Lerp(ACTIVE_COLOR, INACTIVE_COLOR, transitionCounter/TRANSITION_DURATION);
            }
            else{
            selectionTransforms[lastSelection].GetChild(0).GetComponent<Image>().color=Color.Lerp(ACTIVE_COLOR, INACTIVE_COLOR, transitionCounter/TRANSITION_DURATION);
            selectionTransforms[currentSelection].GetChild(0).GetComponent<Image>().color=Color.Lerp(INACTIVE_COLOR, ACTIVE_COLOR, transitionCounter/TRANSITION_DURATION);
            }
        
        } else if (pauseMenuState == PauseMenuState.Closing) {
            // Opposite of Opening, but save for later
        }
    }
}
