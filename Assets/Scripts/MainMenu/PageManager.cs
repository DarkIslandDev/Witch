using System;
using UnityEngine;

public class PageManager : MonoBehaviour
{
    [SerializeField] private int defaultPage;
    [SerializeField] private GameObject[] pages;

    private void Awake()
    {
        SwitchPages(defaultPage);
    }

    public void SwitchPages(int pageIndex)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == pageIndex);
        }
    }
}