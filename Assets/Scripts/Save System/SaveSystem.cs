using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Utils;

public static class SaveSystem
{

    public static string SaveLivingEntity(LivingEntity entity)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("/");
        sb.Append("LE");
        sb.Append("/");
        sb.Append(entity.name);


        return sb.ToString();
    }


}
