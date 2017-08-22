namespace AutoMarkDCTFile
{
    partial class fmain  // : MetroFramework.Forms.MetroForm
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
            this.components = new System.ComponentModel.Container();
            this.timerMonitor = new System.Windows.Forms.Timer(this.components);
            this.lblStartTime = new System.Windows.Forms.Label();
            this.lblEndTime = new System.Windows.Forms.Label();
            this.lblProd = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblTst = new System.Windows.Forms.Label();
            this.lblComment = new System.Windows.Forms.Label();
            this.lbStrT = new System.Windows.Forms.Label();
            this.lbET = new System.Windows.Forms.Label();
            this.lbProd = new System.Windows.Forms.Label();
            this.lbTst = new System.Windows.Forms.Label();
            this.lbnMark = new System.Windows.Forms.Label();
            this.lbErr = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerMonitor
            // 
            this.timerMonitor.Tick += new System.EventHandler(this.timerMonitor_Tick);
            // 
            // lblStartTime
            // 
            this.lblStartTime.AutoSize = true;
            this.lblStartTime.Location = new System.Drawing.Point(7, 15);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new System.Drawing.Size(33, 13);
            this.lblStartTime.TabIndex = 0;
            this.lblStartTime.Text = "Time:";
            // 
            // lblEndTime
            // 
            this.lblEndTime.AutoSize = true;
            this.lblEndTime.Location = new System.Drawing.Point(247, 16);
            this.lblEndTime.Name = "lblEndTime";
            this.lblEndTime.Size = new System.Drawing.Size(52, 13);
            this.lblEndTime.TabIndex = 1;
            this.lblEndTime.Text = "EndTime:";
            // 
            // lblProd
            // 
            this.lblProd.AutoSize = true;
            this.lblProd.Location = new System.Drawing.Point(8, 28);
            this.lblProd.Name = "lblProd";
            this.lblProd.Size = new System.Drawing.Size(47, 13);
            this.lblProd.TabIndex = 2;
            this.lblProd.Text = "Product:";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(6, 44);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(88, 13);
            this.lblTotal.TabIndex = 3;
            this.lblTotal.Text = "TotalMarkdSlder:";
            // 
            // lblTst
            // 
            this.lblTst.AutoSize = true;
            this.lblTst.Location = new System.Drawing.Point(247, 28);
            this.lblTst.Name = "lblTst";
            this.lblTst.Size = new System.Drawing.Size(40, 13);
            this.lblTst.TabIndex = 4;
            this.lblTst.Text = "Tester:";
            // 
            // lblComment
            // 
            this.lblComment.AutoSize = true;
            this.lblComment.Location = new System.Drawing.Point(8, 58);
            this.lblComment.Name = "lblComment";
            this.lblComment.Size = new System.Drawing.Size(54, 13);
            this.lblComment.TabIndex = 6;
            this.lblComment.Text = "Comment:";
            // 
            // lbStrT
            // 
            this.lbStrT.AutoSize = true;
            this.lbStrT.ForeColor = System.Drawing.Color.Blue;
            this.lbStrT.Location = new System.Drawing.Point(46, 16);
            this.lbStrT.Name = "lbStrT";
            this.lbStrT.Size = new System.Drawing.Size(10, 13);
            this.lbStrT.TabIndex = 7;
            this.lbStrT.Text = "-";
            // 
            // lbET
            // 
            this.lbET.AutoSize = true;
            this.lbET.ForeColor = System.Drawing.Color.Blue;
            this.lbET.Location = new System.Drawing.Point(303, 17);
            this.lbET.Name = "lbET";
            this.lbET.Size = new System.Drawing.Size(10, 13);
            this.lbET.TabIndex = 8;
            this.lbET.Text = "-";
            // 
            // lbProd
            // 
            this.lbProd.AutoSize = true;
            this.lbProd.ForeColor = System.Drawing.Color.Blue;
            this.lbProd.Location = new System.Drawing.Point(61, 28);
            this.lbProd.Name = "lbProd";
            this.lbProd.Size = new System.Drawing.Size(10, 13);
            this.lbProd.TabIndex = 9;
            this.lbProd.Text = "-";
            // 
            // lbTst
            // 
            this.lbTst.AutoSize = true;
            this.lbTst.ForeColor = System.Drawing.Color.Blue;
            this.lbTst.Location = new System.Drawing.Point(292, 29);
            this.lbTst.Name = "lbTst";
            this.lbTst.Size = new System.Drawing.Size(10, 13);
            this.lbTst.TabIndex = 10;
            this.lbTst.Text = "-";
            // 
            // lbnMark
            // 
            this.lbnMark.AutoSize = true;
            this.lbnMark.ForeColor = System.Drawing.Color.Blue;
            this.lbnMark.Location = new System.Drawing.Point(100, 44);
            this.lbnMark.Name = "lbnMark";
            this.lbnMark.Size = new System.Drawing.Size(10, 13);
            this.lbnMark.TabIndex = 11;
            this.lbnMark.Text = "-";
            // 
            // lbErr
            // 
            this.lbErr.AutoSize = true;
            this.lbErr.ForeColor = System.Drawing.Color.Blue;
            this.lbErr.Location = new System.Drawing.Point(68, 58);
            this.lbErr.Name = "lbErr";
            this.lbErr.Size = new System.Drawing.Size(10, 13);
            this.lbErr.TabIndex = 13;
            this.lbErr.Text = "-";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblTotal);
            this.groupBox1.Controls.Add(this.lbErr);
            this.groupBox1.Controls.Add(this.lblStartTime);
            this.groupBox1.Controls.Add(this.lbnMark);
            this.groupBox1.Controls.Add(this.lblEndTime);
            this.groupBox1.Controls.Add(this.lbTst);
            this.groupBox1.Controls.Add(this.lblProd);
            this.groupBox1.Controls.Add(this.lbProd);
            this.groupBox1.Controls.Add(this.lblTst);
            this.groupBox1.Controls.Add(this.lbET);
            this.groupBox1.Controls.Add(this.lblComment);
            this.groupBox1.Controls.Add(this.lbStrT);
            this.groupBox1.Location = new System.Drawing.Point(2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(431, 74);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MarkingDetails";
          //  this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // fmain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(437, 79);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "fmain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerMonitor;
        private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.Label lblEndTime;
        private System.Windows.Forms.Label lblProd;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblTst;
        private System.Windows.Forms.Label lblComment;
        private System.Windows.Forms.Label lbStrT;
        private System.Windows.Forms.Label lbET;
        private System.Windows.Forms.Label lbProd;
        private System.Windows.Forms.Label lbTst;
        private System.Windows.Forms.Label lbnMark;
        private System.Windows.Forms.Label lbErr;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

