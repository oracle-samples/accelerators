namespace Accelerator.Siebel.AddressValidationAddIn
{
    partial class AddressValidationControl
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
            this.verifyAddress = new System.Windows.Forms.Button();
            this.useSuggested = new System.Windows.Forms.CheckBox();
            this.addrValidResponseText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // verifyAddress
            // 
            this.verifyAddress.Location = new System.Drawing.Point(3, 3);
            this.verifyAddress.Name = "verifyAddress";
            this.verifyAddress.Size = new System.Drawing.Size(107, 23);
            this.verifyAddress.TabIndex = 0;
            this.verifyAddress.Text = "Verify Address";
            this.verifyAddress.UseVisualStyleBackColor = true;
            this.verifyAddress.Click += new System.EventHandler(this.verifyAddress_click);
            // 
            // useSuggested
            // 
            this.useSuggested.AutoSize = true;
            this.useSuggested.Location = new System.Drawing.Point(160, 7);
            this.useSuggested.Name = "useSuggested";
            this.useSuggested.Size = new System.Drawing.Size(132, 17);
            this.useSuggested.TabIndex = 1;
            this.useSuggested.Text = "Do you want to use it?";
            this.useSuggested.UseVisualStyleBackColor = true;
            this.useSuggested.CheckedChanged += new System.EventHandler(this.useSuggested_changed);
            // 
            // addrValidResponseText
            // 
            this.addrValidResponseText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addrValidResponseText.Location = new System.Drawing.Point(3, 32);
            this.addrValidResponseText.Name = "addrValidResponseText";
            this.addrValidResponseText.ReadOnly = true;
            this.addrValidResponseText.Size = new System.Drawing.Size(534, 20);
            this.addrValidResponseText.TabIndex = 2;
            // 
            // AddressValidationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.addrValidResponseText);
            this.Controls.Add(this.useSuggested);
            this.Controls.Add(this.verifyAddress);
            this.Name = "AddressValidationControl";
            this.Size = new System.Drawing.Size(565, 83);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button verifyAddress;
        public System.Windows.Forms.CheckBox useSuggested;
        public System.Windows.Forms.TextBox addrValidResponseText;       
    }
}
