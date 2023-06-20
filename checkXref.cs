using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;


using Autodesk.AutoCAD.DatabaseServices;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;




[assembly: SecuredApplication(
@"license")]

namespace GetFrameOfRealDwg
{
    class MyHostApplicationServices : Autodesk.AutoCAD.DatabaseServices.HostApplicationServices
    {
        public override System.String FindFile(System.String fileName,
                                                Autodesk.AutoCAD.DatabaseServices.Database database,
                                                 Autodesk.AutoCAD.DatabaseServices.FindFileHint hint
                                                 )
        {

            return string.Empty;
        }
        static public ArrayList GetBlockNames(Database db)
        {
            ArrayList array = new ArrayList();
            Transaction tran = db.TransactionManager.StartTransaction();
            try
            {
                BlockTable bt = (BlockTable)tran.GetObject(db.BlockTableId, OpenMode.ForWrite);
                foreach (ObjectId recordid in bt)
                {
                    BlockTableRecord record = (BlockTableRecord)tran.GetObject(recordid, OpenMode.ForRead);
                    array.Add(record.Name);
                }
            }
            catch
            {
            }
            finally
            {
                tran.Dispose();
            }
            return array;
        }
        static void Main(string[] args)
        {


            MyHostApplicationServices myserver = new MyHostApplicationServices();
            int lcid = 0x00001033; // English
            RuntimeSystem.Initialize(myserver, lcid);
            Database Db = new Database(false, true);
            Db.ReadDwgFile(@"filepath", FileShare.Read, false, "");
            ArrayList ar = GetBlockNames(Db);
            foreach (string str in ar)
            {
                System.Console.WriteLine(str);
            }

            RuntimeSystem.Terminate();
            System.Console.WriteLine();

        }
    }
}
