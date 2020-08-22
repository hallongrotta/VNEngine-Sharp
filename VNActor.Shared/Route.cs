using Studio;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VNActor
{
    public partial class Route : HSNeoOCIProp
    {

        public struct Point : IEquatable<Point>
        {
            public Vector3 pt_pos;
            public Vector3 rot;
            public Vector3 aid_pos;
            public float speed;
            public StudioTween.EaseType easeType;
            public OIRoutePointInfo.Connection connection;
            public bool link;

            public Point(Vector3 pt_pos, Vector3 rot, Vector3 aid_pos, float speed, StudioTween.EaseType easeType, OIRoutePointInfo.Connection connection, bool link)
            {
                this.pt_pos = pt_pos;
                this.rot = rot;
                this.aid_pos = aid_pos;
                this.speed = speed;
                this.easeType = easeType;
                this.connection = connection;
                this.link = link;
            }

            public override bool Equals(object obj)
            {
                return obj is Point point && Equals(point);
            }

            public bool Equals(Point other)
            {
                return pt_pos.Equals(other.pt_pos) &&
                       rot.Equals(other.rot) &&
                       aid_pos.Equals(other.aid_pos) &&
                       speed == other.speed &&
                       easeType == other.easeType &&
                       connection == other.connection &&
                       link == other.link;
            }

            public override int GetHashCode()
            {
                int hashCode = 1637020191;
                hashCode = hashCode * -1521134295 + pt_pos.GetHashCode();
                hashCode = hashCode * -1521134295 + rot.GetHashCode();
                hashCode = hashCode * -1521134295 + aid_pos.GetHashCode();
                hashCode = hashCode * -1521134295 + speed.GetHashCode();
                hashCode = hashCode * -1521134295 + easeType.GetHashCode();
                hashCode = hashCode * -1521134295 + connection.GetHashCode();
                hashCode = hashCode * -1521134295 + link.GetHashCode();
                return hashCode;
            }
        }

        public struct RouteInfo
        {
            public OIRouteInfo.Orient orient;
            public bool loop;
            public List<Point> pts;
            public bool active;

            public RouteInfo(OIRouteInfo.Orient orient, bool loop, List<Point> pts, bool active)
            {
                this.orient = orient;
                this.loop = loop;
                this.pts = pts;
                this.active = active;
            }
        }

        new public OCIRoute objctrl;

        public Route(ObjectCtrlInfo objctrl) : base(objctrl)
        {
        }

        override public Vector3 pos
        {
            get
            {
                return this.objctrl.objectInfo.changeAmount.pos;
            }
            set
            {
                this.objctrl.objectInfo.changeAmount.pos = value;
            }
        }

        override public Vector3 rot
        {
            get
            {
                return this.objctrl.objectInfo.changeAmount.rot;
            }
            set
            {
                this.objctrl.objectInfo.changeAmount.rot = value;
            }
        }

    }
}
