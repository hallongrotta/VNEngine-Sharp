using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADV.Commands.Object;
using MessagePack;
using MessagePack.Resolvers;
using Studio;
using UnityEngine;

namespace VNActor.KKS
{
    public class Text : Prop, IVNObject<Text>
    {

        [MessagePackObject(keyAsPropertyName: true)]
        public class TextData : NEOPropData, IDataClass<Text>
        {

            [Key("Color")] public Color Color;
            [Key("OutlineColor")] public Color OutlineColor;
            [Key("OutlineSize")] public float OutlineSize;
            [Key("TextInfo")] public OITextInfo.TextInfo[] TextInfo;
            [Key("Position")] public Vector3 Position;
            [Key("Rotation")] public Vector3 Rotation;
            [Key("Scale")] public Vector3 Scale;


            public TextData() : base()
            {

            }

            public void Apply(Text t)
            {
                t.Visible = visible;
                if (visible)
                {
                    t.Position = Position;
                    t.Rotation = Rotation;
                    t.Scale = Scale;
                    t.Color = Color;
                    t.OutlineColor = OutlineColor;
                    t.TextInfo = TextInfo;
                    t.OutLineSize = OutlineSize;
                }

                t.oci.Update();
            }

            public TextData(Text t) : base(t)
            {
                if (!visible) return;
                Color = t.Color;
                Position = t.Position;
                Rotation = t.Rotation;
                OutlineColor = t.OutlineColor;
                TextInfo = t.TextInfo;
                OutlineSize = t.OutLineSize;
                Scale = t.Scale;
            }
        }


        internal OCIText oci;

        public Text(ObjectCtrlInfo objctrl) : base(objctrl)
        {
            oci = objctrl as OCIText;
        }

        public override Vector3 Position
        {
            get => oci.objectInfo.changeAmount.pos;
            set => oci.objectInfo.changeAmount.pos = value;
        }

        public override Vector3 Rotation
        {
            get => oci.objectInfo.changeAmount.rot;
            set => oci.objectInfo.changeAmount.rot = value;
        }

        public Vector3 Scale
        {
            get => oci.objectInfo.changeAmount.scale;
            set => oci.objectInfo.changeAmount.scale = value;
        }


        public Color Color
        {
            get => oci.textInfo.color;
            set => oci.SetColor(value);
        }

        public Color OutlineColor
        {
            get => oci.textInfo.outlineColor;
            set => oci.textInfo.outlineColor = value;
        }

        public float OutLineSize
        {
            get => oci.textInfo.outlineSize;
            set => oci.textInfo.outlineSize = value;
        }

        public OITextInfo.TextInfo CopyTextInfo(OITextInfo.TextInfo tf)
        {
            var bytes = Utils.SerializeData(tf);
            var s = Utils.DeserializeData<OITextInfo.TextInfo>(bytes);
            return s;
        }

        public OITextInfo.TextInfo[] TextInfo
        {
            get
            { 
                var array = new OITextInfo.TextInfo[2];
                for (var i = 0; i < array.Length; i++)
                {
                    array[i] = CopyTextInfo(oci.textInfo.textInfos[i]);
                }
                return array;
            }
            set
            {
                if (value is null) return;
                for (var i = 0; i < 2; i++)
                {
                    oci.textInfo.textInfos[i] = CopyTextInfo(value[i]);
                }
            }
        }

        public new IDataClass<Text> export_full_status()
        {
            return new TextData(this);
        }

        public void import_status(IDataClass<Text> status)
        {
            status.Apply(this);
        }
    }
}
