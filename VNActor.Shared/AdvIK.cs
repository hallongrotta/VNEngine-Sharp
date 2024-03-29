﻿using AdvIKPlugin;

namespace VNActor
{
    public class AdvIKData
    {

        public bool ShoulderRotatorEnabled;
        public bool IndependentShoulders;
        public float ShoulderWeight;
        public float ShoulderRightWeight;
        public float ShoulderOffset;
        public float ShoulderRightOffset;
        public float SpineStiffness;

        public void Apply(Character a)
        {
            var advIKcontroller = a.objctrl?.charInfo?.gameObject?.GetComponent<AdvIKCharaController>();
            if (advIKcontroller != null)
            {
                advIKcontroller.ShoulderRotationEnabled = ShoulderRotatorEnabled;
                advIKcontroller.IndependentShoulders = IndependentShoulders;
                advIKcontroller.ShoulderWeight = ShoulderWeight;
                advIKcontroller.ShoulderRightWeight = ShoulderRightWeight;
                advIKcontroller.ShoulderOffset = ShoulderOffset;
                advIKcontroller.ShoulderRightOffset = ShoulderRightOffset;
                advIKcontroller.SpineStiffness = SpineStiffness;
            }
        }

        public AdvIKData(Character a)
        {
            var advIKcontroller = a.objctrl.charInfo?.gameObject?.GetComponent<AdvIKCharaController>();
            if (advIKcontroller != null)
            {
                ShoulderRotatorEnabled = advIKcontroller.ShoulderRotationEnabled;
                IndependentShoulders = advIKcontroller.IndependentShoulders;
                ShoulderWeight = advIKcontroller.ShoulderWeight;
                ShoulderRightWeight = advIKcontroller.ShoulderRightWeight;
                ShoulderOffset = advIKcontroller.ShoulderOffset;
                ShoulderRightOffset = advIKcontroller.ShoulderRightOffset;
                SpineStiffness = advIKcontroller.SpineStiffness;
            }
        }

        public AdvIKData()
        {

        }
    }
}
