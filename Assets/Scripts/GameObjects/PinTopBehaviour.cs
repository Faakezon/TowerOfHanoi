using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class handles the first collision with the ring controlled by the user.
public class PinTopBehaviour : MonoBehaviour
{
    private PinBehaviour parentPinBehaviour;
    private Vector2 overYPos;

    #region SnapProperties
    private bool isSnapped;
    private float snapSpeed;
    private float snapRange;
    #endregion

    private void Start() {
        parentPinBehaviour = this.transform.parent.GetComponent<PinBehaviour>();
        overYPos = new Vector2(this.transform.position.x, this.transform.position.y + 1);
    }

    private void Update()
    {
        if (GameManager.instance.useSnapFunctionality)
        {
            SetSnapProperties();
            SnapMovement();
        }
    }



    #region CollisionHandling
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
            return;
        
        HandleCollision(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        RingBehaviour collidingRing = collision.GetComponent<RingBehaviour>();

        if (collidingRing.gameObject.transform.position.y > this.transform.position.y && collidingRing.GetIsMoving())
        {
            parentPinBehaviour.RemoveRingFromPin(collidingRing);
            collidingRing.SetOnPin(false);
        }
    }
    //Handle Collision, with what is presumed is a ring.
    private void HandleCollision(Collider2D collision)
    {
        GameObject collidingRing;
        RingBehaviour collidingRingBehaviour;
        //Get the collider and its ringbehaviour component.
        // Collision with the Ring, ie Player
        if (collision.gameObject.tag == "Player")
        {
            collidingRing = collision.gameObject;
            collidingRingBehaviour = collidingRing.GetComponent<RingBehaviour>();
        }
        else
        {
            return;
        }

        // if the user wants to use the snap functionality
        if (GameManager.instance.useSnapFunctionality)
        {
            if (isSnapped)
                AddToPin(collidingRingBehaviour);
        }


        if (collidingRingBehaviour.GetIsMoving()) // If we are moving the ring, cant add it then.
            return;
        if (parentPinBehaviour.GetRingsOnThisPin().Contains(collidingRingBehaviour)) // If the pin already contains this ring, cant add it again.
            return;

        if (IsOkToLand(collidingRing)) // If we are above or same level as the pin when colliding.
        {
            if (parentPinBehaviour.GetAmountOfRingsOnThisPin() == 0) // are we first on this pin? (parent pin)
            {
                AddToPin(collidingRingBehaviour);
            }
            else
            {
                if (CheckIfPinHasRings()) // If there are any other rings on this pin
                {
                    //if yes, check size on the top ring on the pin
                    if (CheckSizeOnTopRing(collidingRingBehaviour))
                    {
                        //The ring on the pin is bigger, so we can add this smaller one to the pin.
                        AddToPin(collidingRingBehaviour);
                    }//Else, would be some implementation if the ring that we are dropping is bigger than the one on the pin.
                }
            }
        }
    }
    #endregion

    #region PinHandling
    private void AddToPin(RingBehaviour collidingRingBehaviour)
    {
        parentPinBehaviour.CheckAddCollidingRingToPin(collidingRingBehaviour);
        collidingRingBehaviour.SetOnPin(true, this.transform.position);
    }
    private bool CheckSizeOnTopRing(RingBehaviour collidingRingBehaviour)
    {
        return parentPinBehaviour.GetRingsOnThisPin()[parentPinBehaviour.GetAmountOfRingsOnThisPin() - 1].RingSize > collidingRingBehaviour.RingSize;
    }
    private bool CheckIfPinHasRings()
    {
        return parentPinBehaviour.GetRingsOnThisPin().Count >= 1;
    }
    private bool IsOkToLand(GameObject collidingRing)
    {
        return collidingRing.transform.position.y >= this.transform.position.y;
    }
    #endregion

    #region Snapping
    /// <summary>
    /// Set snap speed and range based on input selected from the user
    /// </summary>
    private void SetSnapProperties()
    {
        if (UserInputManager.instance.INPUT_SELECTED == UserInputManager.INPUT_CHOICE.TOUCH)
        {
            snapSpeed = GameManager.instance.snapSpeedTouch;
            snapRange = GameManager.instance.snapRangeTouch;
        }
        else
        {
            snapSpeed = GameManager.instance.snapSpeedMouse;
            snapRange = GameManager.instance.snapRangeMouse;
        }
    }
    private void SnapMovement()
    {
        if (!UserInputManager.instance.AllowInput)
            return;

        if (UserInputManager.instance.SelectedRing != null && UserInputManager.instance.IsAnyInput())
        {
            if (parentPinBehaviour.GetRingsOnThisPin().Contains(UserInputManager.instance.SelectedRing.GetComponent<RingBehaviour>())) // If the pin already contains this ring, cant add it again.
                return;

            RingBehaviour tempRing = UserInputManager.instance.SelectedRing.GetComponent<RingBehaviour>();


            if (parentPinBehaviour.GetAmountOfRingsOnThisPin() == 0) // are we first on this pin? (parent pin)
            {
                if (Vector2.Distance(UserInputManager.instance.SelectedRing.transform.position, this.overYPos) < snapRange)
                {
                    UserInputManager.instance.SelectedRing.transform.position = Vector2.Lerp(UserInputManager.instance.SelectedRing.transform.position, this.overYPos, Time.deltaTime * snapSpeed);
                    isSnapped = true;
                }
            }
            else
            {
                if (CheckIfPinHasRings()) // If there are any other rings on this pin
                {
                    //if yes, check size on the top ring on the pin
                    if (CheckSizeOnTopRing(tempRing))
                    {
                        //The ring on the pin is bigger, so we can add this smaller one to the pin.
                        if (Vector2.Distance(UserInputManager.instance.SelectedRing.transform.position, this.overYPos) < snapRange)
                        {
                            UserInputManager.instance.SelectedRing.transform.position = Vector2.Lerp(UserInputManager.instance.SelectedRing.transform.position, this.overYPos, Time.deltaTime * snapSpeed);
                            isSnapped = true;
                        }
                    }//Else, would be some implementation if the ring that we are dropping is bigger than the one on the pin.
                }
            }


        }
        isSnapped = false;
    }
    #endregion
}
