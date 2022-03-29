using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;

namespace AGooday.AgPay.QRCode.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QRCodeController : ControllerBase
    {
        private readonly ILogger<QRCodeController> _logger;

        public QRCodeController(ILogger<QRCodeController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetQRCode")]
        public void Get(string text, string content, int moduleSize = 18)
        {
            string iconPath = ""; string backPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\img\qr\�Ŷ���_֧����_ӡˢ_����.jpg");
            using (var memoryStream = GenerateQRCode(text, content, iconPath, backPath, moduleSize))
            {
                Response.ContentType = "image/jpeg";
                Response.Body.WriteAsync(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Length));
                Response.Body.Close();
            }

            //QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
            //QrCode qrCode = new QrCode();
            //if (qrEncoder.TryEncode(content, out qrCode))
            //{
            //    var fCodeSize = new FixedCodeSize(moduleSize, QuietZoneModules.Zero);
            //    GraphicsRenderer renderer = new GraphicsRenderer(fCodeSize, Brushes.Black, Brushes.White);
            //    MemoryStream ms = new MemoryStream();
            //    renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, ms);

            //    Response.ContentType = "image/Png";
            //    Response.Body.WriteAsync(ms.ToArray());
            //    Response.Body.Close();
            //}
        }

        private MemoryStream GenerateQRCode(string text, string content, string iconPath = "", string backPath = "", int moduleSize = 18)
        {
            try
            {
                QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
                QrCode qrCode = qrEncoder.Encode(content);
                var fCodeSize = new FixedModuleSize(moduleSize, QuietZoneModules.Zero);
                GraphicsRenderer render = new GraphicsRenderer(fCodeSize, Brushes.Black, Brushes.White);
                DrawingSize dSize = render.SizeCalculator.GetSize(qrCode.Matrix.Width);

                Bitmap map = new Bitmap(dSize.CodeWidth, dSize.CodeWidth);
                Graphics g = Graphics.FromImage(map);
                render.Draw(g, qrCode.Matrix);

                if (!string.IsNullOrEmpty(iconPath) && System.IO.File.Exists(iconPath))
                {
                    //׷��LogoͼƬ ,ע�����LogoͼƬ��С�Ͷ�ά���С�ı���
                    //PS:׷�ӵ�ͼƬ���󳬹���ά����ݴ��ʻᵼ����Ϣ��ʧ,�޷���ʶ��
                    Image img = Image.FromFile(iconPath);
                    Point imgPoint = new Point((map.Width - img.Width) / 2, (map.Height - img.Height) / 2);
                    g.DrawImage(img, imgPoint.X, imgPoint.Y, img.Width, img.Height);
                }
                MemoryStream memoryStream = new MemoryStream();
                map.Save(memoryStream, ImageFormat.Jpeg);
                if (!string.IsNullOrEmpty(backPath) && System.IO.File.Exists(backPath))
                {
                    var image = Image.FromFile(@backPath);
                    //ȡ�ö�ά��ͼƬimage����
                    var qrcodePic = Image.FromStream(memoryStream);
                    Graphics gr = Graphics.FromImage(image);

                    gr.DrawImage(qrcodePic, (image.Width - qrcodePic.Width) / 2, 620, qrcodePic.Width, qrcodePic.Height);

                    //�ڱ�����Ƭ��������� 
                    Font font = new Font("΢���ź�", 12);//����Ĭ�������ʽ   
                    SizeF sizeF = gr.MeasureString(text, font);//�����ı�����
                    PointF drawPoint = new PointF((image.Width - sizeF.Width) / 2 + 12.0F, 1160.0F);
                    SolidBrush brush = new SolidBrush(Color.Black);  //����Ĭ�ϻ�ˢ��ɫ
                    gr.DrawString(text, font, brush, drawPoint); //ͼƬ���������

                    MemoryStream memoryNewStream = new MemoryStream();
                    image.Save(memoryNewStream, ImageFormat.Jpeg);

                    return memoryNewStream;
                }
                return memoryStream;
            }
            catch (Exception e)
            {
                _logger.LogDebug(e, $"���ɶ�ά�����{text}��{content}��{e.Message}");
                throw;
            }
        }
    }
}