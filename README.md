# Pixel Tower Defense Game
This is a 2D classic tower defense game inspired from PVZ but with a pixel art style, built using the Unity engine. Players must use strategy to place various types of defensive towers to stop waves of increasingly difficult enemies. The game features multiple levels with increasing difficulty, a variety of towers and enemies, and a powerful special ability to turn the tide of battle.

## Key Features

-   **Diverse Tower System:**
    -   **Cannon Tower:** Fires projectiles with standard damage.
    -   **Ballista Tower:** Shoots arrows that can pierce through multiple enemies.
    -   **Wall:** A purely defensive structure with no attack, used to block and redirect enemies.

-   **Enemy Variations:**
    -   An automatic stat-scaling system that increases enemy health and speed with each new level.
    -   The ability to configure different enemy types (e.g., Fast, Tank) for each level.

-   **Level Progression:**
    -   A multi-level structure (Level 1, 2, 3, etc.) with an increasing number of enemies and faster spawn rates.
    -   A win condition based on defeating a specific number of enemies.

-   **"Nuclear Bomb" Special Ability:**
    -   A special button that can be activated for a cost of 100 gold.
    -   The button is automatically disabled if the player has insufficient gold.
    -   When activated, the bomb annihilates all enemies on screen, accompanied by a visual explosion animation, sound effects, and a camera shake.

-   **Economy and Health System:**
    -   A currency system (gold) for purchasing towers.
    -   Enemies drop gold upon defeat.
    -   A core health system that is damaged when enemies reach the end of the path.

-   **Spritesheet-Based Animation:**
    -   Smooth animations for towers, enemies (walk, attack, die), and special effects (explosions, on-hit) using spritesheets and Unity's Animator Controller.
    -   Use of Animation Events for precise synchronization between visuals and game logic.

## How to Play

1.  Start the game from the "Menu" scene or directly from "Level1".
2.  Use the panel at the bottom of the screen to select a tower or wall to build.
3.  Click on a valid tile in the arena to place the tower. Gold will be deducted according to its cost.
4.  Stop all enemies before they reach the end of the path.
5.  In case of an emergency, use the Nuclear Bomb button if you have enough gold.
6.  Defeat the required number of enemies to win the level and proceed to the next.

## Possible Future Development

This project has a strong foundation for further development, such as:

-   Adding more tower and enemy types with unique abilities (e.g., slowing towers, flying enemies).
-   A "Boss Battle" system at the end of every few levels.
-   An upgrade system for each tower.
-   A skill tree for the player.
-   Saving and loading game progress.
