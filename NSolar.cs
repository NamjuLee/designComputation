// NJ Namju Lee (www.njstudio.co.kr)
// nj.namju@gmail.com

#region Solar class
public class Solar{
    public Vector3d VectorOrigin = new Vector3d();
    public Vector3d VectorSun = new Vector3d();
    public Point3d SunPoint;
    public double declination;
    public double solarAltitude;
    public double solarAzimuth;
    public double southDegree = -90;
    public double sunDistance = 100;
    public Line LineSun = new Line();
    public Line LineSouth = new Line();
    public Curve Trajectory;
    public Sphere SphereSun = new Sphere();
    public Sphere SpherePath = new Sphere();
    public double SphereRadius = 10.0;
    public int day = 1;
    public double latitude = 42.3;
    public double hour = 14;
    public Surface LandSurface = null;
    public List<Brep> LandObject = new List<Brep>();
    public List<int> Shadowness = new List<int>();
    public void Update(double Latitude, int theDay, double Hour, double sundistance = 100, double sphereSize = 10.0, double south = -90){
        day = theDay;
        latitude = Latitude;
        hour = Hour;

        southDegree = south;
        sunDistance = sundistance;
        SphereRadius = sphereSize;

        declination = Declination(day);
        solarAltitude = SolarAltitude(latitude, declination, hour - 12);
        solarAzimuth = SolarAzimuth(latitude, declination, hour - 12, solarAltitude);

        VectorSun.X = Math.Cos(NJS.NMath.Radians(southDegree - solarAzimuth));
        VectorSun.Y = Math.Sin(NJS.NMath.Radians(southDegree - solarAzimuth));
        VectorSun.Z = Math.Tan(NJS.NMath.Radians(solarAltitude));

        VectorSun.Unitize();
        Vector3d tempVec = VectorSun * sunDistance;

        LineSun = new Line((Point3d)VectorOrigin, (Point3d)tempVec);
        LineSouth = new Line((Point3d)VectorOrigin, new Point3d((Math.Cos(NJS.NMath.Radians(southDegree)) * sunDistance) * 0.3, (Math.Sin(NJS.NMath.Radians(southDegree)) * sunDistance) * 0.3, 0));
        SphereSun = new Sphere((Point3d)tempVec, SphereRadius);
        SpherePath = new Sphere((Point3d)this.VectorOrigin, sunDistance);
        Trajectory = GetCurveForSolarPath();
        SunPoint = this.LineSun.PointAt(0.5);
        VectorSun = (Vector3d)SunPoint;
        VectorSun.Unitize();
    }
    #region Get Curve For Solar Path
    public Curve GetCurveForSolarPath(){
        List<Point3d> pts = new List<Point3d>();
        for (int j = 1; j <= 365; j += 30){
            for (double i = 0.5; i < 24; i += 3){
                Point3d p = GetPointForSunPath(j, i);
                if (p != null){
                    pts.Add(GetPointForSunPath(j, i));
                }
            }
        }
        Curve c = Rhino.Geometry.NurbsCurve.CreateInterpolatedCurve(pts, 3);
        return c;
    }
    Point3d GetPointForSunPath(int theDay, double theHour){
        day = theDay;
        hour = theHour;
        declination = Declination(day);
        solarAltitude = SolarAltitude(latitude, declination, hour - 12.0);
        solarAzimuth = SolarAzimuth(latitude, declination, hour - 12.0, solarAltitude);

        VectorSun.X = sunDistance * Math.Cos(NJS.NMath.Radians(southDegree - solarAzimuth));
        VectorSun.Y = sunDistance * Math.Sin(NJS.NMath.Radians(southDegree - solarAzimuth));
        VectorSun.Z = sunDistance * Math.Tan(NJS.NMath.Radians(solarAltitude));
        Line ln = new Line((Point3d)VectorOrigin, VectorSun);

        Point3d p1;
        Point3d p2;
        Rhino.Geometry.Intersect.Intersection.LineSphere(ln, SpherePath, out p1, out p2);
        return p2;
    }
    #endregion Get Curve For Solar Path
    public override string ToString(){
        return base.ToString() + ": Latitude = " + latitude + " Day = " + day + " Hour = " + hour + " { the declination :  '" + declination + "' Degree. The Solar Altitude : '" + solarAltitude + "' Degree. The Solar Azimuth : '" + solarAzimuth + "' Degree. } ";
    }
    public double Declination(int day){
        return (23.45 * Math.Sin(NJS.NMath.Radians(360 * (284 + day)) / 365.25));
    }
    public double SolarAltitude(double latitude, double declination, double hour){
        double newValue = (Math.Asin(Math.Cos(NJS.NMath.Radians(latitude)) * Math.Cos(NJS.NMath.Radians(declination)) * Math.Cos(NJS.NMath.Radians(hour * 15)) + Math.Sin(NJS.NMath.Radians(latitude)) * Math.Sin(NJS.NMath.Radians(declination))));
        return NJS.NMath.Degrees(newValue);
    }
    public double SolarAzimuth(double latitude, double declination, double hour, double solarAltitude){
        if (hour == 0){
            hour = 0.1;
        }
        if (hour < 0){
            return (-NJS.NMath.Degrees(Math.Acos(((Math.Sin(NJS.NMath.Radians(solarAltitude)) * Math.Sin(NJS.NMath.Radians(latitude)) - Math.Sin(NJS.NMath.Radians(declination))) / (Math.Cos(NJS.NMath.Radians(solarAltitude)) * Math.Cos(NJS.NMath.Radians(latitude)))))));
        }else{
            return NJS.NMath.Degrees(Math.Acos(((Math.Sin(NJS.NMath.Radians(solarAltitude)) * Math.Sin(NJS.NMath.Radians(latitude)) - Math.Sin(NJS.NMath.Radians(declination))) / (Math.Cos(NJS.NMath.Radians(solarAltitude)) * Math.Cos(NJS.NMath.Radians(latitude))))));
        }
    }
    #region shadowness in a pixel and in a voxel
    public List<double> CalculateShadowness(Surface landSurface, int xRes, int yRes, double zOffset, double Latitude, double south, double sundistance, double sunZVectorMin, List<Brep> building){
        Shadowness.Clear();
        LandSurface = landSurface;
        List<Point3d> ptsFrom = NJS.PointUtils.Point2dGridWithTwoPoints.Pts2dFromSurface(LandSurface, xRes, yRes);
        Brep surBrep = Rhino.Geometry.Brep.CreateFromSurface(landSurface);
        //building.Add(surBrep);
        foreach (Point3d p in ptsFrom){
            Point3d pt = p;
            pt.Z += zOffset;
            Shadowness.Add(0);
            for (double d = 1; d <= 365; d += 30){
                for (double h = 8.5; h < 20; h += 1.0){
                    declination = Declination((int)d);
                    solarAltitude = SolarAltitude(Latitude, declination, h - 12);
                    solarAzimuth = SolarAzimuth(Latitude, declination, h - 12, solarAltitude);
                    VectorSun.X = Math.Cos(NJS.NMath.Radians(southDegree - solarAzimuth));
                    VectorSun.Y = Math.Sin(NJS.NMath.Radians(southDegree - solarAzimuth));
                    VectorSun.Z = Math.Tan(NJS.NMath.Radians(solarAltitude));
                    VectorSun.Unitize();
                    if (VectorSun.Z > sunZVectorMin){
                        Line ln = new Rhino.Geometry.Line(pt, pt + VectorSun * sunDistance);
                        // ray with line
                        Curve nc = ln.ToNurbsCurve();
                        bool b = NJS.Environment.Solar.IsShadow((Curve)nc, building);
                        if (b == true){
                            Shadowness[Shadowness.Count - 1] += 1;
                            //break;
                        }
                    }
                }
            }
        }
        return null;
    }
    public List<double> CalculateShadowness(List<Point3d> pts, double zOffset, double Latitude, double southDegree, double sunDistance, double sunZVectorMin, List<Brep> building){
        Shadowness.Clear();
        foreach (Point3d p in pts){
            Point3d pt = p;
            pt.Z += zOffset;
            Shadowness.Add(0);
            bool insidePt = false;
            foreach(Brep b in building){
                if(b.IsPointInside(p, 0.1, false)){
                    insidePt = true;
                    break;
                }
            }
            if (!insidePt){
                for (double d = 1; d <= 365; d += 30){
                    for (double h = 4.5; h < 17; h += 1.0){
                        declination = Declination((int)d);
                        solarAltitude = SolarAltitude(Latitude, declination, h - 12);
                        solarAzimuth = SolarAzimuth(Latitude, declination, h - 12, solarAltitude);
                        VectorSun.X = Math.Cos(NJS.NMath.Radians(southDegree - solarAzimuth));
                        VectorSun.Y = Math.Sin(NJS.NMath.Radians(southDegree - solarAzimuth));
                        VectorSun.Z = Math.Tan(NJS.NMath.Radians(solarAltitude));
                        VectorSun.Unitize();
                        if (VectorSun.Z > sunZVectorMin){
                            Line ln = new Rhino.Geometry.Line(pt, pt + VectorSun * sunDistance);
                            // ray with line
                            Curve nc = ln.ToNurbsCurve();
                            bool b = NJS.Environment.Solar.IsShadow((Curve)nc, building);
                            if (b == true){
                                Shadowness[Shadowness.Count - 1] += 1;
                                //break;
                            }
                        }
                    }
                }
            }
        }
        return null;
    }
    public List<double> CalValue(List<int> valueList, double scale, int minVal){
        List<double> newVal = new List<double>();
        foreach (int n in valueList){
            if (n > minVal){
                double newnum = n / scale;
                newVal.Add(newnum);
            }else{
                newVal.Add(0);
            }
        }
        return newVal;
    }
    #endregion  shadowness in a pixel and in a voxel
    public static bool IsShadow(Curve sunLine, List<Brep> breps){
        for (int i = 0; i < breps.Count; i++){
            Curve[] overlapCurve;// = new Curve[];
            Point3d[] interPoints;// = new Point3d[];
            if (Rhino.Geometry.Intersect.Intersection.CurveBrep(sunLine, breps[i], 0.1, out overlapCurve, out interPoints)){
                if (interPoints.Length > 0) return true;
            }
        }
        return false;
    }
}
#endregion  Solar class