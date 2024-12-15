using System.Collections;
using UnityEngine;

// For GhostHome, we need some references to some ghost transforms
public class GhostHome : GhostBehavior
{
    public Transform inside;

    public Transform outside;

    private void OnEnable() {
        StopAllCoroutines();
    }

    // When the ghost is at home, they should just be bouncing around
        // When they are no longer at home, we perform this animation of them leaving
    private void OnDisable() {
        if (this.gameObject.activeSelf) {
            StartCoroutine(ExitTransition());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (this.enabled && collision.gameObject.layer == LayerMask.NameToLayer("Obstacle")) {
            this.ghost.movement.SetDirection(-this.ghost.movement.direction);
        }
    }

    // Coroutine allows us to yield, pause execution: we're doing it because the animation is a sequence
        // 1st: we move to the inside position
        // 2nd: then we move from inside to outside position
    private IEnumerator ExitTransition() {
        this.ghost.movement.SetDirection(Vector2.up, true);
        this.ghost.movement.rigidbody.isKinematic = true;
        this.ghost.movement.enabled = false;

        Vector3 position = this.transform.position;

        float duration = 0.5f;
        float elapsed = 0.0f;

        // while elapsed is less than duration, we'll continue updating the position of ghost: move to inside
        while (elapsed < duration) {
            Debug.Log("Move inside.");
            // interpolate between our current position and the transform we're moving towards
            Vector3 newPosition = Vector3.Lerp(position, this.inside.position, elapsed / duration);
            newPosition.z = position.z;
            this.ghost.transform.position = newPosition;
            elapsed += Time.deltaTime;
            yield return null; // its going to wait a frame and then continue where it left off
        }

        // reset elapsed time in between going from inside to outside position
        elapsed = 0.0f;

        // then we move from inside to outside position
        while (elapsed < duration) {
            Debug.Log("Move from inside to outside.");
            // interpolate between our current position and the transform we're moving towards
            Vector3 newPosition = Vector3.Lerp(this.inside.position, this.outside.position, elapsed / duration);
            newPosition.z = position.z;
            this.ghost.transform.position = newPosition;
            elapsed += Time.deltaTime;
            yield return null; // its going to wait a frame and then continue where it left off
        }

        // choose random value between 0 and 1, if less than 0.5, we'll go left else to right
        this.ghost.movement.SetDirection(new Vector2(Random.value < 0.5f ? -1.0f : 1.0f, 0.0f), true); 
        this.ghost.movement.rigidbody.isKinematic = false;
        this.ghost.movement.enabled = true;
    }
}
