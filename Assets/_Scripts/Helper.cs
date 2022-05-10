using System;

public class Helper
{
    public static float Sin(float angle)
    {
        return (float)Math.Sin(DegreeToRadian(angle));
    }

    public static float Cos(float angle)
    {
        return (float)Math.Cos(DegreeToRadian(angle));
    }

    //Girilen açıyı radyana çevirir
    public static float DegreeToRadian(float angle)
    {
        return (float)Math.PI * angle / 180f;
    }
    
}