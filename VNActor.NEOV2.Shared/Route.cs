using Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

namespace VNActor
{
    public partial class Route : HSNeoOCIProp
    {
        public void set_route_play(bool param)
        {
            // param: 1=start/0=stop

                OCIRoute route = (OCIRoute)base.objctrl;
                if (param)
                {
                    route.Play();
                }
                else
                {
                    route.Stop();
                }
            
        }

        public bool get_route_play()
        {

                OCIRoute route = (OCIRoute)base.objctrl;
                return route.isPlay;

        }

        public void set_route_full(bool param)
        {
            this.set_route_play(param);
            return;
        }

        public void set_route_full(RouteInfo param)
        {
            // route info, ref to get_route_full

                try
                {
                    OCIRoute route = (OCIRoute)base.objctrl;
                    // route_f, full route setting
                    route.Stop();
                    RouteInfo cur_status = this.get_route_full();
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
                            this.set_route_play(param.active);
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

        
       
        public RouteInfo get_route_full()
        {
            OCIRoute route = (OCIRoute)base.objctrl;
            OIRouteInfo ri = route.routeInfo;
            var pts = new List<Point>();
            foreach (OIRoutePointInfo pi in ri.route)
            {
                // (pt pos, pt rot, aid pos, speed, easeType, connection, link)
                pts.Add(new Point(pi.changeAmount.pos, pi.changeAmount.rot, pi.aidInfo.changeAmount.pos, pi.speed, pi.easeType, pi.connection, pi.link));
            }
            // (orient, loop, points, active)
            var rs = new RouteInfo(ri.orient, ri.loop, pts, ri.active);
            return rs;
        }
        
        public override IDataClass export_full_status()
        {
            throw new NotImplementedException();
        }

        public override void import_status(IDataClass status)
        {
            throw new NotImplementedException();
        }
    }
}
