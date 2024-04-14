using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class TopDownCharacterController : MonoBehaviour
    {
        public float speed;

        private Animator animator;

        public Animator aanimator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }


        private void Update()
        {
            Vector2 dir = Vector2.zero;
            if (Input.GetKey(KeyCode.A))
            {
                dir.x = -1;
                animator.SetInteger("Direction", 3);
                aanimator.SetBool("Movimiento", true);
                aanimator.SetBool("otrolado", true);
                aanimator.SetBool("abajo", false);
                aanimator.SetBool("idel", false);
                aanimator.SetBool("lado", false);
                aanimator.SetBool("arriba", false);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                dir.x = 1;
                animator.SetInteger("Direction", 2);
                aanimator.SetBool("Movimiento", true);
                aanimator.SetBool("otrolado", false);
                aanimator.SetBool("abajo", false);
                aanimator.SetBool("idel", false);
                aanimator.SetBool("lado", true);
                aanimator.SetBool("arriba", false);
            }

            else if (Input.GetKey(KeyCode.W))
            {
                dir.y = 1;
                animator.SetInteger("Direction", 1);
                aanimator.SetBool("Movimiento", true);
                aanimator.SetBool("otrolado", false);
                aanimator.SetBool("abajo", false);
                aanimator.SetBool("idel", false);
                aanimator.SetBool("lado", false);
                aanimator.SetBool("arriba", true);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                dir.y = -1;
                animator.SetInteger("Direction", 0);
                aanimator.SetBool("Movimiento", true);
                aanimator.SetBool("otrolado", false);
                aanimator.SetBool("abajo", true);
                aanimator.SetBool("idel", false);
                aanimator.SetBool("lado", false);
                aanimator.SetBool("arriba", false);
            }
            else {
                aanimator.SetBool("Movimiento", false);

            }

            dir.Normalize();
            animator.SetBool("IsMoving", dir.magnitude > 0);

            GetComponent<Rigidbody2D>().velocity = speed * dir;
        }
    }
}
