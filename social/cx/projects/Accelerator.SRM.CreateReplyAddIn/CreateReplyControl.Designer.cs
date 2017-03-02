namespace Accelerator.SRM.CreateReplyAddIn
{
    partial class CreateReplyControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dummyHidden = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dummyHidden
            // 
            this.dummyHidden.Enabled = false;
            this.dummyHidden.Location = new System.Drawing.Point(19, 18);
            this.dummyHidden.Name = "dummyHidden";
            this.dummyHidden.Size = new System.Drawing.Size(75, 23);
            this.dummyHidden.TabIndex = 0;
            this.dummyHidden.Text = "button1";
            this.dummyHidden.UseVisualStyleBackColor = true;
            this.dummyHidden.Visible = false;
            this.dummyHidden.Click += new System.EventHandler(this.button1_Click);
            // 
            // CreateReplyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dummyHidden);
            this.Name = "CreateReplyControl";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button dummyHidden;
    }
}
