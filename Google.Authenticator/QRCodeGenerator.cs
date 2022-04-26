using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;

namespace Google.Authenticator
{
    internal class QRCodeGenerator
    {
        private static readonly string[] _SKIA_QR_TYPES = new string[] { "SkiaSharp.QrCode.QRCodeGenerator", "SkiaSharp.QrCode.QRCodeGenerator+EciMode", "SkiaSharp.QrCode.ECCLevel", "SkiaSharp.QrCode.QRCodeData", "SkiaSharp.QrCode.QRCodeRenderer"};
        private static readonly string[] _SKIA_TYPES = new string[] { "SkiaSharp.SKBitmap", "SkiaSharp.SKCanvas", "SkiaSharp.SKColor", "SkiaSharp.SKRect", "SkiaSharp.SKEncodedImageFormat", "SkiaSharp.SKPaint" };
        private static readonly string[] _QRCODER_TYPES = new string[] { "QRCoder.QRCodeGenerator", "QRCoder.QRCodeGenerator+ECCLevel", "QRCoder.QRCode", "QRCoder.QRCodeData"
#if NET6_0
            ,"QRCoder.PngByteQRCode"
#endif
        };
        private static readonly string[] _DRAWING_IMAGE_TYPES = new string[] { "System.Drawing.Imaging.ImageFormat" };

        private static Dictionary<string, Type> _objectTypes;
        private static bool _CAN_USE_SKIA = true;
        private static bool _CAN_USE_QRCODER = true;
        private static object _SKIA_BLACK = null;
        private static object _SKIA_WHITE = null;

        private static readonly decimal _QR_CODE_RATIO = ((decimal)4629*(decimal)21)/((decimal)177*(decimal)177);

        static QRCodeGenerator()
        {
            _objectTypes=new Dictionary<string, Type>();
            Assembly ass = null;
            try
            {
                ass = Assembly.Load("QRCoder");
            }
            catch (Exception e)
            {
                ass=null;
                _CAN_USE_QRCODER=false;
            }
            if (ass!=null)
            {
                foreach (string str in _QRCODER_TYPES)
                {
                    try
                    {
                        _objectTypes.Add(str, ass.GetType(str));
                    }
                    catch (Exception e)
                    {
                        _CAN_USE_QRCODER=false;
                    }
                }
            }
            try
            {
#if NET45
                ass = Assembly.Load(@"System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
#else
                ass = Assembly.Load("System.Drawing.Common");
#endif

            }
            catch (Exception e)
            {
                ass=null;
                _CAN_USE_QRCODER=false;
            }
            if (ass!=null)
            {
                foreach (string str in _DRAWING_IMAGE_TYPES)
                {
                    try
                    {
                        _objectTypes.Add(str, ass.GetType(str));
                    }
                    catch (Exception e)
                    {
                        _CAN_USE_QRCODER=false;
                    }
                }
            }
            try
            {
                ass = Assembly.Load("SkiaSharp.QrCode");
            }
            catch (Exception e) {
                ass=null;
                _CAN_USE_SKIA=false;
            }
            if (ass!=null)
            {
                foreach (string str in _SKIA_QR_TYPES)
                {
                    try
                    {
                        _objectTypes.Add(str, ass.GetType(str));
                    }catch(Exception e) { 
                        _CAN_USE_SKIA=false;
                    }
                }
            }
            try
            {
                ass = Assembly.Load("SkiaSharp");
            }
            catch (Exception e)
            {
                ass=null;
                _CAN_USE_SKIA=false;
            }
            if (ass!=null)
            {
                foreach (string str in _SKIA_TYPES)
                {
                    try
                    {
                        _objectTypes.Add(str, ass.GetType(str));
                    }
                    catch (Exception e)
                    {
                        _CAN_USE_SKIA=false;
                    }
                }
            }
            if (_CAN_USE_SKIA)
            {
                _SKIA_BLACK = _objectTypes["SkiaSharp.SKColor"].GetConstructor(new Type[] { typeof(byte), typeof(byte), typeof(byte) }).Invoke(new object[] { (byte)0, (byte)0, (byte)0 });
                _SKIA_WHITE = _objectTypes["SkiaSharp.SKColor"].GetConstructor(new Type[] { typeof(byte), typeof(byte), typeof(byte) }).Invoke(new object[] { (byte)255, (byte)255, (byte)255 });
            }
        }

        public static string GenerateQrCodeUrl(int qrPixelsPerModule, string provisionUrl)
        {
            if (!_CAN_USE_SKIA&&!_CAN_USE_QRCODER)
                throw new MissingDependencyException("Unable to generate a QR Code without either QRCoder or SkiaSharp.QrCode installed");
            var qrCodeUrl = "";

            try
            {
                using (var qrGenerator = (IDisposable)(_CAN_USE_SKIA ? _objectTypes["SkiaSharp.QrCode.QRCodeGenerator"] : _objectTypes["QRCoder.QRCodeGenerator"]).GetConstructor(Type.EmptyTypes).Invoke(new object[] { }))
                using(var qrCodeData = (IDisposable)_InvokeMethod((_CAN_USE_SKIA ? _objectTypes["SkiaSharp.QrCode.QRCodeGenerator"] : _objectTypes["QRCoder.QRCodeGenerator"]),qrGenerator,"CreateQrCode",new Dictionary<string, object> {
                            {"plainText",provisionUrl },
                            {"eccLevel",Enum.Parse((_CAN_USE_SKIA ? _objectTypes["SkiaSharp.QrCode.ECCLevel"] : _objectTypes["QRCoder.QRCodeGenerator+ECCLevel"]), "Q") }
                        }))
                {
                    using (var ms = new MemoryStream()) 
                    {
                        if (_CAN_USE_SKIA)
                        {
                            int CanvasSize = (int)Math.Ceiling((decimal)provisionUrl.Length*_QR_CODE_RATIO);
                            using (var img = (IDisposable)_objectTypes["SkiaSharp.SKBitmap"].GetConstructor(new Type[] { typeof(int), typeof(int), typeof(bool) }).Invoke(new object[] { CanvasSize, CanvasSize, false }))
                            using (var renderer = (IDisposable)_objectTypes["SkiaSharp.QrCode.QRCodeRenderer"].GetConstructor(Type.EmptyTypes).Invoke(new object[] { }))
                            using (var canvas = (IDisposable)_objectTypes["SkiaSharp.SKCanvas"].GetConstructor(new Type[] { _objectTypes["SkiaSharp.SKBitmap"] }).Invoke(new Object[] { img }))
                            {
                                _InvokeMethod(canvas.GetType(), canvas, "Clear", new Dictionary<string, object>() {});
                                _InvokeMethod(canvas.GetType(), canvas, "Clear", new Dictionary<string, object>() { {"color", _SKIA_WHITE } });
                                _InvokeMethod(_objectTypes["SkiaSharp.QrCode.QRCodeRenderer"], renderer, "Render", new Dictionary<string, object>()
                                {
                                    {"canvas", canvas},
                                    {"area", _objectTypes["SkiaSharp.SKRect"].GetConstructor(new Type[]{typeof(float),typeof(float),typeof(float),typeof(float)}).Invoke(new object[]{0f,0f,(float)CanvasSize, (float)CanvasSize})},
                                    {"data",qrCodeData },
                                    {"qrColor",_SKIA_BLACK }
                                });
                                _InvokeMethod(canvas.GetType(), canvas, "Flush", new Dictionary<string, object>() { });
                                var imgData = _InvokeMethod(img.GetType(), img, "Encode", new Dictionary<string, object>(){
                                    {"format",Enum.Parse(_objectTypes["SkiaSharp.SKEncodedImageFormat"], "Jpeg") },
                                    {"quality",100 }
                                });
                                imgData.GetType().GetMethod("SaveTo", new Type[] { typeof(Stream) }).Invoke(imgData, new object[] { ms });
                            }
                        }
                        else
                        {
#if NET6_0
                            using (var qrCode = (IDisposable)_objectTypes["QRCoder.PngByteQRCode"].GetConstructor(new Type[]{_objectTypes["QRCoder.QRCodeData"]}).Invoke(new object[]{qrCodeData}))
                            {
                                var data = (byte[])_InvokeMethod(qrCode.GetType(),qrCode,"GetGraphic",new Dictionary<string, object>(){
                                    {"pixelsPerModule",qrPixelsPerModule}
                                });
                                ms.Write(data,0,data.Length);
                            }
#else
                            using (var qrCode = (IDisposable)_objectTypes["QRCoder.QRCode"].GetConstructor(new Type[] { _objectTypes["QRCoder.QRCodeData"] }).Invoke(new object[] { qrCodeData }))
                            using (var qrCodeImage = (IDisposable)_InvokeMethod(_objectTypes["QRCoder.QRCode"], qrCode, "GetGraphic", new Dictionary<string, object>(){
                                {"pixelsPerModule", qrPixelsPerModule }
                            }))
                            {
                                _InvokeMethod(qrCodeImage.GetType(), qrCodeImage, "Save", new Dictionary<string, object>()
                                {
                                    {"stream",ms },
                                    {"format", _objectTypes["System.Drawing.Imaging.ImageFormat"].GetProperty("Png").GetValue(null)}
                                });
                            }
#endif
                        }
                        qrCodeUrl = $"data:image/{(_CAN_USE_SKIA ? "jpeg" :"png")};base64,{Convert.ToBase64String(ms.ToArray())}";
                    }
                    qrCodeData.Dispose();
                }
            }
            catch (TypeInitializationException e)
            {
                if (e.InnerException != null
                    && e.InnerException.GetType() == typeof(DllNotFoundException)
                    && e.InnerException.Message.Contains("libgdiplus"))
                {
                    throw new MissingDependencyException(
                        "It looks like libgdiplus has not been installed - see" +
                        " https://github.com/codebude/QRCoder/issues/227",
                        e);
                }
                else
                {
                    throw;
                }
            }
            catch (System.Runtime.InteropServices.ExternalException e)
            {
                if (e.Message.Contains("GDI+") && qrPixelsPerModule > 10)
                {
                    throw new QRException(
                        $"There was a problem generating a QR code. The value of {nameof(qrPixelsPerModule)}" +
                        " should be set to a value of 10 or less for optimal results.",
                        e);
                }
                else
                {
                    throw;
                }
            }
            return qrCodeUrl;
        }

        private static object _InvokeMethod(Type ownerType,object owner, string methodName, Dictionary<string, object> parameters)
        {
            MethodInfo mi = null;
            foreach (MethodInfo m in ownerType.GetMethods())
            {
                if (m.Name == methodName)
                {
                    bool isValid = true;
                    foreach (ParameterInfo pi in m.GetParameters())
                    {
                        if (!pi.IsOptional && !parameters.ContainsKey(pi.Name))
                        {
                            isValid=false;
                            break;
                        }
                    }
                    if (isValid)
                    {
                        mi=m;
                        break;
                    }
                }
            }
            if (mi!=null)
            {
                ParameterInfo[] pars = mi.GetParameters();
                object[] mpars = new object[pars.Length];
                for (int i = 0; i < pars.Length; i++)
                {
                    if (parameters.ContainsKey(pars[i].Name))
                        mpars[i]=parameters[pars[i].Name];
                    else
                        mpars[i]=Type.Missing;
                }
                return mi.Invoke(owner, mpars);
            }
            return null;
        }
    }
}
