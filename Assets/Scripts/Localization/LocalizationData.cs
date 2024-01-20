using System;

[Serializable]
public class LocalizationValue
{
    public string lang;
    public string value;
}

[Serializable]
public class LocalizationMapping
{
    public string key;
    public LocalizationValue[] values;
}

public class LocalizationData
{
    public string[] languages;
    public LocalizationMapping[] table;
}