using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RingBehaviour : MonoBehaviour {

    //Instance Enum
    public enum RING_STATUS {
        MOVABLE,
        CAN_MOVE_UP,
        STUCK
    };
    public RING_STATUS Status;
    private SpriteRenderer spriteRenderer;

    //Instance components - Public
    public Rigidbody2D RingRigidbody { get; set; }
    public BoxCollider2D BoxCollider { get; set; }
    public Sprite[] sprites;

    //Other components
    private BoxCollider2D plate;

    //Instance variables - Private
    private List<RingBehaviour> otherRings = new List<RingBehaviour>();
    private float movementSpeed;
    private bool isSelectedAndInput;
    private bool landed;

    //Instance variables - Public
    public bool onPin;

    //Properties
    public PinBehaviour OnThisPin { get; set; }
    public int RingSize { get; set; }


    private void Start()
    {
        InitRing();
    }

    void Update()
    {
        // First of all, check if we even allow input from the user.
        if (!UserInputManager.instance.AllowInput)
            return;

        // handle collisions
        HandleCollision();

        // if there is a ring that is not on a pin.
        HandleNotOnPin();

        // if we have no input
        HandleResetRingForNoInput();

        // handle sprite changes.
        HandleSpriteChange();

        if (!isSelectedAndInput) //if we have a ring selected and there is input, continue forward, else just break here.
            return;

        // Set the movement speed based on user input, mouse or touch
        SetMovementSpeed();

        // And update Movement based on selected Input 
        UpdateMovementOnInput();
    }


    #region Initialize
    private void InitRing()
    {
        // set ring´s current status to movable.
        Status = RING_STATUS.MOVABLE;
        // set the required components
        RingRigidbody = GetComponent<Rigidbody2D>();
        BoxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        plate = GameObject.Find("Plate").GetComponent<BoxCollider2D>();
        // find out what my size is.
        SetupPinSize();
        // get the other rings to keep track for collision
        otherRings = RingManager.instance.GetOtherRings(this);
    }
    private void SetupPinSize()
    {
        switch (this.gameObject.name)
        {
            case "Red":
                RingSize = 1;
                break;
            case "Green":
                RingSize = 2;
                break;
            case "Blue":
                RingSize = 3;
                break;
            case "Yellow":
                RingSize = 4;
                break;
        }
    }
    #endregion

    #region InputMovement
    /// <summary>
    /// Handle the selected input and control the ring
    /// </summary>
    private void UpdateMovementOnInput()
    {
        // Check ring status, and switch rules for movement.
        Status = CheckRingStatus();

        // Check what input we are using and based on the Status, what condition this ring is in.
        switch (UserInputManager.instance.INPUT_SELECTED)
        {
            case UserInputManager.INPUT_CHOICE.TOUCH:
                switch (Status)
                {
                    case RING_STATUS.MOVABLE:
                        UpdateTouchMovementMovable();
                        break;
                    case RING_STATUS.CAN_MOVE_UP:
                        UpdateTouchMovementCanMoveUp();
                        break;
                    case RING_STATUS.STUCK:
                        break;
                    default:
                        break;
                }
                break;
            case UserInputManager.INPUT_CHOICE.MOUSE:
                switch (Status)
                {
                    case RING_STATUS.MOVABLE:
                        UpdateMouseMovementMovable();
                        break;
                    case RING_STATUS.CAN_MOVE_UP:
                        UpdateMouseMovementCanMoveUp();
                        break;
                    case RING_STATUS.STUCK:
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Movement for TouchInput
    /// </summary>
    private void UpdateTouchMovementMovable()
    {
        SetRingGravityPropertiesToZero();
        // get the touch position from the screen touch to world point
        Vector3 touchedPos = Camera.main.ScreenToWorldPoint(new Vector3(
                Input.touches[0].position.x,
                Input.touches[0].position.y,
                -2));
        // lerp and set the position of the current object to that of the touch, but smoothly over time.
        transform.position = Vector2.Lerp(transform.position, touchedPos, Time.deltaTime * movementSpeed);
        ResetLanded();
    }
    private void UpdateTouchMovementCanMoveUp()
    {
        SetRingGravityPropertiesToZero();
        // get the touch position from the screen touch to world point
        Vector3 touchedPos = new Vector3(
            this.transform.position.x,
            Camera.main.ScreenToWorldPoint(new Vector3(
                0,
                Input.touches[0].position.y, 0)).y,
                -2);

        // lerp and set the position of the current object to that of the touch, but smoothly over time.
        if (touchedPos.y > transform.position.y)
        {
            transform.position = Vector2.Lerp(transform.position, touchedPos, Time.deltaTime * movementSpeed);
            ResetLanded();
        }
    }

    /// <summary>
    /// Movement for MouseInput
    /// </summary>
    private void UpdateMouseMovementMovable()
    {
        SetRingGravityPropertiesToZero();
        // get the touch position from the screen touch to world point
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                -2));
        // lerp and set the position of the current object to that of the touch, but smoothly over time.
        transform.position = Vector2.Lerp(transform.position, mousePos, Time.deltaTime * movementSpeed);
        ResetLanded();

    }
    private void UpdateMouseMovementCanMoveUp()
    {
        SetRingGravityPropertiesToZero();
        // get the mouse position from the screen world point
        Vector3 mousePos = new Vector3(
            this.transform.position.x,
            Camera.main.ScreenToWorldPoint(new Vector3(
                0,
                Input.mousePosition.y, 0)).y,
                -2);
        // only make a change if the mouse pos is above current ring position.
        if (mousePos.y > transform.position.y)
        {
            transform.position = Vector2.Lerp(transform.position, mousePos, Time.deltaTime * movementSpeed);
            ResetLanded();
        }
    }
    #endregion

    #region Handlers
    /// <summary>
    /// Tell the Input Manager that this ring is not on a pin and wants to be the selected ring.
    /// </summary>
    private void HandleNotOnPin()
    {
        if (!onPin)
            UserInputManager.instance.SelectedRing = this.gameObject;
    }
    /// <summary>
    /// If there is no input at all, reset the ring.
    /// </summary>
    internal void HandleResetRingForNoInput()
    {
        if (!UserInputManager.instance.IsAnyInput()) 
        {
            SetIsSelectedAndInput(false);
        }
    }
    private void HandleSpriteChange()
    {
        // IF not OnPin - Use first sprite.
        if (!onPin)
        {
            spriteRenderer.sprite = sprites[0];
        }
        else
        {
            spriteRenderer.sprite = sprites[1];
        }
    }
    
    #endregion

    #region Checks 
    private RING_STATUS CheckRingStatus()
    {
        // If ring is OnPin but not index == count (NOT On Top of other rings) STUCK
        if (CheckIfRingIsStuck())
        {
            return RING_STATUS.STUCK;
        }
        
        // If ring is OnPin and is index == count (On Top of other rings) CAN MOVE UP
        if (CheckIfRingCanMoveUp())
        {
            return RING_STATUS.CAN_MOVE_UP;
        }
        
        // If ring is free or alone on pin. MOVABLE
        if (CheckIfRingIsMovable())
        {
            return RING_STATUS.MOVABLE;
        }

        return RING_STATUS.MOVABLE;
    }
    private bool CheckIfRingIsMovable()
    {
        return !onPin && OnThisPin != null && OnThisPin.GetAmountOfRingsOnThisPin() - 1 == 1 && OnThisPin.GetRingPositionOnPin(this) == OnThisPin.GetAmountOfRingsOnThisPin();
    }
    private bool CheckIfRingCanMoveUp()
    {
        return onPin && OnThisPin.GetRingPositionOnPin(this) == OnThisPin.GetAmountOfRingsOnThisPin() - 1;
    }
    private bool CheckIfRingIsStuck()
    {
        return onPin && OnThisPin.GetRingPositionOnPin(this) != OnThisPin.GetAmountOfRingsOnThisPin() - 1;
    }
    #endregion

    #region Getters, Setters, Resetters
    private void SetMovementSpeed()
    {
        switch (UserInputManager.instance.INPUT_SELECTED)
        {
            case UserInputManager.INPUT_CHOICE.TOUCH:
                movementSpeed = 25;
                break;
            case UserInputManager.INPUT_CHOICE.MOUSE:
                movementSpeed = 5;
                break;
            default:
                break;
        }
    }
    internal void SetIsSelectedAndInput(bool selected)
    {
        isSelectedAndInput = selected;
    }
    internal bool GetIsMoving()
    {
        return isSelectedAndInput;
    }
    
    private void SetRingGravityPropertiesToZero()
    {
        RingRigidbody.velocity = Vector3.zero;
    }
    /// <summary>
    /// Used for removing the Ring from the pin
    /// </summary>
    /// <param name="onPin"></param>
    internal void SetOnPin(bool onPin)
    {
        this.onPin = onPin;
    } 
    /// <summary>
    /// Set Ring and the placement (pin vector)
    /// </summary>
    /// <param name="onPin"></param>
    /// <param name="pinPlacement"></param>
    internal void SetOnPin(bool onPin, Vector2 pinPlacement)
    {
        this.onPin = onPin;
        SetRingPlacement(pinPlacement);
    } 
    private void SetRingPlacement(Vector2 placement)
    {
        this.transform.position = new Vector3(placement.x, this.transform.position.y, this.transform.position.z);
    }

    internal void SetIsKinematicFalse()
    {
        RingRigidbody.isKinematic = false;
    }
    private void ResetLanded() { landed = false; }
    #endregion

    #region Collision
    private void HandleCollision()
    {
        if (!landed && (UserInputManager.instance.IsAnyInput() == false || onPin))
        {

            // check if we collide with the plate, play sound.
            if (BoxCollider.IsTouching(plate))
            {
                landed = true;
                AudioManager.instance.PlaySfx("Collide");
            }
            // also check collision with each ring, play sound.
            foreach (RingBehaviour ring in otherRings) 
            {
                if (BoxCollider.IsTouching(ring.BoxCollider))
                {
                    landed = true;
                    AudioManager.instance.PlaySfx("Collide");
                }
            }

        }
        // handle the switch in collision between on and off pin. 
        IgnoreRingCollision();
    }
    private void IgnoreRingCollision()
    {
        if (!onPin) //If ring is not on pin, ignore all other ring colliders.
        {
            RingManager.instance.SetCollisionHandlingOnRing(this, true);
        }
        else if(onPin) // else, dont ignore them
        {
            RingManager.instance.ResetRingCollisionHandling(this);
        } 
    }
    #endregion
}
