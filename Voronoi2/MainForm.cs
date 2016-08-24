/*
 * Created by SharpDevelop.
 * User: Burhan
 * Date: 11/05/2014
 * Time: 01:02 ص
 * 
 * C# Program by Burhan Joukhadar
 * Permission to use, copy, modify, and distribute this software for any
 * purpose without fee is hereby granted, provided that this entire notice
 * is included in all copies of any software which is or includes a copy
 * or modification of this software and in all copies of the supporting
 * documentation for such software.
 * THIS SOFTWARE IS BEING PROVIDED "AS IS", WITHOUT ANY EXPRESS OR IMPLIED
 * WARRANTY.  IN PARTICULAR, NEITHER THE AUTHORS NOR AT&T MAKE ANY
 * REPRESENTATION OR WARRANTY OF ANY KIND CONCERNING THE MERCHANTABILITY
 * OF THIS SOFTWARE OR ITS FITNESS FOR ANY PARTICULAR PURPOSE.
 * 
 * This is a program that draws Voronoi Diagram using Fortune's Algorithm
 * This program is to evaluate the and view the resulting voronoi diagram
 * Also it gives an example of how to use the voronoi object, it's not optimized actually it's rushed.
 */

using System;
using System.Collections.Generic;
using CSPoint = System.Drawing.Point; // "Point" بسبب وجود تضارب في اسم النوع
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Voronoi2
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		Bitmap bitmap;
		Graphics Graphics;
		Random seeder;
		Voronoi voroObject;

        int canvasSize = 500;
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			MainPictureBox.AutoSize = true;

            bitmap = new Bitmap (canvasSize, canvasSize);
			
			Graphics = Graphics.FromImage (bitmap);
			Graphics.SmoothingMode = SmoothingMode.HighQuality;
			Graphics.Clear (Color.LightGray);

            MainPictureBox.Image = bitmap;

            this.AutoSize = true;
			
			voroObject = new Voronoi ( 0.1 );

		}

        void RunVoronoi()
        {
            //This is the main 
            Graphics.Clear(Color.White);

            List<PointF> sites = new List<PointF>();
            int NumDataPoints = (int)numericUpDown1.Value;

            if (checkUseStdData.Checked == false)
            {
                sites = GenerateRandomSites(NumDataPoints);
            }
            else
            {
                sites = GenerateFixedSites();
            }


            Graphics = DrawAxis(bitmap.Width, bitmap.Height, Graphics);

            Graphics = DrawSites(sites, bitmap.Width/2, bitmap.Height/2, Graphics);

            List<GraphEdge> VoronoiEdges = MakeVoronoiGraph(sites, bitmap.Width, bitmap.Height);

            Graphics = DrawEdges(VoronoiEdges, bitmap.Width / 2, bitmap.Height / 2, Graphics);

            GeneratePolygons(VoronoiEdges, sites.Count);

            MainPictureBox.Image = bitmap;

        }

        List<GraphEdge> MakeVoronoiGraph(List<PointF> sites, int width, int height)
        {
            double[] xVal = new double[sites.Count];
            double[] yVal = new double[sites.Count];
            for (int i = 0; i < sites.Count; i++)
            {
                xVal[i] = sites[i].X;
                yVal[i] = sites[i].Y;
            }
            //return voroObject.generateVoronoi(xVal, yVal, 0, width, 0, height);
            return voroObject.generateVoronoi(xVal, yVal, -width/2, width/2, -height/2, height/2);

        }

        private List<PointF> GenerateRandomSites(int SiteCount)
        {

            List<PointF> sites = new List<PointF>();

            seeder = new Random();
            int seed = seeder.Next();
            Random rand = new Random(seed);

            if (checkUseStdData.Checked == false)
            {
                for (int i = 0; i < SiteCount; i++)
                {
                    sites.Add(new PointF(
                                            (float)(rand.NextDouble() * canvasSize) - canvasSize/2,
                                            (float)(rand.NextDouble() * canvasSize) - canvasSize/2
                                        ) );
                }
            }

            return sites;

        }

        private List<PointF> GenerateFixedSites()
        {

            List<PointF> sites = new List<PointF>();

            float multiplier = 10;

            /*
            sites.Add(new PointF((float)  7.5 * multiplier, (float)  6.0 * multiplier) );
            sites.Add(new PointF((float)  8.0 * multiplier, (float) 16.5 * multiplier) );
            sites.Add(new PointF((float) 14.0 * multiplier, (float) 20.0 * multiplier) );
            sites.Add(new PointF((float) 18.5 * multiplier, (float) 11.0 * multiplier) );
            */

            
            sites.Add(new PointF((float)0 * multiplier, (float)0 * multiplier));
            sites.Add(new PointF((float)5 * multiplier, (float)1 * multiplier));
            sites.Add(new PointF((float)1 * multiplier, (float)-1* multiplier));
            sites.Add(new PointF((float)-1 * multiplier, (float)1 * multiplier));
            sites.Add(new PointF((float)-5 * multiplier, (float)-1 * multiplier));
            
            /*
            sites.Add(new PointF((float)0 * multiplier, (float)0 * multiplier));
            sites.Add(new PointF((float)1 * multiplier, (float)1 * multiplier));
            sites.Add(new PointF((float)1 * multiplier, (float)2 * multiplier));
            sites.Add(new PointF((float)2 * multiplier, (float)1 * multiplier));
            sites.Add(new PointF((float)2 * multiplier, (float)2 * multiplier));
            */

            return sites;

        }

        Graphics DrawSites ( List<PointF> SiteList, int AxisX, int AxisY, Graphics g )
        {

            //Need to translate to the Axis on the Chart
            
            // Draw the sites on the canvas
            for (int i = 0; i < SiteList.Count; i++)
            {
               
                float PointSize = 4;

                Brush PointColour = new SolidBrush(Color.BlueViolet);

                //The X and Y need to be afdjusted by half of the size of the pixel
                int x = AxisX + (int)( SiteList[i].X + 0.5 );
                    x = x - (int)( PointSize / 2 );
                int y = AxisY - (int)( SiteList[i].Y + 0.5 );
                    y = y - (int)(PointSize / 2);

                int w = (int)PointSize;
                int h = (int)PointSize;
                
                g.FillEllipse( PointColour, x, y, w, h );

                //Some Dubug for the Sites Locations
                if (numericUpDown1.Value < 50)
                {
                    richTextBox1.Text += "Site: " + i + "\n      X: " + x + "      Y: " + y + "\n";
                }

            }
            return g; 
        }

        Graphics DrawAxis (int CanvasX, int CanvasY, Graphics g)
        {

            //Draw Horizontal Axis
            CSPoint p1 = new CSPoint(0, CanvasY / 2 );
            CSPoint p2 = new CSPoint(CanvasX, CanvasY / 2);
            
            g.DrawLine(Pens.Yellow, p1, p2);

            //Draw Horizontal Axis
            p1 = new CSPoint(CanvasX / 2, 0);
            p2 = new CSPoint(CanvasX / 2, CanvasY);

            g.DrawLine(Pens.Red, p1, p2);

            return g;
        }

        void GeneratePolygons(List<GraphEdge> VoronoiEdges, int SiteCount )
        {

            List<GraphEdge> SiteData = new List<GraphEdge>();
            
            for (int i = 0; i < VoronoiEdges.Count; i++)
            {
                // Each Line comes from two sites
                // Need get the duplicates out into one list
                // To this end a new strecture is called and site 2 is set to 0

                GraphEdge NewDataPoint = new GraphEdge();

                NewDataPoint.site1 = VoronoiEdges[i].site1;
                NewDataPoint.x1 = VoronoiEdges[i].x1;
                NewDataPoint.y1 = VoronoiEdges[i].y1;
                NewDataPoint.x2 = VoronoiEdges[i].x2;
                NewDataPoint.y2 = VoronoiEdges[i].y2;
                
                SiteData.Add(NewDataPoint);

                NewDataPoint = new GraphEdge();

                NewDataPoint.site1 = VoronoiEdges[i].site2;
                NewDataPoint.x1 = VoronoiEdges[i].x1;
                NewDataPoint.y1 = VoronoiEdges[i].y1;
                NewDataPoint.x2 = VoronoiEdges[i].x2;
                NewDataPoint.y2 = VoronoiEdges[i].y2;

                SiteData.Add(NewDataPoint);

            }


            Console.WriteLine("Before");
            foreach (GraphEdge x in SiteData)

            {
                Console.WriteLine("B: " + x.site1 + " XY1: " + x.x1 + "," + x.y1 + " XY2: " + x.x2 + "," + x.y2);
            }

            SiteData.Sort( new GraphEdgeSorter() );

            Console.WriteLine("After");
            foreach (GraphEdge x in SiteData)

            {
                Console.WriteLine("A: " + x.site1 + " XY1: " + x.x1 + "," + x.y1 + " XY2: " + x.x2 + "," + x.y2);
            }

        }

        Graphics DrawEdges(List<GraphEdge> VoronoiEdges, int AxisX, int AxisY, Graphics g)
        {
          
            // So effectively ge is a list of Site Boundaries Generated by MakeVoronoiGraph
            // Plots should be modified using a 
            for (int i = 0; i < VoronoiEdges.Count; i++)
            {

                int x1 = AxisX + (int)VoronoiEdges[i].x1;
                int y1 = AxisY - (int)VoronoiEdges[i].y1;
                int x2 = AxisX + (int)VoronoiEdges[i].x2;
                int y2 = AxisY - (int)VoronoiEdges[i].y2;

                string linedata = "Point " + i
                                + ": S1:" + VoronoiEdges[i].site1 + ", S2:" + VoronoiEdges[i].site2 + "\n"
                                + "      X1: " + x1 + ", Y1: " + y1 + "\n"
                                + "      X2: " + x2 + ", Y2: " + y2 + "\n";

                if (numericUpDown1.Value < 10)
                {

                    richTextBox1.Text += linedata;

                }

                try
                {

                    CSPoint p1 = new CSPoint( x1, y1 );
                    CSPoint p2 = new CSPoint( x2, y2 );

                    g.DrawLine(Pens.DarkGray, p1, p2);

                }
                catch
                {

                    richTextBox1.Text += linedata;

                }
            }

            return g;
        }

       
		
		
		void Button1Click(object sender, EventArgs e)
		{
			this.richTextBox1.Text += "Run - Button Clicked\n";
			RunVoronoi();
			//background = Clone32BPPBitmap ( bitmap );
		}
		void NumericUpDown1ValueChanged(object sender, EventArgs e)
		{
            this.richTextBox1.Text += "Run - Number Changed Clicked\n";
            checkUseStdData.Checked = false;
            RunVoronoi();
			//background = Clone32BPPBitmap ( bitmap );
		}
		
		void PbMouseMove(object sender, MouseEventArgs e)
		{
            int x =  e.X - bitmap.Width / 2 ;
            int y = bitmap.Height / 2 - e.Y;

            label1.Text =  x + ", " + y;
		}

        private void checkUseStdData_CheckStateChanged(object sender, EventArgs e)
        {
            this.richTextBox1.Text += "Run - Check Changed Clicked\n";
            RunVoronoi();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
