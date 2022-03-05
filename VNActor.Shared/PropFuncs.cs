using System.Collections.Generic;
using UnityEngine;

namespace VNActor
{
    public partial class Item
    {
        //===============================================================================================
        // prop action wrapper functions
        // All scripts: func(prop, param)
        public static void prop_visible(Item prop, ItemData param)
        {
            // param = 0(hide)/1(show)
            prop.Visible = param.visible;
        }

        public static void prop_move(Item prop, ItemData param)
        {
            prop_move(prop, param.position);
        }

        public static void prop_move(Item prop, Vector3 param)
        {
            // param = (pos_delta_x, pos_delta_y, pos_delta_z)
            var cp = prop.Position;
            var ncp = new Vector3(cp.x + param[0], cp.y + param[1], cp.z + param[2]);
            prop.Position = ncp;
        }

        public static void prop_move_to(Item prop, ItemData param)
        {
            prop_move_to(prop, param.position);
        }

        public static void prop_move_to(Item prop, Vector3 param)
        {
            prop.Position = param;
        }

        public static void prop_rotate(Item prop, ItemData param)
        {
            prop_rotate(prop, param.rotation);
        }

        public static void prop_rotate(Item prop, Vector3 param)
        {
            // param = (rot_delta_x, rot_delta_y, rot_delta_z)
            var rt = prop.Rotation;
            var nrt = new Vector3(rt.x + param[0], rt.y + param[1], rt.z + param[2]);
            prop.Rotation = nrt;
        }

        public static void prop_rotate_to(Item prop, ItemData param)
        {
            prop_rotate_to(prop, param.rotation);
        }

        public static void prop_rotate_to(Item prop, Vector3 param)
        {
            prop.Rotation = param;
        }

        public static void prop_scale_to(Item prop, ItemData param)
        {
            prop_scale_to(prop, param.scale);
        }

        public static void prop_scale_to(Item prop, Vector3 param)
        {
            prop.scale = param;
        }

        /*
        public static void prop_color_neo(Item prop, (Color, Color, float, float) param)
        {

        }
        */

        /*
        public static void prop_color_neo(Item prop, ((Color, Color, float, float), (float color, float color2, float, float)) param)
        {
            prop.color = param;
        }

        public static void prop_color_neo(Item prop, int[] param)
        {
            object col2;
            object col1;
            // param = ((Color, Color, float, float), (color, color, float, foat)) or ((Color, Color, float, float),) or (Color, Color, float, float), where Color can be (r,g,b) or (r,g,b,a)
            try
            {
                if (param is tuple && (1, 2).Contains(param.Count) && param[0] is tuple && param[0][0] is Color)
                {
                    // maybe format ((Color, Color, float, float), (color, color, float, foat))
                    prop.set_color(param);
                }
                else if (param is tuple && (1, 2).Contains(param.Count) && param[0] is tuple && param[0][0] is tuple)
                {
                    // maybe format ((Color, Color, float, float), (color, color, float, foat)) where Color is in tuple
                    var colLst = new List<object>();
                    foreach (var pc in param)
                    {
                        if (pc[0].Count == 3)
                        {
                            col1 = Color(pc[0][0], pc[0][1], pc[0][2]);
                        }
                        else
                        {
                            col1 = Color(pc[0][0], pc[0][1], pc[0][2], pc[0][3]);
                        }
                        if (pc[1].Count == 3)
                        {
                            col2 = Color(pc[1][0], pc[1][1], pc[1][2]);
                        }
                        else
                        {
                            col2 = Color(pc[1][0], pc[1][1], pc[1][2], pc[1][3]);
                        }
                        colLst.append((col1, col2, pc[2], pc[3]));
                    }
                    prop.set_color(tuple(colLst));
                }
                else if (param is tuple && param.Count == 4)
                {
                    if (param[0] is Color)
                    {
                        col1 = param[0];
                    }
                    else if (param[0].Count == 3)
                    {
                        col1 = Color(param[0][0], param[0][1], param[0][2]);
                    }
                    else
                    {
                        col1 = Color(param[0][0], param[0][1], param[0][2], param[0][3]);
                    }
                    if (param[1] is Color)
                    {
                        col2 = param[1];
                    }
                    else if (param[1].Count == 3)
                    {
                        col2 = Color(param[1][0], param[1][1], param[1][2]);
                    }
                    else
                    {
                        col2 = Color(param[1][0], param[1][1], param[1][2], param[1][3]);
                    }
                    prop.set_color(ValueTuple.Create((col1, col2, param[2], param[3])));
                }
                else
                {
                    throw new Exception("Unknown format.");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("prop_color param format error:", e);
            }
        }

        */


        public static void prop_fk_set(Item prop, List<Vector3> param)
        {
            // param: a list/tuple of Vector3 or tuple(3), as the rot of prop's FK bone
            prop.import_fk_bone_info(param);
        }

        public static void prop_dynamicbone_enable(Item prop, bool param)
        {
            // param: 0/1
            prop.dynamicbone_enable = param;
        }

        public static void prop_anime_speed(Item prop, float param)
        {
            // param = speed (0~3)
            prop.anime_speed = param;
        }
    }
}