const util = require("../../utils/util.js");
const app = getApp()
Page({

    data:{
        dataList: [],
        userInfo: {},
        hasUserInfo: false,
        policeInfo:{
          jh:'37X60',
          name:'马光磊',
          unitName: '深圳市南山公安分局'
        },
        modalShowStyle: '',
        canIUse: wx.canIUse('button.open-type.getUserInfo')
    },
    //事件处理函数
    bindViewTap: function () {
      wx.navigateTo({
        url: '../logs/logs'
      })
    },

    onLoad:function(options){
        util.showLoading();
        console.log(app.globalData.userInfo)
        if (app.globalData.userInfo) {
          console.log(22)
          this.setData({
            userInfo: app.globalData.userInfo,
            hasUserInfo: true
          })
        } else if (this.data.canIUse) {
          console.log(333);
          // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
          // 所以此处加入 callback 以防止这种情况
          app.userInfoReadyCallback = res => {
            this.setData({
              userInfo: res.userInfo,
              hasUserInfo: true
            })
          }
        } else {
          // 在没有 open-type=getUserInfo 版本的兼容处理
          wx.getUserInfo({
            success: res => {
              app.globalData.userInfo = res.userInfo
              this.setData({
                userInfo: res.userInfo,
                hasUserInfo: true
              })
            }
          })
        }
        /*
        var that = this;
        var parameters = "a=square&c=topic";
        console.log("parameters = "+parameters);
        util.request(parameters,function(res){
            that.setData({
                dataList:res.data.square_list
            });
            setTimeout(function(){
                util.hideToast();
                wx.stopPullDownRefresh();
            },1000);
        });
        */
    },
    //登录
    taplogin:function(){
        wx.navigateTo({
        url: '../login/login',
        success: function(res){
          // success
        },
        fail: function() {
          // fail
        },
        complete: function() {
          // complete
        }
      })
    },
  getUserInfo: function (e) {
    console.log(e)
    app.globalData.userInfo = e.detail.userInfo
    this.setData({
      userInfo: e.detail.userInfo,
      hasUserInfo: true
    })
  },

  jhInput: function (e) {
    let that = this
    let title = e.detail.value

    this.setData({
      title: title
    })
  },
  nameInput: function (e) {
    let that = this
    let title = e.detail.value

    this.setData({
      title: title
    })
  },
  unitNameInput: function (e) {
    let that = this
    let title = e.detail.value

    this.setData({
      title: title
    })
  },
  showModal: function () {
    this.setData({
      modalShowStyle: 'opacity:1;pointer-events:auto;'
    })
  },
  touchCancel: function () {
    this.setData({
      modalShowStyle: '',
      policeInfo:{
        jg:'',
        name:'',
        unitName:''
      }
    })
  },
  touchApplication: function(e){
    this.setData({
      modalShowStyle: ''
    })
  }
});