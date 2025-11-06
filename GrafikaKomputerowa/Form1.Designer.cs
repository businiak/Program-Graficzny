namespace GrafikaKomputerowa
{
    partial class Form1
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Line = new System.Windows.Forms.Button();
            this.Rectangle = new System.Windows.Forms.Button();
            this.Circle = new System.Windows.Forms.Button();
            this.x1TextBox = new System.Windows.Forms.TextBox();
            this.y1TextBox = new System.Windows.Forms.TextBox();
            this.x2TextBox = new System.Windows.Forms.TextBox();
            this.y2TextBox = new System.Windows.Forms.TextBox();
            this.Draw = new System.Windows.Forms.Button();
            this.Select = new System.Windows.Forms.Button();
            this.xselect = new System.Windows.Forms.TextBox();
            this.yselect = new System.Windows.Forms.TextBox();
            this.ManualSelect = new System.Windows.Forms.Button();
            this.Save = new System.Windows.Forms.Button();
            this.Load = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(-2, 37);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(800, 560);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // Line
            // 
            this.Line.Location = new System.Drawing.Point(8, 8);
            this.Line.Name = "Line";
            this.Line.Size = new System.Drawing.Size(37, 23);
            this.Line.TabIndex = 1;
            this.Line.Tag = "Line";
            this.Line.Text = "Line";
            this.Line.UseVisualStyleBackColor = true;
            this.Line.Click += new System.EventHandler(this.Line_Click);
            // 
            // Rectangle
            // 
            this.Rectangle.Location = new System.Drawing.Point(51, 8);
            this.Rectangle.Name = "Rectangle";
            this.Rectangle.Size = new System.Drawing.Size(64, 23);
            this.Rectangle.TabIndex = 2;
            this.Rectangle.Text = "Rectangle";
            this.Rectangle.UseVisualStyleBackColor = true;
            this.Rectangle.Click += new System.EventHandler(this.Rectangle_Click);
            // 
            // Circle
            // 
            this.Circle.Location = new System.Drawing.Point(121, 8);
            this.Circle.Name = "Circle";
            this.Circle.Size = new System.Drawing.Size(43, 23);
            this.Circle.TabIndex = 3;
            this.Circle.Text = "Circle";
            this.Circle.UseVisualStyleBackColor = true;
            this.Circle.Click += new System.EventHandler(this.Circle_Click);
            // 
            // x1TextBox
            // 
            this.x1TextBox.AccessibleName = "x1";
            this.x1TextBox.Location = new System.Drawing.Point(255, 8);
            this.x1TextBox.Name = "x1TextBox";
            this.x1TextBox.Size = new System.Drawing.Size(22, 20);
            this.x1TextBox.TabIndex = 4;
            // 
            // y1TextBox
            // 
            this.y1TextBox.Location = new System.Drawing.Point(283, 8);
            this.y1TextBox.Name = "y1TextBox";
            this.y1TextBox.Size = new System.Drawing.Size(22, 20);
            this.y1TextBox.TabIndex = 5;
            // 
            // x2TextBox
            // 
            this.x2TextBox.Location = new System.Drawing.Point(311, 8);
            this.x2TextBox.Name = "x2TextBox";
            this.x2TextBox.Size = new System.Drawing.Size(22, 20);
            this.x2TextBox.TabIndex = 6;
            // 
            // y2TextBox
            // 
            this.y2TextBox.Location = new System.Drawing.Point(339, 8);
            this.y2TextBox.Name = "y2TextBox";
            this.y2TextBox.Size = new System.Drawing.Size(22, 20);
            this.y2TextBox.TabIndex = 7;
            // 
            // Draw
            // 
            this.Draw.Location = new System.Drawing.Point(367, 8);
            this.Draw.Name = "Draw";
            this.Draw.Size = new System.Drawing.Size(75, 23);
            this.Draw.TabIndex = 8;
            this.Draw.Text = "Draw";
            this.Draw.UseVisualStyleBackColor = true;
            this.Draw.Click += new System.EventHandler(this.Draw_Click);
            // 
            // Select
            // 
            this.Select.Location = new System.Drawing.Point(170, 8);
            this.Select.Name = "Select";
            this.Select.Size = new System.Drawing.Size(51, 23);
            this.Select.TabIndex = 9;
            this.Select.Text = "Select";
            this.Select.UseVisualStyleBackColor = true;
            this.Select.Click += new System.EventHandler(this.Select_Click);
            // 
            // xselect
            // 
            this.xselect.AccessibleName = "xselect";
            this.xselect.Location = new System.Drawing.Point(481, 7);
            this.xselect.Name = "xselect";
            this.xselect.Size = new System.Drawing.Size(22, 20);
            this.xselect.TabIndex = 10;
            // 
            // yselect
            // 
            this.yselect.AccessibleName = "yselect";
            this.yselect.Location = new System.Drawing.Point(509, 7);
            this.yselect.Name = "yselect";
            this.yselect.Size = new System.Drawing.Size(22, 20);
            this.yselect.TabIndex = 11;
            // 
            // ManualSelect
            // 
            this.ManualSelect.Location = new System.Drawing.Point(537, 5);
            this.ManualSelect.Name = "ManualSelect";
            this.ManualSelect.Size = new System.Drawing.Size(75, 23);
            this.ManualSelect.TabIndex = 12;
            this.ManualSelect.Text = "Man. Sel.";
            this.ManualSelect.UseVisualStyleBackColor = true;
            this.ManualSelect.Click += new System.EventHandler(this.ManualSelect_Click);
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(723, 4);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(75, 23);
            this.Save.TabIndex = 13;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Load
            // 
            this.Load.Location = new System.Drawing.Point(723, 33);
            this.Load.Name = "Load";
            this.Load.Size = new System.Drawing.Size(75, 23);
            this.Load.TabIndex = 14;
            this.Load.Text = "Load";
            this.Load.UseVisualStyleBackColor = true;
            this.Load.Click += new System.EventHandler(this.Load_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Location = new System.Drawing.Point(-2, -11);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(809, 611);
            this.panel1.TabIndex = 15;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 601);
            this.Controls.Add(this.Load);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.ManualSelect);
            this.Controls.Add(this.yselect);
            this.Controls.Add(this.xselect);
            this.Controls.Add(this.Select);
            this.Controls.Add(this.Draw);
            this.Controls.Add(this.y2TextBox);
            this.Controls.Add(this.x2TextBox);
            this.Controls.Add(this.y1TextBox);
            this.Controls.Add(this.x1TextBox);
            this.Controls.Add(this.Circle);
            this.Controls.Add(this.Rectangle);
            this.Controls.Add(this.Line);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button Line;
        private System.Windows.Forms.Button Rectangle;
        private System.Windows.Forms.Button Circle;
        private System.Windows.Forms.TextBox x1TextBox;
        private System.Windows.Forms.TextBox y1TextBox;
        private System.Windows.Forms.TextBox x2TextBox;
        private System.Windows.Forms.TextBox y2TextBox;
        private System.Windows.Forms.Button Draw;
        private System.Windows.Forms.Button Select;
        private System.Windows.Forms.TextBox xselect;
        private System.Windows.Forms.TextBox yselect;
        private System.Windows.Forms.Button ManualSelect;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button Load;
        private System.Windows.Forms.Panel panel1;
    }
}

