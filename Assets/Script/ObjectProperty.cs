using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectProperty
{
    None = 0,
    Hard, //坚硬，from 石头
    Soft,
    Flexible, //柔韧
    Flammable, //可燃， from 树枝
    Liquid,
    Long, //长
    Thin, //细
    Warm, //暖
    Light, //光（从上一关获得）
    //Level3 新特性
    Heavy, //重，from 石头
    Cool, //凉，from 雪
    White, //白，from 雪
    Transparent, //透，from 冰锥
    Sharp, //尖，from 冰锥
    //Solid? 水+hard = solid？ 还是说 水+hard = hard
}

