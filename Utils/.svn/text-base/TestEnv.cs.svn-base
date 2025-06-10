using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Utils;
using Microsoft.Xna.Framework;

namespace PraktWS0708.Utils
{
    class TestEnv
    {
        public static void StartTest()
        {
            //CatmullRomFixedTest();
        }
        protected static void CatmullRomFixedTest()
        {
            List<Splines.PositionTangentUpRadius> spline = new List<Splines.PositionTangentUpRadius>();
            Splines.ControlPoint[] cp= new Utils.Splines.ControlPoint[]
								 {
										new PraktWS0708.Utils.Splines.ControlPoint(new Vector3(-10, 10, 0),2),
										new PraktWS0708.Utils.Splines.ControlPoint(new Vector3(0, 0, 10),2),
										new PraktWS0708.Utils.Splines.ControlPoint(new Vector3(10, -10, 0),3),
										new PraktWS0708.Utils.Splines.ControlPoint(new Vector3(20, 0, 0),3),
										new PraktWS0708.Utils.Splines.ControlPoint(new Vector3(10, 10, 0),1),
										new PraktWS0708.Utils.Splines.ControlPoint(new Vector3(0, 0, -10),3),
										new PraktWS0708.Utils.Splines.ControlPoint(new Vector3(-10, -10, 0),3),
										new PraktWS0708.Utils.Splines.ControlPoint(new Vector3(-20, 0, 0),2),
										new PraktWS0708.Utils.Splines.ControlPoint(new Vector3(-10, 10, 0),2)
								 } ;
            Splines.BuildTFList(cp,ref spline,32);
        }
    }

}
