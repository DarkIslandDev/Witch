public class DescriptionUtils
{
    public static string GetUpgradeDescription(string upgradeName, float upgrade)
    {
        string plus = upgrade > 0 ? "+" : "";
        return $"{LocalizationManager.GetTranslate(upgradeName)}: {plus} {upgrade * 100} ";
    }

    public static string GetUpgradeDescription(string upgradeName, int upgrade)
    {
        string plus = upgrade > 0 ? "+" : "";
        return $"{LocalizationManager.GetTranslate(upgradeName)}: {plus} {upgrade} ";
    }
}