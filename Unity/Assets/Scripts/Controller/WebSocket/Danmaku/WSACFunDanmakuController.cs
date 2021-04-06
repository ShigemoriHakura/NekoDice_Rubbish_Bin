using NetworkSocket.WebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace NekoDice
{
    public class WSACFunDanmakuController : MonoBehaviour
    {
        public DanmakuController DanmakuController;
        private DanmakuWebsocketClient Danmakuclient;

        void Start()
        {
            if ((bool)Globle.Settings["Gift_ACFun_StartAtSetup"])
            {
                Invoke("DanmakuClientStart", 3f);
            }
        }

        void Update()
        {
            if (Danmakuclient != null && Danmakuclient.IsConnected)
            {
                var time = DateTime.Now - Danmakuclient.GetHB();
                if (time.TotalSeconds > 5)
                {
                    DanmakuClientStop(false);
                    DanmakuClientStart();
                }
            }
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受
        }

        public async void DanmakuClientStart()
        {
            try
            {
                var uri = new Uri("wss://danmaku.loli.ren/chat");
                Danmakuclient = new DanmakuWebsocketClient(uri, CheckValidationResult);

                var addresses = System.Net.Dns.GetHostAddresses(uri.Host);
                if (addresses.Length == 0)
                {
                    throw new ArgumentException(
                        Globle.LanguageController.GetLang("LOG.DanmakuClientIPFailed"),
                        ""
                    );
                }

                await Danmakuclient.ConnectAsync(addresses[0], uri.Port);

                if (Danmakuclient.IsConnected)
                {
                    Globle.AddDataLog("Danmaku ACFun", Globle.LanguageController.GetLang("LOG.DanmakuClientStarted"));
                    Danmakuclient.SendText("{\"cmd\":1,\"data\":{ \"roomId\":" + (int)Globle.Settings["Gift_ACFun_Roomid"] + ",\"version\":\"9.9.9\", \"config\":{ \"autoTranslate\":false}}}");
                }
                else
                {
                    Globle.AddDataLog("Danmaku ACFun", Globle.LanguageController.GetLang("LOG.DanmakuClientStartFailed"));
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message + "|" + ex.StackTrace);
                Globle.AddDataLog("Danmaku ACFun", Globle.LanguageController.GetLang("LOG.DanmakuClientException", ex.Message));
            }
        }

        public void DanmakuClientStop(bool alert = true)
        {
            if (Danmakuclient != null)
            {
                Danmakuclient.Close();
                Danmakuclient.Dispose();
                Danmakuclient = null;
            }
            if (alert) {
                Globle.AddDataLog("Danmaku ACFun", Globle.LanguageController.GetLang("LOG.DanmakuClientStopped"));
            }
        }

        private void OnDestroy()
        {
            this.DanmakuClientStop();
        }

    }
}