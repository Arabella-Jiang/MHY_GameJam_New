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
    //Solid? 水+hard = solid？ 还是说 水+hard = hard
}

