# Vengeance-is-Best-Served-Cold
Selected scripts from a solo Unity production — stealth AI, isometric camera rotation, collision-based interaction systems, and a custom Shader Graph vignette built for runtime performance. Context for each script is below.#

---

Axe.cs

Manages the enemy's damage output against the player. On trigger contact with the player's hitbox, it decrements a three-life system by deactivating UI life icons sequentially. At zero lives it triggers a game over state — disabling the character, activating the game over screen, and pushing a float value directly into the vignette shader material to visually close the screen. The shader communication here is intentional: rather than running a separate animation or transition system, the game over feedback is handled entirely through the same custom vignette material used for gameplay, keeping the visual language consistent.

---

Bombe.cs

Handles the targeting and throwing logic for the potion projectile. When the player activates the targeting mode, a aim marker (Mira) becomes moveable in the scene while the player's own CharacterController is temporarily disabled to prevent conflicting inputs. On throw, a parabolic velocity is calculated from scratch using kinematic equations — accounting for displacement across all three axes and compensating for gravity on the Y axis — rather than relying on Unity's built-in physics to approximate the arc. This gives precise, predictable projectile behaviour regardless of distance. Position resets are handled via a static event subscription to BombeKontakt, decoupling the throw logic from the collision result.

---

BombeKontakt.cs

Collision detection for the potion projectile, designed around a static C# event (OnPotionHit) to communicate results without direct object references. When the potion hits an enemy it sets a FoeDestruction flag and fires the event; terrain hits fire the event without setting the flag. This separation means the throwing system (Bombe.cs) and the destruction system (Potion.cs) can both respond to the same collision independently, keeping the scripts loosely coupled.

---

Potion.cs

Coordinates the enemy destruction sequence triggered by a successful potion hit. Monitors the FoeDestruction flag from BombeKontakt each frame and, when conditions are met, runs a coroutine that positions an explosion particle system on the enemy's transform, deactivates the enemy, holds the explosion for a timed duration, then cleans up all charge state. The charge UI objects and the TouchOfDeath charge counter are reset here rather than in the individual scripts, centralising the end-of-sequence cleanup in one place.

---

TouchOfDeath.cs

Implements the freeze-and-charge mechanic that forms the core stealth interaction loop. On proximity trigger with player input, it increments a charge counter and activates corresponding UI indicators. Each charge freezes the enemy by disabling its Animator, applying RigidbodyConstraints.FreezeAll, and temporarily overriding the material's base colour to cyan as a visual signal — all reversed after a serialized wait time via coroutine. At three charges the potion throw is unlocked. The material is instantiated on Start to avoid modifying the shared asset, ensuring colour changes are local to this enemy instance.

