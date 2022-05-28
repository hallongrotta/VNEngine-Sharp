using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using Studio;
using UnityEngine;

namespace VNActor.KKS
{
    public class Text : Prop
    {

        [MessagePackObject(keyAsPropertyName: true)]
        public class TextData : NEOPropData, IDataClass<Prop>
        {

            [Key("Color")] public Color Color;
            [Key("OutlineColor")] public Color OutlineColor;
            [Key("OutlineSize")] public Color OutlineSize;
            [Key("TextInfo")] public OITextInfo.TextInfo[] TextInfo;

            public TextData() : base()
            {

            }

            public void Apply(Text t)
            {
                t.Visible = visible;
                if (visible)
                { 
                    t.Color = Color;
                    t.OutlineColor = OutlineColor;
                    t.TextInfo = TextInfo;
                }

                t.oci.Update();
            }

            public TextData(Text t) : base(t)
            {
                if (!visible) return;
                Color = t.Color;
                OutlineColor = t.OutlineColor;
                TextInfo = t.TextInfo;
            }
        }


        internal OCIText oci;

        public Text(ObjectCtrlInfo objctrl) : base(objctrl)
        {
            oci = objctrl as OCIText;
        }

        public Color Color
        {
            get
            {
                return oci.textInfo.color;
            }
            set
            {
                oci.SetColor(value);
            }
        }

        public Color OutlineColor
        {
            get
            {
                return oci.textInfo.outlineColor;
            }
            set
            {
                oci.textInfo.outlineColor = value;
            }
        }

        public float OutLineSize
        {
            get
            {
                return oci.textInfo.outlineSize;
            }
            set
            {
                oci.textInfo.outlineSize = value;
            }
        }

        public struct Effect_s
        {
            internal int idx;
            internal int effect;
        }

        public OITextInfo.TextInfo[] TextInfo
        {
            get
            {
                var array = new OITextInfo.TextInfo[oci.textInfo.textInfos.Length];
                oci.textInfo.textInfos.CopyTo(array, 0);
                return array;
            }
            set
            {
                var array = new OITextInfo.TextInfo[value.Length];
                value.CopyTo(array, 0);
                oci.textInfo.textInfos = array;
            }
        }

        public override Vector3 Position { get; set; }
        public override Vector3 Rotation { get; set; }
    }
}
