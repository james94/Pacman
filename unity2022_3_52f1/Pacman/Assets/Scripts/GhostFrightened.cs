using UnityEngine;

// When your ghost is in frightened state, it will be moving away from the target
public class GhostFrightened : GhostBehavior
{
    public SpriteRenderer body;
    public SpriteRenderer eyes;
    public SpriteRenderer blue;
    public SpriteRenderer white;

    public bool eaten { get; private set; }

    public override void Enable(float duration) {
        base.Enable(duration);

        this.body.enabled = false;
        this.eyes.enabled = false;
        this.blue.enabled = true;
        this.white.enabled = false;

        // After a few seconds, we need ghosts to start flashing to indicate they will be exiting this state
        Invoke(nameof(Flash), duration / 2.0f);
    }

    // when our ghost is no longer frightened, we want to reset
    public override void Disable() {
        base.Disable();

        this.body.enabled = true;
        this.eyes.enabled = true;
        this.blue.enabled = false;
        this.white.enabled = false;
    }

    private void Flash() {
        // only do this if the ghost hasn't been eaten
        if (!this.eaten) {
            this.blue.enabled = false;
            this.white.enabled = true;
            // When our ghost starts flashing again, we need to make sure that animation is reset to the beginning
            this.white.GetComponent<AnimatedSprite>().Restart();
        }
    }

    private void Eaten() {
        this.eaten = true;

        Vector3 position = this.ghost.home.inside.position;
        position.z = this.ghost.transform.position.z;
        this.ghost.transform.position = position;

        // our ghosts who are eaten should remain in the home for however long our frightened duration is
        this.ghost.home.Enable(this.duration);

        this.body.enabled = false;
        this.eyes.enabled = true;
        this.blue.enabled = false;
        this.white.enabled = false;
    }

    private void OnEnable() {
        this.ghost.movement.speedMultiplier = 0.5f;
        this.eaten = false;
    }

    private void OnDisable() {
        this.ghost.movement.speedMultiplier = 1.0f;
        this.eaten = false;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman")) {
            if (this.enabled) {
                Eaten();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Node node = other.GetComponent<Node>();

            // If we have a node, and frightened is enabled, we start at zero, we use max distance initially starting with min
            // we need to loop through all the available directions at this node
            // we need to calculate if we were to move toward this direction, would that put us farther or closer to our target?
                // whichever direction puts us farther from our target is the direction we should go
                // algorithmically, it will do the "PATH FINDING" doing that simple logic
        if (node != null && this.enabled) {
            Vector2 direction = Vector2.zero;
            float maxDistance = float.MinValue;

            foreach (Vector2 availableDirection in node.availableDirections) {
                Vector3 newPosition = this.transform.position + new Vector3(availableDirection.x, availableDirection.y, 0.0f);
                float distance = (this.ghost.target.position - newPosition).sqrMagnitude;

                // if the distance is greater than our current maxDistance, then we want traverse toward that new direction
                if (distance > maxDistance) {
                    direction = availableDirection;
                    maxDistance = distance;
                }
            }

            this.ghost.movement.SetDirection(direction);
        }
    }

}
