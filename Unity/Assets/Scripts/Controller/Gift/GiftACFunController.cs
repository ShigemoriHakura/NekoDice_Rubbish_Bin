using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NekoDice
{
    public class GiftACFunController : MonoBehaviour
    {
        public GiftItemHelper giftItemHelper;
        public WSACFunDanmakuController WSACFunDanmakuController;

        void Start()
        {
            giftItemHelper.Toggle.GetComponent<Toggle>().isOn = (bool)Globle.Settings["Gift_ACFun_StartAtSetup"];
            giftItemHelper.Toggle.GetComponent<Toggle>().onValueChanged.AddListener((bool isOn) => { saveAutoStart(isOn); });

            giftItemHelper.InputField.GetComponent<TMP_InputField>().text = Globle.Settings["Gift_ACFun_Roomid"].ToString();
            giftItemHelper.InputField.GetComponent<TMP_InputField>().onEndEdit.AddListener((string num) => { saveLiveRoomNumber(num); });

            giftItemHelper.Switch.GetComponent<SwitchManager>().OnEvents.AddListener(() => { ToggleACFunSwitch(true); });
            giftItemHelper.Switch.GetComponent<SwitchManager>().OffEvents.AddListener(() => { ToggleACFunSwitch(false); });

            giftItemHelper.GiftSwitch.GetComponent<SwitchManager>().OnEvents.AddListener(() => { ToggleACFunGiftSwitch(true); });
            giftItemHelper.GiftSwitch.GetComponent<SwitchManager>().OffEvents.AddListener(() => { ToggleACFunGiftSwitch(false); });

            giftItemHelper.WeightSwitch.GetComponent<SwitchManager>().OnEvents.AddListener(() => { ToggleACFunWeightSwitch(true); });
            giftItemHelper.WeightSwitch.GetComponent<SwitchManager>().OffEvents.AddListener(() => { ToggleACFunWeightSwitch(false); });

            giftItemHelper.Slider.GetComponent<SliderManager>().mainSlider.value = (float)Globle.Settings["Gift_ACFun_AddPrecentage"];
            giftItemHelper.Slider.GetComponent<SliderManager>().mainSlider.onValueChanged.AddListener((float num) => { saveAddPrecentage(num); });

            if ((bool)Globle.Settings["Gift_ACFun_StartAtSetup"])
            {
                giftItemHelper.Switch.GetComponent<SwitchManager>().isOn = true;
                giftItemHelper.Switch.GetComponent<SwitchManager>().switchAnimator.Play("Switch On");
            }

            if ((bool)Globle.Settings["Gift_ACFun_UseGiftImage"])
            {
                giftItemHelper.GiftSwitch.GetComponent<SwitchManager>().isOn = true;
                giftItemHelper.GiftSwitch.GetComponent<SwitchManager>().switchAnimator.Play("Switch On");
            }

            if ((bool)Globle.Settings["Gift_ACFun_UseGiftWeight"])
            {
                giftItemHelper.WeightSwitch.GetComponent<SwitchManager>().isOn = true;
                giftItemHelper.WeightSwitch.GetComponent<SwitchManager>().switchAnimator.Play("Switch On");
            }
        }

        void saveAutoStart(bool isOn)
        {
            Globle.Settings["Gift_ACFun_StartAtSetup"] = isOn;
            Globle.saveSettings();
        }

        void ToggleACFunSwitch(bool isOn)
        {
            if (isOn)
            {
                WSACFunDanmakuController.DanmakuClientStart();
            }
            else
            {
                WSACFunDanmakuController.DanmakuClientStop();
            }
        }

        void ToggleACFunGiftSwitch(bool isOn)
        {
            Globle.Settings["Gift_ACFun_UseGiftImage"] = isOn;
            Globle.saveSettings();
        }

        void ToggleACFunWeightSwitch(bool isOn)
        {
            Globle.Settings["Gift_ACFun_UseGiftWeight"] = isOn;
            Globle.saveSettings();
        }

        void saveLiveRoomNumber(string num)
        {
            var numRoom = 1;
            int.TryParse(num, out numRoom);
            Globle.Settings["Gift_ACFun_Roomid"] = numRoom;
            Globle.saveSettings();
        }

        void saveAddPrecentage(float num)
        {
            Globle.Settings["Gift_ACFun_AddPrecentage"] = num;
            Globle.saveSettings(false);
        }
    }
}