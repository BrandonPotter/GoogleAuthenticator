using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Google.Authenticator.WebSample
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["key"]))
            {
                Response.Redirect("~/default.aspx?key=" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10));
            }

            this.lblSecretKey.Text = Request.QueryString["key"];

            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            var setupInfo = tfa.GenerateSetupCode("我 & You", "user@example.com", Request.QueryString["key"], false, 10);

            string qrCodeImageUrl = setupInfo.QrCodeSetupImageUrl;
            string manualEntrySetupCode = setupInfo.ManualEntryKey;

            this.imgQrCode.ImageUrl = qrCodeImageUrl;
            this.lblManualSetupCode.Text = manualEntrySetupCode;
        }

        protected void btnValidate_Click(object sender, EventArgs e)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            var result = tfa.ValidateTwoFactorPIN(Request.QueryString["key"], this.txtCode.Text);

            if (result)
            {
                this.lblValidationResult.Text = this.txtCode.Text + " is a valid PIN at UTC time " + DateTime.UtcNow.ToString();
                this.lblValidationResult.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                this.lblValidationResult.Text = this.txtCode.Text + " is not a valid PIN at UTC time " + DateTime.UtcNow.ToString();
                this.lblValidationResult.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}
