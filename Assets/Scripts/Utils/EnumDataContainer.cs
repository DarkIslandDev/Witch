﻿using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnumDataContainer<TEnum, TValue> where TEnum : Enum
{
    [SerializeField] private TValue[] content = null;
    [SerializeField] private TEnum enumType;
    private Dictionary<TEnum, int> valueByEnum;

    public EnumDataContainer()
    {
        TEnum[] enums = (TEnum[])Enum.GetValues(typeof(TEnum));
        content = new TValue[enums.Length];
        valueByEnum = new Dictionary<TEnum, int>();

        for (int i = 0; i < enums.Length; i++)
        {
            valueByEnum[enums[i]] = i;
        }
    }

    public TValue this[int i] => content[i];

    public int Length => content.Length;

    public TValue this[TEnum enumType] => content[valueByEnum[enumType]];
}

[Serializable]
public class EnumDataContainer<TEnum, TValue1, TValue2> where TEnum : Enum
{
    [SerializeField] private TValue1[] content1 = null;
    [SerializeField] private TValue2[] content2 = null;
    [SerializeField] private TEnum enumType;
    private Dictionary<TEnum, int> valueByEnum;

    public EnumDataContainer()
    {
        TEnum[] enums = (TEnum[])Enum.GetValues(typeof(TEnum));
        content1 = new TValue1[enums.Length];
        content2 = new TValue2[enums.Length];
        valueByEnum = new Dictionary<TEnum, int>();
        for (int i = 0; i < enums.Length; i++)
        {
            valueByEnum[enums[i]] = i;
        }
    }

    public (TValue1, TValue2) this[int i] => (content1[i], content2[i]);

    public int Length => content1.Length;

    public (TValue1, TValue2) this[TEnum enumType] => (content1[valueByEnum[enumType]], content2[valueByEnum[enumType]]);
}