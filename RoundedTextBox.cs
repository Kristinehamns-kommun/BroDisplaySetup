using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
class RoundedTextBox : UserControl
{
    private TextBox textBox;
    private const int paddingLeft = 15; // Padding between the text box and the rounded rectangle
    private const int paddingRight = 15; // Padding between the text box and the rounded rectangle
    private const int paddingTop = 15; // Padding between the text box and the rounded rectangle
    private const int paddingBottom = 20; // Padding between the text box and the rounded rectangle

    public RoundedTextBox()
    {
        textBox = new TextBox
        {
            Multiline = false,
            BorderStyle = BorderStyle.FixedSingle,
            TextAlign = HorizontalAlignment.Center
        };

        Controls.Add(textBox);

        // Attach event handlers
        textBox.KeyPress += TextBox_KeyPress;
        textBox.KeyUp += TextBox_KeyUp;
        textBox.KeyDown += TextBox_KeyDown;
        textBox.GotFocus += TextBox_GotFocus;
        textBox.LostFocus += TextBox_LostFocus;
    }

    private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
    {
        //keyPressInRoundedTextBox?.Invoke(this, e);
        base.OnKeyPress(e);
    }

    //add other event handlers
    private void TextBox_KeyUp(object sender, KeyEventArgs e)
    {
        //keyUpInRoundedTextBox?.Invoke(this, e);
        base.OnKeyUp(e);
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        //keyDownInRoundedTextBox?.Invoke(this, e);
        base.OnKeyDown(e);
    }

    private void TextBox_GotFocus(object sender, EventArgs e)
    {
        //gotFocusInRoundedTextBox?.Invoke(this, e);
        base.OnGotFocus(e);
    }

    private void TextBox_LostFocus(object sender, EventArgs e)
    {
        //lostFocusInRoundedTextBox?.Invoke(this, e);
        base.OnLostFocus(e);
    }
    public new bool Focus()
    {
        return base.Focus() && textBox.Focus();
    }

    public new string Text
    {
        get { return textBox.Text; }
        set { textBox.Text = value; }
    }

    public new Font Font
    {
        get { return textBox.Font; }
        set { textBox.Font = value; }
    }

    public new int Width
    {
        get { return base.Width; }
        set
        {
            textBox.Width = value - paddingLeft - paddingRight;
            Size = new Size(value, textBox.Height + paddingTop + paddingBottom);
        }
    }
    public new Point Location
    {
        get { return base.Location; }
        set
        {
            base.Location = value;
            textBox.Location = new Point(paddingLeft, paddingTop);
        }
    }

    public int MaxLength
    {
        get { return textBox.MaxLength; }
        set { textBox.MaxLength = value; }
    }

    public new BorderStyle BorderStyle
    {
        get { return textBox.BorderStyle; }
        set { textBox.BorderStyle = value; }
    }

    public new Color BackColor
    {
        get { return textBox.BackColor; }
        set { textBox.BackColor = value; }
    }

    public new Color ForeColor
    {
        get { return textBox.ForeColor; }
        set { textBox.ForeColor = value; }
    }

    public new object Tag
    {
        get { return textBox.Tag; }
        set { textBox.Tag = value; }
    }

    public HorizontalAlignment TextAlign
    {
        get { return textBox.TextAlign; }
        set { textBox.TextAlign = value; }
    }

    public new Padding Margin
    {
        get { return textBox.Margin; }
        set { textBox.Margin = value; }
    }


    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        using (var path = new GraphicsPath())
        using (var pen = new Pen(Color.White, 2))
        {
            RectangleF rect = new RectangleF(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
            int radius = 10;

            // Create a rounded rectangle path
            path.AddArc(rect.X, rect.Y, 2 * radius, 2 * radius, 180, 90); // Top-left corner
            path.AddArc(rect.X + rect.Width - 2 * radius, rect.Y, 2 * radius, 2 * radius, 270, 90); // Top-right corner
            path.AddArc(rect.X + rect.Width - 2 * radius, rect.Y + rect.Height - 2 * radius, 2 * radius, 2 * radius, 0, 90); // Bottom-right corner
            path.AddArc(rect.X, rect.Y + rect.Height - 2 * radius, 2 * radius, 2 * radius, 90, 90); // Bottom-left corner
            path.CloseFigure();

            // Draw the rounded rectangle
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.FillPath(Brushes.White, path);
            e.Graphics.DrawPath(pen, path);
        }
    }

    protected void OnPaintV1(PaintEventArgs e)
    {
        base.OnPaint(e);

        using (var path = new GraphicsPath())
        using (var pen = new Pen(Color.White, 2))
        {
            RectangleF rect = new RectangleF(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
            int radius = 10;

            // Create a rounded rectangle path
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90); // Top-left corner
            path.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90); // Top-right corner
            path.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90); // Bottom-right corner
            path.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90); // Bottom-left corner
            path.CloseFigure();

            // Draw the rounded rectangle
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.FillPath(Brushes.White, path);
            e.Graphics.DrawPath(pen, path);
        }
    }
}
