using UnityEngine;

public class GameEnums
{
    public enum eScene
    { 
        None,
        Intro,
        SunFlower,
        Room,
        Star,
        Crow,
        EastArt,
        Stage
    }

    public enum eFlowerType
    {
        None,
        pot_grow_up, //화분에서 자라남
        s1, //왼쪽 꽃 움직임
        s2, //
        s3,
        s4,
        wf1,
        wf2,
        wf3,
        wf4,
        wf5,
        wf6,
        wf7
    }

    public enum eFlower
    {
        seed,
        flower,
        withered_flower
    }

    public enum eRooomType
    {
        towel_orange,
        frame_orange3,
        pillow_hat,
        door_cup,
        window_pot,
        bed_shoes,
        table_glass,
        chair_bottle,
        pillow,
        frame_L,
        frame_R,
        frame_B,
        window_frame_L,
        window_frame_R,
        chair
    }

    public enum eStarType
    { 
        moon,
        star_01,
        star_02,
        star_03,
        star_04,
        star_05,
        star_06,
        star_07,
        star_08,
        star_09,
        star_10,
        star_11
    }
}
