﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.Helpers;
using Senparc.Weixin.TenPayV3.Apis;
using Senparc.Weixin.TenPayV3.Apis.BasePay;
using Senparc.Weixin.TenPayV3.Entities;
using Senparc.Weixin.TenPayV3.Helpers;
using Senparc.Weixin.TenPayV3.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Xml.Linq;
using static Senparc.Weixin.TenPayV3.Apis.BasePay.CombineTransactionsRequestData;

namespace Senparc.Weixin.TenPayV3.Apis.Tests
{
    [TestClass()]
    public class BasePayApisTests : BaseTenPayTest
    {

        [TestMethod()]
        public void CertificatesTest()
        {
            BasePayApis basePayApis = new BasePayApis();
            var certs = basePayApis.CertificatesAsync().GetAwaiter().GetResult();
            Assert.IsNotNull(certs);
            Console.WriteLine(certs.ToJson(true));
            Assert.IsTrue(certs.ResultCode.Success);
            Assert.IsNull(certs.VerifySignSuccess);//不参与验证

            Console.WriteLine();

            var tenpayV3Setting = Senparc.Weixin.Config.SenparcWeixinSetting.TenpayV3Setting;
            var cert = certs.data.First();
            var pubKey = ApiSecurityHelper.AesGcmDecryptCiphertext(tenpayV3Setting.TenPayV3_APIv3Key, cert.encrypt_certificate.nonce,
                     cert.encrypt_certificate.associated_data, cert.encrypt_certificate.ciphertext);
            Console.WriteLine(pubKey);
            Assert.IsNotNull(pubKey);
        }

        [TestMethod()]
        public void GetPublicKeysAsyncTest()
        {
            BasePayApis basePayApis = new BasePayApis();
            var publicKeys = basePayApis.GetPublicKeysAsync().GetAwaiter().GetResult();
            Assert.IsNotNull(publicKeys);
            Console.WriteLine(publicKeys.ToJson(true));
        }

        [TestMethod()]
        public void JsAPiAsyncTest()
        {
            var key = TenPayHelper.GetRegisterKey(Config.SenparcWeixinSetting);

            var TenPayV3Info = TenPayV3InfoCollection.Data[key];

            var price = 100;
            var name = "单元测试-" + DateTime.Now.ToString();
            var openId = "olPjZjsXuQPJoV0HlruZkNzKc91E";//换成测试人的 OpenId
            var sp_billno = string.Format("{0}{1}{2}", TenPayV3Info.MchId/*10位*/, SystemTime.Now.ToString("yyyyMMddHHmmss"),
                         TenPayV3Util.BuildRandomStr(6));

            //TODO: JsApiRequestData修改构造函数参数顺序
            TransactionsRequestData jsApiRequestData = new(TenPayV3Info.AppId, TenPayV3Info.MchId, name, sp_billno, new TenpayDateTime(DateTime.Now.AddHours(1)), null, TenPayV3Info.TenPayV3Notify, null, new() { currency = "CNY", total = price }, new(openId), null, null, null);

            BasePayApis basePayApis = new BasePayApis();
            var result = basePayApis.JsApiAsync(jsApiRequestData).GetAwaiter().GetResult();

            Console.WriteLine("微信支付 V3 预支付结果：" + result.ToJson(true));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ResultCode.Success);
            Assert.IsTrue(result.VerifySignSuccess == true);//通过验证
        }

        [TestMethod()]
        public void AppAsyncTest()
        {
            var key = TenPayHelper.GetRegisterKey(Config.SenparcWeixinSetting);

            var TenPayV3Info = TenPayV3InfoCollection.Data[key];

            var price = 100;
            var name = "单元测试-" + DateTime.Now.ToString();
            var openId = "olPjZjsXuQPJoV0HlruZkNzKc91E";//换成测试人的 OpenId
            var sp_billno = string.Format("{0}{1}{2}", TenPayV3Info.MchId/*10位*/, SystemTime.Now.ToString("yyyyMMddHHmmss"),
                         TenPayV3Util.BuildRandomStr(6));

            TransactionsRequestData requestData = new(TenPayV3Info.AppId, TenPayV3Info.MchId, name, sp_billno, new TenpayDateTime(DateTime.Now.AddHours(1)), null, TenPayV3Info.TenPayV3Notify, null, new() { currency = "CNY", total = price }, new(openId), null, null, null);

            BasePayApis basePayApis = new BasePayApis();
            var result = basePayApis.AppAsync(requestData).GetAwaiter().GetResult();

            Console.WriteLine("微信支付 V3 预支付结果：" + result.ToJson(true));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ResultCode.Success);
            Assert.IsTrue(result.VerifySignSuccess == true);//通过验证
        }

        [TestMethod()]
        public void H5AsyncTest()
        {
            var key = TenPayHelper.GetRegisterKey(Config.SenparcWeixinSetting);

            var TenPayV3Info = TenPayV3InfoCollection.Data[key];

            var price = 100;
            var name = "单元测试-" + DateTime.Now.ToString();
            var openId = "olPjZjsXuQPJoV0HlruZkNzKc91E";//换成测试人的 OpenId
            var sp_billno = string.Format("{0}{1}{2}", TenPayV3Info.MchId/*10位*/, SystemTime.Now.ToString("yyyyMMddHHmmss"),
                         TenPayV3Util.BuildRandomStr(6));

            //注意：H5下单scene_info参数必填
            TransactionsRequestData requestData = new(TenPayV3Info.AppId, TenPayV3Info.MchId, name, sp_billno, new TenpayDateTime(DateTime.Now.AddHours(1)), null, TenPayV3Info.TenPayV3Notify, null, new() { currency = "CNY", total = price }, new(openId), null, null, new("14.23.150.211", null, null, new("Android", null, null, null, null)));

            BasePayApis basePayApis = new BasePayApis();
            var result = basePayApis.H5Async(requestData).GetAwaiter().GetResult();

            Console.WriteLine("微信支付 V3 预支付结果：" + result.ToJson(true));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ResultCode.Success);
            Assert.IsTrue(result.VerifySignSuccess == true);//通过验证
        }

        [TestMethod()]
        public void NativeAsyncTest()
        {
            var key = TenPayHelper.GetRegisterKey(Config.SenparcWeixinSetting);

            var TenPayV3Info = TenPayV3InfoCollection.Data[key];

            var price = 100;
            var name = "单元测试-" + DateTime.Now.ToString();
            var openId = "olPjZjsXuQPJoV0HlruZkNzKc91E";//换成测试人的 OpenId
            var sp_billno = string.Format("{0}{1}{2}", TenPayV3Info.MchId/*10位*/, SystemTime.Now.ToString("yyyyMMddHHmmss"),
                         TenPayV3Util.BuildRandomStr(6));

            TransactionsRequestData requestData = new(TenPayV3Info.AppId, TenPayV3Info.MchId, name, sp_billno, new TenpayDateTime(DateTime.Now.AddHours(1)), null, TenPayV3Info.TenPayV3Notify, null, new() { currency = "CNY", total = price }, new(openId), null, null, null);

            BasePayApis basePayApis = new BasePayApis();
            var result = basePayApis.NativeAsync(requestData).GetAwaiter().GetResult();

            Console.WriteLine("微信支付 V3 预支付结果：" + result.ToJson(true));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ResultCode.Success);
            Assert.IsTrue(result.VerifySignSuccess == true);//通过验证
        }

        [TestMethod()]
        public void JsApiCombineAsyncTest()
        {
            var key = TenPayHelper.GetRegisterKey(Config.SenparcWeixinSetting);

            var TenPayV3Info = TenPayV3InfoCollection.Data[key];

            var price = 100;
            var name = "单元测试-" + DateTime.Now.ToString();
            var openId = "olPjZjsXuQPJoV0HlruZkNzKc91E";//换成测试人的 OpenId
            var combine_sp_billno = string.Format("{0}{1}{2}", TenPayV3Info.MchId/*10位*/, SystemTime.Now.ToString("yyyyMMddHHmmss"),
             TenPayV3Util.BuildRandomStr(6));
            var sp_billno = string.Format("{0}{1}{2}", TenPayV3Info.MchId/*10位*/, SystemTime.Now.ToString("yyyyMMddHHmmss"),
                         TenPayV3Util.BuildRandomStr(6));

            CombineTransactionsRequestData requestData = new(TenPayV3Info.AppId, TenPayV3Info.MchId, combine_sp_billno, null, new Sub_Orders[] { new(TenPayV3Info.MchId, name, new(price, "CNY"), sp_billno, null, "子订单测试1", null) }, new(openId), null, null, TenPayV3Info.TenPayV3Notify);

            BasePayApis basePayApis = new BasePayApis();
            var result = basePayApis.JsApiCombineAsync(requestData).GetAwaiter().GetResult();

            Console.WriteLine("微信支付 V3 预支付结果：" + result.ToJson(true));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ResultCode.Success);
            Assert.IsTrue(result.VerifySignSuccess == true);//通过验证
        }

        [TestMethod()]
        public void AppCombineAsyncTest()
        {
            var key = TenPayHelper.GetRegisterKey(Config.SenparcWeixinSetting);

            var TenPayV3Info = TenPayV3InfoCollection.Data[key];

            var price = 100;
            var name = "单元测试-" + DateTime.Now.ToString();
            var openId = "olPjZjsXuQPJoV0HlruZkNzKc91E";//换成测试人的 OpenId
            var combine_sp_billno = string.Format("{0}{1}{2}", TenPayV3Info.MchId/*10位*/, SystemTime.Now.ToString("yyyyMMddHHmmss"),
             TenPayV3Util.BuildRandomStr(6));
            var sp_billno = string.Format("{0}{1}{2}", TenPayV3Info.MchId/*10位*/, SystemTime.Now.ToString("yyyyMMddHHmmss"),
                         TenPayV3Util.BuildRandomStr(6));

            CombineTransactionsRequestData requestData = new(TenPayV3Info.AppId, TenPayV3Info.MchId, combine_sp_billno, null, new Sub_Orders[] { new(TenPayV3Info.MchId, name, new(price, "CNY"), sp_billno, null, "子订单测试1", null) }, new(openId), null, null, TenPayV3Info.TenPayV3Notify);

            BasePayApis basePayApis = new BasePayApis();
            var result = basePayApis.AppCombineAsync(requestData).GetAwaiter().GetResult();

            Console.WriteLine("微信支付 V3 预支付结果：" + result.ToJson(true));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ResultCode.Success);
            Assert.IsTrue(result.VerifySignSuccess == true);//通过验证
        }

        [TestMethod()]
        public void H5CombineAsyncTest()
        {
            var key = TenPayHelper.GetRegisterKey(Config.SenparcWeixinSetting);

            var TenPayV3Info = TenPayV3InfoCollection.Data[key];

            var price = 100;
            var name = "单元测试-" + DateTime.Now.ToString();
            var openId = "olPjZjsXuQPJoV0HlruZkNzKc91E";//换成测试人的 OpenId
            var combine_sp_billno = string.Format("{0}{1}{2}", TenPayV3Info.MchId/*10位*/, SystemTime.Now.ToString("yyyyMMddHHmmss"),
             TenPayV3Util.BuildRandomStr(6));
            var sp_billno = string.Format("{0}{1}{2}", TenPayV3Info.MchId/*10位*/, SystemTime.Now.ToString("yyyyMMddHHmmss"),
                         TenPayV3Util.BuildRandomStr(6));

            //注意：H5下单scene_info参数必填
            CombineTransactionsRequestData requestData = new(TenPayV3Info.AppId, TenPayV3Info.MchId, combine_sp_billno, new("14.23.150.211", null, new("Android", null, null, null, null)), new Sub_Orders[] { new(TenPayV3Info.MchId, name, new(price, "CNY"), sp_billno, null, "子订单测试1", null) }, new(openId), null, null, TenPayV3Info.TenPayV3Notify);

            BasePayApis basePayApis = new BasePayApis();
            var result = basePayApis.H5CombineAsync(requestData).GetAwaiter().GetResult();

            Console.WriteLine("微信支付 V3 预支付结果：" + result.ToJson(true));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ResultCode.Success);
            Assert.IsTrue(result.VerifySignSuccess == true);//通过验证
        }

        [TestMethod()]
        public void NativeCombineAsyncTest()
        {
            var key = TenPayHelper.GetRegisterKey(Config.SenparcWeixinSetting);

            var TenPayV3Info = TenPayV3InfoCollection.Data[key];

            var price = 100;
            var name = "单元测试-" + DateTime.Now.ToString();
            var openId = "olPjZjsXuQPJoV0HlruZkNzKc91E";//换成测试人的 OpenId
            var combine_sp_billno = string.Format("{0}{1}{2}", TenPayV3Info.MchId/*10位*/, SystemTime.Now.ToString("yyyyMMddHHmmss"),
             TenPayV3Util.BuildRandomStr(6));
            var sp_billno = string.Format("{0}{1}{2}", TenPayV3Info.MchId/*10位*/, SystemTime.Now.ToString("yyyyMMddHHmmss"),
                         TenPayV3Util.BuildRandomStr(6));

            CombineTransactionsRequestData requestData = new(TenPayV3Info.AppId, TenPayV3Info.MchId, combine_sp_billno, null, new Sub_Orders[] { new(TenPayV3Info.MchId, name, new(price, "CNY"), sp_billno, null, "子订单测试1", null) }, new(openId), null, null, TenPayV3Info.TenPayV3Notify);

            BasePayApis basePayApis = new BasePayApis();
            var result = basePayApis.NativeCombineAsync(requestData).GetAwaiter().GetResult();

            Console.WriteLine("微信支付 V3 预支付结果：" + result.ToJson(true));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ResultCode.Success);
            Assert.IsTrue(result.VerifySignSuccess == true);//通过验证
        }
    }
}