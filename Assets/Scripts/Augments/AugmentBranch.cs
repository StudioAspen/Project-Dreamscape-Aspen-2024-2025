using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AugmentBranch
{
    // branches to differenate what type of augment they are
    // might not use this as there arent enough augments for use
    // only matters when there are a lot and need to group
    NONE,
    MARIO_BRANCH, // mario sound
    PLAYER_AUGMENT, // when augment affects the player
    WEAPON_AUGMENT, // when augment affects the weapon/attacks (could also apply to player)
    COLOR_BRANCH, // colors enemies
};
