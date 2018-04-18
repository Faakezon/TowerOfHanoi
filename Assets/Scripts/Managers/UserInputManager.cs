using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// TODO Cleanup and refactor.
/// </summary>

public class UserInputManager : MonoBehaviour {

    //Instantiate a static singleton for easy access.
    public static UserInputManager instance = null;

    //Enum for InputChoice
    public enum INPUT_CHOICE {
        TOUCH,
        MOUSE
    };
    public INPUT_CHOICE INPUT_SELECTED;
    public GameObject SelectedRing { get; set; }
    public bool AllowInput { get; set; }

    void Awake () {
        InitUserInputHandler();
        INPUT_SELECTED = INPUT_CHOICE.TOUCH;
    }

    private void InitUserInputHandler()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Update ()
    {
        if (AllowInput) {
            INPUT_SELECTED = CheckInputSelection();
            RunSelectedInput();
            CheckInputAndRingSelected();
        }  
    }

    #region InputSelection
    private INPUT_CHOICE CheckInputSelection()
    {
        if (Input.touchCount > 0)
        {
            return INPUT_CHOICE.TOUCH;
        }
        else
        {
            return INPUT_CHOICE.MOUSE;
        }
    }
    private void RunSelectedInput()
    {
        switch (INPUT_SELECTED)
        {
            case INPUT_CHOICE.TOUCH:
                TouchInput();
                break;
            case INPUT_CHOICE.MOUSE:
                MouseInput();
                break;
            default:
                break;
        }
    }
    private void CheckInputAndRingSelected()
    {
        // If we have Touch input and a ring is selected
        if ((IsTouching() && SelectedRing != null) || (IsClicked() && SelectedRing != null))
        {
            //Tell that ring
            SelectedRing.GetComponent<RingBehaviour>().SetIsSelectedAndInput(true);
        }
        if (!IsAnyInput())
        {
            SelectedRing = null;
        }
    }
    /// <summary>
    /// Is there any input at all, both mouse and touch.
    /// </summary>
    /// <returns>True if any input</returns>
    public bool IsAnyInput()
    {
        if (IsTouching() || IsClicked())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region TouchInput
    /// <summary>
    /// All Input from Touch is handled here.
    /// </summary>
    private void TouchInput()
    {
        if (IsTouching() && SelectedRing == null)
        {
            GameObject selection = GetSelectedRingTouch();
            if (selection == null)
                return;

            AudioManager.instance.PlaySfx("PickUp");

            SelectedRing = selection;
        }

        


    }
    private bool IsTouching()
    {
        if (Input.touchCount == 1)
        {
            if (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Stationary))
            {
                return true;
            }
        }
        return false;
    }
    private bool IsNotTouching()
    {
        if (Input.touchCount == 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    /// <summary> TOUCH VERSION
    /// Returns the Ring as Gameobject if raycast hit. Else return null.
    /// </summary>
    /// <returns>Gameobject</returns>
    GameObject GetSelectedRingTouch()
    {
        // No need for raycast if a ring is already selected.
        if (SelectedRing != null) {
            return SelectedRing;
        }

        RaycastHit2D hit = RayCast();

        // If we did not hit anything that we wanted, just return null.
        if (!hit) return null;

        //If we hit a Ring
        if (hit.transform.gameObject.GetComponent<RingBehaviour>())
        {
            return hit.transform.gameObject;
        }
        //If we hit a PinBehaviourObject
        if (hit.transform.gameObject.GetComponent<PinBehaviour>())
        {
            PinBehaviour temp = hit.transform.gameObject.GetComponent<PinBehaviour>();
            if(temp.GetAmountOfRingsOnThisPin() != 0) // and there is a ring on the pin
                return temp.GetRingsOnThisPin()[temp.GetAmountOfRingsOnThisPin() - 1].gameObject; //return top ring.
        }
        //If nothing else, return null
        return null;
    }
    #endregion

    #region MouseInput
    /// <summary>
    /// All Input from Mouse is handled here.
    /// </summary>
    private void MouseInput()
    {
        if (IsClicked() && SelectedRing == null)
        {
            GameObject selection = GetSelectedRingMouse();
            if (selection == null)
                return;

            AudioManager.instance.PlaySfx("PickUp");

            SelectedRing = selection;
        }

    }
    private bool IsClicked()
    {
        if (Input.GetMouseButton(0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool IsNotClicked()
    {
        if (Input.GetMouseButtonUp(0))
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    
    /// <summary> MOUSE VERSION
    /// Returns the Ring as Gameobject if raycast hit. Else return null.
    /// </summary>
    /// <returns>Gameobject</returns>
    GameObject GetSelectedRingMouse()
    {
        // No need for raycast if a ring is already selected.
        if (SelectedRing != null) {
            return SelectedRing;
        }

        RaycastHit2D hit = RayCast();

        // If we did not hit anything that we wanted, just return null.
        if (!hit) return null;

        //If we hit a Ring
        if (hit.transform.gameObject.GetComponent<RingBehaviour>())
        {
            return hit.transform.gameObject;
        }
        //If we hit a PinBehaviourObject
        if (hit.transform.gameObject.GetComponent<PinBehaviour>())
        {
            PinBehaviour temp = hit.transform.gameObject.GetComponent<PinBehaviour>();
            if (temp.GetAmountOfRingsOnThisPin() != 0) // and there is a ring on the pin
                return temp.GetRingsOnThisPin()[temp.GetAmountOfRingsOnThisPin() - 1].gameObject; //return top ring.
        }
        //If nothing else, return null
        return null;
    }
    #endregion

    #region Raycasting
    /// <summary>
    /// Input specific raycast
    /// </summary>
    /// <returns>Hit information from raycast</returns>
    private RaycastHit2D RayCast()
    {
        RaycastHit2D hit;
        switch (INPUT_SELECTED)
        {
            case INPUT_CHOICE.TOUCH:
                //Converting Touch Pos to World Pos
                Vector2 rayPosTouch = Camera.main.ScreenToWorldPoint(new Vector3(
                            Input.touches[0].position.x,
                            Input.touches[0].position.y,
                            0));
                hit = Physics2D.Raycast(rayPosTouch, Vector2.zero, 10f);
                return hit;
            case INPUT_CHOICE.MOUSE:
                Vector2 rayPosMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                hit = Physics2D.Raycast(rayPosMouse, Vector2.zero, 0f);
                return hit;
                
        }
        return new RaycastHit2D();
    }
    #endregion





}
