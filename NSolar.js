// NJ Namju Lee (www.njstudio.co.kr)
// nj.namju@gmail.com


function NSolarInit(){
  var ns = new NSolar(Latitude, Day, Hour, South);
  ns.GetSunVector();
}

//#region NSolar class
function NSolar(Latitude = 42.3, theDay = 1, Hour = 14, south = -90, sundistance = 10, sphereSize = 10.0){
    this.rajectory;
    this.latitude = Latitude;
    this.day = theDay;
    this.hour = Hour;

    this.sunDistance = sundistance;
    this.SphereRadius = sphereSize;
    this.southDegree = south;   

    this.VectorOrigin = null;
    this.VectorSun = {x:0,y:0,z:0};

    this.declination = null;
    this.solarAltitude = null;
    this.solarAzimuth = null;
}
NSolar.prototype.Update = function(){
    this.declination = this.Declination(this.day);
    this.solarAltitude = this.SolarAltitude(this.latitude, this.declination, this.hour - 12);
    this.solarAzimuth = this.SolarAzimuth(this.latitude, this.declination, this.hour - 12, this.solarAltitude);

    this.VectorSun.x = Math.cos(NSolar.Radians(this.southDegree - this.solarAzimuth));
    this.VectorSun.y = Math.sin(NSolar.Radians(this.southDegree - this.solarAzimuth));
    this.VectorSun.z = Math.tan(NSolar.Radians(this.solarAltitude));
    this.Normalize();
}
NSolar.prototype.Declination = function(day){
    return (23.45 * Math.sin(NSolar.Radians(360 * (284 + this.day)) / 365.25));
}
NSolar.prototype.SolarAltitude = function(latitude, declination, hour){
    var newValue = (Math.asin(Math.cos(NSolar.Radians(this.latitude)) * Math.cos(NSolar.Radians(this.declination)) * Math.cos(NSolar.Radians(hour * 15)) + Math.sin(NSolar.Radians(this.latitude)) * Math.sin(NSolar.Radians(this.declination))));
    return NSolar.Degrees(newValue);
}
NSolar.prototype.SolarAzimuth = function(latitude, declination, hour, solarAltitude){
    if (this.hour == 0){
        this.hour = 0.1;
    }
    if (this.hour < 0){
        return (-NSolar.Degrees(Math.acos(((Math.sin(NSolar.Radians(this.solarAltitude)) * Math.sin(NSolar.Radians(this.latitude)) - Math.sin(NSolar.Radians(this.declination))) / (Math.cos(NSolar.Radians(this.solarAltitude)) * Math.cos(NSolar.Radians(this.latitude)))))));
    }else{
        return NSolar.Degrees(Math.acos(((Math.sin(NSolar.Radians(this.solarAltitude)) * Math.sin(NSolar.Radians(this.latitude)) - Math.sin(NSolar.Radians(this.declination))) / (Math.cos(NSolar.Radians(this.solarAltitude)) * Math.cos(NSolar.Radians(this.latitude))))));
    }
};
NSolar.prototype.GetSunLine = function(){
    // with NGeo lib
    // var v0 = new NVector(this.VectorSun.x, this.VectorSun.y, this.VectorSun.z);
    // var v1 = new NVector(0, 0, 0);
    // return new NVLine(v0, v1);
    // without the lib
    var v0 = [this.VectorSun.x, this.VectorSun.y, this.VectorSun.z];
    var v1 = [0, 0, 0];
    return [v0, v1];
};
NSolar.prototype.GetSunVector = function(){
    // with NGeo lib
    // return new NVector(this.VectorSun.x, this.VectorSun.y, this.VectorSun.z);
    // without the lib
    return [this.VectorSun.x, this.VectorSun.y, this.VectorSun.z];
};
NSolar.prototype.ToString = function(){
    console.log("Latitude = " + this.latitude + " Day = " + this.day + " Hour = " + this.hour + " { the declination :  '" + this.declination + "' Degree. The Solar Altitude : '" + this.solarAltitude + "' Degree. The Solar Azimuth : '" + this.solarAzimuth + "' Degree. } ");
};
NSolar.prototype.Normalize = function(){
    var len = Math.sqrt((this.VectorSun.x * this.VectorSun.x) + (this.VectorSun.y * this.VectorSun.y) + (this.VectorSun.z * this.VectorSun.z));
    this.VectorSun.x /= len;
    this.VectorSun.y /= len;
    this.VectorSun.z /= len;
};
NSolar.Radians = function(degrees){
    return (degrees * (3.14159265358979 / 180.0)); 
};
NSolar.Degrees = function(radians){
    return (radians * (180.0 / 3.14159265358979 ));
};
//# end region NSolar class