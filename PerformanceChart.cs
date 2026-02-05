using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace AmazePerf
{
    /// <summary>
    /// Custom control for drawing real-time performance charts
    /// </summary>
    public class PerformanceChart : Control
    {
        private Queue<float> dataPoints;
        private string chartTitle;
        private Color lineColor;
        private Color gridColor;
        private Color backgroundColor;
        private float maxValue;
        private int maxDataPoints;
        
        public PerformanceChart()
        {
            dataPoints = new Queue<float>();
            maxDataPoints = 60;
            maxValue = 100f;
            lineColor = Color.FromArgb(0, 200, 255);
            gridColor = Color.FromArgb(50, 255, 255, 255);
            backgroundColor = Color.FromArgb(20, 20, 30);
            chartTitle = "Performance";
            
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | 
                         ControlStyles.OptimizedDoubleBuffer, true);
        }
        
        public string ChartTitle
        {
            get { return chartTitle; }
            set { chartTitle = value; Invalidate(); }
        }
        
        public Color LineColor
        {
            get { return lineColor; }
            set { lineColor = value; Invalidate(); }
        }
        
        public float MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; Invalidate(); }
        }
        
        public void AddDataPoint(float value)
        {
            if (dataPoints.Count >= maxDataPoints)
            {
                dataPoints.Dequeue();
            }
            dataPoints.Enqueue(value);
            Invalidate();
        }
        
        public void Clear()
        {
            dataPoints.Clear();
            Invalidate();
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw background
            using (SolidBrush bgBrush = new SolidBrush(backgroundColor))
            {
                g.FillRectangle(bgBrush, ClientRectangle);
            }
            
            // Draw title
            using (Font titleFont = new Font("Segoe UI", 10f, FontStyle.Bold))
            using (SolidBrush titleBrush = new SolidBrush(Color.White))
            {
                g.DrawString(chartTitle, titleFont, titleBrush, 10, 5);
            }
            
            // Chart area
            int chartTop = 30;
            int chartBottom = Height - 25;
            int chartLeft = 40;
            int chartRight = Width - 10;
            int chartHeight = chartBottom - chartTop;
            int chartWidth = chartRight - chartLeft;
            
            if (chartWidth <= 0 || chartHeight <= 0) return;
            
            // Draw grid
            using (Pen gridPen = new Pen(gridColor, 1))
            {
                gridPen.DashStyle = DashStyle.Dot;
                
                // Horizontal grid lines
                for (int i = 0; i <= 4; i++)
                {
                    int y = chartTop + (chartHeight * i / 4);
                    g.DrawLine(gridPen, chartLeft, y, chartRight, y);
                    
                    // Draw value labels
                    float value = maxValue * (4 - i) / 4;
                    using (Font labelFont = new Font("Segoe UI", 7f))
                    using (SolidBrush labelBrush = new SolidBrush(Color.Gray))
                    {
                        g.DrawString(value.ToString("F0"), labelFont, labelBrush, 5, y - 7);
                    }
                }
            }
            
            // Draw data line
            if (dataPoints.Count > 1)
            {
                var points = dataPoints.ToArray();
                var graphPoints = new PointF[points.Length];
                
                for (int i = 0; i < points.Length; i++)
                {
                    float x = chartLeft + (chartWidth * i / (float)(maxDataPoints - 1));
                    float normalizedValue = Math.Min(points[i] / maxValue, 1.0f);
                    float y = chartBottom - (chartHeight * normalizedValue);
                    graphPoints[i] = new PointF(x, y);
                }
                
                // Draw gradient fill under line
                if (graphPoints.Length > 1)
                {
                    var fillPoints = new PointF[graphPoints.Length + 2];
                    Array.Copy(graphPoints, fillPoints, graphPoints.Length);
                    fillPoints[graphPoints.Length] = new PointF(graphPoints[graphPoints.Length - 1].X, chartBottom);
                    fillPoints[graphPoints.Length + 1] = new PointF(graphPoints[0].X, chartBottom);
                    
                    using (LinearGradientBrush fillBrush = new LinearGradientBrush(
                        new Point(0, chartTop),
                        new Point(0, chartBottom),
                        Color.FromArgb(100, lineColor),
                        Color.FromArgb(10, lineColor)))
                    {
                        g.FillPolygon(fillBrush, fillPoints);
                    }
                }
                
                // Draw line
                using (Pen linePen = new Pen(lineColor, 2))
                {
                    g.DrawLines(linePen, graphPoints);
                }
                
                // Draw current value
                if (points.Length > 0)
                {
                    float currentValue = points[points.Length - 1];
                    string valueText = currentValue.ToString("F1") + "%";
                    
                    using (Font valueFont = new Font("Segoe UI", 9f, FontStyle.Bold))
                    using (SolidBrush valueBrush = new SolidBrush(lineColor))
                    {
                        SizeF textSize = g.MeasureString(valueText, valueFont);
                        g.DrawString(valueText, valueFont, valueBrush, 
                                   Width - textSize.Width - 15, 5);
                    }
                }
            }
        }
    }
}
