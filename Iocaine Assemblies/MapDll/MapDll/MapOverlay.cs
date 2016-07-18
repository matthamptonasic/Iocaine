using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Iocaine2.Data.Client
{
    public static partial class Maps
    {
        #region Position <==> Pixel Related Functions
        private static void posToPixels(ushort iZoneId, ushort iMapId, List<short> iPosX, List<short> iPosY, out List<short> oPxlX, out List<short> oPxlY)
        {
            oPxlX = new List<short>();
            oPxlY = new List<short>();

            string filter = "ZoneID=" + iZoneId + " AND MapID=" + iMapId;
            MapInfoDS.MapInfoMapsRow[] mapsRows = (MapInfoDS.MapInfoMapsRow[])mapInfoDS.MapInfoMaps.Select(filter);
            if (mapsRows.Length == 0)
            {
                return;
            }
            float offX = mapsRows[0].X;
            float offY = mapsRows[0].Y;
            //Y is pixel from the top to 0 (which is how the image is drawn, 0,0 is top/left).
            //iPosY gives a y coordinate going up from 0.
            //So iPosY * mult gives number of pixels ABOVE the offY.
            //Our y pixel is offY - (iPosY * mult).
            float mult = mapsRows[0].Multiplier;
            for (int ii = 0; ii < iPosX.Count; ii++)
            {
                oPxlX.Add((short)(offX + (iPosX[ii] * mult)));
                oPxlY.Add((short)(offY - (iPosY[ii] * mult)));
            }
        }
        private static void posToPixels(ushort iZoneId, ushort iMapId, short iPosX, short iPosY, out short oPxlX, out short oPxlY)
        {
            oPxlX = 0;
            oPxlY = 0;
            string filter = "ZoneID=" + iZoneId + " AND MapID=" + iMapId;
            MapInfoDS.MapInfoMapsRow[] mapsRows = (MapInfoDS.MapInfoMapsRow[])mapInfoDS.MapInfoMaps.Select(filter);
            if (mapsRows.Length == 0)
            {
                return;
            }
            float offX = mapsRows[0].X;
            float offY = mapsRows[0].Y;
            float mult = mapsRows[0].Multiplier;
            oPxlX = (short)(offX + (iPosX * mult));
            oPxlY = (short)(offY - (iPosY * mult));
        }
        private static void posToPixels(float iOffX, float iOffY, float iMult, float iPosX, float iPosY, out float oPxlX, out float oPxlY)
        {
            oPxlX = iOffX + (iPosX * iMult);
            oPxlY = iOffY - (iPosY * iMult);
        }
        private static void filterPosPerMap(ushort iZoneId, ushort iMapId, List<short> iPosX, List<short> iPosY, out List<short> oPosX, out List<short> oPosY)
        {
            //This function will parse out any x,y's that do not belong to the specified map.
            oPosX = new List<short>();
            oPosY = new List<short>();

            List<mapBox> mapBoxes = new List<mapBox>();
            string filter = "ZoneID=" + iZoneId + " AND MapID=" + iMapId;
            MapInfoDS.MapInfoBoxesRow[] mapsRows = (MapInfoDS.MapInfoBoxesRow[])mapInfoDS.MapInfoBoxes.Select(filter);
            if (mapsRows.Length == 0)
            {
                return;
            }
            foreach (MapInfoDS.MapInfoBoxesRow row in mapsRows)
            {
                mapBox box = new mapBox();
                box.x1 = row.X1;
                box.x2 = row.X2;
                box.y1 = row.Y1;
                box.y2 = row.Y2;
                box.z1 = row.Z1;
                box.z2 = row.Z2;
                mapBoxes.Add(box);
            }
            for (int ii = 0; ii < iPosX.Count; ii++)
            {
                //Go through each box and, if the x/y falls within a box, add it.
                foreach (mapBox box in mapBoxes)
                {
                    if ((box.x1 <= iPosX[ii]) && (box.x2 >= iPosX[ii]) && (box.y1 <= iPosY[ii]) && (box.y2 >= iPosY[ii]))
                    {
                        oPosX.Add(iPosX[ii]);
                        oPosY.Add(iPosY[ii]);
                        break;
                    }
                }
            }
        }
        #endregion Position <==> Pixel Related Functions

        public abstract class MapOverlay
        {
            protected static Brush brushAllFish = new SolidBrush(Color.Blue);
            protected static Brush brushOneFish = new SolidBrush(Color.Magenta);
            protected static Brush brushNodePos = new SolidBrush(Color.Green);
            protected static Brush brushNodeHub = new SolidBrush(Color.Turquoise);
            protected static Brush brushNodeStart = new SolidBrush(Color.Violet);
            protected static Brush brushNodeEnd = new SolidBrush(Color.Red);
            protected static Brush brushMob = new SolidBrush(Color.DarkRed);
            protected static Brush brushMobClaimedYou = new SolidBrush(Color.Red);
            protected static Brush brushMobClaimedOther = new SolidBrush(Color.Purple);
            protected static Brush brushNpc = new SolidBrush(Color.LawnGreen);
            protected static Brush brushPc = new SolidBrush(Color.MediumSlateBlue);
            protected static Brush brushPet = new SolidBrush(Color.LightGoldenrodYellow);
            protected static Pen penRouteLine = new Pen(Color.White, 2);
            protected static Pen penRangeCircle = new Pen(Color.Blue, 3);
            protected static Pen penTgtNpc = new Pen(Color.Green, 1);
            protected static Pen penTgtPc = new Pen(Color.MediumBlue, 1);
            protected static Pen penTgtMob = new Pen(Color.Red, 1);
            protected static Pen penTgtPet = new Pen(Color.Gold, 1);
            protected const float ellipsePxlsFish = 6;
            protected const float ellipsePxlsNodePos = 4;
            protected const float ellipsePxlsNodeHub = 6;
            protected const float ellipsePxlsNodeStart = 8;
            protected const float ellipsePxlsNodeEnd = 8;
            protected const float ellipsePxlsMob = 5;
            protected const float ellipsePxlsNpc = 5;
            protected const float ellipsePxlsPc = 5;
            protected const float ellipsePxlsPet = 5;
            public enum ItemType
            {
                ARROW,
                ELLIPSE,
                ELLIPSE_ALL_FISH,
                ELLIPSE_ONE_FISH,
                CIRCLE,
                POS_NODE,
                HUB_NODE,
                START_NODE,
                END_NODE,
                LINE,
                LINE_ROUTE,
                LINE_TARGET,
                NPC,
                MONSTER,
                PC,
                PET
            }
            public MapOverlay(float iPxlX, float iPxlY, string iText)
            {
                init(iPxlX, iPxlY, iText);
            }
            public MapOverlay(float iPxlX, float iPxlY)
            {
                init(iPxlX, iPxlY, "");
            }
            public MapOverlay()
            {
                init(0, 0, "");
            }
            private void init(float iPxlX, float iPxlY, string iText)
            {
                pxlX = iPxlX;
                pxlY = iPxlY;
                text = iText;
            }
            protected ItemType type;
            protected float pxlX;
            protected float pxlY;
            protected string text;
            protected bool showText = false;
            private const float defScaling = 0.5f;
            protected float scale = defScaling;
            private float zoom = 1.0f;
            private PointF mapCenter = new PointF(256, 256);
            public ItemType OLType
            {
                get
                {
                    return type;
                }
            }
            public float PxlX
            {
                get
                {
                    return pxlX;
                }
                set
                {
                    pxlX = value;
                }
            }
            public float PxlY
            {
                get
                {
                    return pxlY;
                }
                set
                {
                    pxlY = value;
                }
            }
            public virtual float Scale
            {
                get
                {
                    return scale;
                }
                set
                {
                    scale = value;
                }
            }
            public virtual float Zoom
            {
                get
                {
                    return zoom;
                }
                set
                {
                    zoom = value;
                }
            }
            public virtual PointF MapCenter
            {
                get
                {
                    return mapCenter;
                }
                set
                {
                    mapCenter = value;
                }
            }
            public virtual string Text
            {
                get
                {
                    return text;
                }
                set
                {
                    text = value;
                }
            }
            public virtual bool ShowText
            {
                get
                {
                    return showText;
                }
                set
                {
                    showText = value;
                }
            }
            public abstract void Draw(Graphics iMapGr);
            public void Draw(Image iMap)
            {
                Graphics mapGr = Graphics.FromImage(iMap);
                Draw(mapGr);
                mapGr.Dispose();
            }
            public void TranslateFullToZoom(float iX, float iY, out float oX, out float oY)
            {
                float nbMapPxls = 512 / Zoom;
                float left = MapCenter.X - nbMapPxls / 2;
                float top = MapCenter.Y - nbMapPxls / 2;
                oX = (iX - left) / nbMapPxls * 512;
                oY = (iY - top) / nbMapPxls * 512;
            }
        }
        public class EllipseOverlay : MapOverlay
        {
            public EllipseOverlay(float iPxlX, float iPxlY, string iText)
                : base(iPxlX, iPxlY, iText)
            {
                this.type = ItemType.ELLIPSE;
            }
            public EllipseOverlay(float iPxlX, float iPxlY)
                : base(iPxlX, iPxlY)
            {
                this.type = ItemType.ELLIPSE;
            }
            public EllipseOverlay()
            {
                this.type = ItemType.ELLIPSE;
            }
            protected float nbPxls = 3;
            public virtual float NbPxls
            {
                get
                {
                    return nbPxls;
                }
                set
                {
                    nbPxls = value;
                }
            }
            protected Brush brsh;
            public virtual Brush Brsh
            {
                get
                {
                    return brsh;
                }
                set
                {
                    brsh = value;
                }
            }
            public Brush textBrsh;
            public virtual Brush TextBrsh
            {
                get
                {
                    return textBrsh;
                }
                set
                {
                    textBrsh = value;
                }
            }
            public override void Draw(Graphics iMapGr)
            {
                float centerX;
                float centerY;
                TranslateFullToZoom(pxlX, pxlY, out centerX, out centerY);
                iMapGr.FillEllipse(brsh, centerX - nbPxls / 2, centerY - nbPxls / 2, nbPxls, nbPxls);
                if(showText && (text != ""))
                {
                    iMapGr.DrawString(text, new Font(FontFamily.GenericSansSerif, 9, FontStyle.Regular),
                                      textBrsh, centerX + 2, centerY + 2);
                }
            }
        }
        public abstract class EllipseOverlayLocked : EllipseOverlay
        {
            public EllipseOverlayLocked(float iPxlX, float iPxlY, string iText)
                : base(iPxlX, iPxlY, iText)
            {

            }
            public EllipseOverlayLocked(float iPxlX, float iPxlY)
                : base(iPxlX, iPxlY)
            {
                
            }
            public EllipseOverlayLocked() { }
            public override Brush Brsh
            {
                get
                {
                    return (Brush)brsh.Clone();
                }
            }
            public override Brush TextBrsh
            {
                get
                {
                    return (Brush)textBrsh.Clone();
                }
            }
            public override float NbPxls
            {
                get
                {
                    return nbPxls;
                }
            }
        }
        #region Fish
        public sealed class EllipseAllFishOverlay : EllipseOverlayLocked
        {
            public EllipseAllFishOverlay(float iPxlX, float iPxlY)
                : base(iPxlX, iPxlY, "")
            {
                init();
            }
            public EllipseAllFishOverlay()
            {
                init();
            }
            private void init()
            {
                this.type = ItemType.ELLIPSE_ALL_FISH;
                base.brsh = MapOverlay.brushAllFish;
                base.nbPxls = MapOverlay.ellipsePxlsFish;
            }
        }
        public sealed class EllipseOneFishOverlay : EllipseOverlayLocked
        {
            public EllipseOneFishOverlay(float iPxlX, float iPxlY)
                : base(iPxlX, iPxlY, "")
            {
                init();
            }
            public EllipseOneFishOverlay()
            {
                init();
            }
            private void init()
            {
                this.type = ItemType.ELLIPSE_ONE_FISH;
                base.brsh = MapOverlay.brushOneFish;
                base.nbPxls = MapOverlay.ellipsePxlsFish;
            }
        }
        #endregion Fish
        #region Routes
        public sealed class EllipsePosNodeOverlay : EllipseOverlayLocked
        {
            public EllipsePosNodeOverlay(float iPxlX, float iPxlY)
                : base(iPxlX, iPxlY, "")
            {
                init();
            }
            public EllipsePosNodeOverlay()
            {
                init();
            }
            private void init()
            {
                type = ItemType.POS_NODE;
                brsh = MapOverlay.brushNodePos;
                nbPxls = MapOverlay.ellipsePxlsNodePos;
            }
        }
        public sealed class EllipseHubNodeOverlay : EllipseOverlayLocked
        {
            public EllipseHubNodeOverlay(float iPxlX, float iPxlY)
                : base(iPxlX, iPxlY, "")
            {
                init();
            }
            public EllipseHubNodeOverlay()
            {
                init();
            }
            private void init()
            {
                this.type = ItemType.HUB_NODE;
                base.brsh = MapOverlay.brushNodeHub;
                base.nbPxls = MapOverlay.ellipsePxlsNodeHub;
            }
        }
        public sealed class EllipseStartNodeOverlay : EllipseOverlayLocked
        {
            public EllipseStartNodeOverlay(float iPxlX, float iPxlY)
                : base(iPxlX, iPxlY, "")
            {
                init();
            }
            public EllipseStartNodeOverlay()
            {
                init();
            }
            private void init()
            {
                this.type = ItemType.START_NODE;
                base.brsh = MapOverlay.brushNodeStart;
                base.nbPxls = MapOverlay.ellipsePxlsNodeStart;
            }
        }
        public sealed class EllipseEndNodeOverlay : EllipseOverlayLocked
        {
            public EllipseEndNodeOverlay(float iPxlX, float iPxlY)
                : base(iPxlX, iPxlY, "")
            {
                init();
            }
            public EllipseEndNodeOverlay()
            {
                init();
            }
            private void init()
            {
                this.type = ItemType.END_NODE;
                base.brsh = MapOverlay.brushNodeEnd;
                base.nbPxls = MapOverlay.ellipsePxlsNodeEnd;
            }
        }
        public class LineOverlay : MapOverlay
        {
            public LineOverlay(float iStartPxlX, float iStartPxlY, float iEndPxlX, float iEndPxlY)
                : base(iStartPxlX, iStartPxlY, "")
            {
                this.type = ItemType.LINE;
                endPxlX = iEndPxlX;
                endPxlY = iEndPxlY;
            }
            public LineOverlay()
            {
                this.type = ItemType.LINE;
            }
            protected float endPxlX;
            protected float endPxlY;
            public float EndPxlX
            {
                get
                {
                    return endPxlX;
                }
                set
                {
                    endPxlX = value;
                }
            }
            public float EndPxlY
            {
                get
                {
                    return endPxlY;
                }
                set
                {
                    endPxlY = value;
                }
            }
            protected Pen pn;
            public virtual Pen Pn
            {
                get
                {
                    return pn;
                }
                set
                {
                    pn = value;
                }
            }
            public override void Draw(Graphics iMapGr)
            {
                if(pn != null)
                {
                    float xStart, yStart, xEnd, yEnd;
                    TranslateFullToZoom(pxlX, pxlY, out xStart, out yStart);
                    TranslateFullToZoom(endPxlX, endPxlY, out xEnd, out yEnd);
                    iMapGr.DrawLine(pn, xStart, yStart, xEnd, yEnd);
                }
            }
        }
        public abstract class LineOverlayLocked : LineOverlay
        {
            public LineOverlayLocked(float iStartPxlX, float iStartPxlY, float iEndPxlX, float iEndPxlY)
                : base(iStartPxlX, iStartPxlY, iEndPxlX, iEndPxlY)
            {

            }
            public LineOverlayLocked() { }
            public override Pen Pn
            {
                get
                {
                    return (Pen)pn.Clone();
                }
            }

        }
        public sealed class LineRouteOverlay : LineOverlayLocked
        {
            public LineRouteOverlay(float iStartPxlX, float iStartPxlY, float iEndPxlX, float iEndPxlY)
                : base(iStartPxlX, iStartPxlY, iEndPxlX, iEndPxlY)
            {
                init();
            }
            public LineRouteOverlay()
            {
                init();
            }
            private void init()
            {
                base.type = ItemType.LINE_ROUTE;
                base.pn = MapOverlay.penRouteLine;
            }
            public override void Draw(Graphics iMapGr)
            {
                base.Draw(iMapGr);
            }
        }
        #endregion Routes
        #region NPC/PC's
        public class EllipsePCOverlay : EllipseOverlayLocked
        {
            public EllipsePCOverlay(float iPxlX, float iPxlY, string iText)
                : base(iPxlX, iPxlY, iText)
            {
                init();
            }
            public EllipsePCOverlay(float iPxlX, float iPxlY)
                : base(iPxlX, iPxlY, "")
            {
                init();
            }
            public EllipsePCOverlay()
            {
                init();
            }
            private void init()
            {
                type = ItemType.PC;
                brsh = MapOverlay.brushPc;
                textBrsh = new SolidBrush(Color.Black);
                nbPxls = MapOverlay.ellipsePxlsPc;
            }
        }
        public class EllipseNPCOverlay : EllipseOverlayLocked
        {
            public EllipseNPCOverlay(float iPxlX, float iPxlY, string iText)
                : base(iPxlX, iPxlY, iText)
            {
                init();
            }
            public EllipseNPCOverlay(float iPxlX, float iPxlY)
                : base(iPxlX, iPxlY, "")
            {
                init();
            }
            public EllipseNPCOverlay()
            {
                init();
            }
            private void init()
            {
                type = ItemType.NPC;
                brsh = MapOverlay.brushNpc;
                textBrsh = new SolidBrush(Color.Black);
                nbPxls = MapOverlay.ellipsePxlsNpc;
            }
        }
        public class EllipseMobOverlay : EllipseOverlayLocked
        {
            public enum Claim : byte
            {
                None,
                You,
                Party,
                Other
            }
            public EllipseMobOverlay(float iPxlX, float iPxlY, string iText)
                : base(iPxlX, iPxlY, iText)
            {
                init();
            }
            public EllipseMobOverlay(float iPxlX, float iPxlY)
                : base(iPxlX, iPxlY, "")
            {
                init();
            }
            public EllipseMobOverlay()
            {
                init();
            }
            private void init()
            {
                type = ItemType.MONSTER;
                brsh = MapOverlay.brushMob;
                textBrsh = new SolidBrush(Color.Black);
                nbPxls = MapOverlay.ellipsePxlsMob;
            }
            private Claim hasClaim = Claim.None;
            public Claim HasClaim
            {
                get
                {
                    return hasClaim;
                }
                set
                {
                    hasClaim = value;
                    switch(hasClaim)
                    {
                        case Claim.None:
                            brsh = MapOverlay.brushMob;
                            break;
                        case Claim.You:
                            brsh = MapOverlay.brushMobClaimedYou;
                            textBrsh = MapOverlay.brushMobClaimedYou;
                            break;
                        case Claim.Party:
                            brsh = MapOverlay.brushMobClaimedYou;
                            textBrsh = MapOverlay.brushMobClaimedYou;
                            break;
                        case Claim.Other:
                            brsh = MapOverlay.brushMobClaimedOther;
                            textBrsh = MapOverlay.brushMobClaimedOther;
                            break;
                        default:
                            brsh = MapOverlay.brushMob;
                            break;
                    }
                }
            }
        }
        public class EllipsePetOverlay : EllipseOverlayLocked
        {
            public EllipsePetOverlay(float iPxlX, float iPxlY, string iText)
                : base(iPxlX, iPxlY, iText)
            {
                init();
            }
            public EllipsePetOverlay(float iPxlX, float iPxlY)
                : base(iPxlX, iPxlY, "")
            {
                init();
            }
            public EllipsePetOverlay()
            {
                init();
            }
            private void init()
            {
                type = ItemType.PET;
                brsh = MapOverlay.brushPet;
                textBrsh = new SolidBrush(Color.Black);
                nbPxls = MapOverlay.ellipsePxlsPet;
            }
        }
        public class LineTargetOverlay : LineOverlayLocked
        {
            public enum TgtType : byte
            {
                None,
                NPC,
                PC,
                Mob,
                Pet
            }
            public LineTargetOverlay(float iStartPxlX, float iStartPxlY, float iEndPxlX, float iEndPxlY)
                : base(iStartPxlX, iStartPxlY, iEndPxlX, iEndPxlY)
            {
                init();
            }
            public LineTargetOverlay()
            {
                init();
            }
            private void init()
            {
                base.type = ItemType.LINE_TARGET;
                base.pn = MapOverlay.penRouteLine;
            }
            private TgtType targetType = TgtType.None;
            public TgtType TargetType
            {
                get
                {
                    return targetType;
                }
                set
                {
                    targetType = value;
                    switch(targetType)
                    {
                        case TgtType.NPC:
                            pn = MapOverlay.penTgtNpc;
                            break;
                        case TgtType.PC:
                            pn = MapOverlay.penTgtPc;
                            break;
                        case TgtType.Mob:
                            pn = MapOverlay.penTgtMob;
                            break;
                        case TgtType.Pet:
                            pn = MapOverlay.penTgtPet;
                            break;
                        default:
                            pn = MapOverlay.penRouteLine;
                            break;
                    }
                }
            }
            public override void Draw(Graphics iMapGr)
            {
                float zoomX, zoomY;
                TranslateFullToZoom(endPxlX, endPxlY, out zoomX, out zoomY);
                iMapGr.DrawEllipse(pn, zoomX - 6, zoomY - 6, 12, 12);
                base.Draw(iMapGr);
            }
        }
        #endregion NPC/PC's
        #region Circles
        public class CircleOverlay : MapOverlay
        {
            public CircleOverlay(float iCenterPxlX, float iCenterPxlY, float iTopLeftPxlX, float iTopLeftPxlY)
                : base(iCenterPxlX, iCenterPxlY, "")
            {
                topLeftPxlX = iTopLeftPxlX;
                topLeftPxlY = iTopLeftPxlY;
                init();
            }
            public CircleOverlay()
            {
                init();
            }
            private void init()
            {
                this.type = ItemType.CIRCLE;
                pn = penRangeCircle;
            }
            protected float topLeftPxlX;
            protected float topLeftPxlY;
            public float EndPxlX
            {
                get
                {
                    return topLeftPxlX;
                }
                set
                {
                    topLeftPxlX = value;
                }
            }
            public float EndPxlY
            {
                get
                {
                    return topLeftPxlY;
                }
                set
                {
                    topLeftPxlY = value;
                }
            }
            protected Pen pn;
            public virtual Pen Pn
            {
                get
                {
                    return pn;
                }
                set
                {
                    pn = value;
                }
            }
            public override void Draw(Graphics iMapGr)
            {
                if (pn != null)
                {
                    float xCenter, yCenter, xTopLeft, yTopLeft;
                    TranslateFullToZoom(pxlX, pxlY, out xCenter, out yCenter);
                    TranslateFullToZoom(topLeftPxlX, topLeftPxlY, out xTopLeft, out yTopLeft);
                    float width = 2 * (xCenter - xTopLeft);
                    float height = width;
                    iMapGr.DrawEllipse(pn, xTopLeft, yTopLeft, width, height);
                }
            }
        }
        #endregion Circles
        public class ArrowOverlay : MapOverlay
        {
            public ArrowOverlay(ushort iX, ushort iY, float iAngle)
            {
                this.pxlX = iX;
                this.pxlY = iY;
                this.angle = iAngle;
                init();
            }
            public ArrowOverlay()
            {
                init();
            }
            private void init()
            {
                this.type = ItemType.ARROW;
                System.Reflection.Assembly thisExe = System.Reflection.Assembly.GetExecutingAssembly();
                System.IO.Stream file = thisExe.GetManifestResourceStream("Iocaine2.Images.MapPointer.png");
                img = Image.FromStream(file);
                text = "";
            }
            private const int xCntrOff = 14;
            private const int yCntrOff = 15;
            private const float xCntrNudge = -1.5f;
            private const float yCntrNudge = -1.5f;
            private float angle;
            public float Angle
            {
                get
                {
                    return angle;
                }
                set
                {
                    angle = value;
                    centerSet = false;
                }
            }
            public override float Scale
            {
                get
                {
                    return scale;
                }
                set
                {
                    scale = value;
                    centerSet = false;
                }
            }
            private Image img = null;
            public Image Img
            {
                get
                {
                    return rotate();
                }
            }
            private bool centerSet = false;
            private float centerX = 0;
            public float CenterX
            {
                get
                {
                    if(!centerSet)
                    {
                        setCenters();
                    }
                    return (centerX + xCntrNudge) * scale;
                }
            }
            private float centerY = 0;
            public float CenterY
            {
                get
                {
                    if (!centerSet)
                    {
                        setCenters();
                    }
                    return (centerY + yCntrNudge) * scale;
                }
            }
            public override void Draw(Graphics iMapGr)
            {
                iMapGr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

                Image rotatedArrow = rotate();

                float zoomX, zoomY;
                TranslateFullToZoom(pxlX, pxlY, out zoomX, out zoomY);
                iMapGr.DrawImage(rotatedArrow, new PointF(zoomX - CenterX, zoomY - CenterY));
                iMapGr.FillEllipse(Brushes.Black, zoomX - 1, zoomY - 1, 2, 2);
            }
            private float xShiftRot = 0;
            private float yShiftRot = 0;
            private void setCenters()
            {
                if(centerSet)
                {
                    return;
                }
                //Move the image based on how much it will be rotated.
                //This will keep the full image with the top/left at 0,0.
                float rc = 0;       //rc is the distance to the pointer center when at 0,0.
                float alphac = 0;   //alphac is the original angle from 0,0 to the pointer center.
                rc = (float)Math.Sqrt(Math.Pow(xCntrOff, 2) + Math.Pow(yCntrOff, 2));
                alphac = (float)Math.Atan((float)((float)yCntrOff / (float)xCntrOff));
                float alphacPrime = alphac + angle;
                float xRadial = (float)(rc * Math.Cos(alphacPrime));
                float yRadial = (float)(rc * Math.Sin(alphacPrime));

                float xShiftOrth = 0;
                float yShiftOrth = 0;

                if ((angle <= 0) && (angle > -Math.PI / 2))
                {
                    //NE Quadrant
                    float L = (float)(img.Width * Math.Sin(-angle));
                    xShiftRot = (float)(-L * Math.Cos(Math.PI / 2 + angle));
                    yShiftRot = (float)(L * Math.Cos(angle));
                    xShiftOrth = 0;
                    yShiftOrth = (float)(-img.Width * Math.Sin(angle));
                }
                else if (angle <= -Math.PI / 2)
                {
                    //NW Quadrant
                    float beta = (float)(-Math.PI - angle);
                    float L = (float)(img.Height * Math.Sin(beta));
                    float xPrime = (float)(L * Math.Cos(beta));
                    xShiftRot = xPrime - img.Width;
                    yShiftRot = xPrime / (float)Math.Tan(-beta);
                    xShiftOrth = (float)(img.Width * Math.Cos(Math.PI - angle));
                    yShiftOrth = (float)(img.Height * -Math.Sin(angle + Math.PI / 2));
                    yShiftOrth += (float)(img.Width * Math.Cos(angle + Math.PI / 2));
                }
                else if ((angle > 0) && (angle < Math.PI / 2))
                {
                    //SE Quadrant
                    float L = (float)(img.Height * Math.Sin(angle));
                    xShiftRot = (float)(L * Math.Cos(angle));
                    yShiftRot = (float)(-L * Math.Sin(angle));

                    xShiftOrth = (float)(Math.Sqrt(Math.Pow(xShiftRot, 2) + Math.Pow(yShiftRot, 2)));
                    yShiftOrth = 0;
                }
                else
                {
                    //SW Quadrant
                    float beta = (float)(Math.PI - angle);
                    float L = (float)(img.Width / Math.Tan(beta));
                    float M = (float)(L * Math.Cos(beta));
                    xShiftRot = (float)(-M * Math.Sin(beta));
                    float yPrime = (float)(xShiftRot * Math.Tan(beta));
                    yShiftRot = yPrime - img.Height;

                    xShiftOrth = (float)(img.Height * Math.Sin(beta) + img.Width * Math.Cos(beta));
                    yShiftOrth = (float)(img.Height * Math.Cos(beta));
                }

                centerX = xRadial + xShiftOrth;
                centerY = yRadial + yShiftOrth;

                centerSet = true;
            }
            private Image rotate()
            {
                setCenters();
                System.Drawing.Imaging.PixelFormat pf = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                Bitmap newImg = new Bitmap((int)(img.Width*1.5), (int)(img.Height * 1.5), pf);
                Graphics gr = Graphics.FromImage(newImg);
                gr.Clear(Color.Transparent);
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

                gr.RotateTransform((float)(angle * 180 / Math.PI));
                gr.TranslateTransform(xShiftRot * scale, yShiftRot * scale);
                gr.DrawImage(img, 0, 0, img.Width * scale, img.Height * scale);
                gr.Dispose();
                return newImg;
            }
        }
    }
}
