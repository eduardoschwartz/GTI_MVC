
using System.Web;
using System.Web.Mvc;
using System.Drawing.Imaging;


namespace GTI_MVC {
    public class CaptchaResultNew:ActionResult {
        public string _captchaText;
        public CaptchaResultNew(string captchaText) {
            _captchaText = captchaText;
        }
        public override void ExecuteResult(ControllerContext context) {
            CaptchaNew c = new CaptchaNew();
            c.Text = _captchaText;
            c.Width = 100;
            c.Height = 30;
            c.FamilyName = "";

            HttpContextBase cb = context.HttpContext;

            cb.Response.Clear();
            cb.Response.ContentType = "image/jpeg";
            c.Image.Save(cb.Response.OutputStream, ImageFormat.Jpeg);
            c.Dispose();
        }
    }
}