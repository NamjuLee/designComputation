// NJ Namju Lee (www.njstudio.co.kr)
// nj.namju@gmail.com

import math
import System.DateTime

# input day == List, from 1 to 365 +1 
# =23.45 * SIN( RADIANS(360*(284 + day )/365.25))
def declination(day):
    days = []
    #days = [i for i in range(1, 365+1, 1)]
    days.append(day)
    result = []
    for day in days:
        result.append( 23.45 * math.sin(math.radians(360*(284 + day)/365.25)))
    return result


# input latitude, declinations
# =DEGREES( ASIN( (COS( RADIANS($B$2)) * COS(RADIANS($C10)) *COS( RADIANS($B$3 * 15))) + (SIN(RADIANS($B$2))* SIN(RADIANS($C10))) ))
def solarAltitude(latitude, declinations, hour):
    result = []
    for declination in declinations:
        result.append( math.degrees( math.asin( (math.cos( math.radians(latitude)) * math.cos(math.radians(declination)) *math.cos( math.radians(hour * 15))) + (math.sin(math.radians(latitude))* math.sin(math.radians(declination))) )))
        #result.append( math.degrees( ASIN( (COS( RADIANS(latitude)) * COS(RADIANS(declination)) *COS( RADIANS(hour * 15)))   + (SIN(RADIANS(latitude)) * SIN(RADIANS(declination))) )) )
    return result

# input latitude, declinations
# =DEGREES(ACOS( (SIN(RADIANS($D9)) * SIN(RADIANS($B$2)) - SIN(RADIANS($C9)) ) / (COS(RADIANS($D9)) * COS(RADIANS($B$2)) ) ))
def solarAzimuth(resultLatitude, resultDeclination, resultSolarAltitude): # Solar AzimuthΦ φ (Phi)
    return (math.degrees(math.asin( (math.sin(math.radians(resultSolarAltitude)) * math.sin(math.radians(resultLatitude)) - math.sin(math.radians(resultDeclination)) ) / (math.cos(math.radians(resultSolarAltitude)) * math.cos(math.radians(resultLatitude)) ) )))

# input SurfaceAzimuth, solarAzimuth
# =ABS($B$4+E9)
def SurfaceSolarAzimuth(solarAzimuthSurfaceAzimuth, resultSolarAzimuth):
    result = solarAzimuthSurfaceAzimuth + resultSolarAzimuth
    if result <= 0 : result *= 1
    return result

# input 
# = DEGREES(ATAN(TAN(RADIANS(D9))/COS(RADIANS(F9))))
def profileAngle(resultSolarAltitude, resultSurfaceSolarAzimuth) : #profile angle (Ω ω  Omega)
    return math.degrees(math.atan(math.tan(math.radians(resultSolarAltitude)) / math.cos(math.radians(resultSurfaceSolarAzimuth))))

# parameters
thelatitude = 40
theHour = 3
theSurfaceAzimuth = 30
theDay = 26

stpe1 = declination(theDay)
print "Declination Δ (Delta) =", stpe1

stpe2 = solarAltitude(thelatitude, stpe1, theHour) #latitude, declinations, Hour
print "Solar Altitude β (Beta) =", stpe2

stpe3 = solarAzimuth(thelatitude, stpe1[0], stpe2[0])
print "Solar AzimuthΦ φ (Phi) =", stpe3

stpe4 = SurfaceSolarAzimuth(theSurfaceAzimuth ,stpe3)
print "Surface Solar AzimuthΓ γ  (Gamma) =", stpe4

stpe5 = profileAngle(stpe2[0] ,stpe4)
print "profileAngleΩ ω (Omega) =", stpe5