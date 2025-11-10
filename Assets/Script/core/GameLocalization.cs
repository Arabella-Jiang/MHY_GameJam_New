using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 游戏内基础本地化工具。
/// 目前主要用于特性（ObjectProperty）的中文显示。
/// </summary>
public static class GameLocalization
{
    private static readonly Dictionary<ObjectProperty, string> PropertyDisplayNames = new Dictionary<ObjectProperty, string>
    {
        { ObjectProperty.None, "无" },
        { ObjectProperty.Hard, "坚硬" },
        { ObjectProperty.Soft, "柔软" },
        { ObjectProperty.Flexible, "柔韧" },
        { ObjectProperty.Flammable, "可燃" },
        { ObjectProperty.Long, "修长" },
        { ObjectProperty.Thin, "细长" },
        { ObjectProperty.Light, "光芒" },
        { ObjectProperty.Heavy, "沉重" },
        { ObjectProperty.Cool, "清凉" },
        { ObjectProperty.Transparent, "透明" },
        { ObjectProperty.Sharp, "锋利" },
    };

    /// <summary>
    /// 获取指定特性的中文显示名称。
    /// 如果未设置映射，则返回枚举名。
    /// </summary>
    public static string GetPropertyDisplayName(ObjectProperty property)
    {
        if (PropertyDisplayNames.TryGetValue(property, out string displayName))
        {
            return displayName;
        }
        return property.ToString();
    }

    /// <summary>
    /// 将多个特性转换成中文、逗号分隔的字符串。
    /// </summary>
    public static string GetPropertyDisplayName(IEnumerable<ObjectProperty> properties, string separator = "、")
    {
        if (properties == null) return string.Empty;
        return string.Join(separator, properties.Select(GetPropertyDisplayName));
    }

    /// <summary>
    /// 生成提示文本中常用的“XX属性”格式。
    /// </summary>
    public static string GetPropertyAttributeName(ObjectProperty property)
    {
        return $"{GetPropertyDisplayName(property)}属性";
    }

    /// <summary>
    /// 生成包含多个属性的“XX属性”文本。
    /// </summary>
    public static string GetPropertyAttributeName(IEnumerable<ObjectProperty> properties, string separator = "、")
    {
        if (properties == null) return string.Empty;
        return string.Join(separator, properties.Select(GetPropertyAttributeName));
    }
}

