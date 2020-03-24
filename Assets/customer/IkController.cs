using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkController : MonoBehaviour {

    [SerializeField] float distanceTreshold = 2;
    [SerializeField] float stepDistance = 1;
    [SerializeField] IKLeg[] Leggs;
    [SerializeField] float stepTime = 1f;
    [SerializeField] float height;

    Vector3[] Targets;
  
    private void Start() {
        Targets = new Vector3[Leggs.Length];
        for (int i = 0; i < Targets.Length; i++) {
            Targets[i] = Leggs[i].target;
        }

        StartCoroutine(SetSteps());
    }

    int aaaa;
    bool up;
    Vector3 previousPosition;
    Vector3 stepVector;

    void FixedUpdate() {
        stepVector = (transform.position - previousPosition).normalized * stepDistance;

        for (int i = 0; i < Leggs.Length; i++) {
            Leggs[i].target += (Targets[i] - Leggs[i].target);

            //if (Leggs[i].extended) {
            //    RaycastHit hit;
            //    if (Physics.Raycast(new Ray(Leggs[i].Target.position + transform.up * 2 + stepVector, -transform.up), out hit, 5)) {
            //        Targets[i] = hit.point;
            //    }
            //}
        }

        //if (aaaa >= 6) {
        //    doSteps(true);
        //    aaaa = 0;
        //}
        //else if (aaaa == 3) {
        //    doSteps(false);
        //}
        //aaaa += 1;

        previousPosition = transform.position;
    }

    IEnumerator SetSteps() {
        while (true) {
            doSteps(true);
            doReise(false);
            yield return new WaitForSeconds(stepTime);
            doSteps(false);
            doReise(true);
            yield return new WaitForSeconds(stepTime);
        }
    }

    void doSteps(bool even) {
        int startLeg;
        if (even) startLeg = 0;
        else startLeg = 1;

        for (int i = startLeg; i < Leggs.Length; i += 2)  {
            float distToTarget = Vector3.Distance(Leggs[i].Target.position, Leggs[i].target);

            if (distToTarget >= distanceTreshold) {

                RaycastHit hit;
                if (Physics.Raycast(new Ray(Leggs[i].Target.position + transform.up + stepVector, -transform.up), out hit, 2))  {
                    Targets[i] = hit.point;
                }
            }
        }
    }

    void doReise(bool even) {
        int startLeg;
        if (even) startLeg = 0;
        else startLeg = 1;

        for (int i = startLeg; i < Leggs.Length; i += 2) {
            float distToTarget = Vector3.Distance(Leggs[i].Target.position, Leggs[i].target);

            if (distToTarget >= distanceTreshold) {

                RaycastHit hit;
                if (Physics.Raycast(new Ray(Leggs[i].Target.position + transform.up + stepVector, -transform.up), out hit, 2)) {
                    Targets[i] = hit.point + Vector3.up * height;
                }
            }
        }
    }
}