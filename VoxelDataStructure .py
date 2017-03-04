import Rhino as r # using RhinoCommon

class VoxelMap():
    pixels = []
    def Build(self):
        pass

class Pixel():
    p0 = [];
    p1 = [];
    pmid = [];
    i = -1; 
    j = -1;
    k = -1;
    npix = []

    occupancy = 0.0;
    occupancyTemp = 0.0;
    friction = 0.0;
    isSolid = False;
    isFixed = False;
    staticFlow = [0,0,0]
    flow = [0,0,0]

    def GetCenterPt(self):
        return r.Geometry.Point3d(self.pmid[0], self.pmid[1], self.pmid[2])

thePixel = Pixel()
thePixel.pmid = [1,2,0]

a = thePixel.GetCenterPt();




