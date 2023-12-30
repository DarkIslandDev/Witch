using UnityEngine;

public static class EntityAngleRotation
{
    public static Quaternion AngleRotation(Transform entityTransform, float rotateAngle)
    {
        Quaternion anglrot = Quaternion.AngleAxis(rotateAngle, new Vector3(0, rotateAngle, 0));
        entityTransform.rotation = anglrot;
        return anglrot;
    }
}