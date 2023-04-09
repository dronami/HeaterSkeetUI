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
        // Hide menu on start (transform.localScale to (0.0f, 0.0f, 0.0f))
        // Call startOpening()

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

        if (direction.y > 0.0f) {
            // startTransition(true)
        } else if (direction.y < 0.0f) {
            // startTransition(false)
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
        // Set pauseMenuState to Opening
    }

    private void startTransition(bool transitionUp) {
        // Bail if can't transition, like trying to transition up when at index 0

        // Set lastSelection to currentSelection
        // Increment/decrement currentSelection based on parameter bool

        // Set cursorStartPosition to cursorTransform.localPosition
        // Set cursorNextPosition to selectionTransforms[currentSelection].localPosition + CURSOR_OFFSET

        // Set transitionCounter to zero
        // Set pauseMenuState to Transitioning

    }

    // Update is called once per frame
    void Update() {
        if (pauseMenuState == PauseMenuState.Opening) {
            // Tween transform.localScale from (0.0f, 0.0f, 0.0f) to MENU_SCALE
            
            // When done, set state to PauseMenuState.Idle
        } else if (pauseMenuState == PauseMenuState.Idle) {
            // Nothing to do here. Transitioning is accomplished via callbacks
            // (Transition functions are automatically called when player moves joystick,
            // just bail if they try to transition and menu state isn't Idle)
        } else if (pauseMenuState == PauseMenuState.Transitioning) {
            // Tween cursorTransform.localPosition from cursorStartPosition to cursorEndPosition
            // Tween selectionTransforms[lastSelection].GetChild(0).GetComponent<Image>().color from ACTIVE_COLOR to INACTIVE_COLOR
            // Tween selectionTransforms[currentSelection].GetChild(0).GetComponent<Image>().color from INACTIVE_COLOR to ACTIVE_COLOR

            // When done, set state to PauseMenuState.Idle
        } else if (pauseMenuState == PauseMenuState.Closing) {
            // Opposite of Opening, but save for later
        }
    }
}
