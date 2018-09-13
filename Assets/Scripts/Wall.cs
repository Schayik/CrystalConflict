using UnityEngine;

public class Wall : InteractableObject {

    [SerializeField] private Sprite dmgSprite;
    [SerializeField] private int hp = 4;

    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}

    public override void Interact(Player player)
    {
        base.Interact(player);
        spriteRenderer.sprite = dmgSprite;
        hp -= player.wallDamage;
        if (hp <= 0)
            gameObject.SetActive(false);
    }

}
