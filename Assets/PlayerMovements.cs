using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{

    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    Vector2 movement;
    public Animator anim;

    private float HorInp;
    private bool faceR;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        HorInp = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

    
        if (movement.y != 0 || movement.x != 0) {
            anim.SetBool("move", true);

        }
        else {
            anim.SetBool("move", false);
        }

        if (HorInp < 0 && !faceR) {
            flip();
        }
        else if (HorInp > 0 && faceR) {
            flip();
        }

        //Debug.Log(movement.x);
        //Debug.Log(movement);
    }

    void FixedUpdate() {
        if (movement.x != 0 && movement.y != 0) {
            movement.x = movement.x/1.4f;
            movement.y = movement.y/1.4f;
        }
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void flip() {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        faceR = !faceR;
    }
}
