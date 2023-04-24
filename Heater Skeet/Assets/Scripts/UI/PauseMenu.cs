using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private enum PauseMenuState {
        Inactive,
        Opening,
        Idle,
        Transitioning,
        Closing
    }

    private PauseMenuState pauseMenuState = PauseMenuState.Inactive;

    public Transform[] selectionTransforms;
    public Transform cursorTransform;

    private int transitionCounter;
    private const float INTRO_DURATION = 30.0f;
    private const float TRANSITION_DURATION = 15.0f;
    private float ratio;

    private Vector3 cursorStartPosition;
    private Vector3 cursorEndPosition;
    private readonly Vector3 CURSOR_OFFSET = new Vector3(231.0f, -20.0f, 0.0f);

    private int currentSelection = 0;
    private int lastSelection = -1;

    private readonly Vector3 MENU_SCALE = new Vector3(0.8f, 0.8f, 1.0f);
    private readonly Color INACTIVE_COLOR = new Color(0.12f, 0.12f, 0.12f);
    private readonly Color ACTIVE_COLOR = new Color(1.0f, 0.831f, 0.0f);

    private MainInputActions controls;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("STARTING>>");
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
            Debug.Log("AYE DID IT! UP");
            startTransition(true);
        //Go down
        } else if (direction.y < 0.0f) {
            Debug.Log("downs");
            startTransition(false);
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

    private void startTransition(bool transitionUp) {
        // Bail if can't transition, like trying to transition up when at index 0
        //*** PLACE HOLDER for max
        if (currentSelection==0 && transitionUp==true)
        {
            Debug.Log(currentSelection + " Can't go down");
            return;
        }
        if (currentSelection== 4 && transitionUp==false)
        {
            Debug.Log(currentSelection + " Can't go up");
            return;
        }
        // Set lastSelection to currentSelection
        lastSelection=currentSelection;
        // Increment/decrement currentSelection based on parameter bool
        if(transitionUp==true)
        {
            Debug.Log(currentSelection);
            currentSelection--;
        }
        else if(transitionUp==false)
        {
            Debug.Log(currentSelection);
            currentSelection++;
        }
        // Set cursorStartPosition to cursorTransform.localPosition
        // Set cursorNextPosition to selectionTransforms[currentSelection].localPosition + CURSOR_OFFSET
        cursorStartPosition=cursorTransform.localPosition;
        cursorEndPosition=selectionTransforms[currentSelection].localPosition + CURSOR_OFFSET;

        // Set transitionCounter to zero
        transitionCounter=0;
        // Set pauseMenuState to Transitioning
        pauseMenuState=PauseMenuState.Transitioning;

    }

/**
IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
**/

    // Update is called once per frame
    void Update() {
        int frameCounter=0;
        if (pauseMenuState == PauseMenuState.Opening) {
            // Tween transform.localScale from (0.0f, 0.0f, 0.0f) to MENU_SCALE
            while (frameCounter < INTRO_DURATION)
            {
                transform.localScale=Vector3.Lerp(new Vector3(0.0f, 0.0f, 0.0f), MENU_SCALE, frameCounter/INTRO_DURATION);
                frameCounter++;
            }
            // When done, set state to PauseMenuState.Idle
            pauseMenuState= PauseMenuState.Idle;
            return;

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
            while(transitionCounter<TRANSITION_DURATION){
                cursorTransform.localPosition=Vector3.Lerp(cursorStartPosition, cursorEndPosition, transitionCounter/TRANSITION_DURATION);
                selectionTransforms[lastSelection].GetChild(0).GetComponent<Image>().color=Color.Lerp(ACTIVE_COLOR, INACTIVE_COLOR, transitionCounter/TRANSITION_DURATION);
                selectionTransforms[currentSelection].GetChild(0).GetComponent<Image>().color=Color.Lerp(INACTIVE_COLOR, ACTIVE_COLOR, transitionCounter/TRANSITION_DURATION);
                transitionCounter++;

            }
            

            // When done, set state to PauseMenuState.Idle
            pauseMenuState=PauseMenuState.Idle;
            return;
        } else if (pauseMenuState == PauseMenuState.Closing) {
            // Opposite of Opening, but save for later
        }
    }
}

/**while (frameCounter < INTRO_DURATION)
            {
                transform.localScale=Vector3.Lerp(new Vector3(0.0f, 0.0f, 0.0f), MENU_SCALE, frameCounter/INTRO_DURATION);
                frameCounter++;
            }
            **/
            