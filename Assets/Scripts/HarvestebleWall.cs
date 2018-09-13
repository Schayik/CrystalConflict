using UnityEngine;

public class HarvestebleWall : Wall {

    [SerializeField] private int pointsPerHit = 5;

    public override void Interact(Player player)
    {
        base.Interact(player);
        player.IncreaseScore(pointsPerHit);
    }

}

