using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.SessionState;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using OY.OAuth.Interface;
using OY.OAuth.Model;
using OY.OAuth;

namespace OAuth
{
    /// <summary>
    /// OAuth 的摘要说明
    /// </summary>
    public class OAuth : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            IOAuth<OAuthConfig> _IOAuth = new OAuthManagement<OAuthConfig>(new TranProto<OAuthData<OAuthConfig>>()
            {
                Data = new OAuthData<OAuthConfig>()
                {
                    AuthName = "TopSandBox",
                    RedirectUrl = "http://127.0.0.1/OAuth/OAuth.ashx",
                    OAuthConfig = new OAuthConfig()
                    {
                        AppKey = "82E5140CDEC87C56E7580FC0CCEAB6F7",
                        AppSecret = "08925dbdb26b4aac8500d0c45c86a025",
                        AuthUrl = "https://oauth.jd.com/oauth/authorize",
                        OAuthInfo = new OAuthInfo()
                        {
                            AccessToken = "access_token",
                            RefreshToken = "refresh_token",
                            UserCode = "uid",
                            UserNick = "user_nick"
                        },
                        TokenUrl = "https://oauth.jd.com/oauth/token"
                    },
                    State = "object",
                    View = "web"
                }
            });

            //IOAuth<List<OAuthConfig>> _IOAuth = new OAuthManagement<List<OAuthConfig>>();

            string codeUrl = string.Empty;

            string tokenUrl = string.Empty;

            string code = context.Request["code"];
            if (string.IsNullOrWhiteSpace(code))
            {
                TranProto<string> tp = _IOAuth.GetAuthAddress();
                if (!tp.IsError && tp.Data != null)
                {
                    try
                    {
                        codeUrl = tp.Data;
                        context.Response.Redirect(codeUrl, true);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                TranProto<OY.OAuth.Model.OAuthInfo> tp = _IOAuth.GetTokenCode(code, "post");
                if (!tp.IsError && tp.Data != null)
                {
                    OY.OAuth.Model.OAuthInfo oai = tp.Data;
                    context.Response.Write("AccessToken:" + oai.AccessToken + "<br />");
                    context.Response.Write("RefreshToken:" + oai.RefreshToken + "<br />");
                    context.Response.Write("UserCode:" + oai.UserCode + "<br />");
                    context.Response.Write("UserNick:" + oai.UserNick + "<br />");
                    context.Response.Write("开始刷新令牌<br />");
                    TranProto<OY.OAuth.Model.OAuthInfo> tpRefresh = _IOAuth.GetTokenCode(oai.RefreshToken, "post", false);
                    if (!tpRefresh.IsError && tpRefresh.Data != null)
                    {
                        OY.OAuth.Model.OAuthInfo oaiRefresh = tpRefresh.Data;
                        context.Response.Write("AccessToken:" + oaiRefresh.AccessToken + "<br />");
                        context.Response.Write("RefreshToken:" + oaiRefresh.RefreshToken + "<br />");
                        context.Response.Write("UserCode:" + oaiRefresh.UserCode + "<br />");
                        context.Response.Write("UserNick:" + oaiRefresh.UserNick + "<br />");
                    }
                }
                else
                {
                    context.Response.Write(tp.Msg);
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    public enum OAuthType
    {
        //淘宝
        TOP = 1,
        //京东
        JOS = 2,
        //一号店
        YHD = 4
    }


}