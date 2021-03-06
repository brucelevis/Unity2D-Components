using UnityEngine;
using UnityEngine.Assertions;

public class InventoryManager : BaseBehaviour
{
	private Weapon equippedWeapon;
	private Weapon leftWeapon;
	private Weapon rightWeapon;
	private int left = 0;
	private int equipped = 1;
	private int right = 2;
	private bool levelLoading;
	private GameObject[] weaponBelt;
	private GameObject outgoingWeapon;
	private new Transform transform;

	void Awake()
	{
		transform = GetComponent<Transform>();
		Assert.IsNotNull(transform);
	}

	// input method used by touch controls.
	public void SwitchWeaponOnTouch()
	{
		OnSwitchWeapon(RIGHT);
	}

	void OnInitWeapons(GameObject eWeapon, GameObject lWeapon, GameObject rWeapon)
	{
		// WEAPON GAMEOBJECTS — keep track of weapon GameObjects as they're equipped/stashed
		if (weaponBelt == null) {
			weaponBelt = new GameObject[3];
		}

		weaponBelt[left] = lWeapon;
		weaponBelt[equipped] = eWeapon;
		weaponBelt[right] = rWeapon;

		InitNewEquippedWeapon(eWeapon);
		PassInitialWeaponsToHUD();
		PassEquippedWeaponToWeaponManager();
	}

	void OnEquipNewWeapon(GameObject newWeapon)
	{
		DiscardWeapon();
		InitNewEquippedWeapon(newWeapon);
	}

	void DiscardWeapon()
	{
		outgoingWeapon = weaponBelt[equipped];

		var outgoingTransform = outgoingWeapon.GetComponent<Transform>();
		var outgoingSpriteRenderer = outgoingWeapon.GetComponent<SpriteRenderer>();
		var outgoingRigidbody2D = outgoingWeapon.GetComponent<Rigidbody2D>();

		outgoingTransform.SetLocalPositionXY(0f, .5f);
		outgoingTransform.SetAbsLocalScaleX(1f);
		outgoingTransform.parent = null;
		outgoingSpriteRenderer.enabled = true;
		outgoingSpriteRenderer.sortingLayerName = PICKUP_SORTING_LAYER;
		outgoingRigidbody2D.isKinematic = false;
		outgoingWeapon.layer = PICKUP_PHYSICS_LAYER;
		outgoingWeapon.GetComponentInChildren<PhysicsCollider>().EnablePhysicsCollider();
		outgoingWeapon.GetComponentInChildren<MeleeCollider>().DisableMeleeCollider();
		equippedWeapon.inPlayerInventory = false;
		equippedWeapon.inEnemyInventory = false;
		TossOutgoingWeapon(outgoingRigidbody2D);
		Invoke("EnablePickupCollider", 3f);
	}

	void InitNewEquippedWeapon(GameObject newWeapon)
	{
		weaponBelt[equipped] = newWeapon;

		var equippedTransform = weaponBelt[equipped].GetComponent<Transform>();

		equippedTransform.parent = gameObject.transform;
		equippedTransform.localPosition = new Vector3(.625f, .5625f, 0f);
		equippedTransform.SetLocalScaleXYZ(-1f, 1f, 1f);
		weaponBelt[equipped].layer = PLAYER_DEFAULT_LAYER;
		weaponBelt[equipped].GetComponentInChildren<PhysicsCollider>().DisablePhysicsCollider();
		weaponBelt[equipped].GetComponentInChildren<MeleeCollider>().EnableMeleeCollider();
		weaponBelt[equipped].GetComponentInChildren<WeaponPickupCollider>().DisableWeaponPickupCollider();
		weaponBelt[equipped].GetComponentInChildren<Rigidbody2D>().isKinematic = true;
		weaponBelt[equipped].GetComponent<Weapon>().inPlayerInventory = true;
		weaponBelt[equipped].GetComponent<Weapon>().inEnemyInventory = false;
		CacheAndSetupWeapons();
		PassEquippedWeaponToHUD();
		PassEquippedWeaponToWeaponManager();
	}

	void EnablePickupCollider()
	{
		//turn dropped weapon's pickup collider back on after a short delay
		outgoingWeapon.GetComponentInChildren<WeaponPickupCollider>().EnableWeaponPickupCollider();
	}

	void TossOutgoingWeapon(Rigidbody2D rb)
	{
		if (transform.lossyScale.x > 0f) {
			rb.AddForce(new Vector3(-5, 5, 1), ForceMode2D.Impulse);
		}
		else {
			rb.AddForce(new Vector3(5, 5, 1), ForceMode2D.Impulse);
		}

		outgoingWeapon.GetComponent<Weapon>().OnDiscard();
	}

	void OnSwitchWeapon(int shiftDirection)
	{
		if (!levelLoading) {
			switch (equipped) {
				case 0:
					left = 1;
					equipped = 2;
					right = 0;
					break;
				case 1:
					left = 2;
					equipped = 0;
					right = 1;
					break;
				case 2:
					left = 0;
					equipped = 1;
					right = 2;
					break;
			}

			CacheAndSetupWeapons();
			PassNewWeaponsToHUD();
			PassEquippedWeaponToWeaponManager();
		}
	}

	void CacheAndSetupWeapons()
	{
		// WEAPON GAMEOBJECT'S 'WEAPON' COMPONENT
		// ~~~~~~~~~~~~~~~~~~~~~~~~
		// cache specific weapons (Sword, Hammer, etc) via parent class 'Weapon'
		// use to call currently equipped weapon animations
		leftWeapon = weaponBelt[left].GetComponent<Weapon>();
		equippedWeapon = weaponBelt[equipped].GetComponent<Weapon>();
		rightWeapon = weaponBelt[right].GetComponent<Weapon>();

		// set correct names for weapon gameObjects
		weaponBelt[left].name = "Left";
		weaponBelt[equipped].name = "Equipped";
		weaponBelt[right].name = "Right";

		// set weapons to correct layers
		weaponBelt[left].layer = PLAYER_DEFAULT_LAYER;
		weaponBelt[equipped].layer = PLAYER_DEFAULT_LAYER;
		weaponBelt[right].layer = PLAYER_DEFAULT_LAYER;

		// set weapons to correct sorting layers;
		weaponBelt[left].GetComponent<SpriteRenderer>().sortingLayerName = HERO_WEAPON_SORTING_LAYER;
		weaponBelt[equipped].GetComponent<SpriteRenderer>().sortingLayerName = HERO_WEAPON_SORTING_LAYER;
		weaponBelt[right].GetComponent<SpriteRenderer>().sortingLayerName = HERO_WEAPON_SORTING_LAYER;

		// set weapon colliders to correct layers
		SetWeaponColliders();

		// set pickup colliders to correct layers
		weaponBelt[left].transform.Find("PickupCollider").gameObject.layer = PICKUP_LAYER;
		weaponBelt[equipped].transform.Find("PickupCollider").gameObject.layer = PICKUP_LAYER;
		weaponBelt[right].transform.Find("PickupCollider").gameObject.layer = PICKUP_LAYER;

		// disable SpriteRenderer for all weapon icons
		leftWeapon.spriteRenderer.enabled = false;
		equippedWeapon.spriteRenderer.enabled = false;
		rightWeapon.spriteRenderer.enabled = false;

		// disable colliders for stashed weapons
		leftWeapon.GetComponentInChildren<MeleeCollider>().DisableMeleeCollider();
		equippedWeapon.GetComponentInChildren<MeleeCollider>().EnableMeleeCollider();
		rightWeapon.GetComponentInChildren<MeleeCollider>().DisableMeleeCollider();

		// fade in newly equipped weapon
		equippedWeapon.GetComponent<Weapon>().OnEquip();

		// fade out newly stashed weapons
		leftWeapon.GetComponent<Weapon>().OnStashed();
		rightWeapon.GetComponent<Weapon>().OnStashed();

		SendMessageUpwards("NewWeaponEquipped", equippedWeapon.type);
	}

	void SetWeaponColliders()
	{
		if (leftWeapon.style == Weapon.Style.Melee) {
			weaponBelt[left].transform.Find("MeleeCollider").gameObject.layer = PLAYER_WEAPON_COLLIDER;
		}
		else {
			weaponBelt[left].transform.Find("MeleeCollider").gameObject.layer = NO_COLLISION_LAYER;
		}

		if (equippedWeapon.style == Weapon.Style.Melee) {
			weaponBelt[equipped].transform.Find("MeleeCollider").gameObject.layer = PLAYER_WEAPON_COLLIDER;
		}
		else {
			weaponBelt[equipped].transform.Find("MeleeCollider").gameObject.layer = NO_COLLISION_LAYER;
		}

		if (rightWeapon.style == Weapon.Style.Melee) {
			weaponBelt[right].transform.Find("MeleeCollider").gameObject.layer = PLAYER_WEAPON_COLLIDER;
		}
		else {
			weaponBelt[right].transform.Find("MeleeCollider").gameObject.layer = NO_COLLISION_LAYER;
		}
	}

	void PassInitialWeaponsToHUD()
	{
		EventKit.Broadcast("init stashed weapon", weaponBelt[left], LEFT);
		EventKit.Broadcast("init equipped weapon", weaponBelt[equipped]);
		EventKit.Broadcast("init stashed weapon", weaponBelt[right], RIGHT);
	}

	void PassNewWeaponsToHUD()
	{
		EventKit.Broadcast("change stashed weapon", weaponBelt[left], LEFT);
		EventKit.Broadcast("change equipped weapon", weaponBelt[equipped]);
		EventKit.Broadcast("change stashed weapon", weaponBelt[right], RIGHT);
	}

	void PassEquippedWeaponToHUD()
	{
		EventKit.Broadcast("init new equipped weapon", weaponBelt[equipped]);
	}

	void PassEquippedWeaponToWeaponManager()
	{
		EventKit.Broadcast("new equipped weapon", equippedWeapon);
	}

	void OnLevelLoading(bool status)
	{
		// pause weapon changes while level loading
		levelLoading = true;

		StartCoroutine(Timer.Start(WEAPON_PAUSE_ON_LEVEL_LOAD, false, () => {
			levelLoading = false;
		}));
	}

	void OnEnable()
	{
		EventKit.Subscribe<GameObject, GameObject, GameObject>("init weapons", OnInitWeapons);
		EventKit.Subscribe<GameObject>("equip new weapon", OnEquipNewWeapon);
		EventKit.Subscribe<int>("switch weapon", OnSwitchWeapon);
		EventKit.Subscribe<bool>("level loading", OnLevelLoading);
	}

	void OnDestroy()
	{
		EventKit.Unsubscribe<GameObject, GameObject, GameObject>("init weapons", OnInitWeapons);
		EventKit.Unsubscribe<GameObject>("equip new weapon", OnEquipNewWeapon);
		EventKit.Unsubscribe<int>("switch weapon", OnSwitchWeapon);
		EventKit.Unsubscribe<bool>("level loading", OnLevelLoading);
	}
}
