using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class Draw : MonoBehaviour {

    SteamVR_TrackedObject trackedObj;

	void Awake () { //runs whether or not the game object is enabled
        trackedObj = GetComponent<SteamVR_TrackedObject>(); //assign variable
	}

	void FixedUpdate () {  //called for every physics step (fixed interval between calls). Update settings Unity > Edit > Project Settings > Time: 1/90 or 0.011111
        SteamVR_Controller.Device device = SteamVR_Controller.Input((int)trackedObj.index); //assign device on every fixed update for the input using the index of the tracked object
        if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("You are holding 'Touch' on the Trigger");
        }
	}

    void OnTriggerStay(Collider col)
    {
        Debug.Log("You have collided with " + col.name + " and actived OnTriggerStay");
        SteamVR_Controller.Device device = SteamVR_Controller.Input((int)trackedObj.index);
        if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("You have collided with " + col.name + " while holding down Touch");
            col.gameObject.transform.SetParent(gameObject.transform);
        }
    }
}
