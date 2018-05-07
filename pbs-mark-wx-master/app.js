//app.js
const util = require("./utils/util.js");
App({
  onLaunch: function () {
    //调用API从本地缓存中获取数据
    var that = this
    var logs = wx.getStorageSync('logs') || []
    logs.unshift(Date.now())
    wx.setStorageSync('logs', logs)

    console.log("begin to login")
    // 登录
    wx.login({
      success: res => {
        // 发送 res.code 到后台换取 openId, sessionKey, unionId
        console.log(res.code)
        wx.request({
          //获取openid接口  
          url: 'https://api.weixin.qq.com/sns/jscode2session',
          data: {
            appid: that.globalData.APP_ID,
            secret: that.globalData.APP_SECRET,
            js_code: res.code,
            grant_type: 'authorization_code'
          },
          method: 'GET',
          success: res => {
            console.log(res.data)
            that.globalData.openId = res.data.openid;//获取到的openid  
            that.globalData.sessionKey = res.data.session_key;//获取到session_key    
            that.globalData.unionId = ''            
            this.fetchLoginInfo()
          }
        })        
      }
    })
    
    console.log("begin to getsetting")
    // 获取用户信息
    wx.getSetting({
      success: res => {
        console.log(res)
        if (res.authSetting['scope.userInfo']) {
          // 已经授权，可以直接调用 getUserInfo 获取头像昵称，不会弹框          
          wx.getUserInfo({
            success: res => {
              // 可以将 res 发送给后台解码出 unionId
              this.globalData.userInfo = res.userInfo
              // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
              // 所以此处加入 callback 以防止这种情况
              if (this.userInfoReadyCallback) {
                this.userInfoReadyCallback(res)
              }
            }
          })
        }
      }
    })
  },
  getUserInfo:function(cb){
    var that = this
    console.log("111")
    if(this.globalData.userInfo){
      typeof cb == "function" && cb(this.globalData.userInfo)
    }else{
      //调用登录接口
      wx.login({
        success: function () {
          wx.getUserInfo({
            success: function (res) {
              that.globalData.userInfo = res.userInfo
              typeof cb == "function" && cb(that.globalData.userInfo)
            }
          })
        }
      })
    }
  },
  //获取后台登录信息
  fetchLoginInfo: function () {
    util.showLoading();
    var that = this;
    var parameters = '/Auth/WxLogin?openId=' + that.globalData.openId;
    var header={
      username: '',
      userfullname: '',
      avatarUrl: ''
    }
    console.log("parameters = " + parameters);
    util.request(parameters, function (res) {
      //page = 1;
      console.log(res)
      setTimeout(function () {
        util.hideToast();
        wx.stopPullDownRefresh();
      }, 1000);
    }, "GET", header)
  },
  globalData:{
    userInfo: null,
    openId: null,
    unionId: null,
    sessionKey: null,
    APP_ID:'wx07804b0287610f69',//输入小程序appid  
    APP_SECRET: ''//输入小程序app_secret      
  }
})