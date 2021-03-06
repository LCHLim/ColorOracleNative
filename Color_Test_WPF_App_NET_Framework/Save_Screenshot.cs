﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Color_Test_WPF_App_NET_Framework
{
    /// <summary>
    /// inheritance of the Save_Screenshot
    /// </summary>
    public partial class Save_Screenshot : Form
    {
        Bitmap bmp;
        PictureBox pb = new PictureBox();
        
        /// <summary>
        /// initialize the save_screenshot method which accept the select area form coordinate and size
        /// create the bit image and filter it, show it on the dialog finally 
        /// </summary>
        public Save_Screenshot(Int32 x, Int32 y, Int32 w, Int32 h, Size s)
        {
            InitializeComponent();
            
            Rectangle rect = new Rectangle(x, y, w, h);
            bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, s, CopyPixelOperation.SourceCopy);

           
            bmp = FilterImg(bmp);

            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = bmp.Size;
            
            pb.Dock = DockStyle.Fill;
            pb.Image = bmp;
            this.Controls.Add(pb);
            this.ShowDialog();

            
        }


        /// <summary>
        /// Main image filter process
        /// </summary>
        /// <param name="image">image to be simulated</param>
        /// <returns>simulated result</returns>
        private Bitmap FilterImg(Bitmap image)
        {
            Simulator simulator = new Simulator();
            int[] intArray = getIntArray(image);
            int[] result = simulator.Simulate(intArray);
            byte[] byteArray = intArrayToByteArray(result);

            return (Bitmap) imageFromRawBgrArray(byteArray, image.Width, image.Height, PixelFormat.Format24bppRgb);
        }

        /// <summary>
        /// obtain intArray from image
        /// intArray - 24 bits values of concatenated RGB binary representation  
        /// e.g:format (binary) RRRRRRRRGGGGGGGGBBBBBBBB -> (int) values
        /// </summary>
        /// <param name="image">image</param>
        /// <returns>intArray representation of image</returns>
        private int[] getIntArray(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;
            int[] rgbArray = new int[width * height];
            const int PixelWidth = 3;

            BitmapData data = image.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb
                );

            byte[] pixelData = new byte[data.Stride];
            for (int scanline = 0; scanline < data.Height; scanline++)
            {
                Marshal.Copy(data.Scan0 + (scanline * data.Stride), pixelData, 0, data.Stride);
                for (int pixeloffset = 0; pixeloffset < data.Width; pixeloffset++)
                {
                    // data is stored in memory as BGR.
                    // We want RGB, so we must do some bit-shuffling.
                    rgbArray[(scanline * width) + pixeloffset] =
                        (pixelData[pixeloffset * PixelWidth + 2] << 16) +   // R 
                        (pixelData[pixeloffset * PixelWidth + 1] << 8) +    // G
                        pixelData[pixeloffset * PixelWidth];                // B
                }
            }
            image.UnlockBits(data);
            return rgbArray;
        }



        /// <summary>
        /// convert intArray to ByteArray
        /// An element in intArray could generate three elements (RGB) in ByteArray.
        /// e.g: format [RRRRRRRRGGGGGGGGBBBBBBBB] -> [RRRRRRRR, GGGGGGGG, BBBBBBBB]
        /// </summary>
        /// <param name="data">intArray</param>
        /// <returns>ByteArray</returns>
        private byte[] intArrayToByteArray(int[] data)
        {
            int length = data.Length;
            byte[] dataBytes = new byte[length * 3];
            for (int i = 0; i < length; i++)
            {
                int byteIndex = i * 3;
                int val = data[i];
                // This code clears out everything but a specific part of the value
                // and then shifts the remaining piece down to the lowest byte
                dataBytes[byteIndex + 0] = (byte)(val & 0xFF); // B
                dataBytes[byteIndex + 1] = (byte)((val & 0xFF00) >> 08); // G
                dataBytes[byteIndex + 2] = (byte)((val & 0xFF0000) >> 16); // R
            }
            return dataBytes;
        }


        /// <summary>
        /// convert byteArray into Image
        /// </summary>
        /// <param name="arr">byteArray</param>
        /// <param name="width">width of image</param>
        /// <param name="height">height of image</param>
        /// <param name="pixelFormat">pixel format of image</param>
        /// <returns>Image generated based on the byteArray</returns>
        private Image imageFromRawBgrArray(byte[] arr, int width, int height, PixelFormat pixelFormat)
        {
            var output = new Bitmap(width, height, pixelFormat);
            var rect = new Rectangle(0, 0, width, height);
            var bmpData = output.LockBits(rect, ImageLockMode.ReadWrite, output.PixelFormat);

            // Row-by-row copy ** else it would generate incomplete image **
            var arrRowLength = width * Image.GetPixelFormatSize(output.PixelFormat) / 8;
            var ptr = bmpData.Scan0;
            for (var i = 0; i < height; i++)
            {
                Marshal.Copy(arr, i * arrRowLength, ptr, arrRowLength);
                ptr += bmpData.Stride;
            }

            output.UnlockBits(bmpData);
            return output;
        }



        /// <summary>
        /// save button for local storage when save button has been triggered and hide the form as well
        /// </summary>
        private void Button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.CheckPathExists = true;
            sfd.FileName = "Capture";
            sfd.Filter = "PNG Image(*.png)|*.png|JPG Image(*.jpg)|*.jpg|BMP Image(*.bmp)|*.bmp";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                pb.Image.Save(sfd.FileName);
            }
            this.Hide();
        }
        /// <summary>
        /// Form load placeholder to prevent from getting compilation error
        /// </summary>
         private void Save_Screenshot_Load(object sender, EventArgs e)
        {

        }

    }
}
