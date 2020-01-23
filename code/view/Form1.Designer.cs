namespace view
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
            this.label1 = new System.Windows.Forms.Label();
            this.readsBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.contigsBox = new System.Windows.Forms.TextBox();
            this.readToReadBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.readToContigBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.readsFileButton = new System.Windows.Forms.Button();
            this.contigsFileButton = new System.Windows.Forms.Button();
            this.readToReadFileButton = new System.Windows.Forms.Button();
            this.readToContigFileButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Reads";
            // 
            // readsBox
            // 
            this.readsBox.Location = new System.Drawing.Point(135, 6);
            this.readsBox.Name = "readsBox";
            this.readsBox.Size = new System.Drawing.Size(100, 20);
            this.readsBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Contigs";
            // 
            // contigsBox
            // 
            this.contigsBox.Location = new System.Drawing.Point(135, 32);
            this.contigsBox.Name = "contigsBox";
            this.contigsBox.Size = new System.Drawing.Size(100, 20);
            this.contigsBox.TabIndex = 3;
            // 
            // readToReadBox
            // 
            this.readToReadBox.Location = new System.Drawing.Point(135, 58);
            this.readToReadBox.Name = "readToReadBox";
            this.readToReadBox.Size = new System.Drawing.Size(100, 20);
            this.readToReadBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Read to read overlaps";
            // 
            // readToContigBox
            // 
            this.readToContigBox.Location = new System.Drawing.Point(135, 84);
            this.readToContigBox.Name = "readToContigBox";
            this.readToContigBox.Size = new System.Drawing.Size(100, 20);
            this.readToContigBox.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Read to contig overlaps";
            // 
            // readsFileButton
            // 
            this.readsFileButton.Location = new System.Drawing.Point(241, 4);
            this.readsFileButton.Name = "readsFileButton";
            this.readsFileButton.Size = new System.Drawing.Size(103, 23);
            this.readsFileButton.TabIndex = 8;
            this.readsFileButton.Text = "Otvori";
            this.readsFileButton.UseVisualStyleBackColor = true;
            this.readsFileButton.Click += new System.EventHandler(this.fileButton_Click);
            // 
            // contigsFileButton
            // 
            this.contigsFileButton.Location = new System.Drawing.Point(241, 30);
            this.contigsFileButton.Name = "contigsFileButton";
            this.contigsFileButton.Size = new System.Drawing.Size(103, 23);
            this.contigsFileButton.TabIndex = 9;
            this.contigsFileButton.Text = "Otvori";
            this.contigsFileButton.UseVisualStyleBackColor = true;
            this.contigsFileButton.Click += new System.EventHandler(this.contigsFileButton_Click);
            // 
            // readToReadFileButton
            // 
            this.readToReadFileButton.Location = new System.Drawing.Point(241, 56);
            this.readToReadFileButton.Name = "readToReadFileButton";
            this.readToReadFileButton.Size = new System.Drawing.Size(103, 23);
            this.readToReadFileButton.TabIndex = 10;
            this.readToReadFileButton.Text = "Otvori";
            this.readToReadFileButton.UseVisualStyleBackColor = true;
            // 
            // readToContigFileButton
            // 
            this.readToContigFileButton.Location = new System.Drawing.Point(241, 82);
            this.readToContigFileButton.Name = "readToContigFileButton";
            this.readToContigFileButton.Size = new System.Drawing.Size(103, 23);
            this.readToContigFileButton.TabIndex = 11;
            this.readToContigFileButton.Text = "Otvori";
            this.readToContigFileButton.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 635);
            this.Controls.Add(this.readToContigFileButton);
            this.Controls.Add(this.readToReadFileButton);
            this.Controls.Add(this.contigsFileButton);
            this.Controls.Add(this.readsFileButton);
            this.Controls.Add(this.readToContigBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.readToReadBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.contigsBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.readsBox);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox readsBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox contigsBox;
        private System.Windows.Forms.TextBox readToReadBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox readToContigBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button readsFileButton;
        private System.Windows.Forms.Button contigsFileButton;
        private System.Windows.Forms.Button readToReadFileButton;
        private System.Windows.Forms.Button readToContigFileButton;
    }
}

