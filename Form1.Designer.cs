namespace RogueLike
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblReset = new LinkLabel();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            propertyGrid1 = new PropertyGrid();
            llBuildMap = new LinkLabel();
            llResetMap = new LinkLabel();
            pbMapBuilder = new PictureBox();
            tabPage3 = new TabPage();
            linkLabel2 = new LinkLabel();
            linkLabel1 = new LinkLabel();
            pictureBox2 = new PictureBox();
            propertyGrid2 = new PropertyGrid();
            tabPage4 = new TabPage();
            linkLabel3 = new LinkLabel();
            linkLabel4 = new LinkLabel();
            pictureBox3 = new PictureBox();
            propertyGrid3 = new PropertyGrid();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbMapBuilder).BeginInit();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            SuspendLayout();
            // 
            // lblReset
            // 
            lblReset.AutoSize = true;
            lblReset.Location = new Point(71, 3);
            lblReset.Name = "lblReset";
            lblReset.Size = new Size(54, 25);
            lblReset.TabIndex = 1;
            lblReset.TabStop = true;
            lblReset.Text = "Reset";
            lblReset.LinkClicked += lblReset_LinkClicked;
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Location = new Point(26, 43);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(862, 509);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            pictureBox1.Paint += pictureBox1_Paint;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 3);
            label1.Name = "label1";
            label1.Size = new Size(59, 25);
            label1.TabIndex = 5;
            label1.Text = "label1";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(987, 740);
            tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(pictureBox1);
            tabPage1.Controls.Add(lblReset);
            tabPage1.Controls.Add(label1);
            tabPage1.Location = new Point(4, 34);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(979, 702);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "FOV";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(propertyGrid1);
            tabPage2.Controls.Add(llBuildMap);
            tabPage2.Controls.Add(llResetMap);
            tabPage2.Controls.Add(pbMapBuilder);
            tabPage2.Location = new Point(4, 34);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(979, 702);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Mapbuilder";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            propertyGrid1.Location = new Point(6, 50);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.Size = new Size(281, 644);
            propertyGrid1.TabIndex = 3;
            // 
            // llBuildMap
            // 
            llBuildMap.AutoSize = true;
            llBuildMap.Location = new Point(136, 22);
            llBuildMap.Name = "llBuildMap";
            llBuildMap.Size = new Size(51, 25);
            llBuildMap.TabIndex = 2;
            llBuildMap.TabStop = true;
            llBuildMap.Text = "Build";
            llBuildMap.LinkClicked += llBuildMap_LinkClicked;
            // 
            // llResetMap
            // 
            llResetMap.AutoSize = true;
            llResetMap.Location = new Point(20, 22);
            llResetMap.Name = "llResetMap";
            llResetMap.Size = new Size(54, 25);
            llResetMap.TabIndex = 1;
            llResetMap.TabStop = true;
            llResetMap.Text = "Reset";
            llResetMap.LinkClicked += llResetMap_LinkClicked;
            // 
            // pbMapBuilder
            // 
            pbMapBuilder.BorderStyle = BorderStyle.FixedSingle;
            pbMapBuilder.Location = new Point(293, 50);
            pbMapBuilder.Name = "pbMapBuilder";
            pbMapBuilder.Size = new Size(678, 644);
            pbMapBuilder.TabIndex = 0;
            pbMapBuilder.TabStop = false;
            pbMapBuilder.Paint += pbMapBuilder_Paint;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(linkLabel2);
            tabPage3.Controls.Add(linkLabel1);
            tabPage3.Controls.Add(pictureBox2);
            tabPage3.Controls.Add(propertyGrid2);
            tabPage3.Location = new Point(4, 34);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(979, 702);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Cave Generator";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // linkLabel2
            // 
            linkLabel2.AutoSize = true;
            linkLabel2.Location = new Point(110, 13);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new Size(54, 25);
            linkLabel2.TabIndex = 3;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "Reset";
            linkLabel2.LinkClicked += linkLabel2_LinkClicked;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(14, 13);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(51, 25);
            linkLabel1.TabIndex = 2;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Build";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // pictureBox2
            // 
            pictureBox2.BorderStyle = BorderStyle.FixedSingle;
            pictureBox2.Location = new Point(301, 15);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(670, 681);
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            pictureBox2.Paint += pictureBox2_Paint;
            // 
            // propertyGrid2
            // 
            propertyGrid2.Location = new Point(15, 50);
            propertyGrid2.Name = "propertyGrid2";
            propertyGrid2.Size = new Size(280, 646);
            propertyGrid2.TabIndex = 0;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(linkLabel3);
            tabPage4.Controls.Add(linkLabel4);
            tabPage4.Controls.Add(pictureBox3);
            tabPage4.Controls.Add(propertyGrid3);
            tabPage4.Location = new Point(4, 34);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(3);
            tabPage4.Size = new Size(979, 702);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "Island Generator";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // linkLabel3
            // 
            linkLabel3.AutoSize = true;
            linkLabel3.Location = new Point(107, 10);
            linkLabel3.Name = "linkLabel3";
            linkLabel3.Size = new Size(54, 25);
            linkLabel3.TabIndex = 7;
            linkLabel3.TabStop = true;
            linkLabel3.Text = "Reset";
            linkLabel3.LinkClicked += linkLabel3_LinkClicked;
            linkLabel3.Paint += pictureBox3_Paint;
            // 
            // linkLabel4
            // 
            linkLabel4.AutoSize = true;
            linkLabel4.Location = new Point(11, 10);
            linkLabel4.Name = "linkLabel4";
            linkLabel4.Size = new Size(51, 25);
            linkLabel4.TabIndex = 6;
            linkLabel4.TabStop = true;
            linkLabel4.Text = "Build";
            linkLabel4.LinkClicked += linkLabel4_LinkClicked;
            linkLabel4.Paint += pictureBox3_Paint;
            // 
            // pictureBox3
            // 
            pictureBox3.BorderStyle = BorderStyle.FixedSingle;
            pictureBox3.Location = new Point(298, 12);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(670, 681);
            pictureBox3.TabIndex = 5;
            pictureBox3.TabStop = false;
            pictureBox3.Paint += pictureBox3_Paint;
            // 
            // propertyGrid3
            // 
            propertyGrid3.Location = new Point(12, 47);
            propertyGrid3.Name = "propertyGrid3";
            propertyGrid3.Size = new Size(280, 646);
            propertyGrid3.TabIndex = 4;
            propertyGrid3.Paint += pictureBox3_Paint;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(987, 740);
            Controls.Add(tabControl1);
            KeyPreview = true;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            KeyUp += Form1_KeyUp;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbMapBuilder).EndInit();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private LinkLabel lblReset;
        private PictureBox pictureBox1;
        private Label label1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private PictureBox pbMapBuilder;
        private LinkLabel llBuildMap;
        private LinkLabel llResetMap;
        private PropertyGrid propertyGrid1;
        private TabPage tabPage3;
        private LinkLabel linkLabel2;
        private LinkLabel linkLabel1;
        private PictureBox pictureBox2;
        private PropertyGrid propertyGrid2;
        private TabPage tabPage4;
        private LinkLabel linkLabel3;
        private LinkLabel linkLabel4;
        private PictureBox pictureBox3;
        private PropertyGrid propertyGrid3;
    }
}