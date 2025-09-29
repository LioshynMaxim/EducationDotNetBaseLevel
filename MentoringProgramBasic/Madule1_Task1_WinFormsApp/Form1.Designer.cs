namespace Madule1_Task1_WinFormsApp
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
            button = new Button();
            textBox = new TextBox();
            label = new Label();
            SuspendLayout();
            // 
            // button
            // 
            button.Location = new Point(19, 409);
            button.Name = "button";
            button.Size = new Size(152, 29);
            button.TabIndex = 0;
            button.Text = "Click to show";
            button.UseVisualStyleBackColor = true;
            button.Click += Button1_Click;
            // 
            // textBox
            // 
            textBox.Location = new Point(19, 25);
            textBox.Name = "textBox";
            textBox.Size = new Size(125, 27);
            textBox.TabIndex = 1;
            // 
            // label
            // 
            label.AutoSize = true;
            label.Location = new Point(19, 72);
            label.Name = "label";
            label.Size = new Size(45, 20);
            label.TabIndex = 2;
            label.Text = "Hello";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(303, 450);
            Controls.Add(label);
            Controls.Add(textBox);
            Controls.Add(button);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button;
        private TextBox textBox;
        private Label label;
    }
}
