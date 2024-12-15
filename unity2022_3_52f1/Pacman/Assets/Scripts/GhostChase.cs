using UnityEngine;

// In original pacman, each ghost has a different chase behavior
    // Blinky (red) directly chases pacman
    // one of them will chase pacman and then once it gets close, it flees away
// If we wanted to do this one to one like the original game, that would take more effort and time
    // Future Goal: implement ghost unique behaviors for chasing pacman
public class GhostChase : GhostBehavior
{
    // When GhostChase is disabled, we enable GhostScatter
    private void OnDisable() {
        this.ghost.scatter.Enable();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Node node = other.GetComponent<Node>();

        // Instead of picking a random direction like GhostScatter, we need to have the ghost traverse to whatever the target is
            // If we have a node, and chase is enabled and the ghost is not frightened, we move our ghost toward pacman
            // we need to loop through all the available directions at this node
            // we need to calculate if we were to move toward this direction, would that put us farther or closer to our target?
                // whichever direction puts us closer to our target is the direction we should go
                // algorithmically, it will do the "PATH FINDING" doing that simple logic
        if (node != null && this.enabled && !this.ghost.frightened.enabled) {
            Vector2 direction = Vector2.zero;
            float minDistance = float.MaxValue;

            foreach (Vector2 availableDirection in node.availableDirections) {
                Vector3 newPosition = this.transform.position + new Vector3(availableDirection.x, availableDirection.y, 0.0f);
                float distance = (this.ghost.target.position - newPosition).sqrMagnitude;

                // if the distance is less than our current minDistance, then we want traverse toward that new direction
                if (distance < minDistance) {
                    direction = availableDirection;
                    minDistance = distance;
                }
            }

            this.ghost.movement.SetDirection(direction);
        }
    }
}
