﻿using System;
using System.Collections.Generic;
using Studio;
using UnityEngine;

namespace VNActor
{
    public class Route : Prop
    {
        public new OCIRoute objctrl;

        public Route(ObjectCtrlInfo objctrl) : base(objctrl)
        {
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
                    objctrl.Play();
                else
                    objctrl.Stop();
            }
            get => objctrl.isPlay;
        }

        public RouteInfo route_full
        {
            get
            {
                var route = objctrl;
                var ri = route.routeInfo;
                var pts = new List<Point>();
                foreach (var pi in ri.route)
                    // (pt pos, pt rot, aid pos, speed, easeType, connection, link)
                    pts.Add(new Point(pi.changeAmount.pos, pi.changeAmount.rot, pi.aidInfo.changeAmount.pos, pi.speed,
                        pi.easeType, pi.connection, pi.link));
                // (orient, loop, points, active)
                var rs = new RouteInfo(ri.orient, ri.loop, pts, ri.active);
                return rs;
            }
        }

        public override Vector3 Position
        {
            get => objctrl.objectInfo.changeAmount.pos;
            set => objctrl.objectInfo.changeAmount.pos = value;
        }

        public override Vector3 Rotation
        {
            get => objctrl.objectInfo.changeAmount.rot;
            set => objctrl.objectInfo.changeAmount.rot = value;
        }

        public void SetRouteFull(bool param)
        {
            route_play = param;
        }

        public void SetRouteFull(RouteInfo param)
        {
            // route info, ref to get_route_full
            try
            {
                var route = objctrl;
                // route_f, full route setting
                route.Stop();
                var cur_status = route_full;
                for (var i = 0; i < 4; i++)
                {
                    if (param.pts[i].Equals(cur_status.pts[i])) continue;
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
                        for (var j = 0; j < cur_status.pts.Count; j++)
                        {
                            var pt = param.pts[j];
                            var pt_cur = cur_status.pts[j];
                            if (pt.Equals(pt_cur)) continue;
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
                        route_play = param.active;
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

        // route
        public static void prop_route(Route prop, RouteData param)
        {
            prop.SetRouteFull((RouteInfo) param.route_f);
        }

        public new IDataClass<Route> export_full_status()
        {
            return new RouteData(this);
        }

        public void import_status(IDataClass<Route> status)
        {
            throw new NotImplementedException();
        }

        public struct Point : IEquatable<Point>
        {
            public Vector3 pt_pos;
            public Vector3 rot;
            public Vector3 aid_pos;
            public float speed;
            public StudioTween.EaseType easeType;
            public OIRoutePointInfo.Connection connection;
            public bool link;

            public Point(Vector3 pt_pos, Vector3 rot, Vector3 aid_pos, float speed, StudioTween.EaseType easeType,
                OIRoutePointInfo.Connection connection, bool link)
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
                var hashCode = 1637020191;
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

        public struct RouteData : IDataClass<Route>
        {
            internal RouteInfo? route_f;
            internal bool route_p;

            public RouteData(Route r, bool route_full = false)
            {
                if (route_full)
                    route_f = r.route_full;
                else
                    route_f = null;
                route_p = r.route_play;
            }

            public void Apply(Route item)
            {
                throw new NotImplementedException();
            }

            public void Remove(string key)
            {
                throw new NotImplementedException();
            }
        }
    }
}