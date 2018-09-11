using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class LayerMaskIndex
{
    public static readonly LayerMask StandardBlock = LayerMask.NameToLayer("Standard Block");
    public static readonly LayerMask Climbable     = LayerMask.NameToLayer("Climbable");
    public static readonly LayerMask Player        = LayerMask.NameToLayer("Player");
    public static readonly LayerMask PlayArea      = LayerMask.NameToLayer("Play Area");
    public static readonly LayerMask Enemy         = LayerMask.NameToLayer("Enemy");
    public static readonly LayerMask Weapon        = LayerMask.NameToLayer("Weapon");
    public static readonly LayerMask WeaponPickup  = LayerMask.NameToLayer("Weapon Pickup");
    public static readonly LayerMask Projectiles   = LayerMask.NameToLayer("Projectiles");
}
