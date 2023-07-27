using UnityEngine;
using Elympics;

public class GothMovement : ElympicsMonoBehaviour {
    public float accel = 50f;
    public float friction = 10f;
    public float maxVel = 5f;
    public float jumpVel = 15f;
    private bool jumping = true;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private GameObject sprite;
    [SerializeField] private Transform head;
    private int faceDir = 1;

    private void Start() { rb = GetComponent<Rigidbody2D>(); }

    public void Movement(int direction, int jump, Vector3 mousePos) {
        rb.AddForce(new Vector2(direction * accel, 0));
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxVel, maxVel), rb.velocity.y);
        rb.AddForce(new Vector2(-rb.velocity.x * friction, 0));

        // if (!jumping && jump > 0) {
        //     rb.AddForce(new Vector2(0, jumpVel), ForceMode2D.Impulse);
        //     jumping = true;
        // }

        FaceDirection(mousePos);
    }

    public void FaceDirection(Vector3 mousePos) {
        var headpos = head.position;
        var direction = (mousePos - headpos).normalized;
        if (faceDir > 0) {
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            head.eulerAngles = new Vector3(0, 0, angle + 90);
            if (angle > 90 || angle < -90)
            {
                faceDir = -faceDir;
                sprite.transform.localScale = new Vector3(-1, 1, 1);
            }
        } else {
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 180;
            head.eulerAngles = new Vector3(0, 0, angle - 90);
            if (angle < -90 && angle > -270) {
                faceDir = -faceDir;
                sprite.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
			jumping = false; 
		}
    }

    void OnDrawGizmos() {
        var mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Gizmos.DrawLine(head.position, mousepos);
    }
}
