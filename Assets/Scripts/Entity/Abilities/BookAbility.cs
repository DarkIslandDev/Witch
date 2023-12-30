using System;
using System.Collections.Generic;
using UnityEngine;

public class BookAbility : Ability
{
    [Header("Book stats")] 
    [SerializeField] protected GameObject bookPrefab;
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected UpgradeableProjectileCount projectileCount;
    // [SerializeField] protected UpgradeableAOE radius;
    [SerializeField] protected UpgradeableDamage damage;
    [SerializeField] protected UpgradeableKnockback knockback;
    [SerializeField] protected UpgradeableRotationSpeed speed;

    private List<Book> books;

    protected override void Use()
    {
        base.Use();
        gameObject.SetActive(true);
        projectileCount.OnChanged?.AddListener(RefreshBooks);
        books = new List<Book>();

        for (int i = 0; i < projectileCount.Value; i++)
        {
            AddBook();
        }
    }

    protected override void Upgrade()
    {
        base.Upgrade();
        RefreshBooks();
    }

    private void Update()
    {
        if (books.Count != 0)
        {
            for (int i = 0; i < books.Count; i++)
            {
                float theta = (2 * Mathf.PI * i) / books.Count;
                books[i].transform.localPosition = new Vector3(Mathf.Sin(theta + Time.time * speed.Value),
                    Mathf.Cos(theta + Time.time * speed.Value), 
                    0);
            }
        }
    }

    public void Damage(IDamageable damageable)
    {
        Vector2 knockbackDirection = (damageable.transform.position - player.transform.position).normalized;
        damageable.TakeDamage(damage.Value, knockback.Value * knockbackDirection);
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
        Book book = Instantiate(bookPrefab, player.transform).GetComponent<Book>();
        book.Init(this, layerMask);
        books.Add(book);
    }
}