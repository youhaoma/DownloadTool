namespace BaiduPan.Model
{
    partial class NewAccountForm
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
            this.panWebBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // panWebBrowser
            // 
            this.panWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.panWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.panWebBrowser.Name = "panWebBrowser";
            this.panWebBrowser.ScriptErrorsSuppressed = true;
            this.panWebBrowser.Size = new System.Drawing.Size(1043, 674);
            this.panWebBrowser.TabIndex = 0;
            this.panWebBrowser.Url = new System.Uri("https://pan.baidu.com/", System.UriKind.Absolute);
            this.panWebBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.panWebBrowser_Navigated);
            // 
            // NewAccountForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1043, 674);
            this.Controls.Add(this.panWebBrowser);
            this.Name = "NewAccountForm";
            this.Text = "NewAccountForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NewAccountForm_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser panWebBrowser;
    }
}