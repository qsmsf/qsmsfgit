<template>
  <div >
    <div class="header" data-fixed="top" style="background: #0082ec ;">
      <div class="title row-box" style="background: #0082ec !important;">
        <div class="box-left"></div>
        <div class="span11">
          <h1>深圳E勘察</h1>
        </div>
        <div class="box-right">
          <div class="navbar">
            <ul class="navbar-items" data-menupos="bottom" style="font-size: 22px;">
              <li style="position: relative;">
                <div class="btn-active "  data-role="BTButton" data-type="image" onclick="showOrHide()">
                  <img src="../assets/menu.png" alt=""  />
                </div>
                <div id="my_sonmenu" style="display: none">
                  <div class="triangle-right-black" style="margin-right: 10px;margin-top: -10px;"></div>
                  <ul   style="width:220px;margin-top: 0px;left: -130px;background-color: #434343;">
                    <li style="padding:10px 0 10px 10px;border: none;" onclick="openAboutInfo()" data-role="BTButton">
                      <div style="text-align: left" >
                        <img src="../assets/about.png" style="width: 25px;height: 25px;margin-top: -5px;">
                        <span >关于</span>
                        <div style="height: 1px;background-color: #626262;margin-top: 10px;width: 200px;"></div>
                      </div>
                    </li>
                    <li style="padding: 5px 0 5px 10px;border: none;" onclick="runAppKnow()"data-role="BTButton">
                      <div style="text-align: left">
                        <img src="../assets/feedback.png" style="width: 25px;height: 25px;margin-top: -5px;" >
                        <span>帮助文档</span>
                        <div style="height: 1px;background-color: #626262;margin-top: 10px;width: 200px;"></div>
                      </div>
                    </li>
                    <li style="padding:5px 0 5px 10px;border: none;" onclick="runAppQuestion()" data-role="BTButton">
                      <div style="text-align: left">
                        <img src="../assets/question.png" style="width: 25px;height: 25px;margin-top: -5px;">
                        <span>问题反馈</span>
                        <div style="height: 1px;background-color: #626262;margin-top: 10px;width: 200px;"></div>
                      </div>
                    </li>
                    <li style="padding:5px  0 5px 10px;border: none;" onclick="openResponpolice()" data-role="BTButton"  >
                      <div style="text-align: left">
                        <img src="../assets/police.png" style="width: 25px;height: 25px;margin-top: -5px;">
                        <span style="text-overflow: ellipsis; overflow: hidden;white-space: nowrap;">责任民警-曹健</span>
                        <div style="height: 1px;background-color: #626262;margin-top: 10px;width: 200px;"></div>
                      </div>
                    </li>
                    <li style="padding: 5px 0 5px 10px;border: none;" onclick="app.refresh()" data-role="BTButton" >
                      <div style="text-align: left">
                        <img src="../assets/refresh.png" style="width: 25px;height: 25px;margin-top: -5px;">
                        <span>刷新</span>
                        <div style="height: 1px;background-color: #626262;margin-top: 10px;width: 200px;"></div>
                      </div>
                    </li>
                    <li style="padding: 5px 0 5px 10px;border: none;" onclick="app.exit()" data-role="BTButton">
                      <div style="text-align: left">
                        <img src="../assets/back.png" style="width: 25px;height: 25px;margin-top: -5px;">
                        <span >退出</span>
                        <div style="height: 1px;background-color: #626262;margin-top: 10px;width: 200px;"></div>
                      </div>
                    </li>
                  </ul>
                </div>
              </li>
            </ul>
          </div>
        </div>

      </div>
    </div>
    <div class="vux-demo">
      <img class="logo" src="../assets/gongan.png">
      <h3>深圳E勘查</h3>
    </div>
    <group>
      <x-input title="用户名" placeholder="请输入用户名" novalidate :icon-type="iconType" :show-clear="false" v-model="loginName" ></x-input>
      <x-input title="密码 " placeholder="请输入密码" novalidate :icon-type="iconType" :show-clear="false" v-model='password'
      type="password" ></x-input>
      <x-button type="primary" @click.native="login" :disabled="disable001" >登录</x-button>
      <x-switch title="记住账号" v-model="rememberFlag" ></x-switch>  
      <a href="http://182.61.43.29/webapi/api/Version/GetNewestApk" style="display:none">下载app</a>
    </group>
  </div>
</template>

<script>
import { Group, XInput, XButton, XSwitch } from 'vux'
import { SET_BASEINFO, SET_SYSINFO } from '../mutationTypes'

  var exitFlag = false;
  var appCode = "com.xinghuo.question";//问题反馈的appCode
  var know_appUrl = "modules/question/know_info.html";//帮助文档的路径
  var question_appUrl = "modules/question/work_order_form.html";//问题反馈的路径

  var mAppName = "深云E勘察";//本项目的名称
/*
  app.page.onLoad = function() {
    document.addEventListener("deviceready", function() {
      document.addEventListener("backbutton", backFunc, false);
    }, false);
    ui.Menu.init({
      sectionId: 'index_section',
      duration: 200
    });
    $(document).click(function(){
      $("#my_sonmenu").hide();
    });
  }
*/
var md5 = require('js-md5')

export default {
  components: {
    Group,
    XButton,
    XInput,
    XSwitch
  },
  mounted: function () {
    let url = 'http://182.61.43.29/webapi/'
    //url = 'http://localhost/PbsApi/'
    //url = 'http://68.68.18.242/Pbs/webapi'
    this.$store.commit(SET_SYSINFO, url)
    this.loadAccountInfo()
  },
  methods: {
    login () {
      this.disable001 = true
      this.$vux.loading.show({
        text: '登陆中'
      })
      var loginInfo = 'username=' + this.loginName + '&password=' + md5(this.password)
      this.$http({
        url: this.$store.getters.GetterBaseUrl + 'Auth/Login?' + loginInfo,
        method: 'Get',
        emulateJSON: true,
        headers: {
          contentType: 'application/x-www-form-urlencoded'
        }
      }).then(function (res) {
        // let data = JSON.parse(res.data)
        if (res.data.code === 100) {
          this.$store.commit(SET_BASEINFO, res.data)
          this.$vux.loading.hide()
          this.success()
        } else {
          this.$vux.loading.hide()
          this.$vux.toast.show({
            text: res.data.message,
            type: 'warn'
          })
          this.disable001 = false
        }
      }).catch(err => {
        this.$vux.loading.hide()
        this.$vux.toast.show({
          text: err,
          type: 'warn'
        })
        this.disable001 = false
      })
    },
    success () {
      var mySelf = this
      var userName = mySelf.loginName
      var userPwd = mySelf.password
      //记住密码checkbox的勾选状态 和账号信息的字符串  
      var rememberStatus = mySelf.rememberFlag
      var accountInfo = "";  
      accountInfo = userName + "&" + userPwd

      if (rememberStatus){  
          console.log("勾选了记住密码，现在开始写入cookie")
          mySelf.setCookie('accountInfo',accountInfo,1440*3)
      }  
      else{  
          console.log("没有勾选记住密码，现在开始删除账号cookie")
          mySelf.delCookie('accountInfo')
      }

      this.$store.dispatch('clearList')
      let params = {token: this.$store.getters.GetterToken, baseUrl: this.$store.getters.GetterBaseUrl, myUnitId: this.$store.getters.GetterMe.member.unit_id}
      this.$store.dispatch('fetchUserList', params)
      this.$store.dispatch('fetchUnitList', params)
      this.$router.push({path: '/MainIndex'})
    },
    loadAccountInfo: function(){
      let mySelf = this
      let accountInfo = this.getCookie('accountInfo')
      //如果cookie里没有账号信息  
      if(Boolean(accountInfo) == false){  
        console.log('cookie中没有检测到账号信息！')
        return false 
      }else{  
      //如果cookie里有账号信息  
        console.log('cookie中检测到账号信息！现在开始预填写！')
        let userName = ""
        let passWord = ""
        let index = accountInfo.indexOf("&")

        userName = accountInfo.substring(0,index)
        passWord = accountInfo.substring(index+1)

        mySelf.loginName = userName
        mySelf.password = passWord
        mySelf.rememberFlag = true
      }
    },
    // 设置cookie
    setCookie: function(c_name,value,expiremMinutes){
        var exdate = new Date();
        exdate.setTime(exdate.getTime() + expiremMinutes * 60 * 1000);
        document.cookie= c_name + "=" + escape(value)+((expiremMinutes==null) ? "" : ";expires="+exdate.toGMTString());
    },

    // 读取cookie
    getCookie: function(c_name){
        if (document.cookie.length>0)
        {
            var c_start=document.cookie.indexOf(c_name + "=");
            if (c_start!=-1)
            { 
            c_start=c_start + c_name.length+1;
            var c_end=document.cookie.indexOf(";",c_start);
            if (c_end==-1) 
                c_end = document.cookie.length
                return unescape(document.cookie.substring(c_start, c_end))
            }
        }
        return ""   
    },

    // 删除cookie
    delCookie: function(c_name){
        var exp = new Date();
        exp.setTime(exp.getTime() - 1);
        var cval = this.getCookie(c_name);
        if(cval!=null){
            document.cookie = c_name + "=" + cval + ";expires=" + exp.toGMTString();
        }
    },
    //返回退出
    backFunc() {
      var isIndexPage = ui.Page.history.length <= 1 ? true : false;
      if(isIndexPage) {
        if(exitFlag) {
          app.exit();
        } else {
          exitFlag = true;
          app.hint("再次点击返回退出应用");
          setTimeout("exitFlag = false;", 3000);
        }
      } else {
        //单页后退，多页不处理
        if(ui.settings.appType == 'single') {
          ui.Page.back();
        }
      }
    },
    //跳转到about关于页面，about页面也直接仿照写
    openAboutInfo() {
      showOrHide();
      ui.load({
        url: "about.html",
        params: {}
      });
    },

    //跳转到责任民警页面
    openResponpolice() {
      showOrHide();//隐藏popup框
      //先去问责任民警的警号，再找曹工获取这个userId.
      app.link.startUserCard("3caf2cf4-25f0-4dc8-8a8c-09bf4cc61644");//在应用市场搜索 框架3-->Link API示例-->通讯录-->找到这个民警-->即可看到userId

    },
    //跳转到帮助文档页面即常见问题列表页面
    runAppKnow(){
      showOrHide();//隐藏popup框
      //暂时跳这个页面,这个页面需要自己写
      ui.load({
        url: "support.html",
        params: {}
      });
      //以后跳下个页面
//        var params = {
//          appCode: appCode,
//          data: {
//            APP_NAME: APP_NAME
//          },
//          appUrl: know_appUrl
//        };
//        app.link.runApp(params);
    },
    //跳转到问题反馈页面，只需要把url.js，改了之后，下面代码直接复制
    runAppQuestion(){
      showOrHide();//隐藏popup框
      var params = {
        appCode: appCode,
        data:{
          isPages:"1",
          systemId:"GACLOUD",
          app_code: APP_NAME,
          appName:mAppName
        },
        appUrl:question_appUrl
      };
      app.link.runApp(params);
    },
    //隐藏popup框
    showOrHide(){
      $("#my_sonmenu").toggle();
      event.stopPropagation();//这句是必须
    }
  },
  data () {
    return {
      iconType: '',
      disable001: false,
      loginName: '',
      password: '',
      rememberFlag: false
    }
  }
}
</script>

<style>
  .vux-demo {
    text-align: center;
  }
  .logo {
    width: 100px;
    height: 100px
  }
  .header .title .navbar li [data-role="BTButton"].btn-active {
    background-color: transparent !important;
    border: 0;
  }

  .header .title .navbar {
    width: 80px;
    padding-top: 0;
    text-align: right;
  }

  .navbar ul[data-menupos="bottom"] .angle {
    bottom: -11px;
    left: 42%;
  }

  .angle {
    background: #434343;
  }
  .navbar [data-role="BTButton"] .btn-text{
    line-height: 30px;
    padding-left: 0;
    font-weight: normal;
    text-shadow: none;
  }
  .header .title .navbar li [data-role="BTButton"].btn-active{
    color: #FFFFff;
  }
  .triangle-right-black {
    width: 0;
    height: 0;
    border: 10px solid transparent;
    border-bottom: 10px solid #434343;
  }
</style>
