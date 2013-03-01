using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

// unused
public class AIManager
{
    static AIManager INSTANCE;

    static AIManager GetInstance() {
        if (INSTANCE == null){
            INSTANCE = new AIManager();
        }
        return INSTANCE;
    }


}
