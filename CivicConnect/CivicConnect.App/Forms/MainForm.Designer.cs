namespace CivicConnect.App.Forms
{
    internal sealed partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnSignOut = new System.Windows.Forms.Button();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblAppName = new System.Windows.Forms.Label();
            this.pnlBody = new System.Windows.Forms.Panel();
            this.flowTiles = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnMyRequests = new System.Windows.Forms.Button();
            this.btnQueue = new System.Windows.Forms.Button();
            this.btnAllRequests = new System.Windows.Forms.Button();
            this.lblHint = new System.Windows.Forms.Label();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlBody.SuspendLayout();
            this.flowTiles.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.Controls.Add(this.btnSignOut);
            this.pnlHeader.Controls.Add(this.lblUser);
            this.pnlHeader.Controls.Add(this.lblAppName);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1200, 72);
            this.pnlHeader.TabIndex = 0;
            this.pnlHeader.Tag = "header";
            //
            // btnSignOut
            //
            this.btnSignOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSignOut.Location = new System.Drawing.Point(1050, 16);
            this.btnSignOut.Name = "btnSignOut";
            this.btnSignOut.Size = new System.Drawing.Size(120, 40);
            this.btnSignOut.TabIndex = 2;
            this.btnSignOut.Tag = "ghost";
            this.btnSignOut.Text = "Sign &out";
            this.btnSignOut.Click += new System.EventHandler(this.btnSignOut_Click);
            //
            // lblUser
            //
            this.lblUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(840, 26);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(100, 23);
            this.lblUser.TabIndex = 1;
            this.lblUser.Tag = "subtitle";
            this.lblUser.Text = "Signed in as";
            //
            // lblAppName
            //
            this.lblAppName.AutoSize = true;
            this.lblAppName.Location = new System.Drawing.Point(28, 18);
            this.lblAppName.Name = "lblAppName";
            this.lblAppName.Size = new System.Drawing.Size(180, 41);
            this.lblAppName.TabIndex = 0;
            this.lblAppName.Tag = "title";
            this.lblAppName.Text = "CivicConnect";
            //
            // pnlBody
            //
            this.pnlBody.Controls.Add(this.flowTiles);
            this.pnlBody.Controls.Add(this.lblHint);
            this.pnlBody.Controls.Add(this.lblWelcome);
            this.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBody.Location = new System.Drawing.Point(0, 72);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new System.Windows.Forms.Padding(40, 36, 40, 24);
            this.pnlBody.Size = new System.Drawing.Size(1200, 628);
            this.pnlBody.TabIndex = 1;
            //
            // flowTiles
            //
            this.flowTiles.Controls.Add(this.btnSubmit);
            this.flowTiles.Controls.Add(this.btnMyRequests);
            this.flowTiles.Controls.Add(this.btnQueue);
            this.flowTiles.Controls.Add(this.btnAllRequests);
            this.flowTiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowTiles.Location = new System.Drawing.Point(40, 120);
            this.flowTiles.Name = "flowTiles";
            this.flowTiles.Padding = new System.Windows.Forms.Padding(0, 16, 0, 0);
            this.flowTiles.Size = new System.Drawing.Size(1120, 484);
            this.flowTiles.TabIndex = 2;
            //
            // btnSubmit
            //
            this.btnSubmit.Location = new System.Drawing.Point(3, 19);
            this.btnSubmit.Margin = new System.Windows.Forms.Padding(3, 3, 20, 20);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(320, 110);
            this.btnSubmit.TabIndex = 0;
            this.btnSubmit.Tag = "tile";
            this.btnSubmit.Text = "Submit a new request";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            //
            // btnMyRequests
            //
            this.btnMyRequests.Location = new System.Drawing.Point(346, 19);
            this.btnMyRequests.Margin = new System.Windows.Forms.Padding(3, 3, 20, 20);
            this.btnMyRequests.Name = "btnMyRequests";
            this.btnMyRequests.Size = new System.Drawing.Size(320, 110);
            this.btnMyRequests.TabIndex = 1;
            this.btnMyRequests.Tag = "tile";
            this.btnMyRequests.Text = "My requests";
            this.btnMyRequests.Click += new System.EventHandler(this.btnMyRequests_Click);
            //
            // btnQueue
            //
            this.btnQueue.Location = new System.Drawing.Point(689, 19);
            this.btnQueue.Margin = new System.Windows.Forms.Padding(3, 3, 20, 20);
            this.btnQueue.Name = "btnQueue";
            this.btnQueue.Size = new System.Drawing.Size(320, 110);
            this.btnQueue.TabIndex = 2;
            this.btnQueue.Tag = "tile";
            this.btnQueue.Text = "Staff queue";
            this.btnQueue.Click += new System.EventHandler(this.btnQueue_Click);
            //
            // btnAllRequests
            //
            this.btnAllRequests.Location = new System.Drawing.Point(3, 152);
            this.btnAllRequests.Margin = new System.Windows.Forms.Padding(3, 3, 20, 20);
            this.btnAllRequests.Name = "btnAllRequests";
            this.btnAllRequests.Size = new System.Drawing.Size(320, 110);
            this.btnAllRequests.TabIndex = 3;
            this.btnAllRequests.Tag = "tile";
            this.btnAllRequests.Text = "Overview and all requests";
            this.btnAllRequests.Click += new System.EventHandler(this.btnAllRequests_Click);
            //
            // lblHint
            //
            this.lblHint.AutoSize = true;
            this.lblHint.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHint.Location = new System.Drawing.Point(40, 84);
            this.lblHint.Name = "lblHint";
            this.lblHint.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblHint.Size = new System.Drawing.Size(200, 29);
            this.lblHint.TabIndex = 1;
            this.lblHint.Tag = "muted";
            this.lblHint.Text = "Choose what you would like to do.";
            //
            // lblWelcome
            //
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblWelcome.Location = new System.Drawing.Point(40, 36);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(150, 48);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Tag = "heading";
            this.lblWelcome.Text = "Welcome";
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlHeader);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CivicConnect";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlBody.ResumeLayout(false);
            this.pnlBody.PerformLayout();
            this.flowTiles.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnSignOut;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblAppName;
        private System.Windows.Forms.Panel pnlBody;
        private System.Windows.Forms.FlowLayoutPanel flowTiles;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnMyRequests;
        private System.Windows.Forms.Button btnQueue;
        private System.Windows.Forms.Button btnAllRequests;
        private System.Windows.Forms.Label lblHint;
        private System.Windows.Forms.Label lblWelcome;
    }
}
