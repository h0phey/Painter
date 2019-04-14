using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WatislovPaint
{
    public partial class MainForm : Form
    {
        Graphics graphics;
        SolidBrush brush;
        Bitmap bitmap;

        int color_accuracy = 5;

        Random random = new Random();

        public MainForm()
        {
            InitializeComponent();
            brush = new SolidBrush(Color.Black);
            graphics = DrawingPictureBox.CreateGraphics();
        }

        private void ExitButton_Click(object sender, EventArgs e)//Exit button
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)//Load image button
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.PNG;*.GIF)|*.BMP;*.JPG;*.PNG;*.GIF|All files(*.*)|*.*";

            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Image image = new Bitmap(openFileDialog.FileName);
                    if(image.Width > image.Height)
                    {
                        ImagePictureBox.Image = Resizer(image, true);
                    }
                    else
                    {
                        ImagePictureBox.Image = Resizer(image, false);
                    }
                    StartButton.Enabled = true;
                    graphics.Clear(Color.White);
                    ExportButton.Enabled = false;
                    label2.Visible = false;
                }
                catch
                {
                    MessageBox.Show("Wrong file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void StartButton_Click(object sender, EventArgs e)//Start button
        {
            Draw();
        }

        public void Draw()//Drawing func
        {
            graphics.Clear(Color.Black);
            bool[,] is_painted = new bool[ImagePictureBox.Image.Width, ImagePictureBox.Image.Height];

            int x;
            int y;
            int dx = 0;
            int dy = 0;

            brush.Color = Color.Black;
            Progress.Maximum = ImagePictureBox.Image.Width * ImagePictureBox.Image.Height;
            Progress.Value = 0;
            bitmap = new Bitmap(ImagePictureBox.Image.Width, ImagePictureBox.Image.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(brush, new Rectangle(0, 0, ImagePictureBox.Image.Width, ImagePictureBox.Image.Height));
                g.Dispose();
            }

            for (int k = 0; k < ImagePictureBox.Image.Width * ImagePictureBox.Image.Height; k++)
            {
                x = random.Next(ImagePictureBox.Image.Width);
                y = random.Next(ImagePictureBox.Image.Height);
                Progress.Value++;
                if (is_painted[x, y] == true)
                {

                }
                else
                {
                    dx = 0;
                    dy = 0;
                    Color curr_color = ((Bitmap)ImagePictureBox.Image).GetPixel(x, y);

                    while (x + dx < ImagePictureBox.Image.Width - 1)//search diff color at X
                    {
                        dx++;
                        if (Math.Abs(((Bitmap)ImagePictureBox.Image).GetPixel(x + dx, y).R - curr_color.R) > color_accuracy && Math.Abs(((Bitmap)ImagePictureBox.Image).GetPixel(x + dx, y).G - curr_color.G) > color_accuracy && Math.Abs(((Bitmap)ImagePictureBox.Image).GetPixel(x + dx, y).B - curr_color.B) > color_accuracy)
                        {
                            dx--;
                            break;
                        }

                    }
                    while (y + dy < ImagePictureBox.Image.Height - 1)//search diff color at Y
                    {
                        dy++;
                        if (Math.Abs(((Bitmap)ImagePictureBox.Image).GetPixel(x, y + dy).R - curr_color.R) > color_accuracy && Math.Abs(((Bitmap)ImagePictureBox.Image).GetPixel(x, y + dy).G - curr_color.G) > color_accuracy && Math.Abs(((Bitmap)ImagePictureBox.Image).GetPixel(x, y + dy).B - curr_color.B) > color_accuracy)
                        {
                            dy--;
                            break;
                        }
                    }

                    for (int i = 0; i <= dx; i++)//search diff color at rectangle[x + dx, y + dy]
                    {
                        for (int o = 0; o <= dy; o++)
                        {
                            if (Math.Abs(((Bitmap)ImagePictureBox.Image).GetPixel(x + i, y + o).R - curr_color.R) > color_accuracy && Math.Abs(((Bitmap)ImagePictureBox.Image).GetPixel(x + i, y + o).G - curr_color.G) > color_accuracy && Math.Abs(((Bitmap)ImagePictureBox.Image).GetPixel(x + i, y + o).B - curr_color.B) > color_accuracy && is_painted[x + i, y + o] == true)
                            {
                                dx = i;
                                dy = o;
                                break;
                            }
                        }
                    }

                    for (int i = 0; i <= dx; i++)
                    {
                        for (int o = 0; o <= dy; o++)
                        {
                            is_painted[x + i, y + o] = true;
                        }
                    }
                    brush.Color = curr_color;
                    graphics.FillRectangle(brush, new Rectangle(x, y, dx, dy));
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.FillRectangle(brush, new Rectangle(x, y, dx, dy));
                        g.Dispose();
                    }
                    DrawingPictureBox.Image = bitmap;
                }
            }
            ExportButton.Enabled = true;
            label2.Visible = true;
        }

        public Image Resizer(Image image, bool icase)//Resize image to fit in picturebox
        {
            if (icase)
            {
                double ih = image.Height, iw = image.Width, ipbw = ImagePictureBox.Width;
                int w = ImagePictureBox.Width, h = (int)((ih / iw) * ipbw);
                Bitmap bmp = new Bitmap(w, h);
                Graphics g = Graphics.FromImage(bmp);
                g.DrawImage(image, 0, 0, bmp.Width, bmp.Height);
                g.Dispose();
                return bmp;
            }
            else
            {
                double ih = image.Height, iw = image.Width, ipbh = ImagePictureBox.Height;
                int w = (int)(iw / ih * ipbh), h = ImagePictureBox.Height;
                Bitmap bmp = new Bitmap(w, h);
                Graphics g = Graphics.FromImage(bmp);
                g.DrawImage(image, 0, 0, bmp.Width, bmp.Height);
                g.Dispose();
                return bmp;
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap picture = new Bitmap(bitmap);
                picture.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
    }
}
