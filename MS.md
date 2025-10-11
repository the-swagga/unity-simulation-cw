# Marker help doc - Where to find marking criteria

## Physics
* 1a) Usage of Rigid Bodies Used in Unity - Player, enemies, and projectiles all have rigid bodies / Found in player prefab, cannonball prefab
* 1b) Correct application of impulses to bodies Used in C# - Impulses are applied for player, enemy, and projectile movement / Found in PlayerController.cs, PlayerCannon.cs
* 1c) Objects have appropriate mass quantities - All rigidbody objects have a mass value consistent with their theme/identity
* 1d) Game mechanics is physics-driven - Movement and combat are driven by physics
* 2a) Physics properties are changed via scripts - ?
* 2b) Mass/Physics is a gameplay mechanic - X Combat damage is purely force based rather than pre-set
* 2c) Additional forces beyond simple motion-driven accelerations are provided - Projectile trajectories are calculated using suvat equations and shown to the player / Found in PlayerCannon.cs
* 2d) AI uses calculations to determine projectile forces - Enemies calculate the firing force and angle to hit the player based on the player's position
* 3a/b/c) There is more than one collision volume, The collision volume is appropriate and matches the Game Object’s mesh - Player has a capsule collider, cannonballs have sphere colliders
* 4a) A single Game Object has multiple colliders - X Enemies have a collider for their body and a collider for their heads (for headshot damage)
* 4b) Colliders are enabled/disabled via scripts - ?
* 4c) Trigger volumes are used as part of player mechanics - Powerup pick-up logic is based on OnTriggerEnter / Found in Powerup.cs
* 5a) Rigidbody responds to collisions realistically - ?
* 5b) An object makes use of OnCollisionEnter/Exit - X Projectiles use Enter/Exit/Stay to deal instant damage on impact, extra damage as long as the contact lasts, and damage over time on exit / Found in X
* 5c) Collision Layers are used to separate out collision types - X Customised collision matrix in place, custom layers created
* 5d/6a) Multiple physics materials appearing in the game - X Physic material used on player to simulate slippery (low friction) ground in the icy terrain tile, also used to slow player down in water / Found in X
* 6b) Physics materials are changed at run-time via script - X They are activated and deactivated when the player is in the icy terrain tile or in water / Found in X
* 6C) Trigger volumes are used to trigger gameplay events - X Enter spawns enemies when the player enters a tile, exit despawns enemies when the playher exits a tile, and stay spawns enemies every t seconds while the player is in the tile / Found in

## Graphics
* 7a) Multiple textures appearing in game - ?
* 7b) Appropriate use of lighting - ?
* 7c) GameObjects moving & rotating via script - Powerups rotate and move up/down using a script / Found in PowerupIdleMovement.cs
* 7d) A navigable camera moving in 3D - Player has a third-person camera implemented using Cinemachine and a C# script / Found in ThirdPersonCamera.cs
* 8b) Environment appears to extend infinitely - Environment is generated in tiles according to the player's view distance, tiles are deactivated when out of the view distance / Found in TileManager.cs, BaseTile.cs (and its derived classes)
* 8c) A body of realistic looking water - Water shader used to make realistic looking water, has a chance to spawn in grassland tile / Found in Assets/Graphics/Shaders/Water
* 8d) Scripted lighting/effects (e.g. weather, day/night cycle) - X Day/night cycle implemented / Found in X
* 8e) Change object appearance via script - X Player and enemies have a red damage effect applied when hit with a projectile / Found in X
* 8f) Geometry that changes over time (e.g. plants growing) - ?

## Pathfinding
* 9a) NavMeshAgents are used - X Enemies have a navmeshagent component for pathfinding / Found in Assets/Prefabs/Entities
* 9b) NavMeshObstacles are used - X Environment objects have a navmeshobstacle component / Found in Assets/Prefabs/Map
* 9c) Custom pathfinding code, or external library with some modifications - ?
* 9d) AI makes decisions based on pathfinding - ?

## Artificial Intelligence
* 10.) Usage of probabilistic/stochastic state transitions - ?
* 11.)
    * Usage of Planning techniques - ?
    * Usage of Non-Cooperative Game Strategies (Min-Max trees, 𝛼-𝛽 pruning) - ?
    * Usage of basic Reinforcement Learning techniques - ?
* 12.) An orchestrator or game manager is used to handle multiple and contrasting behaviours,
thus including the generation of random events - ?

## Advanced Features
* A1: Appropriate usage of Prefabs - Prefabs appear in scenes, prefabs are within the project, instantiate is called in many scripts to generate/spawn objects / Found in Assets/Prefabs, PlayerCannon.cs, TileManager.cs, BaseTile.cs (and its derived classes)
* A2: Levels/Menus are separated into distinct scenes - X Main menu, intro, and main game all implemented / Found in X
* A3: Evidence of code for limiting expensive computations (e.g., raycasting) - Object pooling used for projectiles and particles, raycast used by player to check if grounded, raycast used by enemies to get distance to player for trajectory calculations / Found in PlayerCannon.cs, PlayerController.cs, X
* A4: Flocking techniques - X Enemies move in groups, without colliding with eachother or firing at eachother / Found in X
* A5: Usage of Vector Fields - X Vector fields used for particle movement ?????
* A6: Usage of Particle Systems - Firing projectiles from weapons causes particle effects / Found in X
* A7: Implementation of custom AI tools (ExpectedMinMax, et ceteram) - ?
