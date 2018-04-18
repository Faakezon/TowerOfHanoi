using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PinBehaviour : MonoBehaviour {

    //Contains all the rings that resides on this pin.
    public List<RingBehaviour> ringsOnPin = new List<RingBehaviour>();


    internal void CheckAddCollidingRingToPin(RingBehaviour ring)
    {
        AddRingToPin(ring);
        CheckIfWon();

        //Allow Input when left pin is filled
        CheckIfAllowInput();
    }

    internal void RemoveRingFromPin(RingBehaviour ring)
    {
        // Let the ring know it has no OnThisPin
        ring.gameObject.GetComponent<RingBehaviour>().OnThisPin = null;
        // Remove the ring from the pinlist
        ringsOnPin.Remove(ring.gameObject.GetComponent<RingBehaviour>());
    }

    private void AddRingToPin(RingBehaviour ring)
    {
        UserInputManager.instance.SelectedRing = null;
        ringsOnPin.Add(ring.gameObject.GetComponent<RingBehaviour>());
        ring.gameObject.GetComponent<RingBehaviour>().OnThisPin = this;
    }

    public List<RingBehaviour> GetRingsOnThisPin()
    {
        return ringsOnPin;
    }

    public int GetAmountOfRingsOnThisPin()
    {
        return ringsOnPin.Count;
    }

    public int GetRingPositionOnPin(RingBehaviour ring)
    {
        return ringsOnPin.IndexOf(ring);
    }


    #region WinningConditions
    /// <summary>
    /// Check for Win Condition
    /// </summary>
    private void CheckIfWon()
    {
        if (this.gameObject.name == "RightPin" || this.gameObject.name == "MiddlePin")
        {
            if (GetRingsOnThisPin().Count == 4)
            {
                GameManager.instance.WonGame();
            }
        }
    }
    #endregion

    #region Input
    /// <summary>
    /// Check if Left Pin is filled and if so, allow user input. No cheating!
    /// </summary>
    private void CheckIfAllowInput()
    {
        if (UserInputManager.instance.AllowInput == true)
            return;

        if (this.gameObject.name == "LeftPin")
        {
            if (GetRingsOnThisPin().Count == 4)
            {
                UserInputManager.instance.AllowInput = true;
            }
        }
    }
    #endregion

}
