﻿using System;
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

        int color_accuracy = 10;

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
            bool[,] is_painted = new bool[ImagePictureBox.Image.Width, ImagePictureBox.Image.Height];

            int x;
            int y;
            int dx = 0;
            int dy = 0;

            for(int k = 0; k < ImagePictureBox.Image.Width * ImagePictureBox.Image.Height; k++)
            {
                x = random.Next(ImagePictureBox.Image.Width);
                y = random.Next(ImagePictureBox.Image.Height);
                if (is_painted[x, y] == true)
                {
                    k--;
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
                            if (Math.Abs(((Bitmap)ImagePictureBox.Image).GetPixel(x + i, y + o).R - curr_color.R) > color_accuracy && Math.Abs(((Bitmap)ImagePictureBox.Image).GetPixel(x + i, y + o).G - curr_color.G) > color_accuracy && Math.Abs(((Bitmap)ImagePictureBox.Image).GetPixel(x + i, y + o).B - curr_color.B) > color_accuracy)
                            {
                                dx = i;
                                dy = o;
                                break;
                            }
                        }
                    }

                    /*List<Byte> r_colors = new List<Byte>();
                    List<Byte> g_colors = new List<Byte>();
                    List<Byte> b_colors = new List<Byte>();

                    r_colors.Add(curr_color.R);
                    g_colors.Add(curr_color.G);
                    b_colors.Add(curr_color.B);*/
                    
                    for(int i = 0; i <= dx; i++)
                    {
                        is_painted[x + i, (y + dy) / 2] = true;
                        /*r_colors.Add(((Bitmap)ImagePictureBox.Image).GetPixel(x + i, (y + dy) / 2).R);
                        g_colors.Add(((Bitmap)ImagePictureBox.Image).GetPixel(x + i, (y + dy) / 2).G);
                        b_colors.Add(((Bitmap)ImagePictureBox.Image).GetPixel(x + i, (y + dy) / 2).B);*/
                    }
                    for (int i = 0; i <= dy; i++)
                    {
                        is_painted[(x + dx) / 2, y + i] = true;
                        /*r_colors.Add(((Bitmap)ImagePictureBox.Image).GetPixel(x + i, (y + dy) / 2).R);
                        g_colors.Add(((Bitmap)ImagePictureBox.Image).GetPixel(x + i, (y + dy) / 2).G);
                        b_colors.Add(((Bitmap)ImagePictureBox.Image).GetPixel(x + i, (y + dy) / 2).B);*/
                    }

                    /*int r_sum = 0, g_sum = 0, b_sum = 0; 
                    for(int i=0; i < r_colors.Count; i++)
                    {
                        r_sum += r_colors[i];
                        g_sum += g_colors[i];
                        b_sum += b_colors[i];
                    }*/
                    //curr_color = Color.FromArgb(r_sum / r_colors.Count, g_sum / r_colors.Count, b_sum / r_colors.Count);
                   /* r_colors.Clear();
                    g_colors.Clear();
                    b_colors.Clear();*/
                    brush.Color = curr_color;
                    graphics.FillEllipse(brush, new Rectangle(x, y, dx, dy));
                }
            }
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
    }
}