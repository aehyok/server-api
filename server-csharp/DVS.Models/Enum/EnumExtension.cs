using System;
using System.ComponentModel;
using System.Linq;

public static class EnumExtension
{
    /// <summary>
    /// 获取枚举值对应的名称
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static string GetDescription(this Enum val)
    {
        var type = val.GetType();

        var memberInfo = type.GetMember(val.ToString());

        var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attributes == null || attributes.Length != 1)
        {
            //如果没有定义描述，就把当前枚举值的对应名称返回
            return val.ToString();
        }

        return (attributes.Single() as DescriptionAttribute).Description;
    }
}