<template>
  <div>
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
    url = 'http://68.68.18.242/Pbs/webapi'
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
</style>
