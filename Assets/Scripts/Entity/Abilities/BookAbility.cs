using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BookAbility : Ability
{
    [Header("Book stats")] 
    [SerializeField] protected GameObject bookPrefab;
    [SerializeField] protected LayerMask monsterLayer;
    [SerializeField] protected UpgradeableProjectileCount projectileCount;
    [SerializeField] protected UpgradeableAOE radius;
    [SerializeField] protected UpgradeableDamage damage;
    [SerializeField] protected UpgradeableKnockback knockback;
    [SerializeField] protected UpgradeableRotationSpeed speed;
    [SerializeField] protected UpgradeableWeaponCooldown cooldown;
    [SerializeField] protected UpgradeableDuration duration;

    private List<Book> books;
    protected float timeSinceLastAttack = 0;

    protected override void Use()
    {
        base.Use();
        gameObject.SetActive(true);
        projectileCount.OnChanged?.AddListener(RefreshBooks);
        books = new List<Book>();
        timeSinceLastAttack = cooldown.Value;

        for (int i = 0; i < projectileCount.Value; i++)
        {
            AddBook();
        }
    }

    protected override void Upgrade()
    {
        base.Upgrade();
        RefreshBooks();
        
        timeSinceLastAttack = cooldown.Value;
    }

    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;

        if (timeSinceLastAttack >= cooldown.Value)
        {
            StartCoroutine(RotateBooks());
        }

    }

    private IEnumerator RotateBooks()
    {
        
        if (books.Count != 0)
        {
            HideOrShowBooks(true);
            
            for (int i = 0; i < books.Count; i++)
            {
                float theta = (2 * Mathf.PI * i) / books.Count;
                books[i].transform.localPosition = new Vector3(Mathf.Sin(theta + Time.time * speed.Value),
                                                               Mathf.Cos(theta + Time.time * speed.Value),
                                                               0);
            }
        }

        yield return new WaitForSeconds(duration.Value);
        
        timeSinceLastAttack = Mathf.Repeat(timeSinceLastAttack, cooldown.Value);

        HideOrShowBooks(false);
    }

    private void HideOrShowBooks(bool enable)
    {
        foreach (Book book in books)
        {
            book.gameObject.SetActive(enable);
        }
    }

    public void Damage(IDamageable damageable)
    {
        Vector2 knockbackDirection = (damageable.transform.position - player.CenterTransform.position).normalized;
        damageable.TakeDamage((int)damage.Value, knockback.Value * knockbackDirection);
        player.OnDealDamage?.Invoke(damage.Value);
    }

    private void RefreshBooks()
    {
        for (int i = books.Count; i < projectileCount.Value; i++)
        {
            AddBook();
        }
    }

    private void AddBook()
    {
        Book book = Instantiate(bookPrefab, player.CenterTransform).GetComponent<Book>();
        book.Init(this, monsterLayer);
        books.Add(book);
    }
}