// PickupController.cs
using UnityEngine;

public class PickupController : MonoBehaviour
{
    private Rigidbody heldObject; // stores the present we're currently holding
    private float holdDistance; // stores how far away from the camera the object should be held.

    [SerializeField] private AnimationCurve smoothCurve; // used for making Lerps feel less linear
    private float maxDistFromCursor = 10f; // max distance that the held object can fall behind the cursor when moving around while picked up
    private float distBetweenOldAndNewPos; // store the distance between the current position and the new position
    private float drag; // store the value of maxDistFromCursor multiplied by the heldObject's weight
    private float moveEval; // store the return value of the smoothCurve's evaluation value for the third parameter in Vector3.Lerp()
    private Vector3 nextPosition; // store the calculation of the objects next position after evaluating the Lerp

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //hold the left mouse button down to pick up
        {
            PickUp();
        }

        if (Input.GetMouseButtonUp(0)) // when releasing the left mouse button it drops
        {
            Drop();
        }

        if (heldObject != null) // if the item is held move the object.
        {
            MoveObject();
        }
    }

    void PickUp()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Create a ray from the camera through the mouse position on screen
        RaycastHit hit; // Stores information about what the ray actually hit

        //Shoot the ray and see if it hits anything 
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Present")) // checks to see if the item we're clicking on has the present tag (so only the presents are moved)
            {
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>(); //Find the rigidbody component on the thing we hit with the raycast.
                if (rb != null) // if it has a rigidbody we can pick it up
                {
                    heldObject = rb; //stores this as our held object
                    heldObject.useGravity = false; // turn off gravity while the object is in our hand
                    heldObject.angularVelocity = Vector3.zero; // stop any sort of weird present spinning
                    heldObject.constraints = RigidbodyConstraints.FreezeRotation; // Freezes rotation to stop spinning
                    nextPosition = heldObject.gameObject.transform.position; // store the object's initial position
                    holdDistance = Vector3.Distance(Camera.main.transform.position, hit.point); // remember how far away from the camera we grabbed it, keeping the same distance when we move the mouse

                    // Check if this object has any weight to it. Otherwise, just use the maxDistFromCursor value
                    // Should mass be considered in this calculation?
                    drag = heldObject.gameObject.TryGetComponent<Weight>(out Weight obj) ? maxDistFromCursor * obj.weight : maxDistFromCursor;
                    if (obj == null)
                    {
                        Debug.Log("Weight component not found, \nthis probably is grabbing a child object or this object has no weight component.");
                    }
                }
            }
        }
    }


    void MoveObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Create a ray from the camera through the current mouse position

        //Check to see if the mouse scroll wheel moved. 
        //Scroll down (negative) makes the holdDistance bigger which pushes the object away from the camera
        //Scroll up (positive) makes the holdDistance smaller which pulls the object closer to the camera
        holdDistance -= Input.mouseScrollDelta.y * 2f;
        holdDistance = Mathf.Clamp(holdDistance, 8f, 26f); // keeps the hold distance between 8 and 26 units and prevents the object from getting too close or too far away
        

        Vector3 newPosition = ray.origin + ray.direction * holdDistance; // Calculate where the object should be

        Debug.DrawRay(ray.origin, ray.direction * holdDistance, Color.yellow);

        // Don't let it go below ground level to stop clipping into the ground
        if (newPosition.y < 0.25f)
        {
            newPosition.y = 0.25f;
        }

        // Don't let it clip through the walls
        newPosition.x = Mathf.Clamp(newPosition.x, -18f, 18f);

        // Calculate the next position.
        // Get the distance using the sqrMagnitudes
        distBetweenOldAndNewPos = Mathf.Clamp(Mathf.Abs((newPosition - gameObject.transform.position).sqrMagnitude), 0, drag);
        moveEval = smoothCurve.Evaluate(Mathf.Clamp(distBetweenOldAndNewPos, 0, drag));
        nextPosition = Vector3.Lerp(nextPosition, newPosition, 1 - ((drag - moveEval) / Mathf.Clamp(drag, 1, drag)));

        heldObject.MovePosition(nextPosition); // move the rigidbody to the new position, using move position will stop the item being moved from teleporting through things on the scene.
    }

    void Drop()
    {
        if (heldObject != null) // makes sure that we're still holding the object
        {
            heldObject.linearVelocity = Vector3.zero; // stop any sort of momentum building when this object was grabbed whilst falling
            heldObject.useGravity = true; // adds gravity back in when we drop the item so it falls
            heldObject.constraints = RigidbodyConstraints.None; // unfreeze the rotation so it can fall and tumble when it lands

            // Add a small random spin for natural tumbling
            Vector3 randomTorque = new Vector3(
                Random.Range(-2f, 2f),
                Random.Range(-2f, 2f),
                Random.Range(-2f, 2f)
            );
            heldObject.AddTorque(randomTorque, ForceMode.Impulse);


            heldObject = null; // clears the reference and states that we are not holding the object anymore. 
        }
    }
}