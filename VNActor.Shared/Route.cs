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

        public struct RouteData : IDataClass
        {
            internal RouteInfo? route_f;
            internal bool route_p;

            public RouteData(Route r, bool route_full = false)
            {
                if (route_full)
                {
                    route_f = r.route_full;
                }
                else
                {
                    route_f = null;
                }
                route_p = r.route_play;
            }

            public void Remove(string key)
            {
                throw new NotImplementedException();
            }
        }

        public Route(OCIRoute objctrl) : base(objctrl)
        {
            this.objctrl = objctrl;
        }

        public bool route_play
        {
            set
            {
                // param: 1=start/0=stop
                if (value)
                {
                    this.objctrl.Play();
                }
                else
                {
                    this.objctrl.Stop();
                }
            }
            get
            {
                return this.objctrl.isPlay;
            }
        }

        public void SetRouteFull(bool param)
        {
            this.route_play = param;
            return;
        }

        public void SetRouteFull(RouteInfo param)
        {
            // route info, ref to get_route_full
            try
            {
                OCIRoute route = this.objctrl;
                // route_f, full route setting
                route.Stop();
                RouteInfo cur_status = this.route_full;
                foreach (var i in Enumerable.Range(0, 4))
                {
                    if (param.pts[i].Equals(cur_status.pts[i]))
                    {
                        continue;
                    }
                    var ri = route.routeInfo;
                    if (i == 0)
                    {
                        // orient
                        ri.orient = param.orient;
                    }
                    else if (i == 1)
                    {
                        // loop
                        ri.loop = param.loop;
                        route.UpdateLine();
                    }
                    else if (i == 2)
                    {
                        // points
                        foreach (var j in Enumerable.Range(0, cur_status.pts.Count))
                        {
                            var pt = param.pts[j];
                            var pt_cur = cur_status.pts[j];
                            if (pt.Equals(pt_cur))
                            {
                                continue;
                            }
                            var pi = route.routeInfo.route[j];
                            // pt pos
                            pi.changeAmount.pos = pt.pt_pos;

                            // pt rot
                            pi.changeAmount.rot = pt.rot;

                            // aid pos
                            pi.aidInfo.changeAmount.pos = pt.aid_pos;

                            // speed
                            pi.speed = pt.speed;

                            // easeType
                            pi.easeType = pt.easeType;

                            // connection
                            pi.connection = pt.connection;

                            // link
                            pi.link = pt.link;
                        }
                        route.UpdateLine();
                    }
                    else if (i == 3)
                    {
                        // active
                        this.route_play = param.active;
                    }
                    else
                    {
                        throw new Exception("Unknown route info");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("VNGE VNActor Error: Can not set route, ", e);
            }
        }

        public RouteInfo route_full
        {
            get
            {
                OCIRoute route = this.objctrl;
                OIRouteInfo ri = route.routeInfo;
                var pts = new List<Point>();
                foreach (OIRoutePointInfo pi in ri.route)
                {
                    // (pt pos, pt rot, aid pos, speed, easeType, connection, link)
                    pts.Add(new Point(pi.changeAmount.pos, pi.changeAmount.rot, pi.aidInfo.changeAmount.pos, pi.speed, pi.easeType, pi.connection, pi.link));
                }
                // (orient, loop, points, active)
                RouteInfo rs = new RouteInfo(ri.orient, ri.loop, pts, ri.active);
                return rs;
            }
        }

        // route
        public static void prop_route(Route prop, RouteData param)
        {
            prop.SetRouteFull((RouteInfo)param.route_f);
        }

        override public IDataClass export_full_status()
        {
            return new RouteData(this);
        }

        override public void import_status(IDataClass status)
        {
            throw new NotImplementedException();
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
