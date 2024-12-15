using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;
    private int remainingPellets;

    public int ghostMultiplier { get; private set; } = 1;
    public int score { get; private set; }
    public int lives { get; private set; }

    private void Start() {
        NewGame();
    }

    private void Update() {
        if (this.lives<= 0 && Input.anyKeyDown) {
            NewGame();
        }
    }

    private void NewGame() {
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void InitializePelletCount() {
        remainingPellets = GetTotalPelletCount();
    }

    private int GetTotalPelletCount() {
        int count = 0;
        // For Tilemap-based pellets
        // Tilemap pelletTilemap = pellets.GetComponent<Tilemap>();
        // if (pelletTilemap != null) {
        //     // int count = 0;
        //     BoundsInt bounds = pelletTilemap.cellBounds;
        //     for (int x = bounds.min.x; x < bounds.max.x; x++) {
        //         for (int y = bounds.min.y; y < bounds.max.y; y++) {
        //             Vector3Int tilePosition = new Vector3Int(x, y, 0);
        //             if (pelletTilemap.HasTile(tilePosition)) {
        //                 count ++;
        //             }
        //         }
        //     }

        //     return count;
        // }

        // Fallback for Transform-based pellets
        foreach (Transform pellet in this.pellets) {
            pellet.gameObject.SetActive(true);
            count++;
        }
        return count;
    }

    private void NewRound() {
        // foreach (Transform pellet in this.pellets) {
        //     pellet.gameObject.SetActive(true);
        // }
        InitializePelletCount();

        ResetGhostState();
        ResetPacmanState();
    }

    private void ResetGhostState() {
        for (int i = 0; i < this.ghosts.Length; i++) {
            this.ghosts[i].ResetState();
        }
    }

    private void ResetPacmanState() {
        this.pacman.ResetState();
    }

    private void GameOver() {
        for (int i = 0; i < this.ghosts.Length; i++) {
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.pacman.gameObject.SetActive(false);
    }

    private void SetScore(int score) {
        this.score = score;
    }

    private void SetLives(int lives) {
        this.lives = lives;
    }

    public void GhostEaten(Ghost ghost) {
        int extraPoints = ghost.points * this.ghostMultiplier;
        SetScore(this.score + extraPoints);
        this.ghostMultiplier++;
    }

    public void PacmanEaten() {
        this.pacman.gameObject.SetActive(false);

        SetLives(this.lives - 1);

        if (this.lives > 0) {
            // Invoke allows us to call a method after some seconds
            Invoke(nameof(ResetPacmanState), 3.0f);
        }
        else {
            GameOver();
        }
    }

    public void PelletEaten(Pellet pellet) {
        pellet.gameObject.SetActive(false);

        SetScore(this.score + pellet.points);

        this.remainingPellets--; // Decrease counter

        if (!HasRemainingPellets()) {

        // if (this.remainingPellets <= 0) {
            // this.pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRound), 3.0f);
        }
    }
    
    public void PowerPelletEaten(PowerPellet pellet) {
        for (int i = 0; i < this.ghosts.Length; i++) {
            this.ghosts[i].frightened.Enable(pellet.duration);
        }

        PelletEaten(pellet);
        CancelInvoke(); // in case you reach another large power pellet, it will be included in multiplied points
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }

    private bool HasRemainingPellets() {
        foreach (Transform pellet in this.pellets) {
            if (pellet.gameObject.activeSelf) {
                // Debug.Log("Remaining pellet found: " + pellet.name);
                return true;
            }
        }

        // Debug.Log("No remaining pellets.");
        // // no more active pellets
        return false;
        // return remainingPellets > 0;
    }

    private void ResetGhostMultiplier() {
        this.ghostMultiplier = 1;
    }
}
