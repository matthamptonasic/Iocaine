using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

#pragma warning disable 436

namespace Iocaine2
{
    internal static class Navigation
    {
        #region Math Functions
        #region Heading
        public static float AngleToTarget()
        {
            float myX = MemReads.info_position_x(Iocaine_2_Form.mainProc, Iocaine_2_Form.mainModule);
            float myY = MemReads.info_position_y(Iocaine_2_Form.mainProc, Iocaine_2_Form.mainModule);
            float targetX = MemReads.info_target_position_x(Iocaine_2_Form.mainProc, Iocaine_2_Form.mainModule);
            float targetY = MemReads.info_target_position_y(Iocaine_2_Form.mainProc, Iocaine_2_Form.mainModule);
            if (targetX == myX)
            {
                if (targetY > myY)
                {
                    return (float)(-Math.PI / 4);
                }
                else
                {
                    return (float)(Math.PI / 4);
                }
            }
            float theta = (float)Math.Atan((targetY - myY) / (targetX - myX)); // * (float)Math.PI / 180;
            if (targetX > myX)
            {
                theta = theta * -1;
            }
            else if (targetY > myY)
            {
                theta = -3.1415f - theta;
            }
            else
            {
                theta = (float)Math.PI - theta;
            }
            return theta;
        
        }
        public static float AngleToPosition(float iPosX, float iPosY)
        {
            float myX = MemReads.info_position_x(Iocaine_2_Form.mainProc, Iocaine_2_Form.mainModule);
            float myY = MemReads.info_position_y(Iocaine_2_Form.mainProc, Iocaine_2_Form.mainModule);
            if (iPosX == myX)
            {
                if (iPosY > myY)
                {
                    return (float)(-Math.PI / 4);
                }
                else
                {
                    return (float)(Math.PI / 4);
                }
            }
            float theta = (float)Math.Atan((iPosY - myY) / (iPosX - myX)); // * (float)Math.PI / 180;
            if (iPosX > myX)
            {
                theta = theta * -1;
            }
            else if (iPosY > myY)
            {
                theta = (-1 * (float)Math.PI) - theta;
            }
            else
            {
                theta = (float)Math.PI - theta;
            }
            Iocaine_2_Form.debug("angle to next pos = " + theta);
            return theta;
        }
        #endregion Heading
        #region Distance
        public static float DistanceToPosition(float iPosX, float iPosY)
        {
            float myX = MemReads.info_position_x(Iocaine_2_Form.mainProc, Iocaine_2_Form.mainModule);
            float myY = MemReads.info_position_y(Iocaine_2_Form.mainProc, Iocaine_2_Form.mainModule);
            return (float)Math.Sqrt(Math.Pow(iPosX - myX, 2) + Math.Pow(iPosY - myY, 2));
        }
        #endregion Distance
        #endregion Math Functions
    }
}
