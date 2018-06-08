using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModule
{
    static GameModule s_Inst = null;
    public static GameModule Instance()
    {
        if (s_Inst == null)
            s_Inst = new GameModule();
        return s_Inst;
    }
	
    public int curLevelIdx { set; get; }
		
}
