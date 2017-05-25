using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;//ShapefileWorkspaceFactoryClass
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;//IFeatureLayer
using ESRI.ArcGIS.esriSystem;

namespace connectPoint
{
    public partial class Form1 : Form
    {
        string shapeFileFullName = string.Empty;
        string roadDataFullName = string.Empty;
        //IPointCollection pPointColl;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
                IFeatureLayer pFeatureLayer = this.pLayer(roadDataFullName);
                axMapControl1.AddLayer(pFeatureLayer);
                axMapControl1.Refresh();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog pOFD = new OpenFileDialog();
            pOFD.Multiselect = false;
            pOFD.Title = "打开路网文件";
            pOFD.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            pOFD.Filter = "路网文件(*.shp)|*.shp";
            if (pOFD.ShowDialog() == DialogResult.OK)
            {
                roadDataFullName = pOFD.FileName;
                this.textBox1.Text = roadDataFullName;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Shape文件（*.shp)|*.shp";
            DialogResult dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                shapeFileFullName = saveFileDialog.FileName;
            }
            else
            {
                shapeFileFullName = null;
                return;
            }
            this.textBox2.Text = shapeFileFullName;
        }
        private IFeatureLayer pLayer(string roadDataFullName)//IPointCollection  GetAllPoint(string shapeFileFullName)
        {
            int index = roadDataFullName.LastIndexOf('\\');
            string folder = roadDataFullName.Substring(0, index);
            shapeFileFullName = roadDataFullName.Substring(index + 1);
            IWorkspaceFactory pWSF = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace pFWS = (IFeatureWorkspace)pWSF.OpenFromFile(folder, 0);
            
            IFeatureClass pFC = pFWS.OpenFeatureClass(shapeFileFullName);
            IFeatureCursor pFCursor = pFC.Search(null, false);
            IFeature pFeature = pFCursor.NextFeature();
            while (pFeature != null)
            {

                ITopologicalOperator pTO = pFeature.Shape as ITopologicalOperator;
                IPointCollection pPointColl = new PolylineClass();
                IPointCollection pPoints =  pTO.Boundary as IPointCollection;
                pPointColl.AddPointCollection(pPoints );

                IFeature pF = pFC.CreateFeature();
                pF.Shape = pPointColl as IPolyline;//需要是个对象，如IPointCollection pPointColl = new PolylineClass();
                pF.Store();
              //  ICurve pCurve = new PolylineClass();
              //  pCurve = pF as ICurve;
              //  Console.WriteLine(pCurve.Length);
                pFeature = pFCursor.NextFeature();
            }
            IFeatureLayer pFeatureLayer = new FeatureLayerClass();
            pFeatureLayer.FeatureClass = pFC;
            return pFeatureLayer;

        }
    }
}
