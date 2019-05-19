using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mdl_Intensities 
{
    public class IntensitiesFormJSon
    {
        public float[] LOIN0;
        public float[] LOIN1;
        public float[] LOIN2;
        public float[] LOIN3;
        public float[] LOIN4;
        public IntensitiesFormJSon(float[] LOIN0, float[] LOIN1, float[] LOIN2, float[] LOIN3, float[] LOIN4){
            this.LOIN0 = LOIN0;
            this.LOIN1 = LOIN1;
            this.LOIN2 = LOIN2;
            this.LOIN3 = LOIN3;
            this.LOIN4 = LOIN4;
        }
        public IntensitiesFormJSon(){ }
    }
    public Dictionary<string, float> LoIntensities;


    public Mdl_Intensities(float[] LOIN0, float[] LOIN1, float[] LOIN2, float[] LOIN3, float[] LOIN4) {
        LoIntensities = new Dictionary<string, float>
        {
            { "LOIN0", LOIN0[0] },
            { "LOIN1", LOIN1[0] },
            { "LOIN2", LOIN2[0] },
            { "LOIN3", LOIN3[0] },
            { "LOIN4", LOIN4[0] },
            { "LOINDF0", LOIN0[1] },
            { "LOINDF1", LOIN1[1] },
            { "LOINDF2", LOIN2[1] },
            { "LOINDF3", LOIN3[1] },
            { "LOINDF4", LOIN4[1] },
            { "LOINDD0", LOIN0[2] },
            { "LOINDD1", LOIN1[2] },
            { "LOINDD2", LOIN2[2] },
            { "LOINDD3", LOIN3[2] },
            { "LOINDD4", LOIN4[2] }
        };
    }

    public Mdl_Intensities() {
        LoIntensities = new Dictionary<string, float>();
    }

    public int GetNumebrOfLO() {
        return LoIntensities.Count/3;
    }
}
