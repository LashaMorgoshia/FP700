namespace FP700Win
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
            this.components = new System.ComponentModel.Container();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnXReport = new System.Windows.Forms.Button();
            this.btnZReport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // serialPort1
            // 
            this.serialPort1.PortName = "COM10";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(154, 532);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(272, 43);
            this.button1.TabIndex = 0;
            this.button1.Text = "რეალიზაციის გატარება";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(34, 57);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(558, 455);
            this.dataGridView1.TabIndex = 1;
            // 
            // btnXReport
            // 
            this.btnXReport.Location = new System.Drawing.Point(659, 57);
            this.btnXReport.Name = "btnXReport";
            this.btnXReport.Size = new System.Drawing.Size(272, 43);
            this.btnXReport.TabIndex = 0;
            this.btnXReport.Text = "X რეპორტის ამოღება";
            this.btnXReport.UseVisualStyleBackColor = true;
            this.btnXReport.Click += new System.EventHandler(this.btnXReport_Click);
            // 
            // btnZReport
            // 
            this.btnZReport.Location = new System.Drawing.Point(659, 106);
            this.btnZReport.Name = "btnZReport";
            this.btnZReport.Size = new System.Drawing.Size(272, 43);
            this.btnZReport.TabIndex = 0;
            this.btnZReport.Text = "Z რეპორტის ამოღება";
            this.btnZReport.UseVisualStyleBackColor = true;
            this.btnZReport.Click += new System.EventHandler(this.btnZReport_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1113, 700);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnZReport);
            this.Controls.Add(this.btnXReport);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Fiscal Printer Tester";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnXReport;
        private System.Windows.Forms.Button btnZReport;
    }
}

