using NetworkSocket.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;
using UnityEngine;

namespace NekoDice
{
    public class DanmakuWebsocketClient : WebSocketClient
    {
        public DateTime SERVER_HEARTBEAT = System.DateTime.Now;

        public DanmakuWebsocketClient(Uri address)
            : base(address)
        {
        }

        public DanmakuWebsocketClient(Uri address, RemoteCertificateValidationCallback certificateValidationCallback)
            : base(address, certificateValidationCallback)
        {
        }

        public DateTime GetHB()
        {
            return SERVER_HEARTBEAT;
        }

        protected override void OnClose(StatusCodes code, string reason)
        {
            Globle.AddDataLog("Danmaku", reason);
        }

        protected override void OnBinary(FrameRequest frame)
        {
            try
            {
                var text = Encoding.UTF8.GetString(frame.Content);
                //Debug.Log(text);
                processJson(text);
            }
            catch (Exception ex)
            {
                Globle.AddDataLog("Danmaku", Globle.LanguageController.GetLang("LOG.DanmakuClientException", ex.Message, ex.StackTrace));
            }
        }

        protected override void OnText(FrameRequest frame)
        {
            try
            {
                var text = Encoding.UTF8.GetString(frame.Content);
                //Debug.Log(text);
                processJson(text);
            }
            catch (Exception ex)
            {
                Globle.AddDataLog("Danmaku", Globle.LanguageController.GetLang("LOG.DanmakuClientException", ex.Message, ex.StackTrace));
            }
        }

        private void processJson(string text)
        {
            try
            {
                var jsonResult = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(text);
                if ((int)jsonResult["cmd"] == 233)
                {
                    SERVER_HEARTBEAT = System.DateTime.Now;
                }
                if ((int)jsonResult["cmd"] == 3)
                {
                    var i = 0f;
                    float.TryParse((string)jsonResult["data"]["num"], out i);
                    i = i * (float)Globle.Settings["Gift_ACFun_AddPrecentage"];
                    if ((bool)Globle.Settings["Gift_ACFun_UseGiftImage"])
                    {
                        if ((string)jsonResult["data"]["pngPicUrl"] != "")
                        {
                            while (i > 0)
                            {
                                i--;
                                var gift = new GiftObject();
                                gift.uuid = System.Guid.NewGuid().ToString();
                                gift.useGiftImage = true;
                                gift.needCrop = false;
                                gift.url = (string)jsonResult["data"]["pngPicUrl"];
                                gift.price = (float)jsonResult["data"]["totalCoin"];
                                Globle.GiftCache.Add(gift.uuid, gift);
                            }
                        }
                    }
                    else
                    {
                        if ((string)jsonResult["data"]["avatarUrl"] != "")
                        {
                            while (i > 0)
                            {
                                i--;
                                var gift = new GiftObject();
                                gift.uuid = System.Guid.NewGuid().ToString();
                                gift.useGiftImage = false;
                                gift.needCrop = true;
                                gift.url = (string)jsonResult["data"]["avatarUrl"];
                                gift.price = (float)jsonResult["data"]["totalCoin"];
                                Globle.GiftCache.Add(gift.uuid, gift);
                            }
                        }
                    }
                }
            }
            catch { }
        }
    }
}
