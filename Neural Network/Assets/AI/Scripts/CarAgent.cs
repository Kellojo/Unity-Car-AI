using System.Collections.Generic;
using UnityEngine;

public class CarAgent : Agent {

    public float maxDetectionDistance = 10;
    public float speed = 10;
    public float angularSpeed = 2;
    private Rigidbody rb;

    Vector3 lastPosition;
    float distanceTravelled = 0;

    // Use this for initialization
    public override void ExtendedStart() {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    //cast in a direction and return the distance of free space in that direction
    private float Raycast(Vector3 dir) {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, maxDetectionDistance)) {
            Debug.DrawRay(transform.position, dir / maxDetectionDistance * hit.distance, GetSpecies().GetColor(), 0.01f);
            return hit.distance;
        } else {
            Debug.DrawRay(transform.position, dir, GetSpecies().GetColor(), 2);
            return maxDetectionDistance;
        }
    }

    //raycasts in a circle around the game object and return the meassured distances
    private float[] RaycastInCircle(int casts) {
        float degreeDelta = 360 / casts;
        List<float> distances = new List<float>();

        for (int i = 0; i < casts; i++) {
            Quaternion q = Quaternion.AngleAxis(i * degreeDelta, Vector3.up);
            Vector3 d = transform.forward * maxDetectionDistance;

            distances.Add(Raycast(q * d));
        }

        return distances.ToArray();
    }

    //agent update
    public override void runAgentUpdate() {
        distanceTravelled += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        if (neuralNetwork != null) {
            float[] inputs = RaycastInCircle(neuralNetwork.GetInputLayerSize());

            float[] steering = neuralNetwork.FeedForward(inputs);
            rb.velocity = transform.forward * speed;
            rb.angularVelocity = new Vector3(steering[0] * angularSpeed, steering[1] * angularSpeed, 0);
        }
    }
    //agent is dead
    public override void finished() {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void ApplyColor() {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers) {
            renderer.material.color = GetSpecies().GetColor();
        }
    }
    public override float CalcFitnessOnUpdate() {
        return distanceTravelled;
    }
    public override float CalcFitnessOnFinish() {
        return distanceTravelled;
    }


    //destroy agent
    private void OnCollisionEnter(Collision collision) {
        Done();
    }
}
