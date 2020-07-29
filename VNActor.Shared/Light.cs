namespace VNActor
{
    partial class Light
    {
        public static void prop_enable(Light prop, LightData param)
        {
            // param = 0(hide)/1(show)
            prop.enable = param.enable;
        }

        public static void prop_intensity(Light prop, LightData param)
        {
            prop.intensity = param.intensity;
        }

        public static void prop_shadow(Light prop, LightData param)
        {
            prop.shadow = param.shadow;
        }

        public static void prop_angle(Light prop, LightData param)
        {
            prop.angle = param.angle;
        }

        public static void prop_range(Light prop, LightData param)
        {
            prop.range = param.range;
        }


    }
}
