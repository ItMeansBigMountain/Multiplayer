# Project Overview

This project includes a multiplayer arena shooter built in Unity using Photon Pun 2 for networked gameplay. The following scripts are integral to character control, player spawning, shooting mechanics, and map generation, ensuring that all players have synchronized experiences across clients.

## Project Structure

- `ArenaShooter_PlayerControls`: Handles player control input, shooting mechanics, and aiming.
- `BasicRigidBodyPush`: Adds a force to rigid bodies when the player collides with them, allowing interactive pushing.
- `BulletProjectile` & `network_bullet_projectile`: Manages bullet behavior, handling instantiation, movement, and destruction.
- `LoginManager`: Manages player login, connecting to Photon, and joining a lobby or creating a room.
- `MapGenerator`: Dynamically generates the map for the arena, including obstacles, boundaries, and walls.
- `PlayerSpawner`: Responsible for spawning players into the arena, selecting a character prefab randomly from a pool.
- `ThirdPersonController`: Manages character movement, grounded checks, and jump mechanics, with Photon support for synchronization.

---

## Table of Scripts

| Script                     | Purpose | Key Components |
|----------------------------|---------|----------------|
| **ArenaShooter_PlayerControls** | Manages player controls, aiming, and shooting. Integrates with `PhotonView` to sync aiming and shooting only for the local player. | `PhotonView`, `LineRenderer` (for laser aiming), `Cinemachine` virtual camera |
| **BasicRigidBodyPush** | Adds force to rigid bodies upon collision if they are within the designated layers. | `LayerMask`, `ControllerColliderHit` |
| **BulletProjectile** | Handles bullet movement and destruction on impact. `network_bullet_projectile` syncs bullet instantiation and destruction across clients. | `PhotonNetwork.Instantiate`, `PhotonNetwork.Destroy` |
| **LoginManager** | Manages player login, connects to Photon server, and joins or creates rooms in the lobby. | `PhotonNetwork.JoinLobby`, `PhotonNetwork.JoinRandomRoom` |
| **MapGenerator** | Generates obstacles and boundaries in the arena; only the master client performs map generation to avoid duplication. | `LayerMask`, `PhotonNetwork.IsMasterClient` check |
| **PlayerSpawner** | Spawns players in random positions around the map using random character prefabs with the "Multiplayer-Character-" prefix. | `PhotonNetwork.Instantiate`, Random character selection |
| **ThirdPersonController** | Controls character movement, jumping, and grounded state. Synchronizes movement and animations across clients with Photon. | `CharacterController`, `PhotonAnimatorView`, `PhotonView` |

---

### Additional Notes

- **PhotonView**: Used across all player-related scripts to check `PhotonView.IsMine` before applying movement, aiming, and other actions, ensuring actions are applied only to the local player.
- **Synchronizing Parameters**: `ThirdPersonController` uses `PhotonAnimatorView` to synchronize animation parameters (such as speed) across clients.
- **Random Character Selection**: `PlayerSpawner` loads prefabs dynamically based on a naming convention, selecting one randomly for each player instance.

This project is structured to be modular and optimized for a multiplayer shooter, with key Photon integrations for real-time synchronization and modular design for easy asset swapping.