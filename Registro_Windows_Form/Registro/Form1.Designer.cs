namespace Registro
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.User_Name = new System.Windows.Forms.TextBox();
            this.Welcome_Label = new System.Windows.Forms.Label();
            this.Log_In_Button = new System.Windows.Forms.Button();
            this.Password = new System.Windows.Forms.TextBox();
            this.User_Name_Label = new System.Windows.Forms.Label();
            this.Password_Label = new System.Windows.Forms.Label();
            this.Sign_Up_Button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(72, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 0;
            // 
            // User_Name
            // 
            this.User_Name.Location = new System.Drawing.Point(75, 60);
            this.User_Name.Name = "User_Name";
            this.User_Name.Size = new System.Drawing.Size(100, 20);
            this.User_Name.TabIndex = 1;
            // 
            // Welcome_Label
            // 
            this.Welcome_Label.AutoSize = true;
            this.Welcome_Label.BackColor = System.Drawing.SystemColors.ControlDark;
            this.Welcome_Label.Location = new System.Drawing.Point(72, 35);
            this.Welcome_Label.Name = "Welcome_Label";
            this.Welcome_Label.Size = new System.Drawing.Size(58, 13);
            this.Welcome_Label.TabIndex = 2;
            this.Welcome_Label.Text = "Welcome!!";
            // 
            // Log_In_Button
            // 
            this.Log_In_Button.BackColor = System.Drawing.SystemColors.Control;
            this.Log_In_Button.Location = new System.Drawing.Point(55, 112);
            this.Log_In_Button.Name = "Log_In_Button";
            this.Log_In_Button.Size = new System.Drawing.Size(75, 23);
            this.Log_In_Button.TabIndex = 3;
            this.Log_In_Button.Text = "LOG IN";
            this.Log_In_Button.UseVisualStyleBackColor = false;
            this.Log_In_Button.Click += new System.EventHandler(this.Log_In_Button_Click);
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(75, 86);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(100, 20);
            this.Password.TabIndex = 4;
            // 
            // User_Name_Label
            // 
            this.User_Name_Label.AutoSize = true;
            this.User_Name_Label.Location = new System.Drawing.Point(181, 63);
            this.User_Name_Label.Name = "User_Name_Label";
            this.User_Name_Label.Size = new System.Drawing.Size(106, 13);
            this.User_Name_Label.TabIndex = 5;
            this.User_Name_Label.Text = "Enter your Username";
            // 
            // Password_Label
            // 
            this.Password_Label.AutoSize = true;
            this.Password_Label.Location = new System.Drawing.Point(181, 86);
            this.Password_Label.Name = "Password_Label";
            this.Password_Label.Size = new System.Drawing.Size(103, 13);
            this.Password_Label.TabIndex = 6;
            this.Password_Label.Text = "Enter your password";
            // 
            // Sign_Up_Button
            // 
            this.Sign_Up_Button.BackColor = System.Drawing.SystemColors.Control;
            this.Sign_Up_Button.Location = new System.Drawing.Point(136, 112);
            this.Sign_Up_Button.Name = "Sign_Up_Button";
            this.Sign_Up_Button.Size = new System.Drawing.Size(75, 23);
            this.Sign_Up_Button.TabIndex = 10;
            this.Sign_Up_Button.Text = "SIGN UP";
            this.Sign_Up_Button.UseVisualStyleBackColor = false;
            this.Sign_Up_Button.Click += new System.EventHandler(this.Sign_Up_Button_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Sign_Up_Button);
            this.Controls.Add(this.Password_Label);
            this.Controls.Add(this.User_Name_Label);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.Log_In_Button);
            this.Controls.Add(this.Welcome_Label);
            this.Controls.Add(this.User_Name);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox User_Name;
        private System.Windows.Forms.Label Welcome_Label;
        private System.Windows.Forms.Button Log_In_Button;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Label User_Name_Label;
        private System.Windows.Forms.Label Password_Label;
        private System.Windows.Forms.Button Sign_Up_Button;
    }
}

