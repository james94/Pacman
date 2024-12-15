using UnityEngine;

public class GhostScatter : GhostBehavior
{
    // When GhostScatter is disabled, we enable GhostChase
    private void OnDisable() {
        this.ghost.chase.Enable();
    }

    // Check when one of the ghosts enters one of those green nodes (tilemap renderer ON shows them, else invisible)
    private void OnTriggerEnter2D(Collider2D other) {
        Node node = other.GetComponent<Node>();

        // If we have a node and our behavior is enabled and we're not frightened, then execute ghost scatter
        if (node != null && this.enabled && !this.ghost.frightened.enabled) {
            int index = Random.Range(0, node.availableDirections.Count);

            if (node.availableDirections.Count > 1 && node.availableDirections[index] == -this.ghost.movement.direction) {
                index++; // we know the next node the ghost goes to will be different

                // This causes ghosts to go back to their index that they started at, but I found it also causes pacman to reset
                if (index >= node.availableDirections.Count) {
                    index = 0;
                }
            }

            this.ghost.movement.SetDirection(node.availableDirections[index]);
        }
    }
} 
