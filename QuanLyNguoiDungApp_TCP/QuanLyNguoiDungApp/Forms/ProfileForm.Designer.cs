namespace QuanLyNguoiDungApp.Forms
{
    partial class ProfileForm
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
            this.DoBField = new System.Windows.Forms.TextBox();
            this.PhonenumField = new System.Windows.Forms.TextBox();
            this.UsernameField = new System.Windows.Forms.TextBox();
            this.EmailField = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDangXuat = new System.Windows.Forms.Button();
            this.RoleField = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(84, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(476, 55);
            this.label1.TabIndex = 0;
            this.label1.Text = "Thông tin người dùng";
            // 
            // DoBField
            // 
            this.DoBField.Location = new System.Drawing.Point(106, 487);
            this.DoBField.Name = "DoBField";
            this.DoBField.ReadOnly = true;
            this.DoBField.Size = new System.Drawing.Size(430, 31);
            this.DoBField.TabIndex = 9;
            // 
            // PhonenumField
            // 
            this.PhonenumField.Location = new System.Drawing.Point(106, 390);
            this.PhonenumField.Name = "PhonenumField";
            this.PhonenumField.ReadOnly = true;
            this.PhonenumField.Size = new System.Drawing.Size(430, 31);
            this.PhonenumField.TabIndex = 8;
            // 
            // UsernameField
            // 
            this.UsernameField.BackColor = System.Drawing.SystemColors.Control;
            this.UsernameField.Location = new System.Drawing.Point(106, 301);
            this.UsernameField.Name = "UsernameField";
            this.UsernameField.ReadOnly = true;
            this.UsernameField.Size = new System.Drawing.Size(430, 31);
            this.UsernameField.TabIndex = 7;
            // 
            // EmailField
            // 
            this.EmailField.Location = new System.Drawing.Point(106, 213);
            this.EmailField.Name = "EmailField";
            this.EmailField.ReadOnly = true;
            this.EmailField.Size = new System.Drawing.Size(430, 31);
            this.EmailField.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(99, 447);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(192, 37);
            this.label5.TabIndex = 4;
            this.label5.Text = "Date of birth";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(99, 350);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(227, 37);
            this.label4.TabIndex = 3;
            this.label4.Text = "Phone number";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(99, 261);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(164, 37);
            this.label3.TabIndex = 2;
            this.label3.Text = "Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(99, 173);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 37);
            this.label2.TabIndex = 1;
            this.label2.Text = "Email";
            // 
            // btnDangXuat
            // 
            this.btnDangXuat.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnDangXuat.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDangXuat.Location = new System.Drawing.Point(216, 734);
            this.btnDangXuat.Name = "btnDangXuat";
            this.btnDangXuat.Size = new System.Drawing.Size(196, 62);
            this.btnDangXuat.TabIndex = 11;
            this.btnDangXuat.Text = "Sign out";
            this.btnDangXuat.UseVisualStyleBackColor = false;
            this.btnDangXuat.Click += new System.EventHandler(this.btnDangXuat_Click);
            // 
            // RoleField
            // 
            this.RoleField.Location = new System.Drawing.Point(106, 585);
            this.RoleField.Name = "RoleField";
            this.RoleField.ReadOnly = true;
            this.RoleField.Size = new System.Drawing.Size(430, 31);
            this.RoleField.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(99, 544);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 37);
            this.label6.TabIndex = 5;
            this.label6.Text = "Role";
            // 
            // ProfileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 831);
            this.Controls.Add(this.btnDangXuat);
            this.Controls.Add(this.DoBField);
            this.Controls.Add(this.PhonenumField);
            this.Controls.Add(this.UsernameField);
            this.Controls.Add(this.EmailField);
            this.Controls.Add(this.RoleField);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ProfileForm";
            this.Text = "Profile";
            this.ResumeLayout(false);
            this.PerformLayout();
            this.FormClosed += ProfileForm_FormClosed;

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox DoBField;
        private System.Windows.Forms.TextBox PhonenumField;
        private System.Windows.Forms.TextBox UsernameField;
        private System.Windows.Forms.TextBox EmailField;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDangXuat;
        private System.Windows.Forms.TextBox RoleField;
        private System.Windows.Forms.Label label6;
    }
}