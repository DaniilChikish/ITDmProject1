using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace ITDmProject
{
    public enum MotorState { Move, Delay, Free}
    public class MotionController : MonoBehaviour
    {
        public MotorState Status;
        private float speedMotion;
        private float speedRotation;
        private float alive;
        public float duration;
        public float delay;
        public float maxHop;
        private float distance;
        private float backCount;
        private GlobalControllerDesktop GC;
        public Queue<Transform> targets;
        public Transform camBody;
        public GameObject camEff;
        private Rigidbody body;

        // Use this for initialization
        void Start()
        {
            GC = GameObject.FindObjectOfType<GlobalControllerDesktop>();
            body = this.GetComponent<Rigidbody>();
            targets = new Queue<Transform>();
            Status = MotorState.Free;
        }

        // Update is called once per frame
        void Update()
        {
            if (backCount > 0)
                backCount -= Time.deltaTime;
            alive += Time.deltaTime;
            if (targets.Count > 0 && targets.Peek() == null)
            {
				targets.Dequeue();
				Status = MotorState.Free;
            }
            switch (Status)
            {
                case MotorState.Move:
                    {
                        if (backCount > 0)
                        {
                            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, targets.Peek().rotation, Time.deltaTime * speedRotation);
                            float speedFactor = Mathf.Cos(alive * Mathf.PI * 2 / duration + Mathf.PI) + 1;
                            speedMotion = distance / duration * speedFactor;
							//body.velocity = (targets.Peek().position - this.transform.position).normalized * speedMotion; 
							//body.AddForce((target.position - this.transform.position).normalized * speedMotion, ForceMode.Acceleration);
							this.transform.Translate((targets.Peek().position - this.transform.position).normalized * speedMotion * Time.deltaTime, Space.World);
                        }
                        else
                        {
                            body.velocity = Vector3.zero;
                            this.transform.rotation = targets.Peek().rotation;
                            Status = MotorState.Delay;
                            backCount = delay;
                        }
                        break;
                    }

                case MotorState.Delay:
                    {
                        if (backCount <= 0)
                        {
                            targets.Dequeue();
                            Status = MotorState.Free;
                        }
                        break;
                    }
                case MotorState.Free:
                    {
                        if (targets.Count > 0)
                        {
                            SetNextTarget();
                        }
                        break;
                    }
            }
        }
        private void SetNextTarget()
        {
            Status = MotorState.Move;
            backCount = duration;
            alive = 0;
            distance = Vector3.Distance(targets.Peek().position, this.transform.position);
            speedRotation = Quaternion.Angle(this.transform.rotation, targets.Peek().rotation) / backCount;
        }
        public void Capture(Transform target)
        {
            this.targets.Enqueue(target);
            //SetNextTarget();
        }
    }
}