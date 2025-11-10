using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 集中管理所有需要显示给玩家的文字提示。
/// 方便 UI 系统调用，同时也可以用于生成 TextMeshPro 字体所需的字符列表。
/// </summary>
public static class GameMessageCatalog
{
    /// <summary>
    /// 单条提示信息的数据。
    /// </summary>
    public readonly struct MessageEntry
    {
        public readonly string Level;
        public readonly string Trigger;
        public readonly string Text;

        public MessageEntry(string level, string trigger, string text)
        {
            Level = level;
            Trigger = trigger;
            Text = text;
        }
    }

    /// <summary>
    /// 固定提示列表（不包含动态组合）。
    /// </summary>
    private static readonly MessageEntry[] BaseMessages =
    {
        // Level 1 - 教程提示
        new MessageEntry("Level1", "夜幕将临", "夜晚快到了，必须有光才能继续前进。"),
        new MessageEntry("Level1", "石碑说明", "石碑上的文字是沟通神意与自然的通道，试试将文字所需的能量注入其中，或许会获取意想不到的力量。"),
        new MessageEntry("Level1", "石碑指引", "石碑需要两个组件：火、人。请继续探索吧。"),

        //石头
        new MessageEntry("Level1", "靠近石头", "试试长按e获得石头的特性吧"),
        new MessageEntry("Level1", "石头特性获取成功", $"{GameLocalization.GetPropertyDisplayName(ObjectProperty.Hard)}已放入背包1号位"),
        new MessageEntry("Level1", "获得Hard后", $"已获得 {GameLocalization.GetPropertyDisplayName(ObjectProperty.Hard)}。尝试对水面使用特性，穿过河流吧"),

        //水面
        new MessageEntry("Level1", "水面空气墙阻挡", "水流太急，太危险了，试试其他办法吧。"),
        new MessageEntry("Level1", "水面硬化成功", "水面已硬化，可以通过啦。"),

        //树枝
        new MessageEntry("Level1", "靠近树枝", "树枝现在太软啦，不能被点燃，试着对树枝使用特性吧"),
        new MessageEntry("Level1", "树枝硬化成功", $"树枝变得{GameLocalization.GetPropertyDisplayName(ObjectProperty.Hard)}且可燃！"),
        new MessageEntry("Level1", "树枝摩擦失败", "需要两根树枝都变得坚硬后，再尝试摩擦点火"),
        new MessageEntry("Level1", "两根树枝已硬化", "两根树枝已变硬。试试看对着任意树枝按Q进行摩擦点火吧"),
        new MessageEntry("Level1", "细树枝已点燃说明", "细树枝已点燃。"),

        new MessageEntry("Level1", "树枝未点燃充能失败", "树枝还没有点燃，无法充能"),
        
        //石碑
        new MessageEntry("Level1", "充能提示", "需要手持点燃树枝才能充能石碑。"),
        new MessageEntry("Level1", "‘火’文字重复充能", "‘火’文字已经点亮过了"),
        new MessageEntry("Level1", "‘火’文字点亮", "‘火’已点亮！"),
        new MessageEntry("Level1", "‘人’文字重复充能", "‘人’已经点亮过了"),
        new MessageEntry("Level1", "‘人’文字点亮", "‘人’已点亮！"),
        new MessageEntry("Level1", "所有文字点亮", "石碑两个文字部分都已点亮！恭喜你获得了光！"),
        new MessageEntry("Level1", "结语", "人本就是大地之子，与世界共鸣也是世界的一部分。"),
        new MessageEntry("Level1", "跳转下一关", "正在跳转到Level2..."),

        // Level 2 - 开场与流程提示

        //开始
        new MessageEntry("Level2", "开场石碑信息", "石碑需要三个组件：木、羽、日。你已有跟随光源，继续收集其他组件吧。"),
        new MessageEntry("Level2", "开场日组建说明", "你已拥有跟随光源（日组件）"),

        //老藤
        new MessageEntry("Level2", "第一次注视老藤时的提示1", "老藤有多种特性：长、柔韧、细。长按E理解特性，然后按数字键1/2/3选择要理解的特性。"),
        new MessageEntry("Level2", "对着老藤按e成功后", "选择要理解的特性： 1. 细 2. 长 3. 柔韧"),
        
        new MessageEntry("Level2", "获得羽毛后", "已获得羽毛。接下来需要获取木组件。"),
        new MessageEntry("Level2", "获得木组件但没有日组件", "已获得木组件。你需要先获得光能量（日组件）。"),
        new MessageEntry("Level2", "获得木组件且已有日组件", "已获得木组件。手持组件前往石碑充能（短按E）。"),
        new MessageEntry("Level2", "获得日组件后", "已拥有光能量（日组件）。前往石碑充能（使用特性或手持物品）。"),
        new MessageEntry("Level2", "三组件全部充能完成", "石碑被点亮！春的意义，即是唤醒世界上一切律动的能力。"),
        new MessageEntry("Level2", "结语", "春的意义，即是唤醒世界上一切律动的能力。"),
        new MessageEntry("Level2", "拾取羽毛时", "获得了羽毛！"),
        new MessageEntry("Level2", "拾取木组件时", "获得了木组件！"),
        new MessageEntry("Level2", "拾取日组件时", "获得了日组件！"),
        new MessageEntry("Level2", "羽毛重复充能", "羽毛已经充能过了"),
        new MessageEntry("Level2", "羽毛充能成功", "✅ 羽毛已充能到石碑！"),
        new MessageEntry("Level2", "木组件重复充能", "木组件已经充能过了"),
        new MessageEntry("Level2", "木组件充能成功", "✅ 木组件已充能到石碑！"),
        new MessageEntry("Level2", "日组件重复充能", "日组件已经充能过了"),
        new MessageEntry("Level2", "缺少日组件尝试充能", "你还没有光能量（日组件）"),
        new MessageEntry("Level2", "日组件充能成功", "✅ 日组件已充能到石碑！"),
        new MessageEntry("Level2", "尝试充能但手持物品错误", "当前手持的物品不是需要的组件，或已经充能过了"),
        new MessageEntry("Level2", "缺少手持组件时提示", "需要手持组件（羽毛或木）才能充能石碑，或拥有日组件（空手按Q键）"),
        new MessageEntry("Level2", "结语", "春的意义，即是唤醒世界上一切律动的能力。"),

        // Level 3 - 开场与流程提示
        new MessageEntry("Level3", "开场：故事引导", "星光照亮寻找神灵的旅途，连世界也会为勇敢者歌唱赞歌"),
        new MessageEntry("Level3", "开场：说明目标组件", "石碑需要两个组件：生、星点。继续探索吧。"),
        new MessageEntry("Level3", "获得生组件后", "已获得\"生\"组件。接下来需要获取\"星点\"组件。"),
        new MessageEntry("Level3", "获得星点组件后", "已获得\"星点\"组件。前往石碑充能（短按E）。"),
        new MessageEntry("Level3", "充能完成", "石碑被点亮！星星,照应我们所处的位置在宇宙的何方,知道脚下在哪里,才明白未来何去何从。"),
        new MessageEntry("Level3", "结语", "星星照应我们所处的位置在宇宙的何方，知道脚下在哪里，才明白未来何去何从。"),
        new MessageEntry("Level3", "拾取生组件时", "获得了\"生\"组件！"),
        new MessageEntry("Level3", "拾取星点组件时", "获得了\"星点\"组件！"),
        new MessageEntry("Level3", "生组件重复充能", "\"生\"组件已经充能过了"),
        new MessageEntry("Level3", "生组件充能成功", "✅ \"生\"组件已充能到石碑！"),
        new MessageEntry("Level3", "星点组件重复充能", "\"星点\"组件已经充能过了"),
        new MessageEntry("Level3", "星点组件充能成功", "✅ \"星点\"组件已充能到石碑！"),
        new MessageEntry("Level3", "凿冰面缺少硬属性时", $"需要手持具有{GameLocalization.GetPropertyAttributeName(ObjectProperty.Hard)}的物品才能凿开冰面"),
        new MessageEntry("Level3", "未手持组件尝试充能石碑", "需要手持组件才能充能石碑。请先拾取\"生\"或\"星点\"组件。"),
        new MessageEntry("Level3", "凿冰面时未拿冰锥", "需要手持冰锥才能凿开冰面"),
        new MessageEntry("Level3", "凿冰面时冰锥缺少Hard属性", $"冰锥还不够{GameLocalization.GetPropertyDisplayName(ObjectProperty.Hard)}，无法凿开冰面"),
        new MessageEntry("Level3", "手持物品错误或已充能", "当前手持的物品不是需要的组件，或已经充能过了"),
        new MessageEntry("Level3", "结语", "星星照应我们所处的位置在宇宙的何方，知道脚下在哪里，才明白未来何去何从。"),

        // 全局提示
        new MessageEntry("Global", "特性获得提示", "已获得 {0} 特性。"),
        new MessageEntry("Global", "特性替换提示", "背包已满，已用 {0} 替换 {1}。"),
    };

    /// <summary>
    /// Level 2 在充能进度提示中可能出现的动态组合。
    /// </summary>
    private static IEnumerable<MessageEntry> EnumerateLevel2ProgressMessages()
    {
        string level = "Level2";
        string trigger = "充能进度提示";

        string[] components = { "羽毛", "木", "日" };
        for (int chargedCount = 0; chargedCount <= 2; chargedCount++)
        {
            foreach (IEnumerable<string> missingCombination in GetNonEmptyCombinations(components))
            {
                string missing = string.Join(" ", missingCombination);
                yield return new MessageEntry(level, trigger, $"还需要充能：{missing} ({chargedCount}/3)");
            }
        }
    }

    /// <summary>
    /// Level 3 在充能进度提示中可能出现的动态组合。
    /// </summary>
    private static IEnumerable<MessageEntry> EnumerateLevel3ProgressMessages()
    {
        string level = "Level3";
        string trigger = "充能进度提示";

        string[] components = { "生", "星点" };
        for (int chargedCount = 0; chargedCount <= 1; chargedCount++)
        {
            foreach (IEnumerable<string> missingCombination in GetNonEmptyCombinations(components))
            {
                string missing = string.Join(" ", missingCombination);

                // 如果两者都缺少，则最多提示一次 (0/2)
                int maxCount = missingCombination.Count();
                int displayCount = chargedCount;
                if (maxCount == 2 && chargedCount > 0)
                {
                    // 当只缺一个组件时不会显示两个名称，因此跳过 chargedCount=1 且缺少两个的组合
                    continue;
                }

                yield return new MessageEntry(level, trigger, $"还需要充能：{missing} ({displayCount}/2)");
            }
        }
    }

    /// <summary>
    /// Level 3 「需要手持组件才能充能石碑」消息的所有可能组合。
    /// </summary>
    private static IEnumerable<MessageEntry> EnumerateLevel3ComponentReminderVariants()
    {
        string level = "Level3";
        string trigger = "缺少手持组件提示";

        yield return new MessageEntry(level, trigger, "需要手持组件才能充能石碑。请先拾取\"生\"组件。");
        yield return new MessageEntry(level, trigger, "需要手持组件才能充能石碑。请先拾取或\"星点\"组件。");
        yield return new MessageEntry(level, trigger, "需要手持组件才能充能石碑。请先拾取\"生\"或\"星点\"组件。");
        yield return new MessageEntry(level, trigger, "需要手持组件才能充能石碑。请先拾取组件。");
    }

    /// <summary>
    /// 返回所有提示信息（包含动态组合）。
    /// </summary>
    public static IEnumerable<MessageEntry> GetAllMessages()
    {
        foreach (var entry in BaseMessages)
        {
            yield return entry;
        }

        foreach (var entry in EnumerateLevel2ProgressMessages())
        {
            yield return entry;
        }

        foreach (var entry in EnumerateLevel3ProgressMessages())
        {
            yield return entry;
        }

        foreach (var entry in EnumerateLevel3ComponentReminderVariants())
        {
            yield return entry;
        }
    }

    /// <summary>
    /// 获取所有提示文本。
    /// </summary>
    public static IEnumerable<string> GetAllMessageTexts()
    {
        return GetAllMessages().Select(m => m.Text).Distinct();
    }

    /// <summary>
    /// 根据关卡与触发标识尝试获取消息。
    /// </summary>
    public static bool TryGetMessage(string level, string trigger, out MessageEntry entry)
    {
        foreach (var message in GetAllMessages())
        {
            if (message.Level == level && message.Trigger == trigger)
            {
                entry = message;
                return true;
            }
        }

        entry = default;
        return false;
    }

    /// <summary>
    /// 根据关卡与触发标识尝试获取消息文本。
    /// </summary>
    public static bool TryGetMessageText(string level, string trigger, out string text)
    {
        if (TryGetMessage(level, trigger, out var entry))
        {
            text = entry.Text;
            return true;
        }

        text = string.Empty;
        return false;
    }

    /// <summary>
    /// 生成所有提示文本所包含的字符集合，按Unicode排序。
    /// </summary>
    public static string GetCharacterSet()
    {
        HashSet<char> characters = new HashSet<char>();
        foreach (string text in GetAllMessageTexts())
        {
            foreach (char c in text)
            {
                characters.Add(c);
            }
        }

        var ordered = characters.OrderBy(c => c).ToArray();
        return new string(ordered);
    }

    /// <summary>
    /// 将所有提示文本写入文件。
    /// </summary>
    public static void WriteMessagesToFile(string filePath)
    {
        StringBuilder builder = new StringBuilder();
        foreach (var entry in GetAllMessages())
        {
            builder.AppendLine($"[{entry.Level}] {entry.Trigger}");
            builder.AppendLine(entry.Text);
            builder.AppendLine();
        }

        System.IO.File.WriteAllText(filePath, builder.ToString(), Encoding.UTF8);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    /// <summary>
    /// 将所有字符（去重）写入文件，方便用作TextMeshPro的Character From File输入。
    /// </summary>
    public static void WriteCharactersToFile(string filePath)
    {
        string characters = GetCharacterSet();
        System.IO.File.WriteAllText(filePath, characters, Encoding.UTF8);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    /// <summary>
    /// 生成非空组合。
    /// </summary>
    private static IEnumerable<IEnumerable<T>> GetNonEmptyCombinations<T>(IReadOnlyList<T> items)
    {
        int count = items.Count;
        int combinationCount = 1 << count;

        for (int mask = 1; mask < combinationCount; mask++)
        {
            List<T> combination = new List<T>();
            for (int i = 0; i < count; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    combination.Add(items[i]);
                }
            }
            yield return combination;
        }
    }
}

/// <summary>
/// 游戏通知辅助工具。
/// </summary>
public static class GameNotification
{
    private static NotificationUIManager cachedManager;

    /// <summary>
    /// 根据关卡与触发标识显示消息。
    /// </summary>
    public static bool ShowByTrigger(string level, string trigger)
    {
        if (GameMessageCatalog.TryGetMessageText(level, trigger, out string text) && !string.IsNullOrWhiteSpace(text))
        {
            ShowRaw(text);
            return true;
        }

        Debug.LogWarning($"GameNotification: 找不到消息 [{level}] {trigger}");
        return false;
    }

    /// <summary>
    /// 显示原始文本消息。
    /// </summary>
    public static void ShowRaw(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        NotificationUIManager manager = GetManager();
        if (manager != null)
        {
            manager.ShowNotification(message);
        }
        else
        {
            Debug.Log($"UI提示: {message}");
        }
    }

    private static NotificationUIManager GetManager()
    {
        if (cachedManager == null)
        {
            cachedManager = Object.FindObjectOfType<NotificationUIManager>();
        }
        else if (cachedManager.gameObject == null)
        {
            cachedManager = null;
            return GetManager();
        }

        return cachedManager;
    }
}

