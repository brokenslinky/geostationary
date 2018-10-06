namespace geostationary
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.label1 = new System.Windows.Forms.Label();
            this.periapsisInput = new System.Windows.Forms.TextBox();
            this.apoapsisInput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.inclinationInput = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.AoPinput = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.to_geostationary = new System.Windows.Forms.Button();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(616, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Periapsis";
            // 
            // periapsisInput
            // 
            this.periapsisInput.Location = new System.Drawing.Point(619, 45);
            this.periapsisInput.Name = "periapsisInput";
            this.periapsisInput.Size = new System.Drawing.Size(100, 20);
            this.periapsisInput.TabIndex = 1;
            // 
            // apoapsisInput
            // 
            this.apoapsisInput.Location = new System.Drawing.Point(619, 85);
            this.apoapsisInput.Name = "apoapsisInput";
            this.apoapsisInput.Size = new System.Drawing.Size(100, 20);
            this.apoapsisInput.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(616, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Apoapsis";
            // 
            // inclinationInput
            // 
            this.inclinationInput.Location = new System.Drawing.Point(619, 125);
            this.inclinationInput.Name = "inclinationInput";
            this.inclinationInput.Size = new System.Drawing.Size(100, 20);
            this.inclinationInput.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(616, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Inclination";
            // 
            // AoPinput
            // 
            this.AoPinput.Location = new System.Drawing.Point(619, 165);
            this.AoPinput.Name = "AoPinput";
            this.AoPinput.Size = new System.Drawing.Size(100, 20);
            this.AoPinput.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(616, 148);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Argument of Periapsis";
            // 
            // to_geostationary
            // 
            this.to_geostationary.Location = new System.Drawing.Point(619, 192);
            this.to_geostationary.Name = "to_geostationary";
            this.to_geostationary.Size = new System.Drawing.Size(100, 23);
            this.to_geostationary.TabIndex = 8;
            this.to_geostationary.Text = "to_geostationary";
            this.to_geostationary.UseVisualStyleBackColor = true;
            this.to_geostationary.Click += new System.EventHandler(this.to_geostationary_Click);
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(12, 26);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Color = System.Drawing.Color.Red;
            series1.Legend = "Legend1";
            series1.Name = "Initial Orbit";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            series2.Legend = "Legend1";
            series2.Name = "After Prograde";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series3.Color = System.Drawing.Color.Lime;
            series3.Legend = "Legend1";
            series3.Name = "After Inclination Adjustment";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series4.Color = System.Drawing.Color.Aqua;
            series4.Legend = "Legend1";
            series4.Name = "After Inclination Prograde";
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series5.Color = System.Drawing.Color.Blue;
            series5.Legend = "Legend1";
            series5.Name = "Geostationary";
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series6.Color = System.Drawing.Color.Teal;
            series6.Legend = "Legend1";
            series6.Name = "Earth";
            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Series.Add(series3);
            this.chart1.Series.Add(series4);
            this.chart1.Series.Add(series5);
            this.chart1.Series.Add(series6);
            this.chart1.Size = new System.Drawing.Size(591, 288);
            this.chart1.TabIndex = 9;
            this.chart1.Text = "chart1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(733, 357);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.to_geostationary);
            this.Controls.Add(this.AoPinput);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.inclinationInput);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.apoapsisInput);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.periapsisInput);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox periapsisInput;
        private System.Windows.Forms.TextBox apoapsisInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox inclinationInput;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox AoPinput;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button to_geostationary;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
    }
}

