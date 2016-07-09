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
        private static void posToPixels(UInt16 iZoneId, UInt16 iMapId, List<Int16> iPosX, List<Int16> iPosY, out List<Int16> oPxlX, out List<Int16> oPxlY)
        {
            oPxlX = new List<short>();
            oPxlY = new List<short>();

            String filter = "ZoneID=" + iZoneId + " AND MapID=" + iMapId;
            MapInfoDS.MapInfoMapsRow[] mapsRows = (MapInfoDS.MapInfoMapsRow[])mapInfoDS.MapInfoMaps.Select(filter);
            if (mapsRows.Length == 0)
            {
                return;
            }
            Single offX = mapsRows[0].X;
            Single offY = mapsRows[0].Y;
            //Y is pixel from the top to 0 (which is how the image is drawn, 0,0 is top/left).
            //iPosY gives a y coordinate going up from 0.
            //So iPosY * mult gives number of pixels ABOVE the offY.
            //Our y pixel is offY - (iPosY * mult).
            Single mult = mapsRows[0].Multiplier;
            for (int ii = 0; ii < iPosX.Count; ii++)
            {
                oPxlX.Add((Int16)(offX + (iPosX[ii] * mult)));
                oPxlY.Add((Int16)(offY - (iPosY[ii] * mult)));
            }
        }
        private static void posToPixels(UInt16 iZoneId, UInt16 iMapId, Int16 iPosX, Int16 iPosY, out Int16 oPxlX, out Int16 oPxlY)
        {
            oPxlX = 0;
            oPxlY = 0;
            String filter = "ZoneID=" + iZoneId + " AND MapID=" + iMapId;
            MapInfoDS.MapInfoMapsRow[] mapsRows = (MapInfoDS.MapInfoMapsRow[])mapInfoDS.MapInfoMaps.Select(filter);
            if (mapsRows.Length == 0)
            {
                return;
            }
            Single offX = mapsRows[0].X;
            Single offY = mapsRows[0].Y;
            Single mult = mapsRows[0].Multiplier;
            oPxlX = (Int16)(offX + (iPosX * mult));
            oPxlY = (Int16)(offY - (iPosY * mult));
        }
        private static void posToPixels(Single iOffX, Single iOffY, Single iMult, Single iPosX, Single iPosY, out Single oPxlX, out Single oPxlY)
        {
            oPxlX = iOffX + (iPosX * iMult);
            oPxlY = iOffY - (iPosY * iMult);
        }
        private static void filterPosPerMap(UInt16 iZoneId, UInt16 iMapId, List<Int16> iPosX, List<Int16> iPosY, out List<Int16> oPosX, out List<Int16> oPosY)
        {
            //This function will parse out any x,y's that do not belong to the specified map.
            oPosX = new List<short>();
            oPosY = new List<short>();

            List<mapBox> mapBoxes = new List<mapBox>();
            String filter = "ZoneID=" + iZoneId + " AND MapID=" + iMapId;
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
            protected const Single ellipsePxlsFish = 6;
            protected const Single ellipsePxlsNodePos = 4;
            protected const Single ellipsePxlsNodeHub = 6;
            protected const Single ellipsePxlsNodeStart = 8;
            protected const Single ellipsePxlsNodeEnd = 8;
            protected const Single ellipsePxlsMob = 5;
            protected const Single ellipsePxlsNpc = 5;
            protected const Single ellipsePxlsPc = 5;
            protected const Single ellipsePxlsPet = 5;
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
            public MapOverlay(Single iPxlX, Single iPxlY, String iText)
            {
                init(iPxlX, iPxlY, iText);
            }
            public MapOverlay(Single iPxlX, Single iPxlY)
            {
                init(iPxlX, iPxlY, "");
            }
            public MapOverlay()
            {
                init(0, 0, "");
            }
            private void init(Single iPxlX, Single iPxlY, String iText)
            {
                pxlX = iPxlX;
                pxlY = iPxlY;
                text = iText;
            }
            protected ItemType type;
            protected Single pxlX;
            protected Single pxlY;
            protected String text;
            protected Boolean showText = false;
            private const Single defScaling = 0.5f;
            protected Single scale = defScaling;
            private Single zoom = 1.0f;
            private PointF mapCenter = new PointF(256, 256);
            public ItemType OLType
            {
                get
                {
                    return type;
                }
            }
            public Single PxlX
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
            public Single PxlY
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
            public virtual Single Scale
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
            public virtual Single Zoom
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
            public virtual String Text
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
            public virtual Boolean ShowText
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
            public void TranslateFullToZoom(Single iX, Single iY, out Single oX, out Single oY)
            {
                Single nbMapPxls = 512 / Zoom;
                Single left = MapCenter.X - nbMapPxls / 2;
                Single top = MapCenter.Y - nbMapPxls / 2;
                oX = (iX - left) / nbMapPxls * 512;
                oY = (iY - top) / nbMapPxls * 512;
            }
        }
        public class EllipseOverlay : MapOverlay
        {
            public EllipseOverlay(Single iPxlX, Single iPxlY, String iText)
                : base(iPxlX, iPxlY, iText)
            {
                this.type = ItemType.ELLIPSE;
            }
            public EllipseOverlay(Single iPxlX, Single iPxlY)
                : base(iPxlX, iPxlY)
            {
                this.type = ItemType.ELLIPSE;
            }
            public EllipseOverlay()
            {
                this.type = ItemType.ELLIPSE;
            }
            protected Single nbPxls = 3;
            public virtual Single NbPxls
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
                Single centerX;
                Single centerY;
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
            public EllipseOverlayLocked(Single iPxlX, Single iPxlY, String iText)
                : base(iPxlX, iPxlY, iText)
            {

            }
            public EllipseOverlayLocked(Single iPxlX, Single iPxlY)
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
            public override Single NbPxls
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
            public EllipseAllFishOverlay(Single iPxlX, Single iPxlY)
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
            public EllipseOneFishOverlay(Single iPxlX, Single iPxlY)
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
            public EllipsePosNodeOverlay(Single iPxlX, Single iPxlY)
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
            public EllipseHubNodeOverlay(Single iPxlX, Single iPxlY)
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
            public EllipseStartNodeOverlay(Single iPxlX, Single iPxlY)
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
            public EllipseEndNodeOverlay(Single iPxlX, Single iPxlY)
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
            public LineOverlay(Single iStartPxlX, Single iStartPxlY, Single iEndPxlX, Single iEndPxlY)
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
            protected Single endPxlX;
            protected Single endPxlY;
            public Single EndPxlX
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
            public Single EndPxlY
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
                    Single xStart, yStart, xEnd, yEnd;
                    TranslateFullToZoom(pxlX, pxlY, out xStart, out yStart);
                    TranslateFullToZoom(endPxlX, endPxlY, out xEnd, out yEnd);
                    iMapGr.DrawLine(pn, xStart, yStart, xEnd, yEnd);
                }
            }
        }
        public abstract class LineOverlayLocked : LineOverlay
        {
            public LineOverlayLocked(Single iStartPxlX, Single iStartPxlY, Single iEndPxlX, Single iEndPxlY)
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
            public LineRouteOverlay(Single iStartPxlX, Single iStartPxlY, Single iEndPxlX, Single iEndPxlY)
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
            public EllipsePCOverlay(Single iPxlX, Single iPxlY, String iText)
                : base(iPxlX, iPxlY, iText)
            {
                init();
            }
            public EllipsePCOverlay(Single iPxlX, Single iPxlY)
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
            public EllipseNPCOverlay(Single iPxlX, Single iPxlY, String iText)
                : base(iPxlX, iPxlY, iText)
            {
                init();
            }
            public EllipseNPCOverlay(Single iPxlX, Single iPxlY)
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
            public EllipseMobOverlay(Single iPxlX, Single iPxlY, String iText)
                : base(iPxlX, iPxlY, iText)
            {
                init();
            }
            public EllipseMobOverlay(Single iPxlX, Single iPxlY)
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
            public EllipsePetOverlay(Single iPxlX, Single iPxlY, String iText)
                : base(iPxlX, iPxlY, iText)
            {
                init();
            }
            public EllipsePetOverlay(Single iPxlX, Single iPxlY)
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
            public LineTargetOverlay(Single iStartPxlX, Single iStartPxlY, Single iEndPxlX, Single iEndPxlY)
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
                Single zoomX, zoomY;
                TranslateFullToZoom(endPxlX, endPxlY, out zoomX, out zoomY);
                iMapGr.DrawEllipse(pn, zoomX - 6, zoomY - 6, 12, 12);
                base.Draw(iMapGr);
            }
        }
        #endregion NPC/PC's
        #region Circles
        public class CircleOverlay : MapOverlay
        {
            public CircleOverlay(Single iCenterPxlX, Single iCenterPxlY, Single iTopLeftPxlX, Single iTopLeftPxlY)
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
            protected Single topLeftPxlX;
            protected Single topLeftPxlY;
            public Single EndPxlX
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
            public Single EndPxlY
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
                    Single xCenter, yCenter, xTopLeft, yTopLeft;
                    TranslateFullToZoom(pxlX, pxlY, out xCenter, out yCenter);
                    TranslateFullToZoom(topLeftPxlX, topLeftPxlY, out xTopLeft, out yTopLeft);
                    Single width = 2 * (xCenter - xTopLeft);
                    Single height = width;
                    iMapGr.DrawEllipse(pn, xTopLeft, yTopLeft, width, height);
                }
            }
        }
        #endregion Circles
        public class ArrowOverlay : MapOverlay
        {
            public ArrowOverlay(UInt16 iX, UInt16 iY, Single iAngle)
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
            private const Int32 xCntrOff = 14;
            private const Int32 yCntrOff = 15;
            private const Single xCntrNudge = -1.5f;
            private const Single yCntrNudge = -1.5f;
            private Single angle;
            public Single Angle
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
            public override Single Scale
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
            private Boolean centerSet = false;
            private Single centerX = 0;
            public Single CenterX
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
            private Single centerY = 0;
            public Single CenterY
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

                Single zoomX, zoomY;
                TranslateFullToZoom(pxlX, pxlY, out zoomX, out zoomY);
                iMapGr.DrawImage(rotatedArrow, new PointF(zoomX - CenterX, zoomY - CenterY));
                iMapGr.FillEllipse(Brushes.Black, zoomX - 1, zoomY - 1, 2, 2);
            }
            private Single xShiftRot = 0;
            private Single yShiftRot = 0;
            private void setCenters()
            {
                if(centerSet)
                {
                    return;
                }
                //Move the image based on how much it will be rotated.
                //This will keep the full image with the top/left at 0,0.
                Single rc = 0;       //rc is the distance to the pointer center when at 0,0.
                Single alphac = 0;   //alphac is the original angle from 0,0 to the pointer center.
                rc = (Single)Math.Sqrt(Math.Pow(xCntrOff, 2) + Math.Pow(yCntrOff, 2));
                alphac = (Single)Math.Atan((Single)((Single)yCntrOff / (Single)xCntrOff));
                Single alphacPrime = alphac + angle;
                Single xRadial = (Single)(rc * Math.Cos(alphacPrime));
                Single yRadial = (Single)(rc * Math.Sin(alphacPrime));

                Single xShiftOrth = 0;
                Single yShiftOrth = 0;

                if ((angle <= 0) && (angle > -Math.PI / 2))
                {
                    //NE Quadrant
                    Single L = (Single)(img.Width * Math.Sin(-angle));
                    xShiftRot = (Single)(-L * Math.Cos(Math.PI / 2 + angle));
                    yShiftRot = (Single)(L * Math.Cos(angle));
                    xShiftOrth = 0;
                    yShiftOrth = (Single)(-img.Width * Math.Sin(angle));
                }
                else if (angle <= -Math.PI / 2)
                {
                    //NW Quadrant
                    Single beta = (Single)(-Math.PI - angle);
                    Single L = (Single)(img.Height * Math.Sin(beta));
                    Single xPrime = (Single)(L * Math.Cos(beta));
                    xShiftRot = xPrime - img.Width;
                    yShiftRot = xPrime / (Single)Math.Tan(-beta);
                    xShiftOrth = (Single)(img.Width * Math.Cos(Math.PI - angle));
                    yShiftOrth = (Single)(img.Height * -Math.Sin(angle + Math.PI / 2));
                    yShiftOrth += (Single)(img.Width * Math.Cos(angle + Math.PI / 2));
                }
                else if ((angle > 0) && (angle < Math.PI / 2))
                {
                    //SE Quadrant
                    Single L = (Single)(img.Height * Math.Sin(angle));
                    xShiftRot = (Single)(L * Math.Cos(angle));
                    yShiftRot = (Single)(-L * Math.Sin(angle));

                    xShiftOrth = (Single)(Math.Sqrt(Math.Pow(xShiftRot, 2) + Math.Pow(yShiftRot, 2)));
                    yShiftOrth = 0;
                }
                else
                {
                    //SW Quadrant
                    Single beta = (Single)(Math.PI - angle);
                    Single L = (Single)(img.Width / Math.Tan(beta));
                    Single M = (Single)(L * Math.Cos(beta));
                    xShiftRot = (Single)(-M * Math.Sin(beta));
                    Single yPrime = (Single)(xShiftRot * Math.Tan(beta));
                    yShiftRot = yPrime - img.Height;

                    xShiftOrth = (Single)(img.Height * Math.Sin(beta) + img.Width * Math.Cos(beta));
                    yShiftOrth = (Single)(img.Height * Math.Cos(beta));
                }

                centerX = xRadial + xShiftOrth;
                centerY = yRadial + yShiftOrth;

                centerSet = true;
            }
            private Image rotate()
            {
                setCenters();
                System.Drawing.Imaging.PixelFormat pf = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                Bitmap newImg = new Bitmap((Int32)(img.Width*1.5), (Int32)(img.Height * 1.5), pf);
                Graphics gr = Graphics.FromImage(newImg);
                gr.Clear(Color.Transparent);
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

                gr.RotateTransform((Single)(angle * 180 / Math.PI));
                gr.TranslateTransform(xShiftRot * scale, yShiftRot * scale);
                gr.DrawImage(img, 0, 0, img.Width * scale, img.Height * scale);
                gr.Dispose();
                return newImg;
            }
        }
    }
}
